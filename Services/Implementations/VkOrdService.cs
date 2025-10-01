using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using VkOrdApiWrapper.Configuration;
using VkOrdApiWrapper.Models.Requests;
using VkOrdApiWrapper.Models.Responses;
using VkOrdApiWrapper.Models.VkOrd;
using VkOrdApiWrapper.Services.Interfaces;
using VkOrdApiWrapper.Models.DaData;
using System.Text.Json;
using System.Net.Http;
using VkOrdApiWrapper.Data;
using VkOrdApiWrapper.Security;

namespace VkOrdApiWrapper.Services.Implementations
{
    /// <summary>
    /// Сервис для работы с VK ОРД
    /// </summary>
    public class VkOrdService : IVkOrdService
    {
        private readonly IVkOrdApiClientFactory _vkOrdClientFactory;
        private readonly VkOrdConfiguration _config;
        private readonly ILogger<VkOrdService> _logger;
        private readonly IDistributedCache _cache;
        private readonly IDaDataService _daDataService;
        private readonly ApplicationDbContext _db;
        private readonly ISecretProtector _protector;

        public VkOrdService(
            IVkOrdApiClientFactory vkOrdClientFactory,
            IOptions<VkOrdConfiguration> config,
            ILogger<VkOrdService> logger,
            IDistributedCache cache,
            IDaDataService daDataService,
            ApplicationDbContext db,
            ISecretProtector protector)
        {
            _vkOrdClientFactory = vkOrdClientFactory;
            _config = config.Value;
            _logger = logger;
            _cache = cache;
            _daDataService = daDataService;
            _db = db;
            _protector = protector;
        }

        #region Контракты

        private async Task<VkApiContext> ResolveContextAsync(Guid userId, string? environment)
        {
            var cred = await _db.ApiCredentials
                .Where(c => c.UserId == userId && (environment == null || c.Environment.ToLower() == environment.ToLower()))
                .OrderByDescending(c => c.UpdatedAt)
                .FirstOrDefaultAsync();
            if (cred == null)
            {
                throw new InvalidOperationException("VK ORD credentials not found for user");
            }
            var token = _protector.Decrypt(cred.TokenEncrypted);
            var route = cred.Environment.Equals("Production", StringComparison.OrdinalIgnoreCase) || cred.Environment.Equals("prod", StringComparison.OrdinalIgnoreCase)
                ? "prod" : "sandbox";
            return new VkApiContext { ApiKey = token, Route = route };
        }

        public async Task<CreateContractResponse> CreateOrUpdateContractAsync(CreateContractRequest request, Guid userId, string? environment = null)
        {
            try
            {
                var apiContext = await ResolveContextAsync(userId, environment);
                var vkOrdClient = _vkOrdClientFactory.CreateClient(apiContext);

                var vkOrdContract = new VkOrdContract
                {
                    Type = "service",
                    ClientExternalId = request.ClientExternalId,
                    ContractorExternalId = request.ContractorExternalId,
                    Date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                    DateEnd = request.PayDateEnd ?? DateTime.UtcNow.ToString("yyyy-MM-dd"),
                    Serial = DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss"),
                    ActionType = "distribution",
                    SubjectType = "distribution",
                    ParentContractExternalId = null,
                    Flags = new List<string> { VkContactFlags.vat_included.ToString() },
                    Amount = request.PaySum.ToString()
                };

                _logger.LogInformation($"Creating/updating contract with external_id: {request.ExternalId} using route: {apiContext.Route}");

                var response = await vkOrdClient.CreateOrUpdateContractAsync(
                    request.ExternalId, vkOrdContract);

                if (response?.IsSuccess ?? true)
                {
                    var result = new CreateContractResponse
                    {
                        ExternalId = request.ExternalId,
                        Success = true,
                        CreatedAt = DateTime.UtcNow
                    };


                    await _cache.SetStringAsync(
                        $"contract_{request.ExternalId}",
                        "1",
                        new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24) }
                    );
                    _logger.LogInformation($"Contract created/updated successfully");

                    return result;
                }
                else
                {
                    _logger.LogError($"Failed to create/update contract: {response.Error}");
                    return new CreateContractResponse
                    {
                        ExternalId = request.ExternalId,
                        Success = false,
                        ErrorMessage = response.Error
                    };
                }
            }
            catch (Refit.ApiException refit)
            {
                return new CreateContractResponse
                {
                    ExternalId = request.ExternalId,
                    Success = false,
                    ErrorMessage = refit.Content ?? "Ошибка создания/обновления контракта в VK ОРД"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating/updating contract");
                return new CreateContractResponse
                {
                    ExternalId = request.ExternalId,
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<ContractResponse> GetContractAsync(string externalId, Guid userId, string? environment = null)
        {
            try
            {
                var apiContext = await ResolveContextAsync(userId, environment);
                // Проверяем кэш
                var cachedFlag = await _cache.GetStringAsync($"contract_{externalId}");
                if (!string.IsNullOrEmpty(cachedFlag))
                {
                    return ContractResponse.FromVkOrdResponse(
                        new VkOrdResponse<VkOrdContract>
                        {
                            Data = new VkOrdContract() // В кэше хранится только результат создания
                        }, externalId);
                }

                var vkOrdClient = _vkOrdClientFactory.CreateClient(apiContext);
                var response = await vkOrdClient.GetContractAsync(externalId);

                return ContractResponse.FromVkOrdResponse(response, externalId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting contract {externalId}");
                return new ContractResponse
                {
                    Success = false,
                    Message = ex.Message,
                    ExternalId = externalId
                };
            }
        }

        #endregion

        #region Креативы

        public async Task<CreateCreativeResponse> CreateCreativeAsync(CreateCreativeRequest request, Guid userId, string? environment = null)
        {
            try
            {
                var apiContext = await ResolveContextAsync(userId, environment);
                var vkOrdClient = _vkOrdClientFactory.CreateClient(apiContext);

                var vkOrdCreative = new VkOrdCreative()
                {
                    ExternalId = request.ExternalId,
                    Name = request.Name,
                    ContractExternalIds = request.ContractExternalIds,
                    MediaExternalIds = request.MediaExternalIds,
                    Form = request.Format.ToString(),
                    TargetUrls = request.ContentUrls,
                    Targeting = request.TargetAudience,
                    KKTYCodes = request.KKTYCodes,
                    PayType = VkCreativePayType.cpa.ToString(),
                    Texts = new List<string> { request.Text },
                    Flags = new List<string> { "native" }
                };

                _logger.LogInformation($"Creating creative with external_id: {vkOrdCreative.ExternalId} using route: {apiContext.Route}");

                var response = await vkOrdClient.CreateOrUpdateCreativeAsync(
                    vkOrdCreative.ExternalId, vkOrdCreative);

                if (response.IsSuccess)
                {
                    // Кэшируем результат
                    var result = new CreateCreativeResponse
                    {
                        Erid = response.Erid,
                        Success = true,
                    };


                    var createdJson = JsonSerializer.Serialize(result);
                    await _cache.SetStringAsync(
                        $"creative_{vkOrdCreative.ExternalId}",
                        createdJson,
                        new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24) }
                    );
                    _logger.LogInformation($"Creative created successfully. ERID: {response.Erid}");

                    return result;
                }
                else
                {
                    _logger.LogError($"Failed to create creative: {response.Error}");
                    return new CreateCreativeResponse
                    {
                        ExternalId = vkOrdCreative.ExternalId,
                        Success = false,
                        ErrorMessage = response.Error
                    };
                }
            }
            catch (Refit.ApiException refit)
            {
                return new CreateCreativeResponse
                {
                    ExternalId = request.ExternalId,
                    Success = false,
                    ErrorMessage = refit.Content ?? "Ошибка создания креатива в VK ОРД"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating creative");
                return new CreateCreativeResponse
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<CreateCreativeResponse> GetCreativeAsync(string externalId, Guid userId, string? environment = null)
        {
            try
            {
                var apiContext = await ResolveContextAsync(userId, environment);
                // Проверяем кэш
                var cachedJson = await _cache.GetStringAsync($"creative_{externalId}");
                if (!string.IsNullOrEmpty(cachedJson))
                {
                    var cached = JsonSerializer.Deserialize<CreateCreativeResponse>(cachedJson);
                    if (cached != null) return cached;
                }

                var vkOrdClient = _vkOrdClientFactory.CreateClient(apiContext);
                var response = await vkOrdClient.GetCreativeAsync(externalId);

                if (response.IsSuccess)
                {
                    var result = new CreateCreativeResponse
                    {
                        ExternalId = externalId,
                        Erid = response.Erid,
                        Success = true
                    };

                    var json = JsonSerializer.Serialize(result);
                    await _cache.SetStringAsync(
                        $"creative_{externalId}",
                        json,
                        new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24) }
                    );
                    return result;
                }
                else
                {
                    return new CreateCreativeResponse
                    {
                        ExternalId = externalId,
                        Success = false,
                        ErrorMessage = response.Error
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting creative {externalId}");
                return new CreateCreativeResponse
                {
                    ExternalId = externalId,
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<VkOrdStatusResponse> GetCreativeStatusAsync(string externalId, Guid userId, string? environment = null)
        {
            try
            {
                var apiContext = await ResolveContextAsync(userId, environment);
                var vkOrdClient = _vkOrdClientFactory.CreateClient(apiContext);
                return await vkOrdClient.GetCreativeStatusAsync(externalId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting creative status {externalId}");
                return new VkOrdStatusResponse
                {
                    Status = "error",
                    Message = ex.Message
                };
            }
        }

        public async Task<bool> DeleteCreativeAsync(string externalId, Guid userId, string? environment = null)
        {
            try
            {
                var apiContext = await ResolveContextAsync(userId, environment);
                var vkOrdClient = _vkOrdClientFactory.CreateClient(apiContext);
                var response = await vkOrdClient.DeleteCreativeAsync(externalId);

                if (response.IsSuccessStatusCode)
                {
                    await _cache.RemoveAsync($"creative_{externalId}");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting creative {externalId}");
                return false;
            }
        }

        public async Task<List<CreateCreativeResponse>> CreateBulkCreativesAsync(List<CreateCreativeRequest> requests, Guid userId, string? environment = null)
        {
            var results = new List<CreateCreativeResponse>();
            var semaphore = new SemaphoreSlim(_config.MaxConcurrentRequests, _config.MaxConcurrentRequests);
            var tasks = requests.Select(async request =>
            {
                await semaphore.WaitAsync();
                try
                {
                    return await CreateCreativeAsync(request, userId, environment);
                }
                finally
                {
                    semaphore.Release();
                }
            });

            results.AddRange(await Task.WhenAll(tasks));
            return results;
        }

        public async Task<bool> IsCreativeVerifiedAsync(string externalId, Guid userId, string? environment = null, int maxWaitTimeMinutes = 120)
        {
            var startTime = DateTime.UtcNow;
            var maxWaitTime = TimeSpan.FromMinutes(maxWaitTimeMinutes);

            while (DateTime.UtcNow - startTime < maxWaitTime)
            {
                var status = await GetCreativeStatusAsync(externalId, userId, environment);

                if (status.Status == "verified")
                {
                    return true;
                }
                else if (status.Status == "error")
                {
                    _logger.LogError($"Creative {externalId} failed verification: {status.Message}");
                    return false;
                }

                // Ждем 2 минуты перед следующей проверкой
                await Task.Delay(TimeSpan.FromMinutes(2));
            }

            _logger.LogWarning($"Creative {externalId} verification timeout after {maxWaitTimeMinutes} minutes");
            return false;
        }

        private string GenerateExternalId()
        {
            return $"creative_{DateTime.UtcNow:yyyyMMdd}_{Guid.NewGuid():N}";
        }

        #endregion

        #region Контрагенты

        public async Task<StatusResponse> CreateCounterpartyFromInnAsync(string inn, List<string> types, Guid userId, string? environment = null)
        {
            if (string.IsNullOrWhiteSpace(inn))
            {
                return StatusResponse.Error("ИНН не указан");
            }

            try
            {
                var dadata = await _daDataService.FindPartyByInnAsync(inn);
                if (dadata == null)
                {
                    return StatusResponse.Error("Контрагент по ИНН не найден в DaData");
                }

                var roles = types.Select(type => type.ToString()).ToList();
                VkPersonType type;
                // dadata.Type:  LEGAL — юридическое лицо, INDIVIDUAL — индивидуальный предприниматель
                type = dadata.Type == "LEGAL" ? VkPersonType.juridical : VkPersonType.ip;

                var name = dadata.Value
                    ?? dadata.Name?.FullWithOpf
                    ?? dadata.Name?.Full
                    ?? dadata.Inn
                    ?? string.Empty;

                // Map to VK ORD person schema
                var person = new VkOrdPerson
                {
                    Name = name,
                    Roles = roles,
                    RsUrl = null,
                    JuridicalDetails = new VkOrdPersonJuridicalDetails
                    {
                        Type = type.ToString(),
                        ModelScheme = "russia",
                        Inn = dadata.Inn,
                        Kpp = dadata.Kpp,
                        Phone = dadata.Phone,
                        ForeignEpaymentMethod = null,
                        ForeignRegistrationNumber = null,
                        ForeignInn = null,
                        ForeignOksmCountryCode = null
                    }
                };

                var externalId = dadata.Inn ?? inn;
                var apiContext = await ResolveContextAsync(userId, environment);
                var vkOrdClient = _vkOrdClientFactory.CreateClient(apiContext);
                try
                {
                    var response = await vkOrdClient.CreateOrUpdatePersonAsync(externalId, person);
                    if (response.IsSuccess)
                    {
                        // Обновляем кэш контрагента
                        var cacheKey = $"person_{userId}_{environment ?? "default"}_{externalId}";
                        var json = JsonSerializer.Serialize(person);
                        await _cache.SetStringAsync(
                            cacheKey,
                            json,
                            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24) }
                        );
                        _logger.LogInformation($"Контрагент создан и закэширован: {externalId}");
                        return StatusResponse.Success("Контрагент создан");
                    }
                }
                catch (Refit.ApiException refit)
                {
                    return StatusResponse.Error(refit.Content ?? "Ошибка создания контрагента в VK ОРД");
                }
                catch (Exception e)
                {
                    return StatusResponse.Error(e.Message + e.InnerException?.Message ?? "Ошибка создания контрагента в VK ОРД");
                }
                   
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка создания контрагента по ИНН {Inn}", inn);
                return StatusResponse.Error(ex.Message);
            }

            // Должны вернуться ранее; fallback на случай непредвиденного поведения
            return StatusResponse.Error("Не удалось создать контрагента");
        }

        public async Task<GetCounterpartiesResponse> GetAllCounterpartiesAsync(Guid userId, string? environment = null, int? offset = null, int? limit = null)
        {
            try
            {
                var apiContext = await ResolveContextAsync(userId, environment);
                var vkOrdClient = _vkOrdClientFactory.CreateClient(apiContext);

                _logger.LogInformation($"Fetching counterparties using route: {apiContext.Route} (offset: {offset}, limit: {limit})");

                var response = await vkOrdClient.GetPersonsAsync(offset, limit);

                _logger.LogInformation($"VK ORD API response - ExternalIds count: {response?.ExternalIds?.Count ?? 0}, TotalItemsCount: {response?.TotalItemsCount}, Limit: {response?.Limit}");

                if (response?.ExternalIds != null)
                {
                    var externalIds = response.ExternalIds;
                    var totalItemsCount = response.TotalItemsCount;
                    var responseLimit = response.Limit;

                    _logger.LogInformation($"Found {externalIds.Count} counterparties (total: {totalItemsCount}, responseLimit: {responseLimit}), fetching full data for each");

                    // Получаем полные данные для каждого контрагента последовательно
                    var counterparties = new List<VkOrdPerson>();

                    foreach (var externalId in externalIds)
                    {
                        var counterpartyResponse = await GetCounterpartyByIdAsync(externalId, userId, environment);
                        if (counterpartyResponse.Success && counterpartyResponse.Person != null)
                        {
                            counterparties.Add(counterpartyResponse.Person);
                        }
                    }

                    _logger.LogInformation($"Successfully fetched {counterparties.Count} out of {externalIds.Count} counterparties");

                    return new GetCounterpartiesResponse
                    {
                        Success = true,
                        Counterparties = counterparties,
                        TotalItemsCount = totalItemsCount,
                        Limit = responseLimit
                    };
                }
                else
                {
                    _logger.LogError("Failed to fetch counterparties: response is null or ExternalIds is null");
                    return new GetCounterpartiesResponse
                    {
                        Success = false,
                        ErrorMessage = "Не удалось получить список контрагентов"
                    };
                }
            }
            catch (Refit.ApiException refit)
            {
                _logger.LogError(refit, "API error while fetching counterparties");
                return new GetCounterpartiesResponse
                {
                    Success = false,
                    ErrorMessage = $"Ошибка получения списка контрагентов из VK ОРД: {refit.Message}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching counterparties");
                return new GetCounterpartiesResponse
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<GetCounterpartyResponse> GetCounterpartyByIdAsync(string externalId, Guid userId, string? environment = null)
        {
            try
            {
                // Проверяем кэш
                var cacheKey = $"person_{userId}_{environment ?? "default"}_{externalId}";
                var cachedJson = await _cache.GetStringAsync(cacheKey);
                if (!string.IsNullOrEmpty(cachedJson))
                {
                    var cached = JsonSerializer.Deserialize<VkOrdPerson>(cachedJson);
                    if (cached != null)
                    {
                        _logger.LogInformation($"Контрагент {externalId} получен из кэша");
                        return new GetCounterpartyResponse
                        {
                            Success = true,
                            ExternalId = externalId,
                            Person = cached
                        };
                    }
                }
                var apiContext = await ResolveContextAsync(userId, environment);
                // Получаем из VK ORD API
                var vkOrdClient = _vkOrdClientFactory.CreateClient(apiContext);
                _logger.LogInformation($"Fetching counterparty {externalId} using route: {apiContext.Route}");

                var person = await vkOrdClient.GetPersonAsync(externalId);

                if (person != null)
                {
                    // Кэшируем результат
                    var json = JsonSerializer.Serialize(person);
                    await _cache.SetStringAsync(
                        cacheKey,
                        json,
                        new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24) }
                    );

                    return new GetCounterpartyResponse
                    {
                        Success = true,
                        ExternalId = externalId,
                        Person = person
                    };
                }
                else
                {
                    _logger.LogError($"Failed to fetch counterparty {externalId}: person is null");
                    return new GetCounterpartyResponse
                    {
                        Success = false,
                        ExternalId = externalId,
                        ErrorMessage = "Не удалось получить контрагента"
                    };
                }
            }
            catch (Refit.ApiException refit)
            {
                _logger.LogError(refit, $"API error while fetching counterparty {externalId}");
                return new GetCounterpartyResponse
                {
                    Success = false,
                    ExternalId = externalId,
                    ErrorMessage = $"Ошибка получения контрагента {externalId} из VK ОРД: {refit.Message}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching counterparty {externalId}");
                return new GetCounterpartyResponse
                {
                    Success = false,
                    ExternalId = externalId,
                    ErrorMessage = ex.Message
                };
            }
        }

        #endregion

        #region Медиа файлы

        public async Task<UploadMediaResponse> UploadMediaAsync(UploadMediaRequest request, Guid userId, string? environment = null)
        {
            try
            {
                var apiContext = await ResolveContextAsync(userId, environment);
                var vkOrdClient = _vkOrdClientFactory.CreateClient(apiContext);

                var streamPart = new Refit.StreamPart(request.FileStream, request.FileName, request.ContentType ?? "application/octet-stream");

                _logger.LogInformation($"Uploading media file with external_id: {request.ExternalId} using route: {apiContext.Route}");

                var response = await vkOrdClient.UploadMediaAsync(request.ExternalId, streamPart);

                if (response.IsSuccess)
                {
                    var result = new UploadMediaResponse
                    {
                        Success = true,
                        ExternalId = request.ExternalId,
                        Erid = response.Erid,
                        Url = response.Data?.Url ?? string.Empty
                    };

                    // Кэшируем результат
                    var json = JsonSerializer.Serialize(result);
                    await _cache.SetStringAsync(
                        $"media_{request.ExternalId}",
                        json,
                        new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24) }
                    );

                    _logger.LogInformation($"Media file uploaded successfully. ERID: {response.Erid}");
                    return result;
                }
                else
                {
                    _logger.LogError($"Failed to upload media file: {response.Error}");
                    return new UploadMediaResponse
                    {
                        Success = false,
                        ExternalId = request.ExternalId,
                        ErrorMessage = response.Error
                    };
                }
            }
            catch (Refit.ApiException refit)
            {
                return new UploadMediaResponse
                {
                    Success = false,
                    ExternalId = request.ExternalId,
                    ErrorMessage = refit.Content ?? "Ошибка загрузки медиа файла в VK ОРД"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading media file");
                return new UploadMediaResponse
                {
                    Success = false,
                    ExternalId = request.ExternalId,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<GetMediaResponse> GetMediaAsync(string externalId, Guid userId, string? environment = null)
        {
            try
            {
                var apiContext = await ResolveContextAsync(userId, environment);
                // Проверяем кэш
                var cachedJson = await _cache.GetStringAsync($"media_{externalId}");
                if (!string.IsNullOrEmpty(cachedJson))
                {
                    var cached = JsonSerializer.Deserialize<GetMediaResponse>(cachedJson);
                    if (cached != null)
                    {
                        return cached;
                    }
                }

                var vkOrdClient = _vkOrdClientFactory.CreateClient(apiContext);
                var response = await vkOrdClient.GetMediaAsync(externalId);

                if (response.IsSuccess)
                {
                    var result = new GetMediaResponse
                    {
                        Success = true,
                        ExternalId = externalId,
                        Media = response.Data
                    };

                    // Кэшируем результат
                    var json = JsonSerializer.Serialize(result);
                    await _cache.SetStringAsync(
                        $"media_{externalId}",
                        json,
                        new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24) }
                    );

                    return result;
                }
                else
                {
                    return new GetMediaResponse
                    {
                        Success = false,
                        ExternalId = externalId,
                        ErrorMessage = response.Error
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting media {externalId}");
                return new GetMediaResponse
                {
                    Success = false,
                    ExternalId = externalId,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<bool> DeleteMediaAsync(string externalId, Guid userId, string? environment = null)
        {
            try
            {
                var apiContext = await ResolveContextAsync(userId, environment);
                var vkOrdClient = _vkOrdClientFactory.CreateClient(apiContext);
                var response = await vkOrdClient.DeleteMediaAsync(externalId);

                if (response.IsSuccessStatusCode)
                {
                    await _cache.RemoveAsync($"media_{externalId}");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting media {externalId}");
                return false;
            }
        }

        #endregion
    }
}

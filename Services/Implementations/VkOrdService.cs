using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using VkOrdApiWrapper.Configuration;
using VkOrdApiWrapper.Models.Requests;
using VkOrdApiWrapper.Models.Responses;
using VkOrdApiWrapper.Models.VkOrd;
using VkOrdApiWrapper.Services.Interfaces;
using VkOrdApiWrapper.Models.DaData;
using System.Text.Json;

namespace VkOrdApiWrapper.Services.Implementations
{
    /// <summary>
    /// Сервис для работы с VK ОРД
    /// </summary>
    public class VkOrdService : IVkOrdService
    {
        private readonly IVkOrdApiClient _vkOrdClient;
        private readonly VkOrdConfiguration _config;
        private readonly ILogger<VkOrdService> _logger;
        private readonly IDistributedCache _cache;
        private readonly IDaDataService _daDataService;

        public VkOrdService(
            IVkOrdApiClient vkOrdClient,
            IOptions<VkOrdConfiguration> config,
            ILogger<VkOrdService> logger,
            IDistributedCache cache,
            IDaDataService daDataService)
        {
            _vkOrdClient = vkOrdClient;
            _config = config.Value;
            _logger = logger;
            _cache = cache;
            _daDataService = daDataService;
        }

        #region Контракты

        public async Task<CreateContractResponse> CreateOrUpdateContractAsync(CreateContractRequest request)
        {
            try
            {
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
                var authHeader = $"Bearer {_config.ApiToken}";

                _logger.LogInformation($"Creating/updating contract with external_id: {request.ExternalId}");
                
                var response = await _vkOrdClient.CreateOrUpdateContractAsync(
                    request.ExternalId, vkOrdContract, authHeader);

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

        public async Task<ContractResponse> GetContractAsync(string externalId)
        {
            try
            {
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

                var authHeader = $"Bearer {_config.ApiToken}";
                var response = await _vkOrdClient.GetContractAsync(externalId, authHeader);

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

        public async Task<CreateCreativeResponse> CreateCreativeAsync(CreateCreativeRequest request)
        {
            try
            {
                var vkOrdCreative = new VkOrdCreative()
                {
                    ExternalId = request.ExternalId,
                    Name = request.Text,
                    ContractExternalIds = request.ContractExternalIds,
                    Form = request.Format.ToString(),   
                    TargetUrls = request.ContentUrls,
                    Targeting = request.TargetAudience,
                    KKTYCodes = request.KKTYCodes,
                    PayType = VkCreativePayType.cpa.ToString(),
                    Texts = new List<string> { request.Text },
                    Flags = new List<string> { "native" }
                };
                var authHeader = $"Bearer {_config.ApiToken}";

                _logger.LogInformation($"Creating creative with external_id: {vkOrdCreative.ExternalId}");

                var response = await _vkOrdClient.CreateOrUpdateCreativeAsync(
                    vkOrdCreative.ExternalId, vkOrdCreative, authHeader);

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

        public async Task<CreateCreativeResponse> GetCreativeAsync(string externalId)
        {
            try
            {
                // Проверяем кэш
                var cachedJson = await _cache.GetStringAsync($"creative_{externalId}");
                if (!string.IsNullOrEmpty(cachedJson))
                {
                    var cached = JsonSerializer.Deserialize<CreateCreativeResponse>(cachedJson);
                    if (cached != null) return cached;
                }

                var authHeader = $"Bearer {_config.ApiToken}";
                var response = await _vkOrdClient.GetCreativeAsync(externalId, authHeader);

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

        public async Task<VkOrdStatusResponse> GetCreativeStatusAsync(string externalId)
        {
            try
            {
                var authHeader = $"Bearer {_config.ApiToken}";
                return await _vkOrdClient.GetCreativeStatusAsync(externalId, authHeader);
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

        public async Task<bool> DeleteCreativeAsync(string externalId)
        {
            try
            {
                var authHeader = $"Bearer {_config.ApiToken}";
                var response = await _vkOrdClient.DeleteCreativeAsync(externalId, authHeader);

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

        public async Task<List<CreateCreativeResponse>> CreateBulkCreativesAsync(List<CreateCreativeRequest> requests)
        {
            var results = new List<CreateCreativeResponse>();
            var semaphore = new SemaphoreSlim(_config.MaxConcurrentRequests, _config.MaxConcurrentRequests);
            var tasks = requests.Select(async request =>
            {
                await semaphore.WaitAsync();
                try
                {
                    return await CreateCreativeAsync(request);
                }
                finally
                {
                    semaphore.Release();
                }
            });

            results.AddRange(await Task.WhenAll(tasks));
            return results;
        }

        public async Task<bool> IsCreativeVerifiedAsync(string externalId, int maxWaitTimeMinutes = 120)
        {
            var startTime = DateTime.UtcNow;
            var maxWaitTime = TimeSpan.FromMinutes(maxWaitTimeMinutes);

            while (DateTime.UtcNow - startTime < maxWaitTime)
            {
                var status = await GetCreativeStatusAsync(externalId);

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

        public async Task<StatusResponse> CreateCounterpartyFromInnAsync(string inn, List<string> types)
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
                var authHeader = $"Bearer {_config.ApiToken}";
                try
                {
                    var response = await _vkOrdClient.CreateOrUpdatePersonAsync(externalId, person, authHeader);
                    if (response.IsSuccess)
                    {
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
    }
}

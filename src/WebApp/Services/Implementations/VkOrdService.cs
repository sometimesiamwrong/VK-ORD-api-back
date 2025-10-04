using System.Text.Json;
using Microsoft.Extensions.Options;
using VkOrdApi.Contract;
using VkOrdApi.Person;
using WebApp.Configuration;
using WebApp.Models.Requests;
using WebApp.Models.Responses;
using WebApp.Repositories.Interfaces;
using WebApp.Services.Interfaces;

namespace WebApp.Services.Implementations
{
    /// <summary>
    /// Сервис для работы с VK ОРД
    /// </summary>
    public class VkOrdService : IVkOrdService
    {
        private readonly IVkOrdContractRepository _contractRepository;
        private readonly IVkOrdCreativeRepository _creativeRepository;
        private readonly IVkOrdCounterpartyRepository _counterpartyRepository;
        private readonly IVkOrdMediaRepository _mediaRepository;
        private readonly ICacheRepository _cacheRepository;
        private readonly IDaDataService _daDataService;
        private readonly VkOrdConfiguration _config;
        private readonly ILogger<VkOrdService> _logger;

        public VkOrdService(
            IVkOrdContractRepository contractRepository,
            IVkOrdCreativeRepository creativeRepository,
            IVkOrdCounterpartyRepository counterpartyRepository,
            IVkOrdMediaRepository mediaRepository,
            ICacheRepository cacheRepository,
            IDaDataService daDataService,
            IOptions<VkOrdConfiguration> config,
            ILogger<VkOrdService> logger)
        {
            _contractRepository = contractRepository;
            _creativeRepository = creativeRepository;
            _counterpartyRepository = counterpartyRepository;
            _mediaRepository = mediaRepository;
            _cacheRepository = cacheRepository;
            _daDataService = daDataService;
            _config = config.Value;
            _logger = logger;
        }

        #region Контракты

        public Task CreateOrUpdateContractAsync(
            CreateContractRequest request,
            CancellationToken cancellationToken)
        {
            var vkOrdContract = new VkOrdCreateUpdateContractRequest
            {
                ClientExternalId = request.ClientExternalId,
                ContractorExternalId = request.ContractorExternalId,
                Type = VkOrdContractType.Service,
                Amount = request.PaySum.ToString(),
                Flags = new List<VkOrdContractFlag> { VkOrdContractFlag.VatIncluded },
                ActionType = VkOrdActionType.Other,
                SubjectType = VkOrdSubjectType.Distribution
            };

            return _contractRepository.CreateOrUpdateContractAsync(request.ExternalId, vkOrdContract, cancellationToken);
        }

        public async Task<ContractResponse> GetContractAsync(string externalId, long userId)
        {
            var environment = _config.Environment ?? "prod";
            var apiContext = await ResolveContextAsync(userId, environment);
            // Проверяем кэш
            var cachedFlag = await _cacheRepository.GetCachedContractFlagAsync(externalId);
            if (!string.IsNullOrEmpty(cachedFlag))
            {
                return ContractResponse.FromVkOrdResponse(
                    new VkOrdErrorResponse<VkOrdContract>
                    {
                        Data = new VkOrdContract() // В кэше хранится только результат создания
                    }, externalId);
            }

            return await _contractRepository.GetContractAsync(externalId, apiContext, default);
        }

        public async Task<CreateCreativeResponse> CreateCreativeAsync(CreateCreativeRequest request, long userId)
        {
            var apiContext = await ResolveContextAsync(userId, _config.Environment ?? "prod");
            var result = await _creativeRepository.CreateCreativeAsync(request, apiContext, default);

            if (result.Success)
            {
                await _cacheRepository.SetCachedCreativeAsync(request.ExternalId, result);
            }

            return result;
        }

        public async Task<CreateCreativeResponse> GetCreativeAsync(string externalId, long userId)
        {
            
            var apiContext = await ResolveContextAsync(userId, _config.Environment ?? "prod");
            // Проверяем кэш
            var cachedJson = await _cacheRepository.GetCachedCreativeAsync(externalId);
            if (!string.IsNullOrEmpty(cachedJson))
            {
                var cached = JsonSerializer.Deserialize<CreateCreativeResponse>(cachedJson);
                if (cached != null) return cached;
            }

            var result = await _creativeRepository.GetCreativeAsync(externalId, apiContext, default);

            if (result.Success && result is CreateCreativeResponse createResult)
            {
                await _cacheRepository.SetCachedCreativeAsync(externalId, createResult);
                return createResult;
            }
            else
            {
                return new CreateCreativeResponse
                {
                    ExternalId = externalId,
                    Success = false,
                    ErrorMessage = "Не удалось получить креатив"
                };
            }
        }

        public async Task<GetCreativesResponse> GetAllCreativesAsync(long userId, int? offset = null, int? limit = null)
        {
            try
            {
                var apiContext = await ResolveContextAsync(userId, _config.Environment ?? "prod");
                return await _creativeRepository.GetAllCreativesAsync(apiContext, offset, limit, default);
            }
            catch (Refit.ApiException refit)
            {
                _logger.LogError(refit, "API error while fetching creatives");
                return new GetCreativesResponse
                {
                    Success = false,
                    ErrorMessage = $"Ошибка получения списка креативов из VK ОРД: {refit.Message}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching creatives");
                return new GetCreativesResponse
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<CreativeResponse> GetCreativeByEridAsync(string erid, long userId)
        {
            try
            {
                var apiContext = await ResolveContextAsync(userId, _config.Environment ?? "prod");
                return await _creativeRepository.GetCreativeByEridAsync(erid, apiContext, default);
            }
            catch (Refit.ApiException refit)
            {
                _logger.LogError(refit, $"API error while fetching creative by ERID {erid}");
                return new CreativeResponse
                {
                    Success = false,
                    Message = $"Ошибка получения креатива по ERID из VK ОРД: {refit.Message}",
                    ExternalId = string.Empty
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching creative by ERID {erid}");
                return new CreativeResponse
                {
                    Success = false,
                    Message = ex.Message,
                    ExternalId = string.Empty
                };
            }
        }

        public async Task<VkOrdStatusResponse> GetCreativeStatusAsync(string externalId, long userId)
        {
            try
            {
                var apiContext = await ResolveContextAsync(userId, _config.Environment ?? "prod");
                return await _creativeRepository.GetCreativeStatusAsync(externalId, apiContext, default);
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

        public async Task<bool> DeleteCreativeAsync(string externalId, long userId)
        {
            try
            {
                var apiContext = await ResolveContextAsync(userId, _config.Environment ?? "prod");
                var result = await _creativeRepository.DeleteCreativeAsync(externalId, apiContext, default);

                if (result)
                {
                    await _cacheRepository.RemoveFromCacheAsync($"creative_{externalId}");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting creative {externalId}");
                return false;
            }
        }

        public async Task<List<CreateCreativeResponse>> CreateBulkCreativesAsync(List<CreateCreativeRequest> requests, long userId)
        {
            var apiContext = await ResolveContextAsync(userId, _config.Environment ?? "prod");
            return await _creativeRepository.CreateBulkCreativesAsync(requests, apiContext, default);
        }

        public async Task<bool> IsCreativeVerifiedAsync(string externalId, long userId, int maxWaitTimeMinutes = 120)
        {
            var startTime = DateTime.UtcNow;
            var maxWaitTime = TimeSpan.FromMinutes(maxWaitTimeMinutes);

            while (DateTime.UtcNow - startTime < maxWaitTime)
            {
                var status = await GetCreativeStatusAsync(externalId, userId);

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

        public async Task CreateCounterpartyFromInnAsync(string inn, List<VkOrdPersonRoles> types, CancellationToken cancellationToken = default)
        {
            /*var dadata = await _daDataService.FindPartyByInnAsync(inn, cancellationToken);
            if (dadata == null)
            {
                throw Bro("Контрагент по ИНН не найден в DaData");
            }
            */

            var result = await _counterpartyRepository.CreateCounterpartyFromInnAsync(inn, types, cancellationToken);

            if (result.Success) // Assuming StatusResponse has Success, adjust if needed
            {
                var externalId = dadata.Inn ?? inn;
                await _cacheRepository.SetCachedCounterpartyAsync(userId, _config.Environment ?? "prod", externalId, null); // person будет получен из репозитория
            }

            return result;
        }

        public async Task<GetCounterpartiesResponse> GetAllCounterpartiesAsync(long userId, int? offset = null, int? limit = null)
        {
            try
            {
                var apiContext = await ResolveContextAsync(userId, _config.Environment ?? "prod");
                return await _counterpartyRepository.GetAllCounterpartiesAsync(apiContext, offset, limit, default);
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

        public async Task<GetCounterpartyResponse> GetCounterpartyByIdAsync(string externalId, long userId)
        {
            try
            {
                // Проверяем кэш
                var cached = await _cacheRepository.GetCachedCounterpartyAsync(userId, _config.Environment ?? "prod", externalId);
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

                var apiContext = await ResolveContextAsync(userId, _config.Environment ?? "prod");
                var result = await _counterpartyRepository.GetCounterpartyByIdAsync(externalId, apiContext, default);

                if (result.Success && result.Person != null)
                {
                    await _cacheRepository.SetCachedCounterpartyAsync(userId, _config.Environment ?? "prod", externalId, result.Person);
                }

                return result;
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

        public async Task<UploadMediaResponse> UploadMediaAsync(UploadMediaRequest request, long userId)
        {
            try
            {
                var apiContext = await ResolveContextAsync(userId, _config.Environment ?? "prod");
                var result = await _mediaRepository.UploadMediaAsync(request, apiContext, default);

                if (result.Success)
                {
                    await _cacheRepository.SetCachedMediaAsync(request.ExternalId, result);
                }

                return result;
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

        public async Task<GetMediaResponse> GetMediaAsync(string externalId, long userId)
        {
            try
            {
                var apiContext = await ResolveContextAsync(userId, _config.Environment ?? "prod");
                // Проверяем кэш
                var cachedJson = await _cacheRepository.GetCachedMediaAsync(externalId);
                if (!string.IsNullOrEmpty(cachedJson))
                {
                    var cached = JsonSerializer.Deserialize<GetMediaResponse>(cachedJson);
                    if (cached != null)
                    {
                        return cached;
                    }
                }

                var result = await _mediaRepository.GetMediaAsync(externalId, apiContext, default);

                if (result.Success)
                {
                    var uploadResponse = new UploadMediaResponse
                    {
                        Success = result.Success,
                        ExternalId = result.ExternalId,
                        Erid = result.Media?.Erid ?? string.Empty,
                        Url = result.Media?.Url ?? string.Empty
                    };
                    await _cacheRepository.SetCachedMediaAsync(externalId, uploadResponse);
                }

                return result;
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

        public async Task<bool> DeleteMediaAsync(string externalId, long userId)
        {
            try
            {
                var apiContext = await ResolveContextAsync(userId, _config.Environment ?? "prod");
                var result = await _mediaRepository.DeleteMediaAsync(externalId, apiContext, default);

                if (result)
                {
                    await _cacheRepository.RemoveFromCacheAsync($"media_{externalId}");
                }

                return result;
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

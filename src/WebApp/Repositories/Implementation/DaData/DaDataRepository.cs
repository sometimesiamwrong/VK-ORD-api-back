using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using WebApp.Configuration;
using WebApp.Models.DaData;
using WebApp.Models.Responses;
using WebApp.Repositories.Interfaces.DaData;
using WebApp.Services.Interfaces;

namespace WebApp.Repositories.Implementation.DaData;

/// <summary>
/// Репозиторий для работы с DaData API
/// </summary>
public class DaDataRepository : IDaDataRepository
{
    private readonly IDaDataApiClient _client;
    private readonly DaDataConfiguration _config;
    private readonly ILogger<DaDataRepository> _logger;
    private readonly IDistributedCache _cache;

    public DaDataRepository(
        IDaDataApiClient client,
        IOptions<DaDataConfiguration> config,
        ILogger<DaDataRepository> logger,
        IDistributedCache cache)
    {
        _client = client;
        _config = config.Value;
        _logger = logger;
        _cache = cache;
    }

    public async Task<DaDataPartyShortResponse?> FindPartyByInnAsync(string inn, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(inn)) return null;

        try
        {
            var cacheKey = $"dadata:party:{inn}";
            var cachedJson = await _cache.GetStringAsync(cacheKey, cancellationToken);
            if (!string.IsNullOrEmpty(cachedJson))
            {
                return JsonSerializer.Deserialize<DaDataPartyShortResponse>(cachedJson);
            }

            var response = await _client.FindByIdPartyAsync(new DaDataFindByIdRequest { Query = inn });
            var item = response.Suggestions.FirstOrDefault();
            if (item == null || item.Data == null) return null;

            var result = new DaDataPartyShortResponse
            {
                Value = item.Value,
                Status = item.Data.State?.Status,
                Opf = item.Data.Opf == null ? null : new DaDataOpfShort
                {
                    Type = item.Data.Opf.Type,
                    Code = item.Data.Opf.Code,
                    Full = item.Data.Opf.Full,
                    Short = item.Data.Opf.Short
                },
                Name = item.Data.Name == null ? null : new DaDataNameShort
                {
                    FullWithOpf = item.Data.Name.FullWithOpf,
                    ShortWithOpf = item.Data.Name.ShortWithOpf,
                    Latin = item.Data.Name.Latin,
                    Full = item.Data.Name.Full,
                    Short = item.Data.Name.Short
                },
                Inn = item.Data.Inn,
                Ogrn = item.Data.Ogrn,
                Okpo = item.Data.Okpo,
                Okato = item.Data.Okato,
                Oktmo = item.Data.Oktmo,
                Okogu = item.Data.Okogu,
                Okfs = item.Data.Okfs,
                Okved = item.Data.Okved,
                Phone = item.Data.Phones?.FirstOrDefault(),
                Kpp = item.Data.Kpp,
                Email = item.Data.Emails?.FirstOrDefault(),
                Fio = item.Data.Fio == null ? null : new DaDataFioShort
                {
                    Surname = item.Data.Fio.Surname,
                    Name = item.Data.Fio.Name,
                    Patronymic = item.Data.Fio.Patronymic
                },
                Type = item.Data.Type
            };

            var json = JsonSerializer.Serialize(result);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
            };
            await _cache.SetStringAsync(cacheKey, json, options, cancellationToken);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DaData lookup failed for INN {Inn}", inn);
            return null;
        }
    }
}

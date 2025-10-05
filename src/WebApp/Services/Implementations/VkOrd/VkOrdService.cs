using WebApp.Repositories.Interfaces;
using WebApp.Repositories.Interfaces.VkOrd.Contract;
using WebApp.Repositories.Interfaces.VkOrd.Counterparty;
using WebApp.Repositories.Interfaces.VkOrd.Creative;
using WebApp.Services.Interfaces;

namespace WebApp.Services.Implementations.VkOrd
{
    /// <summary>
    /// Сервис для работы с VK ОРД
    /// </summary>
    public partial class VkOrdService : IVkOrdService
    {
        private readonly ICreateOrUpdateContractRepository _createContractRepository;
        private readonly IGetContractRepository _getContractRepository;
        private readonly IGetPageContractRepository _getPageContractRepository;

        private readonly ICreateCounterpartyRepository _createCounterpartyRepository;
        private readonly IGetCounterpartyByIdRepository _getCounterpartyByIdRepository;
        private readonly IGetPageCounterpartiesRepository _getPageCounterpartiesRepository; 

        private readonly ICreateCreativeRepository _createCreativeRepository;
        private readonly IGetCreativeRepository _getCreativeRepository;
        private readonly IGetAllCreativesRepository _getAllCreativesRepository;
        private readonly IGetCreativeByEridRepository _getCreativeByEridRepository;
        
        private readonly IVkOrdMediaRepository _mediaRepository;

        private readonly IDaDataService _daDataService;
        private readonly ILogger<VkOrdService> _logger;

        public VkOrdService(
            ICreateOrUpdateContractRepository createContractRepository,
            ICreateCounterpartyRepository createCounterpartyRepository,
            IGetContractRepository getContractRepository,
            IGetPageCounterpartiesRepository getPageCounterpartiesRepository,
            ICreateCreativeRepository createCreativeRepository,
            IGetCreativeRepository getCreativeRepository,
            IGetAllCreativesRepository getAllCreativesRepository,
            IGetCreativeByEridRepository getCreativeByEridRepository,
            IGetCounterpartyByIdRepository getCounterpartyByIdRepository,
            IVkOrdMediaRepository mediaRepository,
            IDaDataService daDataService,
            ILogger<VkOrdService> logger, IGetPageContractRepository getPageContractRepository)
        {
            _createContractRepository = createContractRepository;
            _createCounterpartyRepository = createCounterpartyRepository;
            _getContractRepository = getContractRepository;
            _getPageCounterpartiesRepository = getPageCounterpartiesRepository;
            _createCreativeRepository = createCreativeRepository;
            _getCreativeRepository = getCreativeRepository;
            _getAllCreativesRepository = getAllCreativesRepository;
            _getCreativeByEridRepository = getCreativeByEridRepository;
            _getCounterpartyByIdRepository = getCounterpartyByIdRepository;
            _mediaRepository = mediaRepository;
            _daDataService = daDataService;
            _logger = logger;
            _getPageContractRepository = getPageContractRepository;
        }
    }
}

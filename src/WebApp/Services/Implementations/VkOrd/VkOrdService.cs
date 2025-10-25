using WebApp.Repositories.Interfaces;
using WebApp.Repositories.Interfaces.VkOrd.Contract;
using WebApp.Repositories.Interfaces.VkOrd.Counterparty;
using WebApp.Repositories.Interfaces.VkOrd.Creative;
using WebApp.Repositories.Interfaces.VkOrd.Invoice;
using WebApp.Repositories.Interfaces.VkOrd.Statistics;
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
        private readonly IGetContractsByCounterpartyRepository _getContractsByCounterpartyRepository;
        private readonly IGetContractDetailsRepository _getContractDetailsRepository;

        private readonly ICreateCounterpartyRepository _createCounterpartyRepository;
        private readonly IGetCounterpartyByIdRepository _getCounterpartyByIdRepository;
        private readonly IGetPageCounterpartiesRepository _getPageCounterpartiesRepository; 

        private readonly ICreateCreativeRepository _createCreativeRepository;
        private readonly IGetCreativeRepository _getCreativeRepository;
        private readonly IGetPageCreativesRepository _getPageCreativesRepository;
        private readonly IGetCreativeByEridRepository _getCreativeByEridRepository;
        
        private readonly IVkOrdMediaRepository _mediaRepository;

        private readonly ICreateOrUpdateInvoiceRepository _createOrUpdateInvoiceRepository;
        private readonly IGetInvoiceRepository _getInvoiceRepository;
        private readonly IGetPageInvoiceRepository _getPageInvoiceRepository;
        private readonly IDeleteInvoiceRepository _deleteInvoiceRepository;
        private readonly IAddContractsToInvoiceRepository _addContractsToInvoiceRepository;
        private readonly IDeleteContractsFromInvoiceRepository _deleteContractsFromInvoiceRepository;
        private readonly ISendInvoiceToErirRepository _sendInvoiceToErirRepository;
        private readonly ICreateOrUpdateInvoiceHeaderRepository _createOrUpdateInvoiceHeaderRepository;
        private readonly IGetInvoiceHeaderRepository _getInvoiceHeaderRepository;

        private readonly ICreateOrUpdateStatisticsRepository _createOrUpdateStatisticsRepository;
        private readonly IGetStatisticsListRepository _getStatisticsListRepository;
        private readonly IDeleteStatisticsRepository _deleteStatisticsRepository;

        private readonly IDaDataService _daDataService;
        private readonly ILogger<VkOrdService> _logger;

        public VkOrdService(
            ICreateOrUpdateContractRepository createContractRepository,
            ICreateCounterpartyRepository createCounterpartyRepository,
            IGetContractRepository getContractRepository,
            IGetPageCounterpartiesRepository getPageCounterpartiesRepository,
            ICreateCreativeRepository createCreativeRepository,
            IGetCreativeRepository getCreativeRepository,
            IGetPageCreativesRepository getAllCreativesRepository,
            IGetCreativeByEridRepository getCreativeByEridRepository,
            IGetCounterpartyByIdRepository getCounterpartyByIdRepository,
            IVkOrdMediaRepository mediaRepository,
            ICreateOrUpdateInvoiceRepository createOrUpdateInvoiceRepository,
            IGetInvoiceRepository getInvoiceRepository,
            IGetPageInvoiceRepository getPageInvoiceRepository,
            IDeleteInvoiceRepository deleteInvoiceRepository,
            IAddContractsToInvoiceRepository addContractsToInvoiceRepository,
            IDeleteContractsFromInvoiceRepository deleteContractsFromInvoiceRepository,
            ISendInvoiceToErirRepository sendInvoiceToErirRepository,
            ICreateOrUpdateInvoiceHeaderRepository createOrUpdateInvoiceHeaderRepository,
            IGetInvoiceHeaderRepository getInvoiceHeaderRepository,
            ICreateOrUpdateStatisticsRepository createOrUpdateStatisticsRepository,
            IGetStatisticsListRepository getStatisticsListRepository,
            IDeleteStatisticsRepository deleteStatisticsRepository,
            IDaDataService daDataService,
            ILogger<VkOrdService> logger,
            IGetPageContractRepository getPageContractRepository,
            IGetContractsByCounterpartyRepository getContractsByCounterpartyRepository,
            IGetContractDetailsRepository getContractDetailsRepository)
        {
            _createContractRepository = createContractRepository;
            _createCounterpartyRepository = createCounterpartyRepository;
            _getContractRepository = getContractRepository;
            _getContractsByCounterpartyRepository = getContractsByCounterpartyRepository;
            _getPageCounterpartiesRepository = getPageCounterpartiesRepository;
            _createCreativeRepository = createCreativeRepository;
            _getCreativeRepository = getCreativeRepository;
            _getPageCreativesRepository = getAllCreativesRepository;
            _getCreativeByEridRepository = getCreativeByEridRepository;
            _getCounterpartyByIdRepository = getCounterpartyByIdRepository;
            _mediaRepository = mediaRepository;
            _createOrUpdateInvoiceRepository = createOrUpdateInvoiceRepository;
            _getInvoiceRepository = getInvoiceRepository;
            _getPageInvoiceRepository = getPageInvoiceRepository;
            _deleteInvoiceRepository = deleteInvoiceRepository;
            _addContractsToInvoiceRepository = addContractsToInvoiceRepository;
            _deleteContractsFromInvoiceRepository = deleteContractsFromInvoiceRepository;
            _sendInvoiceToErirRepository = sendInvoiceToErirRepository;
            _createOrUpdateInvoiceHeaderRepository = createOrUpdateInvoiceHeaderRepository;
            _getInvoiceHeaderRepository = getInvoiceHeaderRepository;
            _createOrUpdateStatisticsRepository = createOrUpdateStatisticsRepository;
            _getStatisticsListRepository = getStatisticsListRepository;
            _deleteStatisticsRepository = deleteStatisticsRepository;
            _daDataService = daDataService;
            _logger = logger;
            _getPageContractRepository = getPageContractRepository;
            _getContractDetailsRepository = getContractDetailsRepository;
        }
    }
}

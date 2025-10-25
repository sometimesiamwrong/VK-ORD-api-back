using System.Text.Json;
using Domain.BrokenRules;
using Domain.Entities.VkOrd;
using Domain.Extensions;
using WebApp.Repositories.Interfaces.VkOrd.Contract;
using WebApp.Repositories.Interfaces.VkOrd.Counterparty;
using WebApp.Repositories.Interfaces.VkOrd.Creative;

namespace WebApp.Services.Implementations.FlowTemplateServices
{
    public class WizardFlowTemplateService : IWizardFlowTemplateService
    {
        private readonly IGetCounterpartyByIdRepository _getCounterpartyByIdRepository;
        private readonly IGetContractRepository _getContractRepository;
        private readonly IGetCreativeRepository _getCreativeRepository;
        private readonly JsonSerializerOptions _options;

        public WizardFlowTemplateService(
            IGetCounterpartyByIdRepository getCounterpartyByIdRepository,
            IGetContractRepository getContractRepository,
            IGetCreativeRepository getCreativeRepository,
            JsonSerializerOptions options)
        {
            _getCounterpartyByIdRepository = getCounterpartyByIdRepository;
            _getContractRepository = getContractRepository;
            _getCreativeRepository = getCreativeRepository;
            _options = options;
        }

        public async Task<object> GetData(string value, CancellationToken cancellationToken)
        {
            var data = JsonSerializer.Deserialize<WizardFlowTemplateData>(value, _options);
            if (data == null)
            {
                throw BrokenRuleCodes.FlowTemplateProcessingError.AsExn();
            }

            var contract = await _getContractRepository.Get(data.ContractExternalId, cancellationToken);
            var contractor = await _getCounterpartyByIdRepository.Get(data.ContractorExternalId, cancellationToken);
            var client = await _getCounterpartyByIdRepository.Get(data.ClientExternalId, cancellationToken);
            var creative = await _getCreativeRepository.Get(data.CreativeExternalId, cancellationToken);

            return new WizardFlowTemplateResponse
            {
                Contract = contract,
                Contractor = contractor,
                Client = client,
                Creative = creative,
            };
        }

        public async Task CheckRequest(string value, CancellationToken cancellationToken)
        {
            var data = JsonSerializer.Deserialize<WizardFlowTemplateData>(value, _options);
            if (data == null)
            {
                throw BrokenRuleCodes.FlowTemplateProcessingError.AsExn();
            }

            var contract = await _getContractRepository.Get(data.ContractExternalId, cancellationToken);
            var contractor = await _getCounterpartyByIdRepository.Get(data.ContractorExternalId, cancellationToken);
            var client = await _getCounterpartyByIdRepository.Get(data.ClientExternalId, cancellationToken);
            var creative = await _getCreativeRepository.Get(data.CreativeExternalId, cancellationToken);

            if(contract == null)
            {
                throw BrokenRuleCodes.ContractNotFound.AsExn();
            }
            if(contractor == null || client == null)
            {
                throw BrokenRuleCodes.DataIsEmpty.AsExn();
            }
            if(creative == null)
            {
                throw BrokenRuleCodes.CreativeNotFound.AsExn();
            }
        }
    }

    public class WizardFlowTemplateData
    {
        public string ContractExternalId { get; set; }
        public string ContractorExternalId { get; set; }
        public string ClientExternalId { get; set; }
        public string CreativeExternalId { get; set; }
    }

    public class WizardFlowTemplateResponse
    {
        public VkOrdContract Contract { get; set; }
        public VkOrdCounterparty Contractor { get; set; }
        public VkOrdCounterparty Client { get; set; }
        public VkOrdCreative Creative { get; set; }
    }
}
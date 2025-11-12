using Domain.Repositories.Implementations;
using Domain.Repositories.Implementations.ApiCredentials;
using Domain.Repositories.Implementations.DaData;
using Domain.Repositories.Implementations.DatabaseScripts;
using Domain.Repositories.Implementations.RefreshTokens;
using Domain.Repositories.Implementations.Users;
using Domain.Repositories.Implementations.VkOrd.Contract;
using Domain.Repositories.Implementations.VkOrd.Counterparty;
using Domain.Repositories.Implementations.VkOrd.Creative;
using Domain.Repositories.Implementations.VkOrd.ErirStatus;
using Domain.Repositories.Implementations.VkOrd.Invoice;
using Domain.Repositories.Implementations.VkOrd.Statistics;
using Domain.Repositories.Interfaces;
using Domain.Repositories.Interfaces.ApiCredentials;
using Domain.Repositories.Interfaces.DaData;
using Domain.Repositories.Interfaces.DatabaseScripts;
using Domain.Repositories.Interfaces.RefreshTokens;
using Domain.Repositories.Interfaces.Users;
using Domain.Repositories.Interfaces.VkOrd.Contract;
using Domain.Repositories.Interfaces.VkOrd.Counterparty;
using Domain.Repositories.Interfaces.VkOrd.Creative;
using Domain.Repositories.Interfaces.VkOrd.ErirStatus;
using Domain.Repositories.Interfaces.VkOrd.Invoice;
using Domain.Repositories.Interfaces.VkOrd.Statistics;
using Domain.Services.Implementations;
using Domain.Services.Implementations.VkOrd;
using Domain.Services.Interfaces;
using WebApp.Security;
using Scrutor;

namespace WebApp.Startup
{
    /// <summary>
    /// Класс для регистрации репозиториев в DI контейнере
    /// </summary>
    public static class RepositoryStartup
    {
        /// <summary>
        /// Регистрирует все репозитории в DI
        /// </summary>
        public static void AddRepositories(this IServiceCollection services)
        {
            // Основные репозитории
            services.AddScoped<IDaDataRepository, DaDataRepository>();
            services.AddScoped<IVkOrdMediaRepository, VkOrdMediaRepository>();
            services.AddScoped<ISecretProtector, SecretProtector>();
            services.AddScoped<IGetApiCredentialByGuidRepository, GetApiCredentialByGuidRepository>();
            services.Decorate<IGetApiCredentialByGuidRepository, GetApiCredentialByGuidCacheRepository>();

            // Users репозитории
            services.AddScoped<ISaveUserRepository, SaveUserRepository>();
            services.AddScoped<IGetUserByIdRepository, GetUserByIdRepository>();
            services.AddScoped<IGetUserByNameRepository, GetUserByNameRepository>();
            services.AddScoped<IGetUsersListRepository, GetUsersListRepository>();
            services.AddScoped<IDeleteUserRepository, DeleteUserRepository>();

            // ApiCredentials репозитории
            services.AddScoped<ISaveApiCredentialRepository, SaveApiCredentialRepository>();
            services.AddScoped<IGetApiCredentialByIdRepository, GetApiCredentialByIdRepository>();
            services.AddScoped<IGetApiCredentialsListRepository, GetApiCredentialsListRepository>();
            services.AddScoped<IDeleteApiCredentialRepository, DeleteApiCredentialRepository>();

            // RefreshTokens репозитории
            services.AddScoped<ISaveRefreshTokenRepository, SaveRefreshTokenRepository>();
            services.AddScoped<IGetRefreshTokenByIdRepository, GetRefreshTokenByIdRepository>();
            services.AddScoped<IGetRefreshTokenByHashRepository, GetRefreshTokenByHashRepository>();
            services.AddScoped<IGetRefreshTokensListRepository, GetRefreshTokensListRepository>();
            services.AddScoped<IDeleteRefreshTokenRepository, DeleteRefreshTokenRepository>();

            // DatabaseScripts репозитории
            services.AddScoped<ISaveDatabaseScriptRepository, SaveDatabaseScriptRepository>();
            services.AddScoped<IGetDatabaseScriptByIdRepository, GetDatabaseScriptByIdRepository>();
            services.AddScoped<IGetDatabaseScriptsListRepository, GetDatabaseScriptsListRepository>();
            services.AddScoped<IDeleteDatabaseScriptRepository, DeleteDatabaseScriptRepository>();

            // VK ORD репозитории
            services.AddScoped<ICreateOrUpdateContractRepository, CreateOrUpdateContractRepository>();
            services.AddScoped<IGetContractRepository, GetContractRepository>();
            services.AddScoped<IGetPageContractRepository, GetPageContractRepository>();
            services.AddScoped<IGetContractsByCounterpartyRepository, GetContractsByCounterpartyRepository>();
            services.AddScoped<IGetContractDetailsRepository, GetContractDetailsRepository>();
            services.AddScoped<ICreateCounterpartyRepository, CreateCounterpartyRepository>();
            services.AddScoped<IGetCounterpartyByIdRepository, GetCounterpartyByIdRepository>();
            services.AddScoped<IGetPageCounterpartiesRepository, GetPageCounterpartiesRepository>();
            services.AddScoped<IGetAllCounterpartyRepository, GetAllCounterpartyRepository>();
            
            services.AddScoped<ICreateCreativeRepository, CreateCreativeRepository>();
            services.AddScoped<IGetCreativeRepository, GetCreativeRepository>();
            services.AddScoped<IGetPageCreativesRepository, GetPageCreativesRepository>();
            services.AddScoped<IGetCreativeByEridRepository, GetCreativeByEridRepository>();

            services.AddScoped<ICreateOrUpdateInvoiceRepository, CreateOrUpdateInvoiceRepository>();
            services.AddScoped<IGetInvoiceRepository, GetInvoiceRepository>();
            services.AddScoped<IGetPageInvoiceRepository, GetPageInvoiceRepository>();
            services.AddScoped<IDeleteInvoiceRepository, DeleteInvoiceRepository>();
            services.AddScoped<IAddContractsToInvoiceRepository, AddContractsToInvoiceRepository>();
            services.AddScoped<IDeleteContractsFromInvoiceRepository, DeleteContractsFromInvoiceRepository>();
            services.AddScoped<ISendInvoiceToErirRepository, SendInvoiceToErirRepository>();
            services.AddScoped<ICreateOrUpdateInvoiceHeaderRepository, CreateOrUpdateInvoiceHeaderRepository>();
            services.AddScoped<IGetInvoiceHeaderRepository, GetInvoiceHeaderRepository>();

            services.AddScoped<ICreateOrUpdateStatisticsRepository, CreateOrUpdateStatisticsRepository>();
            services.AddScoped<IGetStatisticsListRepository, GetStatisticsListRepository>();
            services.AddScoped<IGetStatisticsByIdRepository, GetStatisticsByIdRepository>();
            services.AddScoped<IDeleteStatisticsRepository, DeleteStatisticsRepository>();

            services.AddScoped<IGetAllLogicalAccountsRepository, GetAllLogicalAccountsRepository>();
            services.AddScoped<IVkOrdErirStatusRepository, VkOrdErirStatusRepository>();

            // VK ORD сервисы
            services.AddScoped<IVkOrdService, VkOrdService>();
            services.AddScoped<IVkOrdApiClientFactory, VkOrdApiClientFactory>();
            services.AddScoped<IErirStatusSyncService, ErirStatusSyncService>();
            
            // VK ORD Data Service
            services.AddScoped(typeof(IVkOrdDataService<,>), typeof(VkOrdDataService<,>));
            
            // ApiCredentials репозитории
            services.AddScoped<IGetApiCredentialByIdRepository, GetApiCredentialByIdRepository>();
        }
    }
}

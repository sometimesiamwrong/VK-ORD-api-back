using WebApp.Repositories.Interfaces;
using WebApp.Repositories.Interfaces.ApiCredentials;
using WebApp.Repositories.Interfaces.DaData;
using WebApp.Repositories.Interfaces.DatabaseScripts;
using WebApp.Repositories.Interfaces.RefreshTokens;
using WebApp.Repositories.Interfaces.Users;
using WebApp.Repositories.Interfaces.VkOrd.Contract;
using WebApp.Repositories.Interfaces.VkOrd.Counterparty;
using WebApp.Repositories.Interfaces.VkOrd.Creative;
using WebApp.Security;
using WebApp.Services.Implementations;
using WebApp.Services.Interfaces;
using Scrutor;
using WebApp.Repositories.Implementations;
using WebApp.Repositories.Implementations.ApiCredentials;
using WebApp.Repositories.Implementations.DaData;
using WebApp.Repositories.Implementations.DatabaseScripts;
using WebApp.Repositories.Implementations.RefreshTokens;
using WebApp.Repositories.Implementations.Users;
using WebApp.Repositories.Implementations.VkOrd.Contract;
using WebApp.Repositories.Implementations.VkOrd.Counterparty;
using WebApp.Repositories.Implementations.VkOrd.Creative;
using WebApp.Services.Implementations.VkOrd;

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
            
            services.AddScoped<ICreateCounterpartyRepository, CreateCounterpartyRepository>();
            services.AddScoped<IGetCounterpartyByIdRepository, GetCounterpartyByIdRepository>();
            services.AddScoped<IGetPageCounterpartiesRepository, GetPageCounterpartiesRepository>();
            
            services.AddScoped<ICreateCreativeRepository, CreateCreativeRepository>();
            services.AddScoped<IGetCreativeRepository, GetCreativeRepository>();
            services.AddScoped<IGetAllCreativesRepository, GetAllCreativesRepository>();
            services.AddScoped<IGetCreativeByEridRepository, GetCreativeByEridRepository>();


            // VK ORD сервисы
            services.AddScoped<IVkOrdService, VkOrdService>();
            services.AddScoped<IVkOrdApiClientFactory, VkOrdApiClientFactory>();
            
            // VK ORD Data Service
            services.AddScoped(typeof(IVkOrdDataService<,>), typeof(VkOrdDataService<,>));
            
            // ApiCredentials репозитории
            services.AddScoped<IGetApiCredentialByIdRepository, GetApiCredentialByIdRepository>();
        }
    }
}

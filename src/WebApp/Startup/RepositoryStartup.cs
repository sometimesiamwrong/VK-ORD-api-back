using VkOrdApi.Services.Implementations;
using VkOrdApi.Services.Interfaces;
using WebApp.Repositories.Implementation;
using WebApp.Repositories.Implementation.ApiCredentials;
using WebApp.Repositories.Implementation.DaData;
using WebApp.Repositories.Implementation.DatabaseScripts;
using WebApp.Repositories.Implementation.RefreshTokens;
using WebApp.Repositories.Implementation.Users;
using WebApp.Repositories.Interfaces;
using WebApp.Repositories.Interfaces.ApiCredentials;
using WebApp.Repositories.Interfaces.DaData;
using WebApp.Repositories.Interfaces.DatabaseScripts;
using WebApp.Repositories.Interfaces.RefreshTokens;
using WebApp.Repositories.Interfaces.Users;
using WebApp.Security;
using WebApp.Services.Implementations;
using WebApp.Services.Interfaces;
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
            services.AddScoped<IVkOrdContractRepository, VkOrdContractRepository>();
            services.AddScoped<IVkOrdCreativeRepository, VkOrdCreativeRepository>();
            services.AddScoped<IVkOrdCounterpartyRepository, VkOrdCounterpartyRepository>();
            services.AddScoped<IVkOrdMediaRepository, VkOrdMediaRepository>();
            services.AddScoped<ICacheRepository, CacheRepository>();
            services.AddScoped<ISecretProtector, SecretProtector>();
            services.AddScoped<IGetApiCredentialByGuidRepository, GetApiCredentialByGuidRepository>();
            services.Decorate<IGetApiCredentialByGuidRepository, GetApiCredentialByGuidCacheRepository>();
            services.AddScoped<IVkApiContextRepository, VkApiContextRepository>();

            // Users репозитории
            services.AddScoped<ISaveUserRepository, SaveUserRepository>();
            services.AddScoped<IGetUserByIdRepository, GetUserByIdRepository>();
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
            services.AddScoped<IGetRefreshTokensListRepository, GetRefreshTokensListRepository>();
            services.AddScoped<IDeleteRefreshTokenRepository, DeleteRefreshTokenRepository>();

            // DatabaseScripts репозитории
            services.AddScoped<ISaveDatabaseScriptRepository, SaveDatabaseScriptRepository>();
            services.AddScoped<IGetDatabaseScriptByIdRepository, GetDatabaseScriptByIdRepository>();
            services.AddScoped<IGetDatabaseScriptsListRepository, GetDatabaseScriptsListRepository>();
            services.AddScoped<IDeleteDatabaseScriptRepository, DeleteDatabaseScriptRepository>();

            // VK ORD сервисы
            services.AddScoped<IVkOrdService, VkOrdService>();
            services.AddScoped<IVkOrdApiClientFactory, VkOrdApiClientFactory>();
        }
    }
}

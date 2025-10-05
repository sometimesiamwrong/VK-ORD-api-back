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

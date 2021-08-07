using AuthenticationService.API.Data;
using Data;
using Domain.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IoC
{
    public static class DependencyContainer
    {
        private static void RegisterServices(IServiceCollection services, IConfiguration config)
        {
            // Data layer
            var connectionString = config.GetSection("ConnectionStrings").GetValue("DefaultConnection", "");
            services.AddDbContext<PharmacyDatabaseContext>(options => { options.UseNpgsql(connectionString); }
            );
            services.AddTransient<IAuthRepository, AuthRepository>();
            services.AddTransient<ICryptoService, CryptoService>();
            services.AddTransient<ITokenService, TokenService>();
        }

        /// <summary>
        ///     It creates services if not provided
        /// </summary>
        /// <param name="configBasePath">appsettings.json </param>
        /// <returns>Collections of services</returns>
        public static IServiceCollection AddConfigAndRegisterServices(string configBasePath,
            IServiceCollection services,
            string[]? args = null)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(configBasePath)
                .AddJsonFile("appsettings.json", false, true);
            if (args != null) builder.AddCommandLine(args);
            var config = builder.Build();
            RegisterServices(services, config);
            return services;
        }
    }
}
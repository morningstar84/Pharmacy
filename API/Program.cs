using System;
using Data;
using Domain.Models;
using Domain.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using NLog.Web;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Info("[WEB API]: started");
                var builder = CreateHostBuilder(args).Build();
                using (var scope = builder.Services.CreateScope())
                {
                    var service = scope.ServiceProvider;
                    try
                    {
                        var dataContext = service.GetRequiredService<PharmacyDatabaseContext>();
                        dataContext.Database.Migrate();
                        var cryptoService = service.GetRequiredService<ICryptoService>();
                        UserSeed.Seed(dataContext, cryptoService);
                    }
                    catch (Exception e)
                    {
                        logger.Error(e, "Failed to load users inside DB");
                    }
                }

                builder.Run();
            }
            catch (Exception e)
            {
                logger.Error(e, "Stopped program because of exception");
                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }
    }
}
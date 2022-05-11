using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProgramEngineering.DB;
using System;

namespace ProgramEngineering
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var context = services.GetRequiredService<ApiDbContext>();
                var logger = services.GetRequiredService<ILogger<Program>>();

                try
                {
                    DbInitializer.Initialize(context);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred creating the DB.");
                }
            }

            host.Run();
        }
    

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

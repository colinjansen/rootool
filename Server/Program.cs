using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Replication.RooTool.Data;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Replication.RooTool
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json")
               .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true)
               .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            try
            {
                var host = CreateHostBuilder(args, configuration).Build();
                MigrateDatabase(host).GetAwaiter().GetResult();
                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal("in main", ex);
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args, IConfiguration configuration)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseSerilog();
        }

        private static async Task MigrateDatabase(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var appContext = scope.ServiceProvider.GetRequiredService<RooToolDbContext>();
            await appContext.Database.MigrateAsync().ConfigureAwait(false);
        }
    }
}

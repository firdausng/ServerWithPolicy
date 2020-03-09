using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServerWithPolicy.Entities.Main;
using ServerWithPolicy.Infra.Data;
using ServerWithPolicy.Seed;

namespace ServerWithPolicy
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.GetService<IServiceScopeFactory>().CreateScope())
            {
                var services = scope.ServiceProvider;
                var seedDb = services.GetService<SeedDb>();
                seedDb.SeedTenants();
                seedDb.SeedUsers();
                seedDb.SeedPolicy();
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

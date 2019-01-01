using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GiveNTake.Model;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GiveNTake
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();
            using(var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetService<GiveNTakeContext>();
                var roleManager = services.GetService<RoleManager<IdentityRole>>();

                try
                {
                    context.Database.Migrate();
                    context.SeedData();
                    context.SeedRolesAsync(roleManager).Wait();
                }
                catch (Exception)
                {

                }
            }
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseApplicationInsights()
                .ConfigureLogging(cfg =>
                {
                    cfg.AddConsole();
                    cfg.AddDebug();
                })
                .UseStartup<Startup>();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<AppDbContext>();
            SeedData(context);
        }

        private static void SeedData(AppDbContext context)
        {
            if (!context.Platforms.Any())
            {
                Console.WriteLine("Seeding data...");
                context.Platforms.AddRange(
                    new Platform
                    {
                        Name="Dot Net",
                        Publisher="Microsoft",
                        Cost="Free"
                    },
                    new Platform
                    {
                        Name="SQL Server Express",
                        Publisher="Microsoft",
                        Cost="Free"
                    },
                    new Platform
                    {
                        Name="Kubernetes",
                        Publisher="Cloud Native Computing Foundation",
                        Cost="Free"
                    }
                );
                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("We already have data");
            }
        }
    }
}
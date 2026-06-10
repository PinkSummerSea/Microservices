using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandService.Models;
using CommandService.SyncDataServices.Grpc;

namespace CommandService.Data
{
    public static class PrepDb
    {
        // cannot use dependency injection because this is a static class.
        // in order to get services, we need to create scope 
        public static void PrepPopulation(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;
            var commandRepo = services.GetRequiredService<ICommandRepo>();
            var grpcClient = services.GetRequiredService<IPlatformDataClient>();
            var platforms = grpcClient.ReturnAllPlatforms();
            SeedData(commandRepo, platforms);
        }

        private static void SeedData(ICommandRepo commandRepo, IEnumerable<Platform> platforms)
        {
            Console.WriteLine("seeding new platforms...");
            foreach(var plat in platforms)
            {
                if (!commandRepo.ExternalPlatformExists(plat.ExternalId))
                {
                    commandRepo.CreatePlatform(plat);   
                }
                commandRepo.SaveChanges();
            }
        }
    }
}
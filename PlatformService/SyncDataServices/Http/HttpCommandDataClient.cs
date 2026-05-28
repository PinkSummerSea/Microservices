using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PlatformService.Dtos;

namespace PlatformService.SyncDataServices.Http
{
    public class HttpCommandDataClient : ICommandDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public HttpCommandDataClient(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }
        public async Task SendPlatformToCommandAsync(PlatformReadDto plat)
        {
            var httpContent = new StringContent(
                JsonSerializer.Serialize(plat),
                Encoding.UTF8,
                "application/json");
            
            var response = await _httpClient.PostAsync(_config["CommandService"],httpContent);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("sync post to command service was OK");
            }
            else
            {
                Console.WriteLine("sync post to command service was not OK");
            }
        }
    }
}
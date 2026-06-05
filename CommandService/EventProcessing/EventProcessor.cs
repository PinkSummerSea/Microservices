using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;

namespace CommandService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            // singleton service cannot constructor inject scoped service
            // in order to access command repo, we need to mannually create a scope using scope factory
            _scopeFactory = scopeFactory;
            _mapper = mapper;
        }
        public void ProcessEvent(string message)
        {
            var eventType = DetermineEventType(message);
            switch (eventType)
            {
                case EventType.PlatformPublished:
                    AddPlatform(message); 
                    break;
                default:
                    break;
            }
        }

        private EventType DetermineEventType(string notificationMessage)
        {
            Console.WriteLine("determining event...");
            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage) ?? throw new Exception("cannot deserialize event type");
            switch (eventType.Event)
            {
                case "Platform_Published":
                    Console.WriteLine("platform published event detected");
                    return EventType.PlatformPublished;
                default:
                    Console.WriteLine("could not determine event type");
                    return EventType.Undetermined;
            }
        }
        private void AddPlatform(string platformPublishMessage)
        {
            using var scope = _scopeFactory.CreateScope();
            var commandRepo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();
            var platformPublishDto = JsonSerializer.Deserialize<PlatformPublishDto>(platformPublishMessage);
            try
            {
                var platform = _mapper.Map<Platform>(platformPublishDto);
                if (!commandRepo.ExternalPlatformExists(platform.ExternalId))
                {
                    commandRepo.CreatePlatform(platform);
                    commandRepo.SaveChanges();
                }
                else
                {
                    Console.WriteLine("platform already exists in db");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"cannot add platform to db {ex.Message}");
            }
        }
    }

    enum EventType
    {
        PlatformPublished,
        Undetermined
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController:ControllerBase
    {
        private readonly IPlatformRepo _repo;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;
        private readonly IMessageBusClient _messageBusClient;

        public PlatformsController(IPlatformRepo repo, IMapper mapper, ICommandDataClient commandDataclient, IMessageBusClient messageBusClient)
        {
            _repo = repo;
            _mapper=mapper;
            _commandDataClient = commandDataclient;
            _messageBusClient = messageBusClient;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            Console.WriteLine("getting platforms...");
            var platforms = _repo.GetAllPlatforms();
            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
        }

        [HttpGet("{id}", Name = "GetPlatformById")]
        public ActionResult<PlatformReadDto> GetPlatformById(int id)
        {
            Console.WriteLine("getting platform with id...");
            var platform = _repo.GetPlatformById(id);
            if(platform != null)
            {
                return Ok(_mapper.Map<PlatformReadDto>(platform));
            }
          
            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformCreateDto)
        {
            Console.WriteLine("adding platform...");
            var platform = _mapper.Map<Platform>(platformCreateDto);
            _repo.CreatePlatform(platform);
            _repo.SaveChanges();
            var platformReadDto = _mapper.Map<PlatformReadDto>(platform);

            // send sync message
            try
            {
                await _commandDataClient.SendPlatformToCommandAsync(platformReadDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"cound not send synchronously: {ex.Message}");
            }

            // send async message
            try
            {
                var platformPublishDto = _mapper.Map<PlatformPublishDto>(platformReadDto);
                platformPublishDto.Event="Platform_Published";
                await _messageBusClient.PublishNewPlatformAsync(platformPublishDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"cound not send asynchronously: {ex.Message}");
            }
            
            return CreatedAtRoute(nameof(GetPlatformById), new { id = platformReadDto.Id}, platformReadDto);
        }
    }
}
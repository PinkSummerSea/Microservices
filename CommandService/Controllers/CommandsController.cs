using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers
{
    [ApiController]
    [Route("api/c/platforms/{platformId}/[controller]")]
    public class CommandsController:ControllerBase
    {
        private readonly ICommandRepo _repo;
        private readonly IMapper _mapper;

        public CommandsController(ICommandRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
        {
            Console.WriteLine($"hit GetCommandsForPlatform: {platformId}");
            if(!_repo.PlatformExists(platformId)) return NotFound();
            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(_repo.GetCommandsForPlatform(platformId)));
        }

        [HttpGet("{commandId}", Name = nameof(GetCommand))]
        public ActionResult<CommandReadDto> GetCommand(int platformId, int commandId)
        {
            Console.WriteLine($"hit GetCommand: {platformId}/{commandId}");
            if(!_repo.PlatformExists(platformId)) return NotFound();
            var command = _repo.GetCommand(platformId, commandId);
            if(command == null) return NotFound();
            return Ok(_mapper.Map<CommandReadDto>(command));
        }

        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommand(int platformId, CommandCreateDto commandCreateDto)
        {
            Console.WriteLine($"hit CreateCommand: {platformId}");
            if(!_repo.PlatformExists(platformId)) return NotFound();
            var command = _mapper.Map<Command>(commandCreateDto);
            _repo.CreateCommand(platformId, command);
            _repo.SaveChanges();
            var commandReadDto = _mapper.Map<CommandReadDto>(command);
            return CreatedAtRoute(nameof(GetCommand), new { platformId, commandId = commandReadDto.Id}, commandReadDto);
        }
    }
}
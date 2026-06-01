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
    [Route("api/c/[controller]")]
    [ApiController]
    public class PlatformsController:ControllerBase
    {
        private readonly ICommandRepo _repo;
        private readonly IMapper _mapper;

        public PlatformsController(ICommandRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetAllPlatforms()
        {
            Console.WriteLine("getting platforms from CommandService");
            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(_repo.GetAllPlatforms()));
        }

        [HttpPost]
        public ActionResult TestInboundConnection()
        {
            Console.WriteLine("inbound post # command service");
            return Ok("inbound test ok from platforms controller");
        }

         
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CommandService.Dtos;
using CommandService.Models;
using PlatformService;

namespace CommandService.Profiles
{
    public class CommandsProfile:Profile
    {
        public CommandsProfile()
        {
            // source --> target
            CreateMap<Platform, PlatformReadDto>();
            CreateMap<Command, CommandReadDto>();
            CreateMap<CommandCreateDto, Command>();
            CreateMap<PlatformPublishDto, Platform>().ForMember(dest => dest.ExternalId, opt=>opt.MapFrom(src => src.Id));
            CreateMap<GrpcPlatformModel, Platform>().ForMember(dest => dest.ExternalId, opt=>opt.MapFrom(src => src.PlatformId));
        }
    }
}
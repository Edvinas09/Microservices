using CommandsService.Dtos;
using CommandsService.Models;
using PlatformService;

namespace CommandsService.Profiles
{
    public class CommandsProfile : AutoMapper.Profile
    {
        public CommandsProfile()
        {
            //Source -> Target
            CreateMap<Platform, PlatformReadDto>();
            CreateMap<CommandCreateDto, Command>();
            CreateMap<Command, CommandReadDto>();
            CreateMap<PlatformPublishedDto, Platform>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
            CreateMap<GrpcPlatformModel, Platform>()
                .ForMember(dest => dest.ExternalID, opt => opt.MapFrom(src => src.PlatformId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Commands, opt => opt.Ignore());
        }
    }
}
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using RadarService.Authorization.Models;
using RadarService.Entities.Models;
using RadarService.WebApp.Areas.Radar.Dtos;
using RadarService.WebApp.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadarService.WebApp.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
      
            CreateMap<ApplicationUser, UserDto>();
            CreateMap<ApplicationRole, RoleDto>();
            CreateMap<ApplicationUserRole, UserRoleDto>();
            CreateMap<IdentityUserRole<string>, UserRoleDto>();

            CreateMap<Command, CommandDto>().ReverseMap();
            CreateMap<Step, StepDto>().ReverseMap();
            CreateMap<Request, RequestDto>().ReverseMap();
            CreateMap<FormParameter, FormParameterDto>().ReverseMap();
            CreateMap<StepRequest, StepRequestDto>().ReverseMap();
            CreateMap<Device,DeviceDto>().ReverseMap();
            CreateMap<Scheduler, SchedulerDto>().ReverseMap();
            CreateMap<DeviceScheduler, DeviceSchedulerDto>().ReverseMap();
            CreateMap<DeviceCommand, DeviceCommandDto>().ReverseMap();

        }

    }
}

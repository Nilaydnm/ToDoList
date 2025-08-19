using Business.DTOs;
using Entities;
using System;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ToDo, ToDoDeadlineDto>()
                .ForMember(d => d.ToDoId, m => m.MapFrom(s => s.Id))
                .ForMember(d => d.Deadline, m => m.MapFrom(s => s.Deadline))
                .ForMember(d => d.Delta, m => m.Ignore()); 

            CreateMap<ToDoGroup, ToDoGroupStatsDto>()
                .ForMember(d => d.GroupId, m => m.MapFrom(s => s.Id))
                .ForMember(d => d.Title, m => m.MapFrom(s => s.Title ?? string.Empty))
                .ForMember(d => d.Total, m => m.Ignore())
                .ForMember(d => d.Completed, m => m.Ignore())
                .ForMember(d => d.Active, m => m.Ignore())
                .ForMember(d => d.Overdue, m => m.Ignore());
        }
    }
}

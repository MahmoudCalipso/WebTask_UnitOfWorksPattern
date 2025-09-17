using AutoMapper;
using WebTask.DTO;
using WebTask.Entities;

namespace WebTask.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Entity <-> DTO
            CreateMap<Tasks, TasksDto>().ReverseMap();

            // Create -> Entity
            CreateMap<CreateTasksDto, Tasks>();

            // Update -> Entity
            CreateMap<UpdateTasksDto, Tasks>();
        }
    }
}

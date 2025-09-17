using WebTask.DTO;
using WebTask.Entities;

namespace WebTask.IServices
{
    public interface ITasksService
    {
        public Task<TasksDto> GetTaskByIdAsync(Guid id);
        public Task<IEnumerable<TasksDto>> GetAllTasksAsync();
        public Task<IEnumerable<TasksDto>> GetTasksByStatusAsync(EnumStatus status);
        public Task<IEnumerable<TasksDto>> GetTasksByPriorityAsync(EnumPriority priority);
        public Task<IEnumerable<TasksDto>> GetOverdueTasksAsync();
        public Task<IEnumerable<TasksDto>> GetTasksDueTodayAsync();
        public Task<TasksListDto> GetPaginatedTasksAsync(int pageNumber, int pageSize, EnumStatus? status = null, EnumPriority? priority = null, string? Title = null);
        public Task<TasksDto> CreateTaskAsync(CreateTasksDto createTaskDto);
        public Task<TasksDto> UpdateTaskAsync(Guid id, UpdateTasksDto updateTaskDto);
        public Task<bool> DeleteTaskAsync(Guid id);
        
        public Task<TasksDto> MarkTaskAsCompletedAsync(Guid id);
       
    }
}

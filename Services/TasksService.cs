using AutoMapper;
using System.Data.SqlTypes;
using WebTask.DTO;
using WebTask.Entities;
using WebTask.Interfaces;
using WebTask.IServices;
using WebTask.Repositories;

namespace WebTask.Services
{
    public class TasksService : ITasksService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public TasksService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;

        }
        public async Task<TasksDto> CreateTaskAsync(CreateTasksDto createTaskDto)
        {
            if (createTaskDto.DueDate < DateTime.Today)
            {
                throw new ArgumentException("Due date cannot be in the past");
            }

            var task = new Tasks
            {
                Id = Guid.NewGuid(),
                Title = createTaskDto.Title.Trim(),
                Description = createTaskDto.Description?.Trim(),
                DueDate = createTaskDto.DueDate,
                Priority = createTaskDto.Priority,
                Status = createTaskDto.Status,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _unitOfWork.Tasks.AddAsync(task);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<TasksDto>(task);
        }

        public async Task<bool> DeleteTaskAsync(Guid id)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(id);
            if (task == null)
            {
                return false;
            }

            await _unitOfWork.Tasks.SoftDeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<TasksDto>> GetAllTasksAsync()
        {
            var tasks = await _unitOfWork.Tasks.GetAllAsync();
            return _mapper.Map<IEnumerable<TasksDto>>(tasks);
        }

        public async Task<IEnumerable<TasksDto>> GetOverdueTasksAsync()
        {
            var tasks = await _unitOfWork.Tasks.GetOverdueTasksAsync();
            return _mapper.Map<IEnumerable<TasksDto>>(tasks);
        }

        public async Task<IEnumerable<TasksDto>> GetPaginatedTasksAsync(int pageNumber, int pageSize, EnumStatus? status = null, EnumPriority? priority = null, string? Title = null)
        {
            var tasks = await _unitOfWork.Tasks.GetPaginatedTasksAsync(pageNumber, pageSize, status, priority, Title);
            return _mapper.Map<IEnumerable<TasksDto>>(tasks);
        }

        public async Task<TasksDto> GetTaskByIdAsync(Guid id)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(id);
            return task == null ? new TasksDto() : _mapper.Map<TasksDto>(task);
        }

        public async Task<IEnumerable<TasksDto>> GetTasksByPriorityAsync(EnumPriority priority)
        {
            var tasks = await _unitOfWork.Tasks.GetTasksByPriorityAsync(priority);
            return _mapper.Map<IEnumerable<TasksDto>>(tasks);
        }

        public async Task<IEnumerable<TasksDto>> GetTasksByStatusAsync(EnumStatus status)
        {
            var tasks = await _unitOfWork.Tasks.GetTasksByStatusAsync(status);
            return _mapper.Map<IEnumerable<TasksDto>>(tasks); 
        }

        public async Task<IEnumerable<TasksDto>> GetTasksDueTodayAsync()
        {
            var tasks = await _unitOfWork.Tasks.GetTasksDueTodayAsync();
            return _mapper.Map<IEnumerable<TasksDto>>(tasks);
        }

        

        public async Task<TasksDto> MarkTaskAsCompletedAsync(Guid id)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(id);
            if (task == null)
            {
                throw new ArgumentException("Task not found");
            }

            if (task.Status == EnumStatus.Completed)
            {
                throw new InvalidOperationException("Task is already completed");
            }

            task.Status = EnumStatus.Completed;
            _unitOfWork.Tasks.Update(task);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<TasksDto>(task); 
        }

        public async Task<TasksDto> UpdateTaskAsync(Guid id, UpdateTasksDto updateTaskDto)
        {
            var existingTask = await _unitOfWork.Tasks.GetByIdAsync(id);
            if (existingTask == null)
            {
                throw new ArgumentException("Task not found");
            }

            // Validation
            if (updateTaskDto.DueDate < DateTime.Today && updateTaskDto.Status != EnumStatus.Completed)
            {
                throw new ArgumentException("Due date cannot be in the past for non-completed tasks");
            }


            existingTask.Title = updateTaskDto.Title.Trim();
            existingTask.Description = updateTaskDto.Description?.Trim();
            existingTask.DueDate = updateTaskDto.DueDate;
            existingTask.Priority = updateTaskDto.Priority;
            existingTask.Status = updateTaskDto.Status;

            _unitOfWork.Tasks.Update(existingTask);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<TasksDto>(existingTask);
        }
       
    }
}

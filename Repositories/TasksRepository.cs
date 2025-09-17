using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebTask.DTO;
using WebTask.Entities;
using WebTask.Interfaces;

namespace WebTask.Repositories
{
    public class TasksRepository : GenericRepository<Tasks>, ITasksRepository
    {    
        private readonly IMapper _mapper;
        public TasksRepository(DbTaskContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }



        public async Task<IEnumerable<Tasks>> GetTasksByStatusAsync(EnumStatus status)
        {
            return await _dbSet
                .Where(t => t.Status == status)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Tasks>> GetTasksByPriorityAsync(EnumPriority priority)
        {
            return await _dbSet
                .Where(t => t.Priority == priority)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Tasks>> GetOverdueTasksAsync()
        {
            return await _dbSet
                .Where(t => t.DueDate < DateTime.Now && t.Status != EnumStatus.Completed)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Tasks>> GetTasksDueTodayAsync()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            return await _dbSet
                .Where(t => t.DueDate >= today && t.DueDate < tomorrow && t.Status != EnumStatus.Completed)
                .OrderBy(t => t.Priority)
                .ToListAsync();
        }

        public async Task<IEnumerable<Tasks>> GetTasksByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(t => t.DueDate >= startDate && t.DueDate <= endDate)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Tasks>> SearchTasksAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllAsync();

            return await _dbSet
                .Where(t => t.Title.Contains(searchTerm) || t.Description.Contains(searchTerm))
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<TasksListDto> GetPaginatedTasksAsync(int pageNumber, int pageSize, EnumStatus? status = null, EnumPriority? priority = null, string? Title = null)
        {
            var query = _dbSet.AsQueryable();

            if (status.HasValue)
                query = query.Where(t => t.Status == status.Value);

            if (priority.HasValue)
                query = query.Where(t => t.Priority == priority.Value);
            if (!string.IsNullOrWhiteSpace(Title))
                query = query.Where(t => t.Title.Contains(Title) || t.Description.Contains(Title));
            // Récupérer le nombre total d'éléments
            var totalCount = await query.CountAsync();

            // Récupérer les données paginées
            var tasks = await query
                .OrderBy(t => t.DueDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return new TasksListDto
            {
                Tasks = _mapper.Map<List<TasksDto>>(tasks),
                TotalCount = totalCount,
                PageSize = pageSize,
                CurrentPage = pageNumber
            };
        }

        public async Task<Dictionary<EnumStatus, int>> GetTaskCountByStatusAsync()
        {
            return await _dbSet
                .GroupBy(t => t.Status)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }

        public async Task<Dictionary<EnumPriority, int>> GetTaskCountByPriorityAsync()
        {
            return await _dbSet
                .GroupBy(t => t.Priority)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }

        public async Task SoftDeleteAsync(Guid id)
        {
            var task = await _context.Tasks.IgnoreQueryFilters().FirstOrDefaultAsync(t => t.Id == id);
            if (task != null)
            {
                task.IsDeleted = true;
                Update(task);
            }
        }

        public async Task<IEnumerable<Tasks>> GetDeletedTasksAsync()
        {
            return await _context.Tasks
                .IgnoreQueryFilters()
                .Where(t => t.IsDeleted)
                .OrderBy(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task RestoreTaskAsync(Guid id)
        {
            var task = await _context.Tasks.IgnoreQueryFilters().FirstOrDefaultAsync(t => t.Id == id);
            if (task != null && task.IsDeleted)
            {
                task.IsDeleted = false;
                Update(task);
            }
        }
    }
}

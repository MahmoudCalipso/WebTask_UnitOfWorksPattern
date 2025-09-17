using WebTask.Entities;

namespace WebTask.Interfaces
{
    public interface ITasksRepository : IGenericRepository<Tasks>
    {
        public Task<IEnumerable<Tasks>> GetTasksByStatusAsync(EnumStatus status);
        public Task<IEnumerable<Tasks>> GetTasksByPriorityAsync(EnumPriority priority);
        public Task<IEnumerable<Tasks>> GetOverdueTasksAsync();
        public Task<IEnumerable<Tasks>> GetTasksDueTodayAsync();
        public Task<IEnumerable<Tasks>> GetTasksByDateRangeAsync(DateTime startDate, DateTime endDate);
        public Task<IEnumerable<Tasks>> SearchTasksAsync(string searchTerm);
        public Task<IEnumerable<Tasks>> GetPaginatedTasksAsync(int pageNumber, int pageSize, EnumStatus? status = null, EnumPriority? priority = null, string? Title = null);
        public Task<Dictionary<EnumStatus, int>> GetTaskCountByStatusAsync();
        public Task<Dictionary<EnumPriority, int>> GetTaskCountByPriorityAsync();
        public Task SoftDeleteAsync(Guid id);
        public Task<IEnumerable<Tasks>> GetDeletedTasksAsync();
        public Task RestoreTaskAsync(Guid id);
    }
}

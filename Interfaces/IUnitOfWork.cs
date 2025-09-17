namespace WebTask.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
       ITasksRepository Tasks { get; }
       public Task<int> SaveChangesAsync();
       public Task BeginTransactionAsync();
       public Task CommitTransactionAsync();
       public Task RollbackTransactionAsync();
    }
}

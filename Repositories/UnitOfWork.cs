using AutoMapper;
using Microsoft.EntityFrameworkCore.Storage;
using WebTask.Entities;
using WebTask.Interfaces;

namespace WebTask.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbTaskContext _context;
        private IDbContextTransaction _transaction;
        private bool _disposed = false;
        private readonly IMapper _mapper;
        public UnitOfWork(DbTaskContext context , IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            Tasks = new TasksRepository(_context, _mapper);
        }

        public ITasksRepository Tasks { get;}

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await SaveChangesAsync();
                await _transaction?.CommitAsync();
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            await _transaction?.RollbackAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _transaction?.Dispose();
                    _context.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}


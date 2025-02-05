using Booking.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Booking.BaseRepo
{

    public class ManagerTransastions : IDisposable
    {
        private readonly ApplicationDbContext _dbContext;
        private IDbContextTransaction _transaction;

        public ManagerTransastions(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Bắt đầu một transaction mới.
        /// </summary>
        public async Task BeginTransactionAsync()
        {
            if (_transaction == null)
            {
                _transaction = await _dbContext.Database.BeginTransactionAsync();
            }
        }

        /// <summary>
        /// Commit transaction hiện tại.
        /// </summary>
        public async Task CommitAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await DisposeTransactionAsync();
            }
        }

        /// <summary>
        /// Rollback transaction hiện tại.
        /// </summary>
        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await DisposeTransactionAsync();
            }
        }

        /// <summary>
        /// Hủy transaction hiện tại.
        /// </summary>
        private async Task DisposeTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        /// <summary>
        /// Thực thi hành động bên trong transaction một cách an toàn.
        /// </summary>
        /// <param name="action">Hành động cần thực thi.</param>
        public async Task ExecuteInTransactionAsync(Func<Task> action)
        {
            try
            {
                await BeginTransactionAsync();
                await action();
                await CommitAsync();
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
        }

        public void Dispose()
        {
            DisposeTransactionAsync().Wait();
        }

    }
}

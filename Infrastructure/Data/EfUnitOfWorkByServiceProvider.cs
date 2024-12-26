using ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class EfUnitOfWorkByServiceProvider : IUnitOfWork
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly StellarDBContext _dbContext;
        private IDbContextTransaction _transaction;

        public EfUnitOfWorkByServiceProvider(IServiceProvider serviceProvider, StellarDBContext dbContext)
        {
            _serviceProvider = serviceProvider;
            _dbContext = dbContext;
        }

        public IRepository<T> GetRepository<T>() where T : class
        {
            return _serviceProvider.GetRequiredService<IRepository<T>>();

        }
        public async Task BeginAsync()
        {
            _transaction = await _dbContext.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            await _transaction.CommitAsync();
        }


        public async Task RollbackAsync()
        {
            await _transaction.RollbackAsync();
        }
        public void Dispose()
        {
            _transaction?.Dispose();
        }

    }
}

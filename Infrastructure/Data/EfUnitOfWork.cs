using ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly StellarDBContext _dbContext;
        private readonly Dictionary<Type, object> _repositories;
        private IDbContextTransaction _transaction;

        public EfUnitOfWork(StellarDBContext dbContext)
        {
            _dbContext = dbContext;
            _repositories = new Dictionary<Type, object>();
        }
        public IRepository<T> GetRepository<T>() where T : class
        {
            if (_repositories.ContainsKey(typeof(T)))
            {
                return (IRepository<T>)_repositories[typeof(T)];
            }
            var repo = new EfRepository<T>(_dbContext);
            _repositories.Add(typeof(T), repo);
            return repo;
        }
        public async Task BeginAsync()
        {
           _transaction=await _dbContext.Database.BeginTransactionAsync();
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

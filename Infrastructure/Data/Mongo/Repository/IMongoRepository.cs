using MongoDB.Driver;

namespace Infrastructure.Data.Mongo.Repository;

public interface IMongoRepository<T> where T : class
{
    IMongoCollection<T> Collection { get; }
    Task<List<T>> GetByIdAsync(string field,string id);
    Task<List<T>> GetAllAsync();
    Task CreateAsync(T entity);
    Task UpdateAsync(string id, T entity);
    Task DeleteAsync(string id);
    Task CreateRangeAsync(IEnumerable<T> entities);
}
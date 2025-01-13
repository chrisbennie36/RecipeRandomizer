using RecipeRandomizer.Api.Data.Entities;

namespace RecipeRandomizer.Api.Data.Repositories;

public interface IEntityRepository<T> where T : EntityBase
{
    public Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    public Task<IEnumerable<T>> GetMultiByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
    public Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    public Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    public Task AddMultiAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    public Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    public Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    public Task DeleteMultiAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
}

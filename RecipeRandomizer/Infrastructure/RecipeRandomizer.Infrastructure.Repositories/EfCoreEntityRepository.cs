using Microsoft.EntityFrameworkCore;
using RecipeRandomizer.Infrastructure.Repositories.Entities;
using Serilog;

namespace RecipeRandomizer.Infrastructure.Repositories;

public class EfCoreEntityRepository<T> : IEntityRepository<T> where T : EntityBase
{
    private readonly AppDbContext appDbContext;
    private readonly DbSet<T> dbEntities;

    //ToDo: At the moment, we are saving after every function call. An improvement would be to keep track
    //of the transcation in a Unit of Work and only save at the end of the transaction
    public EfCoreEntityRepository(AppDbContext appDbContext)
    {
        ArgumentNullException.ThrowIfNull(appDbContext);
        this.appDbContext = appDbContext;

        if(appDbContext.Set<T>() == null)
        {
            throw new NullReferenceException("Could not retrieve an entity collection to work with");
        }

        dbEntities = appDbContext.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbEntities.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<T>> GetMultiByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        return await dbEntities.Where(e => ids.Contains(e.Id)).ToListAsync(cancellationToken);
    }

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        try
        {
            await appDbContext.AddAsync(entity);
            await appDbContext.SaveChangesAsync(cancellationToken);

            return entity;
        }
        catch(DbUpdateException e)
        {
            Log.Error($"Could not create a {nameof(T)} in the database: {e.Message} {e.InnerException?.Message}");
            throw;
        }
    }

    public async Task AddMultiAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        foreach(T entity in entities)
        {
            await appDbContext.AddAsync(entity);
        }
        try
        {
            await appDbContext.SaveChangesAsync(cancellationToken);
        }
        catch(DbUpdateException e)
        {
            Log.Error($"Could not create a {nameof(T)} in the database: {e.Message} {e.InnerException?.Message}");
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        //Note that this results in two operations, a GET and a DELETE. It's also possible to create a stub Message object with just the ID and then call the context.Remove method which will
        //only trigger one DELETE operation: https://www.learnentityframeworkcore.com/dbcontext/deleting-data
        T? entityToDelete = await GetByIdAsync(id, cancellationToken);

        if(entityToDelete == null)
        {
            Log.Warning($"Could not find {nameof(T)} entity to delete with ID: {id}");
            return;
        }

        dbEntities.Remove(entityToDelete);

        try
        {
            await appDbContext.SaveChangesAsync(cancellationToken); 
        }
        catch(DbUpdateException e)
        {
            Log.Error($"Could not delete a {nameof(T)} in the database: {e.Message} {e.InnerException?.Message}");
            throw;
        }
    }

    public async Task DeleteMultiAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        IEnumerable<T> entitiesToDelete = await GetMultiByIdsAsync(ids, cancellationToken);

        if(!entitiesToDelete.Any())
        {
            return;
        }

        dbEntities.RemoveRange(entitiesToDelete);

        try
        {
            await appDbContext.SaveChangesAsync(cancellationToken);
        }
        catch(DbUpdateException e)
        {
            Log.Error($"Could not delete {nameof(T)} from the database: {e.Message} {e.InnerException?.Message}");
            throw;
        }
    }

    public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await dbEntities.AsNoTracking().SingleOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        appDbContext.Entry(entity).State = EntityState.Modified;

        try
        {
            await appDbContext.SaveChangesAsync(cancellationToken);
        }
        catch(DbUpdateException e)
        {
            Log.Error($"Could not update a {nameof(T)} in the database: {e.Message} {e.InnerException?.Message}");
            throw;
        }
    }

    protected DbSet<T> GetDbSet()
    {
        return dbEntities;
    }
}

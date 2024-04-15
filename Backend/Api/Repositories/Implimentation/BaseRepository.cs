namespace ModernTenon.Api.Repositories.Implimentation;

public abstract class BaseRepository
{
    internal readonly DatabaseContext _context;

    public BaseRepository(DatabaseContext context)
    {
        _context = context;
    }

    protected async Task<T> CommitAsync<T>(Func<Task<T>> func)
    {
        var result = await func();

        await _context.SaveChangesAsync();

        return result;
    }
}

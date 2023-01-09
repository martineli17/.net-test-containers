namespace App.Controllers
{
    public interface IRedisContext
    {
        Task AddAsync<TContent>(string key, TContent content, int minutes);
        Task<TContent?> GetAsync<TContent>(string key);
    }
}

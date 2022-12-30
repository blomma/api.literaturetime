namespace API.LiteratureTime.Core.Interfaces;

public interface ICacheProvider
{
    Task<T?> GetAsync<T>(string key);
}

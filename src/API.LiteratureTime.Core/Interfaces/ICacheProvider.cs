namespace API.LiteratureTime.Core.Interfaces;

public interface ICacheProvider
{
    Task<T?> GetRandomAsync<T>(string key);
}

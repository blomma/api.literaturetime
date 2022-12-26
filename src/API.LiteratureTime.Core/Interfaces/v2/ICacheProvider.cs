namespace API.LiteratureTime.Core.Interfaces.v2;

public interface ICacheProvider
{
    Task<T?> GetAsync<T>(string key);
}

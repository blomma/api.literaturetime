namespace API.LiteratureTime.Core.Interfaces;

using Models;

public interface ILiteratureService
{
    public Task<LiteratureTime> GetRandomLiteratureTimeAsync(string hour, string minute);
    public Task<LiteratureTime> GetLiteratureTimeAsync(string hash);
}

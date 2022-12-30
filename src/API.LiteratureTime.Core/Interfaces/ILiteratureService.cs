namespace API.LiteratureTime.Core.Interfaces;

using API.LiteratureTime.Core.Models;

public interface ILiteratureService
{
    public Task<LiteratureTime> GetRandomLiteratureTimeAsync(string hour, string minute);
    public Task<LiteratureTime> GetLiteratureTimeAsync(string hash);
}

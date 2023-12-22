namespace API.LiteratureTime.Core.Interfaces;

using Models;

public interface ILiteratureService
{
    public IEnumerable<string> GetMissingLiteratureTimesAsync();
    public Task<List<LiteratureTime>> GetLiteratureTimesAsync();
    public Task<LiteratureTime> GetRandomLiteratureTimeAsync(string hour, string minute);
    public Task<LiteratureTime> GetLiteratureTimeAsync(string hash);
}

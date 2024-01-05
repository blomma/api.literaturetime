namespace API.LiteratureTime.Core.Interfaces;

public interface ILiteratureService
{
    public Task<Models.LiteratureTime> GetRandomLiteratureTimeAsync(string hour, string minute);
    public Task<Models.LiteratureTime> GetLiteratureTimeAsync(string hash);
}

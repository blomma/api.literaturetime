namespace API.Literature.Core.Interfaces;

using API.Literature.Core.Models;

public interface ILiteratureService
{
    public LiteratureTime GetRandomLiteratureTime(string hour, string minute);
    public LiteratureTime GetLiteratureTime(string hour, string minute, string hash);
    public List<LiteratureTime> GetLiteratureTimes();
}
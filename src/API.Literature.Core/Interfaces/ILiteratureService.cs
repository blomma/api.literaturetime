namespace API.Literature.Core.Interfaces;

using API.Literature.Core.Models;

public interface ILiteratureService
{
    public LiteratureTime GetLiteratureTime(long milliseconds);
    public List<LiteratureTime> GetLiteratureTimes();
}
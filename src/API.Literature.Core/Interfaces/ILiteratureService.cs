namespace API.Literature.Core.Interfaces;

using API.Literature.Core.Models;

public interface ILiteratureService
{
    public LiteratureTime GetLiteratureTime(string hourMinute);
    public List<LiteratureTime> GetLiteratureTimes();
}
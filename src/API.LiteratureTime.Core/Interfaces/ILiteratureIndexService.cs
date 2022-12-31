namespace API.LiteratureTime.Core.Interfaces;

public interface ILiteratureIndexService
{
    List<string>? GetLiteratureTimeHashes(string hour, string minute);
    Task PopulateIndexAsync();
}

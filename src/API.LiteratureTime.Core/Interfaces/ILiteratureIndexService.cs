namespace API.LiteratureTime.Core.Interfaces;

public interface ILiteratureIndexService
{
    List<string>? GetLiteratureTimeHashes(string time);
    Task PopulateIndexAsync();
}

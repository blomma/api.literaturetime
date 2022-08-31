namespace API.LiteratureTime.Infrastructure.Providers;

using System.Collections.Generic;
using API.LiteratureTime.Core.Interfaces;

public class LiteratureProvider : ILiteratureProvider
{
    public List<string> GetLiteratureTimes()
    {
        return File.ReadAllLines("litclock_annotated.csv").ToList();
    }
}
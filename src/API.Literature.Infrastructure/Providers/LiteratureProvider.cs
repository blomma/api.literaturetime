namespace API.Literature.Infrastructure.Providers;

using System.Collections.Generic;
using API.Literature.Core.Interfaces;

public class LiteratureProvider : ILiteratureProvider
{
    public List<string> GetLiteratureTimes()
    {
        return File.ReadAllLines("litclock_annotated.csv").ToList();
    }
}
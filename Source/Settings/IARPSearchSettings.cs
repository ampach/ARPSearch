using System;
using System.Collections.Generic;

namespace ARPSearch.Settings
{
    public interface IARPSearchSettings
    {
        Dictionary<Guid, string> Facets { get; }
        Dictionary<Guid, string> ResultsMapping { get; }
        string SearchServiceAjaxRequestUrl { get; }
    }
}
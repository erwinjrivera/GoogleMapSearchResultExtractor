using GoogleMapSearchResultExtractor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleMapSearchResultExtractor.Service
{
    public interface IGoogleService
    {
        List<MapResult> GoogleMapSearch(string keyword, int offset);

        List<MapResult> GetListOfMapResult(string keyword, int offset);

        MapResult GetCardDetails(string url, string title = "");

        //List<OrganicResult> GoogleSearchOrganic(string keyword, int limit = 10, int offset = 0);
    }
}

using GoogleMapSearchResultExtractor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleMapSearchResultExtractor.View
{
    public interface IMainFormView
    {
        //void AddItemResult(MapResult item);

        void AddItemResult(ICardView item);

        bool SearchBoxEnabled { set; }

        bool SearchButtonEnabled { set; }

        bool ExtractButtonEnabled { set; }

        bool Searching { get; set; }

        string ResultCount { set; }

        string StatusText { set; }

        void ChangeButtonBackground(string backgroundName);

        void ClearItems();
    }
}

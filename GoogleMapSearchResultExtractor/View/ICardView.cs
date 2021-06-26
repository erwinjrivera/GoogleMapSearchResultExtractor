using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleMapSearchResultExtractor.View
{
    public interface ICardView
    {

        string Title { set; }

        string URL { set; }

        string Description { set; }

        string Rating { set; }

        string Reviews { set; }

        string Address { set; }

        string Hours { set; }

        string Phone { set; }

        string Website { set; }

        string Direction { set; }

        string Email { set; }

        void DisplayRatingAndReviews();
    }
}

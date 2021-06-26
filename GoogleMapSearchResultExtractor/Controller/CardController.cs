using GoogleMapSearchResultExtractor.Model;
using GoogleMapSearchResultExtractor.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleMapSearchResultExtractor.Controller
{
    public class CardController
    {
        private ICardView _view;

        private MapResult _model;

        public CardController(ICardView view, MapResult model)
        {
            _view = view;
            _model = model;
        }

        public void Initialize()
        {
            _view.Title = _model.Title;
            _view.Description = _model.Description;

            _model.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Model_PropertyChanged);
        }


        public void UpdateModel(MapResult m)
        {
            if (m != null)
            {
                //sequence

                _model.Description = m.Description;
                _model.Rating = m.Rating;
                _model.Reviews = m.Reviews;
                _model.Address = m.Address;
                _model.Hours = m.Hours;
                _model.Phone = m.Phone;
                _model.Website = m.Website;
                _model.Email = m.Email;

                //_model.Direction = m.Direction;
                

                _view.DisplayRatingAndReviews();
            }
            
        }












        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Description":
                    _view.Description = _model.Description;
                    break;
                case "Rating":
                    _view.Rating = _model.Rating;
                    break;
                case "Reviews":
                    _view.Reviews = _model.Reviews;
                    break;
                case "Address":
                    _view.Address = _model.Address;
                    break;
                case "Hours":
                    _view.Hours = _model.Hours;
                    break;
                case "Website":
                    _view.Website = _model.Website;
                    break;
                case "Phone":
                    _view.Phone = _model.Phone;
                    break;
                case "Email":
                    _view.Email = _model.Email;
                    break;
                case "Direction":
                    _view.Direction = _model.Direction;
                    break;

            } 
        }
    }
}

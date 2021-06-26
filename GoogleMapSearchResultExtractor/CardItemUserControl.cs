using GoogleMapSearchResultExtractor.Model;
using GoogleMapSearchResultExtractor.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GoogleMapSearchResultExtractor
{
    public partial class CardItemUserControl : UserControl, ICardView
    {
        private string _title;

        private string _description;

        private string _url;

        private string _rating;

        private string _reviews;

        private string _address;

        private string _hours;

        private string _phone;

        private string _email;

        private string _direction;

        private string _website;

        public string Title
        {
            set
            {
                _title = value;

                txtTitle.Text = _title;
            }
        }

        public string Description
        {
            set
            {
                _description = value;

                txtDescription.Text = _description;
            }
        }
        
        public string Rating
        {
            set
            {
                _rating = value;
            }
        }

        public string Reviews
        {
            set
            {
                _reviews = value;
            }
        }

        public string URL
        {
            set
            {
                _url = value;
            }
        }

        public string Address
        {
            set
            {
                _address = value;

                if (!string.IsNullOrEmpty(_address))
                    txtDetails.Text += "" + _address + Environment.NewLine;
            }
        }

        public string Hours
        {
            set
            {
                _hours = value;

                if (!string.IsNullOrEmpty(_hours))
                    txtDetails.Text += "" + _hours + Environment.NewLine;
            }
        }

        public string Phone
        {
            set
            {
                _phone = value;

                if (!string.IsNullOrEmpty(_phone))
                    txtDetails.Text += "" + _phone + Environment.NewLine;
            }
        }

        public string Website
        {
            set
            {
                _website = value;

                if (!string.IsNullOrEmpty(_website))
                    txtDetails.Text += "" + _website + Environment.NewLine;
            }
        }

        public string Direction
        {
            set
            {
                _direction = value;
            }
        }

        public string Email
        {
            set
            {
                _email = value;

                if (!String.IsNullOrEmpty(_email))
                {
                    if (_email.Split(',').Length > 1)
                    {
                        txtDetails.Text += "Emails: " + _email + Environment.NewLine;
                    }
                    else
                    {
                        txtDetails.Text += "" + _email + Environment.NewLine;
                    }
                }
            }
        }


        public void DisplayRatingAndReviews()
        {
            string rating = null;
            string reviews = null;

            if (!String.IsNullOrEmpty(_rating))
            {
                rating = $"Rating: {_rating}";
            }
            else
            {
                rating = "Rating: 0";
            }

            if (!String.IsNullOrEmpty(_reviews))
            {
                int result = 0;
                int.TryParse(_reviews.Replace(",", string.Empty), out result);

                reviews = "(" + String.Format("{0:n0}", result) + " " + (result > 1 ? "reviews" : "review") + ")";
            }
            else
            {
                reviews = "(no reviews)";
            }

            txtReviews.Text = $"{rating} {reviews}";
        }

        public void DisplayAddressAndContactDetails()
        {

        }









        public CardItemUserControl()
        {
            InitializeComponent();
        }

        private void CardItemUserControl_Load(object sender, EventArgs e)
        {
            this.Width = this.Parent.Width - 25;
        }


        public CardItemUserControl(MapResult item)
        {
            InitializeComponent();

            string details = "";
            int heightDeduction = 0;

            txtTitle.Text = item.Title;

            string rating = string.Empty;
            string reviews = string.Empty;

            if (!String.IsNullOrEmpty(item.Rating))
            {
                rating = $"Rating: {item.Rating}";
                ///txtRating.Text = rating;
            }
            else
            {
                rating = "Rating: 0";
                //txtRating.Text = rating;
            }

            if (!String.IsNullOrEmpty(item.Reviews))
            {
                int result = 0;
                int.TryParse(item.Reviews.Replace(",", string.Empty), out result);

                reviews = "(" + String.Format("{0:n0}", result) + " " + (result > 1 ? "reviews" : "review") + ")";

                //txtReviews.Text = reviews;
            }
            else
            {
                reviews = "(No reviews)";
                //txtReviews.Text = reviews;
            }

            txtReviews.Text = $"{rating} {reviews}";

            txtDescription.Text = item.Description;
            txtAddress.Text = $"Address: {item.Address}";
            txtHours.Text = $"Hours: {item.Hours}";
            txtWebsite.Text = item.Website;
            txtEmail.Text = item.Email;
            txtPhone.Text = $"Phone: {item.Phone}";

            if (!String.IsNullOrEmpty(item.Address))
            {
                txtDetails.Text += "Address: " + item.Address + Environment.NewLine;
            }
            else
            {
                heightDeduction += 18;
            }
            if (!String.IsNullOrEmpty(item.Hours))
            {
                txtDetails.Text += "Hours: " + item.Hours + Environment.NewLine;

            }
            else
            {
                heightDeduction += 18;
            }

            if (!String.IsNullOrEmpty(item.Phone))
            {
                txtDetails.Text += "Phone: " + item.Phone + Environment.NewLine;
            }
            else
            {
                heightDeduction += 18;
            }

            if (!String.IsNullOrEmpty(item.Website))
            {
                if (item.Website != null)
                {
                    txtDetails.Text += "Website: " + item.Website + Environment.NewLine;
                }
            }
            else
            {
                heightDeduction += 18;
            }

            if (!String.IsNullOrEmpty(item.Email))
            {
                if (item.Email != null)
                {
                    if (item.Email.Split(',').Length > 1)
                    {
                        /*
                        string[] emails = item.Email.Split(',');

                        foreach (var email in emails)
                        {
                            string mailto = $@"<a href='{email}'>{email}</a>";

                            txtDetails.Text += mailto;
                        }
                        */
                        txtDetails.Text += "Émails: " + item.Email + Environment.NewLine;
                    }
                    else
                        txtDetails.Text += "Émail: " + item.Email + Environment.NewLine;
                }
            }
            else
            {
                heightDeduction += 18;
            }



            txtDetails.Text = txtDetails.Text.Trim();

            this.Height = this.Height - heightDeduction;
        }

        private void CardItemUserControl_Paint(object sender, PaintEventArgs e)
        {
            //ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, Color.Gainsboro, ButtonBorderStyle.Solid);

            using (SolidBrush brush = new SolidBrush(BackColor))
            {
                e.Graphics.FillRectangle(brush, ClientRectangle);
            }

            e.Graphics.DrawRectangle(Pens.WhiteSmoke, -2, 0, ClientSize.Width + 2, ClientSize.Height);
        }


    }
}

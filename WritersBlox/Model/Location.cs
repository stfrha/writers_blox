using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WritersBlox
{
    [Serializable()]
    public class Location : Blox
    {
        #region Declarations

        private string _headline;
        private string _longitude;
        private string _latitude;
        private string _world;
        private string _country;
        private string _town;
        private string _address;
        private string _description;

        #endregion

        #region Constructor

        public Location()
        {
            Headline = "Okänd";
            Description = "Okänd";
            World = "Okänd";
            Country = "Okänd";
            Town = "Okänd";
            Address = "Okänd";
            Longitude = "Okänd";
            Latitude = "Okänd";
            ProfileImage = new SerializableBitmapImage( new Uri("pack://application:,,,/WritersBlox;component/Images/Icons/tag_512.png"));
        }

        public Location(string headline, string description, string world, string country, string town, string address, string longitude, string latitude)
            : base(true)
        {
            Headline = headline;
            Description = description;
            World = world;
            Country = country;
            Town = town;
            Address = address;
            Longitude = longitude;
            Latitude = latitude;
            ProfileImage = new SerializableBitmapImage(new Uri("pack://application:,,,/WritersBlox;component/Images/Icons/tag_512.png"));
        }

        #endregion

        #region Properties

        public string Description
        {
            get { return _description;  }
            set
            {
                _description = value;
                OnPropertyChanged("Description");
            }
        }

        public string World
        {
            get { return _world; }
            set
            {
                _world = value;
                OnPropertyChanged("World");
            }
        }

        public string Country
        {
            get { return _country; }
            set
            {
                _country = value;
                OnPropertyChanged("Country");
            }
        }

        public string Headline
        {
            get { return _headline; }
            set
            {
                _headline = value;
                OnPropertyChanged("Headline");
            }
        }

        public string Town
        {
            get { return _town; }
            set
            {
                _town = value;
                OnPropertyChanged("Town");
            }
        }

        public string Address
        {
            get { return _address; }
            set
            {
                _address = value;
                OnPropertyChanged("Address");
            }
        }

        public string Longitude
        {
            get { return _longitude; }
            set
            {
                _longitude = value;
                OnPropertyChanged("Longitude");
            }
        }

        public string Latitude
        {
            get { return _latitude; }
            set
            {
                _latitude = value;
                OnPropertyChanged("Latitude");
            }
        }

        #endregion

    }
}

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
    public class Character : Blox
    {
        #region Declarations

        private string _alias;
        private string _title;
        private string _name;
        private string _description;

        #endregion

        #region Constructor

        public Character()
        {
            Alias = "Okänd";
            Description = "Okänd";
            Title = "Okänd";
            Name = "Okänd";

            ProfileImage = new SerializableBitmapImage(new Uri("pack://application:,,,/WritersBlox;component/Images/Icons/user_512.png"));
        }

        public Character(string name, string alias, string title, string description)
            : base(true)
        {
            Name = name;
            Alias = alias;
            Title = title;
            Description = description;

            ProfileImage = new SerializableBitmapImage(new Uri("pack://application:,,,/WritersBlox;component/Images/Icons/user_512.png"));
        }

        #endregion

        #region Properties

        public string Alias
        {
            get { return _alias; }
            set
            {
                _alias = value;
                OnPropertyChanged("Alias");
            }
        }

        public string Description
        {
            get { return _description;  }
            set
            {
                _description = value;
                OnPropertyChanged("Description");
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged("Title");
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        #endregion
    }
}

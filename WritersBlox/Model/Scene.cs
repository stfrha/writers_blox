using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WritersBlox
{
    [Serializable]
    public class Scene : Blox
    {
        #region Declarations

        private string _headline;
        private string _description;

        #endregion

        #region Constructor

        public Scene()
        {
            Headline = "Okänd";
            Description = "Okänd";
            ProfileImage = new SerializableBitmapImage(new Uri("pack://application:,,,/WritersBlox;component/Images/Icons/bubble_512.png"));
        }

        public Scene(string headline, string description) : base(true)
        {
            Headline = headline;
            Description = description;
            ProfileImage = new SerializableBitmapImage(new Uri("pack://application:,,,/WritersBlox;component/Images/Icons/bubble_512.png"));
        }

        #endregion

        #region Properties

        public string Headline
        {
            get { return _headline; }
            set
            {
                _headline = value;
                OnPropertyChanged("Headline");
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

        #endregion

    }
}

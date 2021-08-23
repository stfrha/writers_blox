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
    public class Article : Blox
    {
        #region Declarations

        private string _headline;
        private string _description;

        #endregion

        #region Constructor

        public Article()
        {
            Headline = "Okänd";
            Description = "Okänd";
            ProfileImage = new SerializableBitmapImage(new Uri("pack://application:,,,/WritersBlox;component/Images/Icons/browser_512.png"));
        }

        public Article(string headline, string description)
            : base(true)
        {
            Headline = headline;
            Description = description;
            ProfileImage = new SerializableBitmapImage(new Uri("pack://application:,,,/WritersBlox;component/Images/Icons/browser_512.png"));
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

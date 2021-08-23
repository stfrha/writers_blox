using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Serialization;

namespace WritersBlox
{
    [Serializable()]
    public class Blox : WBReferencedObject
    {
        #region Declarations

        //private ObservableCollection<StoryEvent> _storyEvents = new ObservableCollection<StoryEvent>();
        private WBObjectList<StoryEvent> _storyEvents = new WBObjectList<StoryEvent>();

        private ObservableCollection<SerializableBitmapImage> _images = new ObservableCollection<SerializableBitmapImage>();

        private ObservableCollection<Article> _articles = new ObservableCollection<Article>();

        private SerializableBitmapImage _profileImage;

        #endregion

        #region Constructors

        public Blox()  {        }

        public Blox(bool autoCreateGuid)  : base(true)
        {
        }

        #endregion

        #region Properties

        public WBObjectList<StoryEvent> StoryEvents
        {
            get { return _storyEvents; }
            set { _storyEvents = value; }
        }

        //[XmlIgnore]
        public ObservableCollection<SerializableBitmapImage> Images
        {
            get { return _images; }
            set { _images = value; }
        }


        //[XmlIgnore]
        public SerializableBitmapImage ProfileImage
        {
            get { return _profileImage; }
            set
            {
                _profileImage = value;
                OnPropertyChanged("ProfileImage");
            }
        }

        public ObservableCollection<Article> Articles
        {
            get { return _articles; }
            set { _articles = value; }
        }

        #endregion

        #region Methods

        public void Rehydrate(Func<Guid, WBReferencedObject> FindGUIDObject )
        {
            foreach (Guid id in StoryEvents.DehydratedList)
            {
                WBReferencedObject se = FindGUIDObject(id);
                StoryEvents.Add(se as StoryEvent);
            }
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;


#region Types

//public enum ImageSizes { eSmallImage = 60, eMediumImage = 120, eLargeImage = 240 };
public enum ImageSizes { eSmallImage, eMediumImage, eLargeImage };

#endregion

namespace WritersBlox
{
    [Serializable()] 
    public class WBObjectViewModel : ViewModelBase
    {
        #region Declarations

        private bool _isSelected;
        private bool _isExpanded;
        private bool _invalidDrop;
        private ImageSizes _imageSize;
        private SerializableBitmapImage _selectedImage;

        #endregion

        #region Construction

        public WBObjectViewModel() 
        {
            IsSelected = false;
            IsExpanded = false;
            InvalidDrop = false;
            ImageSize = ImageSizes.eMediumImage;
        }

        #endregion

        #region Properties

        [XmlIgnore]
        public bool InvalidDrop
        {
            get { return _invalidDrop; }
            set { 
                _invalidDrop = value;
                OnPropertyChanged("InvalidDrop");
            }
        }

        [XmlIgnore]
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        [XmlIgnore]
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                _isExpanded = value;
                OnPropertyChanged("IsExpanded");
            }
        }

        [XmlIgnore]
        public ImageSizes ImageSize
        {
            get { return _imageSize; }
            set
            {
                _imageSize = value;
                OnPropertyChanged("ImageSize");
            }
        }

        [XmlIgnore]
        public SerializableBitmapImage SelectedImage
        {
            get { return _selectedImage; }
            set
            {
                _selectedImage = value;
                OnPropertyChanged("SelectedImage");
            }
        }

        #endregion

    }
}

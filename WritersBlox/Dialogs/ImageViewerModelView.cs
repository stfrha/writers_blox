using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WritersBlox
{
    class ImageViewerModelView : ViewModelBase
    {
        #region Declarations

        private ObservableCollection<SerializableBitmapImage> _images = new ObservableCollection<SerializableBitmapImage>();
        private SerializableBitmapImage _selectedImage;

        #endregion

        #region Constructor

        public ImageViewerModelView()
        {
        }

        #endregion

        #region Properties

        public ObservableCollection<SerializableBitmapImage> Images
        {
            get { return _images; }
            set { _images = value; }
        }

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

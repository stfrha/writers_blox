using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Serialization;

namespace WritersBlox
{
    public class SerializableBitmapImage : ViewModelBase, IXmlSerializable
    {
        private BitmapImage _image;
        private bool _brokenLink;

        public SerializableBitmapImage()
        {
            Image = new BitmapImage();
            BrokenLink = false;
        }

        public SerializableBitmapImage(Uri uriSource)
        {
            Image = new BitmapImage();
            Image.BeginInit();
            Image.UriSource = uriSource;
            Image.CacheOption = BitmapCacheOption.OnLoad;
            Image.EndInit();
            BrokenLink = false;
        }

        public BitmapImage Image
        {
            get { return _image; }
            set
            {
                _image = value;
                OnPropertyChanged("Image");
            }
        }

        [XmlIgnore]
        public bool BrokenLink
        {
            get { return _brokenLink; }
            set
            {
                _brokenLink = value;
                OnPropertyChanged("BrokenLink");
            }
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("FileName", _image.UriSource.OriginalString);
        }

        public void ReadXml(XmlReader reader)
        {
            reader.MoveToFirstAttribute();

            // I cant use File.Exist on reader.value since it could be a pack-resource which is a valid file
            // but would return false, so we try if we can load it and if not, set broken link

            try
            {
                Image.BeginInit();
                Image.UriSource = new Uri(Uri.UnescapeDataString(reader.Value), UriKind.Absolute);
                Image.CacheOption = BitmapCacheOption.OnLoad;
                Image.EndInit();
            }

            catch (Exception e)
            {
                BrokenLink = true;
            }
            
            /*
            if (!File.Exists(reader.Value))
            {
                BrokenLink = true;
                Image.UriSource = new Uri(Uri.UnescapeDataString(reader.Value), UriKind.Absolute);
            }
            else
            {
                Image.BeginInit();
                Image.UriSource = new Uri(Uri.UnescapeDataString(reader.Value), UriKind.Absolute);
                Image.CacheOption = BitmapCacheOption.OnLoad;
                Image.EndInit();
            }
            */
            reader.Read();
            
        }

        public void RelinkImage( Uri uriSource)
        {
            Image = new BitmapImage();
            Image.BeginInit();
            Image.UriSource = uriSource;
            Image.CacheOption = BitmapCacheOption.OnLoad;
            Image.EndInit();
            BrokenLink = false;
        }
    }
}

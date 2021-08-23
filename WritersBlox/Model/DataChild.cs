using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace WritersBlox
{
    [Serializable()]
    public class DataChild : WBObjectViewModel, IXmlSerializable
    {
        #region Declarations

        private string _folderName;

//        private bool _isSelected;
//        private bool _isExpanded;
//        private bool _invalidDrop;

        private IList _children = new CompositeCollection() 
        {
            new CollectionContainer { Collection = new List<DataChild>() },
            new CollectionContainer { Collection = new List<Scene>() },
            new CollectionContainer { Collection = new List<Character>() },
            new CollectionContainer { Collection = new List<Location>() },
            new CollectionContainer { Collection = new List<StoryEvent>() }
        };

        #endregion

        #region Constructor

        public DataChild()
        {
            FolderName = "Okänd";
            IsSelected = false;
            IsExpanded = false;
        }
        
        public DataChild( string name )
        {
            FolderName = name;
            IsSelected = false;
            IsExpanded = false;
        }
        
        #endregion

        #region Properties

        public IList Children
        {
            get { return _children; }
            set { _children = value; }
        }


        public string FolderName
        {
            get { return _folderName; }
            set
            {
                _folderName = value;
                OnPropertyChanged("FolderName");
            }
        }

        #endregion


        #region Methods

        public System.Xml.Schema.XmlSchema GetSchema() { return null; }

        #region WriteXml

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("FolderName", FolderName);

            foreach (Object item in _children)
            {
                if (item.GetType() == typeof(DataChild))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(DataChild));
                    xmlSerializer.Serialize(writer, item as DataChild);
                }
                else if (item.GetType() == typeof(Scene))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(Scene));
                    xmlSerializer.Serialize(writer, item as Scene);
                }
                else if (item.GetType() == typeof(Character))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(Character));
                    xmlSerializer.Serialize(writer, item as Character);
                }
                else if (item.GetType() == typeof(Location))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(Location));
                    xmlSerializer.Serialize(writer, item as Location);
                }
                else if (item.GetType() == typeof(StoryEvent))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(StoryEvent));
                    xmlSerializer.Serialize(writer, item as StoryEvent);
                }
            }
        }


        #endregion

        #region ReadXml

        public void ReadXml(XmlReader reader)
        {
            // bool nonEmptyList;
            if (reader.IsEmptyElement)
            {
                reader.MoveToFirstAttribute();
                FolderName = reader.Value;
                reader.Read();
            }
            else
            {

                reader.MoveToFirstAttribute();
                FolderName = reader.Value;
                reader.ReadStartElement("DataChild");
                while (reader.IsStartElement())
                {
                    string name = reader.Name;

                    if (name == "Scene")
                    {
                        XmlSerializer serial = new XmlSerializer(typeof(Scene));
                        _children.Add((Scene)serial.Deserialize(reader));
                    }
                    else if (name == "Character")
                    {
                        XmlSerializer serial = new XmlSerializer(typeof(Character));
                        _children.Add((Character)serial.Deserialize(reader));
                    }
                    else if (name == "Location")
                    {
                        XmlSerializer serial = new XmlSerializer(typeof(Location));
                        _children.Add((Location)serial.Deserialize(reader));
                    }
                    else if (name == "StoryEvent")
                    {
                        XmlSerializer serial = new XmlSerializer(typeof(StoryEvent));
                        _children.Add((StoryEvent)serial.Deserialize(reader));
                    }
                    else if (name == "DataChild")
                    {
                        XmlSerializer serial = new XmlSerializer(typeof(DataChild));
                        _children.Add((DataChild)serial.Deserialize(reader));
                    }
                }
                reader.ReadEndElement();
            }
        }

        #endregion


        #region FindBloxCollection

        public IList FindBloxCollection(Blox findMe)
        {
            if (_children.Contains(findMe)) return _children;

            IEnumerable<DataChild> qDC = _children.OfType<DataChild>();

            foreach (DataChild lDC in qDC)
            {
                IList childList = lDC.FindBloxCollection(findMe);
                if (childList != null) return childList;
            }
            return null;
        }

        #endregion

        #region FindObjectCollection

        public IList FindObjectCollection(Object findMe)
        {
            if (_children.Contains(findMe)) return _children;

            IEnumerable<DataChild> qDC = _children.OfType<DataChild>();

            foreach (DataChild lDC in qDC)
            {
                IList childList = lDC.FindObjectCollection(findMe);
                if (childList != null) return childList;
            }

            return null;
        }

        #endregion

        #region FindObjectOwner

        public DataChild FindObjectOwner(Object findMe)
        {
            if (_children.Contains(findMe)) return this;

            IEnumerable<DataChild> qDC = _children.OfType<DataChild>();

            foreach (DataChild lDC in qDC)
            {
                DataChild c = lDC.FindObjectOwner(findMe);
                if (c != null) return c;
            }
            return null;
        }

        #endregion

        #region FindGuid

        public WBReferencedObject FindGuid(Guid id)
        {
            foreach (Scene item in _children.OfType<Scene>())
            {
                if (item.FindGuid(id) != null) return item;
            }

            foreach (Character item in _children.OfType<Character>())
            {
                if (item.FindGuid(id) != null) return item;
            }

            foreach (Location item in _children.OfType<Location>())
            {
                if (item.FindGuid(id) != null) return item;
            }

            foreach (StoryEvent item in _children.OfType<StoryEvent>())
            {
                if (item.FindGuid(id) != null) return item;
            }

            WBReferencedObject t;

            foreach (DataChild item in _children.OfType<DataChild>())
            {
                t = item.FindGuid(id);
                if (t != null) return t;
            }

            return null;
        }

        #endregion

        #region Rehydrate

        public void Rehydrate(Func<Guid, WBReferencedObject> FindGUIDObject)
        {
            foreach (Scene item in _children.OfType<Scene>())
            {
                item.Rehydrate(FindGUIDObject);
            }

            foreach (Character item in _children.OfType<Character>())
            {
                item.Rehydrate(FindGUIDObject);
            }

            foreach (Location item in _children.OfType<Location>())
            {
                item.Rehydrate(FindGUIDObject);
            }

            foreach (StoryEvent item in _children.OfType<StoryEvent>())
            {
                item.Rehydrate(FindGUIDObject);
            }

            foreach (DataChild item in _children.OfType<DataChild>())
            {
                item.Rehydrate(FindGUIDObject);
            }


        }

        #endregion

        #region PopulateWithFilter

        public void PopulateWithFilter(DataChild origDC, Type filterType)
        {
            foreach (var obj in origDC.Children)
            {
                if (obj.GetType() == filterType)
                {
                    Children.Add(obj);
                }

                if (obj.GetType().ToString() == "DataChild")
                {
                    DataChild tDC = obj as DataChild;

                    // Create new DataChild with same FolderName but with the empty list
                    DataChild nC = new DataChild(tDC.FolderName);

                    // Add new DataChild to collection
                    Children.Add(nC);

                    // Ask DataCHild to populate its collection with filterType and sub-DataChild objects (recursively)
                    nC.PopulateWithFilter(tDC, filterType);
                }

            }

        }

        #endregion

        public void InvalidateAscendantsDrop(bool notDropable)
        {

            IEnumerable<WBObjectViewModel> iB = _children.OfType<WBObjectViewModel>();

            foreach (WBObjectViewModel bI in iB)
            {
                bI.InvalidDrop = notDropable;
            }

            IEnumerable<DataChild> qDC = _children.OfType<DataChild>();

            foreach (DataChild dCI in qDC)
            {
                dCI.InvalidateAscendantsDrop(notDropable);
            }
        }


        #endregion


    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace WritersBlox   
{
    [Serializable()]
    public class WBObjectList<T> : ObservableCollection<T>, IXmlSerializable where T : WBReferencedObject, new()
    {
        #region Declarations

        private List<Guid> _dehydratedList = new List<Guid>();

        #endregion

        #region Constructors

        public WBObjectList()
        {
        }

        #endregion

        #region Properties

        public List<Guid> DehydratedList
        {
            get { return _dehydratedList; }
            set { _dehydratedList = value; }
        }

        #endregion
                
        #region Methods

        public System.Xml.Schema.XmlSchema GetSchema() { return null; }

        #region WriteXml

        public void WriteXml(XmlWriter writer)
        {
            string elementName = typeof(T).Name;
            string startElementName = elementName + "s";
            //writer.WriteStartElement(startElementName);

            foreach (T item in this)
            {
                // Quite a risk to assume this is of type WBReferencedObject, but... it will be!
                WBReferencedObject wbR = item as WBReferencedObject;
                writer.WriteElementString(elementName, wbR.WBGuid.ToString());
            }

            //writer.WriteEndElement();
        }

        #endregion

        #region ReadXml

        public void ReadXml(XmlReader reader)
        {
            if (reader.IsEmptyElement)
            {
            }
            else
            {
                string elementName = typeof(T).Name;
                string startElementName = elementName + "s";

                reader.ReadStartElement(startElementName);

                while (reader.IsStartElement(elementName))
                {
                    reader.Read();

                    Guid newId = new Guid(reader.Value);
                    DehydratedList.Add(newId);

                    reader.Read();
                    reader.ReadEndElement();
                }
                reader.ReadEndElement();
            }
        }

        #endregion

        #endregion

    }
}

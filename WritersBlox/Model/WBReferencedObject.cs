using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WritersBlox

{
    [Serializable()]
    public class WBReferencedObject : WBObjectViewModel
    {
        #region Declarations

        private Guid _wbGuid;

        #endregion

        #region Constructors

        public WBReferencedObject()
        {
        }

        public WBReferencedObject(Guid wbGuid)
        {
            WBGuid = wbGuid;
        }

        public WBReferencedObject(bool autoCreate)
        {
            if (autoCreate) WBGuid = Guid.NewGuid();
        }

        #endregion

        #region Properties

        public Guid WBGuid
        {
            get { return _wbGuid; }
            set
            {
                _wbGuid = value;
                OnPropertyChanged("WBGuid");
            }
        }

        #endregion

        #region Methods

        public WBReferencedObject FindGuid(Guid id)
        {
            if (id == WBGuid) return this;
            return null;
        }
        #endregion
    }
}

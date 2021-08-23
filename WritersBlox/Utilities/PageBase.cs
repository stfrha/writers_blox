using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WritersBlox
{
    public class PageBase : Page
    {
        #region Declarations

        object _myObject;

    	#endregion

        public PageBase( object myObject )
        {
            _myObject = myObject;
        }

    }
}

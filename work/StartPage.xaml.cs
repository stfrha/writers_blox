using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WritersBlox.Pages
{
    /// <summary>
    /// Interaction logic for EditEventPage.xaml
    /// </summary>
    public partial class StartPage : Page
    {
        private WritersBloxVewModel _theModel;

        public StartPage(WritersBloxVewModel theModel)
        {
            InitializeComponent();
            this.TheModel = theModel;
            DataContext = theModel;
        }

        public WritersBloxVewModel TheModel
        {
            get { return _theModel; }
            set { _theModel = value; }
        }
    }
}

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

namespace WritersBlox
{
    /// <summary>
    /// Interaction logic for EditLocationPage.xaml
    /// </summary>
    public partial class EditLocationPage : Page
    {
        private WritersBloxVewModel _theModel;

        public EditLocationPage(Location editLocation, WritersBloxVewModel theModel)
        {
            InitializeComponent();
            this.TheModel = theModel;
            this.DataContext = editLocation;
        }

        private new void PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

            if (treeViewItem != null)
            {
                treeViewItem.IsSelected = true;
                treeViewItem.Focus();
                e.Handled = true;

                // Possibly handle change of window focus since selection may span multiple treeviews.
            }
        }

        static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }

        public WritersBloxVewModel TheModel
        {
            get { return _theModel; }
            set { _theModel = value; }
        }
    }
}

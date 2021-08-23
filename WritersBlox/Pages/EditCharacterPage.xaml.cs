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
    /// Interaction logic for EditCharacterPage.xaml
    /// </summary>
    public partial class EditCharacterPage : Page
    {

        private WritersBloxVewModel _theModel;

        public EditCharacterPage(Character editCharacter, WritersBloxVewModel theModel )
        {
            InitializeComponent();
            this.TheModel = theModel;
            this.DataContext = editCharacter;
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

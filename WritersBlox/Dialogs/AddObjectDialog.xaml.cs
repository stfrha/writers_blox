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
using System.Windows.Shapes;

namespace WritersBlox
{
    /// <summary>
    /// Interaction logic for AddObjectDialog.xaml
    /// </summary>
    public partial class AddObjectDialog : Window
    {
        public AddObjectDialog()
        {
            InitializeComponent();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            ObjName.SelectAll();
            ObjName.Focus();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Would like to validate that name is valid but how to do that to the ModelView?

            this.DialogResult = true;
        }


    }
}

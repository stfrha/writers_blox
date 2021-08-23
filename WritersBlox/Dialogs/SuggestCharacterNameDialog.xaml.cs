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
    /// Interaction logic for SuggestCharacterNameDialog.xaml
    /// </summary>
    public partial class SuggestCharacterNameDialog : Window
    {
        public SuggestCharacterNameDialog()
        {
            InitializeComponent();
        }

        private void GenderSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SuggestNameViewModel vm = DataContext as SuggestNameViewModel;

            vm.SetSelectedGender();
        }

        private void CountrySelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SuggestNameViewModel vm = DataContext as SuggestNameViewModel;

            vm.SetSelectedCountry();
        }

        private void DecadeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SuggestNameViewModel vm = DataContext as SuggestNameViewModel;

            vm.SetSelectedDecade();
        }

        private void NameSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SuggestNameViewModel vm = DataContext as SuggestNameViewModel;

            vm.SetSelectedName();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}

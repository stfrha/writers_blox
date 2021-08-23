using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using WritersBlox.Pages;

namespace WritersBlox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _removeMeFromBackNavigation;

        public MainWindow()
        {
            InitializeComponent();
            MyViewModel.MainWindow = this;
            MyViewModel.EditFrame = _editFrame;
            MyViewModel.TimelineGrid = _timelineGrid;
            MyViewModel.MainTree = mainTree;
            _removeMeFromBackNavigation = false;
            this.Closing += new System.ComponentModel.CancelEventHandler(MainWindow_Closing);
            MyViewModel.LoadRecentList();
            MyViewModel.LoadPersonNames();
            MyViewModel.GeInstalled();
            Thread.Sleep(3000);
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

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            // Get the x and y coordinates of the mouse pointer.
            System.Windows.Point position = e.GetPosition(this);

            // MyViewModel.XPos = position.X;
            // MyViewModel.YPos = position.Y;
        }

        private void TimelineRuler_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            bool handle = (Keyboard.Modifiers & ModifierKeys.Control) > 0;
            if (!handle)
                return;

            Point mP = e.GetPosition(sender as IInputElement);

            if (e.Delta != 0)
            {
                _timelineGrid.Zoom(mP.X, 1 - e.Delta / 120 * 0.15);
            }
        }

        private void _editFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (e == null) return;
            if (sender == null) return;

            Frame myFrame = sender as Frame;
            Page newPage = e.Content as Page;

            // Remove history when loaded a new project (after new projet or open project)
            if (MyViewModel.ArmRemoveNavigationHistory)
            {
                MyViewModel.ArmRemoveNavigationHistory = false;

                if (!myFrame.CanGoBack && !myFrame.CanGoForward)
                {
                    return;
                }

                var entry = myFrame.RemoveBackEntry();
                while (entry != null)
                {
                    entry = myFrame.RemoveBackEntry();
                }
            }
        


            if (_removeMeFromBackNavigation)
            {
                _removeMeFromBackNavigation = false;
                myFrame.NavigationService.RemoveBackEntry();
            }

            if (newPage is SplashPage) _removeMeFromBackNavigation = true;


            MyViewModel.BeingEdited = newPage.DataContext;

            if (newPage.DataContext.GetType().IsSubclassOf(typeof(Blox))) 
            {
                Blox eventBlox = newPage.DataContext as Blox;
                MyViewModel.EventsBeingEdited = eventBlox.StoryEvents;
            }
            else MyViewModel.EventsBeingEdited = null;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MyViewModel.SetSplashScreen();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = MyViewModel.CanClose();
        }        

    }
}

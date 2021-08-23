using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml;
using System.Xml.Serialization;
using MicroMvvm;
using GongSolutions.Wpf.DragDrop;
using GongSolutions.Wpf.DragDrop.Utilities;
using DragDrop = GongSolutions.Wpf.DragDrop.DragDrop;
using System.Windows.Controls;
using WritersBlox.Pages;
using WritersBlox.Views;
using System.Windows.Media.Imaging;
using WritersBlox.Dialogs;
using System.Diagnostics;
using Microsoft.Win32;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Device.Location;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Navigation;
using System.Reflection;
using System.Windows.Resources;
using System.Xml.Schema;

namespace WritersBlox
{
    public class WritersBloxVewModel : ViewModelBase, IDropTarget, IDragSource
    {
        #region Declarations
        
        ObservableCollection<DataChild> _root = new ObservableCollection<DataChild>();
      
        ObservableCollection<ProjectInformation> _recentList = new ObservableCollection<ProjectInformation>();

        private Object _beingEdited;
        private WBObjectList<StoryEvent> _eventsBeingEdited;
        private bool _isModified;
        private TimelineDataGrid _timelineGrid;
        private double _xPos;
        private double _yPos;

        private string _message;
        private string _message2;

        private Frame _editFrame;
        private TreeView _mainTree;

        private ProjectInformation _currentProject;

        private bool _armRemoveNavigationHistory;

        private List<Player> _listOfPersons = new List<Player>();

        private Window _mainWindow;

        private bool _geInstalled;

        #endregion

        #region Constructors

         public WritersBloxVewModel()
        {
            _isModified = false;
            _beingEdited = null;
            _geInstalled = false;

            ArmRemoveNavigationHistory = false;
        }

        #endregion

        #region Properties

        public ObservableCollection<DataChild> Root
        {
            get { return _root; }
            set { _root = value; }
        }

        public ObservableCollection<ProjectInformation> RecentList
        {
            get { return _recentList; }
            set { _recentList = value; }
        }

        public Frame EditFrame
        {
            get { return _editFrame; }
            set { _editFrame = value; }
        }

        public TreeView MainTree
        {
            get { return _mainTree; }
            set { _mainTree = value; }
        }

        public Object BeingEdited
        {
            get { return _beingEdited; }
            set
            {
                _beingEdited = value;
                OnPropertyChanged("BeingEdited");
            }
        }

        public WBObjectList<StoryEvent> EventsBeingEdited
        {
            get { return _eventsBeingEdited; }
            set
            {
                _eventsBeingEdited = value;
                OnPropertyChanged("EventsBeingEdited");
            }
        }

        public TimelineDataGrid TimelineGrid
        {
            get { return _timelineGrid; }
            set
            {
                _timelineGrid = value;
                OnPropertyChanged("TimelineGrid");
            }
        }

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged("Message");
            }
        }

        public string Message2
        {
            get { return _message2; }
            set
            {
                _message2 = value;
                OnPropertyChanged("Message2");
            }
        }

        public double XPos
        {
            get { return _xPos; }
            set
            {
                _xPos = value;
                Message = value.ToString();
                OnPropertyChanged("XPos");
            }
        }

        public double YPos
        {
            get { return _yPos; }
            set
            {
                _yPos = value;
                OnPropertyChanged("YPos");
            }
        }

        public ProjectInformation CurrentProject
        {
            get { return _currentProject; }
            set
            {
                _currentProject = value;
                OnPropertyChanged("CurrentProject");
            }
        }

        public bool ArmRemoveNavigationHistory
        {
            get { return _armRemoveNavigationHistory; }
            set { _armRemoveNavigationHistory = value; }
        }

        public Window MainWindow
        {
            get { return _mainWindow; }
            set { _mainWindow = value; }
        }

        #endregion

        #region Commands

        #region AddScene

        void AddSceneExecute(Object parameter)
        {
            if (parameter == null)
            {
                if (MainTree.SelectedItem is DataChild)
                {
                    parameter = MainTree.SelectedItem;
                }
                else
                {
                    parameter = FindObjectOwner(MainTree.SelectedItem);
                    if (parameter == null) return;
                }
            }

            // Find the parameter
            DataChild curChild = parameter as DataChild;

            // TODO: We are about to change edit view, possibly should we ask if this is ok!

            // Instantiate the dialog box
            AddObjectDialog dlg = new AddObjectDialog();

            // Instantiate the corresponding ModelView
            AddObjectDialogModelView mvDlg = new AddObjectDialogModelView("Lägg till en scen", "Skriv scenens rubrik:", "Scenens rubrik");

            // Set Datacontext of dialog to ModelView:
            dlg.DataContext = mvDlg;

            dlg.Owner = MainWindow;

            // Open the dialog box modally 
            dlg.ShowDialog();

            // Process data entered by user if dialog box is accepted
            if (dlg.DialogResult == true)
            {
                // Create the Scene here
                Scene nS = new Scene(mvDlg.NewObjectName, "");
                BeingEdited = nS;
                curChild.Children.Add(nS);
                nS.IsSelected = true;
                curChild.IsExpanded = true;
                _isModified = true;
                EditScenePage nextPage = new EditScenePage(nS, this);
                EditFrame.NavigationService.Navigate(nextPage);

            }
        }

        bool CanAddSceneExecute(Object parameter)
        {
            if (parameter is DataChild) return true;

            if (MainTree == null) return false;

            if (MainTree.SelectedItem == null) return false;

            return true;
        }

        public ICommand AddScene
        {
            get
            {
                return new RelayCommand<Object>(parameter => AddSceneExecute(parameter), parameter => CanAddSceneExecute(parameter));
            }
        }

        #endregion

        #region AddCharacter

        void AddCharacterExecute(Object parameter)
        {
            if (parameter == null)
            {
                if (MainTree.SelectedItem is DataChild)
                {
                    parameter = MainTree.SelectedItem;
                }
                else
                {
                    parameter = FindObjectOwner(MainTree.SelectedItem);
                    if (parameter == null) return;
                }
            }

            // Find the parameter
            DataChild curChild = parameter as DataChild;

            // TODO: We are about to change edit view, possibly should we ask if this is ok!

            // Instantiate the dialog box
            SuggestCharacterNameDialog dlg = new SuggestCharacterNameDialog();

            // Instantiate the corresponding ModelView
            SuggestNameViewModel mvDlg = new SuggestNameViewModel(_listOfPersons);

            // Set Datacontext of dialog to ModelView:
            dlg.DataContext = mvDlg;
            mvDlg.InitialiseData();

            // Open the dialog box modally 
            dlg.Owner = MainWindow;
            dlg.ShowDialog();

            // Process data entered by user if dialog box is accepted
            if (dlg.DialogResult == true)
            {
                // Create the Character here

                Character nC = new Character(mvDlg.SelectedFullname, "", "", "");
                // BeingEdited = nC;
                curChild.Children.Add(nC);
                nC.IsSelected = true;
                curChild.IsExpanded = true;
                _isModified = true;
                EditCharacterPage nextPage = new EditCharacterPage(nC, this);
                EditFrame.NavigationService.Navigate(nextPage);
            }

        }

        bool CanAddCharacterExecute(Object parameter)
        {
            if (parameter is DataChild) return true;

            if (MainTree == null) return false;

            if (MainTree.SelectedItem == null) return false;

            return true;
        }

        public ICommand AddCharacter
        {
            get
            {
                return new RelayCommand<Object>(parameter => AddCharacterExecute(parameter), parameter => CanAddCharacterExecute(parameter));
            }
        }

        #endregion

        #region SuggestCharacterName

        void SuggestCharacterNameExecute()
        {
            // Instantiate the dialog box
            SuggestCharacterNameDialog dlg = new SuggestCharacterNameDialog();

            // Instantiate the corresponding ModelView
            SuggestNameViewModel mvDlg = new SuggestNameViewModel(_listOfPersons);

            // Set Datacontext of dialog to ModelView:
            dlg.DataContext = mvDlg;
            mvDlg.InitialiseData();

            // Open the dialog box modally 
            dlg.Owner = MainWindow;
            dlg.ShowDialog();

            // Process data entered by user if dialog box is accepted
            if (dlg.DialogResult == true)
            {
                Character be = BeingEdited as Character;

                be.Name = mvDlg.SelectedFullname;
            }
        }

        bool CanSuggestCharacterNameExecute()
        {
            return true;
        }

        public ICommand SuggestCharacterName
        {
            get
            {
                return new RelayCommand(SuggestCharacterNameExecute, CanSuggestCharacterNameExecute);
            }
        }

        #endregion

        #region AddLocation

        void AddLocationExecute(Object parameter)
        {
            if (parameter == null)
            {
                if (MainTree.SelectedItem is DataChild)
                {
                    parameter = MainTree.SelectedItem;
                }
                else
                {
                    parameter = FindObjectOwner(MainTree.SelectedItem);
                    if (parameter == null) return;
                }
            }

            // Find the parameter
            DataChild curChild = parameter as DataChild;

            // TODO: We are about to change edit view, possibly should we ask if this is ok!

            // Instantiate the dialog box
            AddObjectDialog dlg = new AddObjectDialog();

            // Instantiate the corresponding ModelView
            AddObjectDialogModelView mvDlg = new AddObjectDialogModelView("Lägg till en plats", "Skriv in platsens rubruk:", "Rubrik på platsen");

            // Set Datacontext of dialog to ModelView:
            dlg.DataContext = mvDlg;

            // Open the dialog box modally 
            dlg.Owner = MainWindow;
            dlg.ShowDialog();

            // Process data entered by user if dialog box is accepted
            if (dlg.DialogResult == true)
            {
                // Create the Scene here
                Location nL = new Location(mvDlg.NewObjectName, "", "", "", "", "", "", "");
                // BeingEdited = nL;
                curChild.Children.Add(nL);
                nL.IsSelected = true;
                curChild.IsExpanded = true;
                _isModified = true;
                EditLocationPage nextPage = new EditLocationPage(nL, this);
                EditFrame.NavigationService.Navigate(nextPage);

            }
        }

        bool CanAddLocationExecute(Object parameter)
        {
            if (parameter is DataChild) return true;

            if (MainTree == null) return false;

            if (MainTree.SelectedItem == null) return false;

            return true;
        }

        public ICommand AddLocation
        {
            get
            {
                return new RelayCommand<Object>(parameter => AddLocationExecute(parameter), parameter => CanAddLocationExecute(parameter));
            }
        }

        #endregion

        #region AddStoryEvent

        void AddStoryEventExecute(Object parameter)
        {
            if (parameter == null)
            {
                if (MainTree.SelectedItem is DataChild)
                {
                    parameter = MainTree.SelectedItem;
                }
                else
                {
                    parameter = FindObjectOwner(MainTree.SelectedItem);
                    if (parameter == null) return;
                }
            }

            // Find the parameter
            DataChild curChild = parameter as DataChild;

            // TODO: We are about to change edit view, possibly should we ask if this is ok!

            // Instantiate the dialog box
            AddObjectDialog dlg = new AddObjectDialog();

            // Instantiate the corresponding ModelView
            AddObjectDialogModelView mvDlg = new AddObjectDialogModelView("Lägg till en händelse", "Skriv in en rubrik på händelsen:", "Rubrik på händelsen");

            // Set Datacontext of dialog to ModelView:
            dlg.DataContext = mvDlg;

            // Open the dialog box modally 
            dlg.Owner = MainWindow;
            dlg.ShowDialog();

            // Process data entered by user if dialog box is accepted
            if (dlg.DialogResult == true)
            {
                // Create the Scene here
                StoryEvent nSE = new StoryEvent(mvDlg.NewObjectName, "");
                // BeingEdited = nSE;
                curChild.Children.Add(nSE);
                nSE.IsSelected = true;
                curChild.IsExpanded = true;
                _isModified = true;
                EditEventPage nextPage = new EditEventPage(nSE, this);
                EditFrame.NavigationService.Navigate(nextPage);

            }
        }

        bool CanAddStoryEventExecute(Object parameter)
        {
            if (parameter is DataChild) return true;

            if (MainTree == null) return false;

            if (MainTree.SelectedItem == null) return false;

            return true;
        }

        public ICommand AddStoryEvent
        {
            get
            {
                return new RelayCommand<Object>(parameter => AddStoryEventExecute(parameter), parameter => CanAddStoryEventExecute(parameter));
            }
        }

        #endregion

        #region AddFolder

        void AddFolderExecute(Object parameter)
        {
            if (parameter == null)
            {
                if (MainTree.SelectedItem is DataChild)
                {
                    parameter = MainTree.SelectedItem;
                }
                else
                {
                    parameter = FindObjectOwner(MainTree.SelectedItem);
                    if (parameter == null) return;
                }
            }


            // Find the parameter
            DataChild curChild = parameter as DataChild;

            // Instantiate the dialog box
            AddObjectDialog dlg = new AddObjectDialog();

            // Instantiate the corresponding ModelView
            AddObjectDialogModelView mvDlg = new AddObjectDialogModelView("Lägg till en katalog", "Skriv in namnet på katalogen:", "Katalogens namn");

            // Set Datacontext of dialog to ModelView:
            dlg.DataContext = mvDlg;

            // Open the dialog box modally 
            dlg.ShowDialog();

            // Process data entered by user if dialog box is accepted
            if (dlg.DialogResult == true)
            {
                // Create the DataChild here
                DataChild nDC = new DataChild( mvDlg.NewObjectName );

                if (curChild != null)
                {
                    curChild.Children.Insert(5, nDC);
                    curChild.IsExpanded = true;
                }
                else
                {
                    Root.Insert(5, nDC);
                }
                nDC.IsSelected = true;
                nDC.IsExpanded = true;
                _isModified = true;
            }
        }

        bool CanAddFolderExecute(Object parameter)
        {

            if (parameter is DataChild) return true;

            if (MainTree == null) return false;

            if (MainTree.SelectedItem == null) return false;

            return true;


            //// Only allow to add folder if we point at a folder or nothing at all, 
            //// in which case we assume that the folder should be added to the root.
            //if ((parameter is DataChild) || (parameter == null))
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
        }

        public ICommand AddFolder
        {
            get
            {
                return new RelayCommand<Object>(parameter => AddFolderExecute(parameter), parameter => CanAddFolderExecute(parameter));
            }
        }

        #endregion

        #region DeleteObject

        void DeleteObjectExecute(Object parameter)
        {
            if (parameter == null)
            {
                if ((MainTree.SelectedItem is DataChild) || (MainTree.SelectedItem is StoryEvent) || (MainTree.SelectedItem is Blox))
                {
                    parameter = MainTree.SelectedItem;
                }
                else
                {
                    return;
                }
            }

            if (parameter is Blox)
            {
                if (parameter is Character)
                {
                    if (MessageBox.Show(MainWindow, "Vill du ta bort karaktären " + ((Character)parameter).Name + " från projektet?", "Ta bort karaktär", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                    {
                        return;
                    }
                }

                if (parameter is Location)
                {
                    if (MessageBox.Show(MainWindow, "Vill du ta bort platsen " + ((Location)parameter).Headline + " från projektet?", "Ta bort plats", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                    {
                        return;
                    }
                }

                if (parameter is Scene)
                {
                    if (MessageBox.Show(MainWindow, "Vill du ta bort scenen " + ((Scene)parameter).Headline + " från projektet?", "Ta bort scen", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                    {
                        return;
                    }
                }

                DeleteBloxExecute(parameter);
            }

            if (parameter is StoryEvent)
            {
                if (MessageBox.Show(MainWindow, "Vill du ta bort händelsen " + ((StoryEvent)parameter).Headline + " från projektet?", "Ta bort händelse", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    return;
                }

                DeleteEventExecute(parameter);
            }

            if (parameter is DataChild)
            {
                if (MessageBox.Show(MainWindow, "Vill du ta bort katalogen " + ((DataChild)parameter).FolderName + " och allt dess innehåll från projektet?", "Ta bort katalog", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    return;
                }

                DeleteFolderExecute(parameter);
            }
        }

        bool CanDeleteObjectExecute(Object parameter)
        {
            if (parameter is DataChild) return true;
            if (parameter is Scene) return true;
            if (parameter is Character) return true;
            if (parameter is Location) return true;
            if (parameter is StoryEvent) return true;

            if (MainTree == null) return false;

            if (MainTree.SelectedItem == null) return false;

            return true;
        }

        public ICommand DeleteObject
        {
            get
            {
                return new RelayCommand<Object>(parameter => DeleteObjectExecute(parameter), parameter => CanDeleteObjectExecute(parameter));
            }
        }

        #endregion

        #region AddSceneToEvent

        void AddSceneToEventExecute(Object parameter)
        {
            // Instantiate the dialog box
            SelectObjectDialog dlg = new SelectObjectDialog();

            ObservableCollection<DataChild> tColl = PrepareObjectSelectionCollection(typeof(Scene));

            SelectObjectDialogViewModel mvDlg = new SelectObjectDialogViewModel("Välj scen", "Välj scenen som skall läggas till händelsen",  tColl);
            
            // Set Datacontext of dialog to ModelView:
            dlg.DataContext = mvDlg;

            // Open the dialog box modally 
            dlg.ShowDialog();

            // Process data entered by user if dialog box is accepted
            if (dlg.DialogResult == true)
            {
                Scene selectedScene = mvDlg.SelectedObject as Scene;
                StoryEvent sE = BeingEdited as StoryEvent;
                sE.Scenes.Add(selectedScene);
                selectedScene.StoryEvents.Add(sE);
            }
        }

        bool CanAddSceneToEventExecute(Object parameter)
        {
            return true;
        }

        public ICommand AddSceneToEvent
        {
            get
            {
                return new RelayCommand<Object>(parameter => AddSceneToEventExecute(parameter), parameter => CanAddSceneToEventExecute(parameter));
            }
        }

        #endregion

        #region AddCharacterToEvent

        void AddCharacterToEventExecute(Object parameter)
        {
            // Instantiate the dialog box
            SelectObjectDialog dlg = new SelectObjectDialog();

            ObservableCollection<DataChild> tColl = PrepareObjectSelectionCollection(typeof(Character));

            SelectObjectDialogViewModel mvDlg = new SelectObjectDialogViewModel("Välj karaktär", "Välj karaktären som skall läggas till händelsen", tColl);

            // Set Datacontext of dialog to ModelView:
            dlg.DataContext = mvDlg;

            // Open the dialog box modally 
            dlg.ShowDialog();

            // Process data entered by user if dialog box is accepted
            if (dlg.DialogResult == true)
            {
                Character selectedCharacter = mvDlg.SelectedObject as Character;
                StoryEvent sE = BeingEdited as StoryEvent;
                sE.Characters.Add(selectedCharacter);
                selectedCharacter.StoryEvents.Add(sE);
            }
        }

        bool CanAddCharacterToEventExecute(Object parameter)
        {
            return true;
        }

        public ICommand AddCharacterToEvent
        {
            get
            {
                return new RelayCommand<Object>(parameter => AddCharacterToEventExecute(parameter), parameter => CanAddCharacterToEventExecute(parameter));
            }
        }

        #endregion

        #region AddLocationToEvent

        void AddLocationToEventExecute(Object parameter)
        {
            // Instantiate the dialog box
            SelectObjectDialog dlg = new SelectObjectDialog();

            ObservableCollection<DataChild> tColl = PrepareObjectSelectionCollection(typeof(Location));

            SelectObjectDialogViewModel mvDlg = new SelectObjectDialogViewModel("Välj plats", "Välj platsen som skall läggas till händelsen", tColl);

            // Set Datacontext of dialog to ModelView:
            dlg.DataContext = mvDlg;

            // Open the dialog box modally 
            dlg.ShowDialog();

            // Process data entered by user if dialog box is accepted
            if (dlg.DialogResult == true)
            {
                Location selectedLocation = mvDlg.SelectedObject as Location;
                StoryEvent sE = BeingEdited as StoryEvent;
                sE.Locations.Add(selectedLocation);
                selectedLocation.StoryEvents.Add(sE);
            }
        }

        bool CanAddLocationToEventExecute(Object parameter)
        {
            return true;
        }

        public ICommand AddLocationToEvent
        {
            get
            {
                return new RelayCommand<Object>(parameter => AddLocationToEventExecute(parameter), parameter => CanAddLocationToEventExecute(parameter));
            }
        }

        #endregion

        #region AddEventToBlox

        void AddEventToBloxExecute(Object parameter)
        {
            string typeName = "";


            // Determine the type of Blox and set data accordingly, 
            // may seem ridiculuse but I want to be safe from class name changes.
            switch(BeingEdited.GetType().ToString())
            {
                case "WritersBlox.Scene":
                    typeName = "scenen";
                    break;
                case "WritersBlox.Character":
                    typeName = "karaktären";
                    break;
                case "WritersBlox.Location":
                    typeName = "platsen";
                    break;
            }

            // Instantiate the dialog box
            SelectObjectDialog dlg = new SelectObjectDialog();

            ObservableCollection<DataChild> tColl = PrepareObjectSelectionCollection(typeof(StoryEvent));

            SelectObjectDialogViewModel mvDlg = new SelectObjectDialogViewModel("Välj händelse", "Välj händelsen som skall läggas till " + typeName, tColl);

            // Set Datacontext of dialog to ModelView:
            dlg.DataContext = mvDlg;

            // Open the dialog box modally 
            dlg.ShowDialog();

            // Process data entered by user if dialog box is accepted
            if (dlg.DialogResult == true)
            {
                StoryEvent selectedEvent = mvDlg.SelectedObject as StoryEvent;
                Blox cB = BeingEdited as Blox;
                cB.StoryEvents.Add(selectedEvent);

                if (BeingEdited is Scene)
                {
                    selectedEvent.Scenes.Add(cB as Scene);
                }
                else if (BeingEdited is Character)
                {
                    selectedEvent.Characters.Add(cB as Character);
                }
                else if (BeingEdited is Location)
                {
                    selectedEvent.Locations.Add(cB as Location);
                }
            }
        }

        bool CanAddEventToBloxExecute(Object parameter)
        {
            return true;
        }

        public ICommand AddEventToBlox
        {
            get
            {
                return new RelayCommand<Object>(parameter => AddEventToBloxExecute(parameter), parameter => CanAddEventToBloxExecute(parameter));
            }
        }

        #endregion

        #region DeleteBlox

        void DeleteBloxExecute(Object parameter)
        {
            Blox blox = parameter as Blox;

            IList bloxCollection = FindObjectCollection(blox);

            if (bloxCollection == null)
            {
                // Something went wrong
                return;
            }
            else
            {
                // Remove references to this Blox in all events

                foreach (StoryEvent sE in blox.StoryEvents)
                {
                    sE.Scenes.Remove(blox as Scene);
                    sE.Characters.Remove(blox as Character);
                    sE.Locations.Remove(blox as Location);
                }

                // Remove Blox from its DataChild
                bloxCollection.Remove(blox);
            }
        }

        bool CanDeleteBloxExecute(Object parameter)
        {
            if (parameter is Blox)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public ICommand DeleteBlox
        {
            get
            {
                return new RelayCommand<Object>(parameter => DeleteBloxExecute(parameter), parameter => CanDeleteBloxExecute(parameter));
            }
        }

        #endregion

        #region DeleteEvent

        void DeleteEventExecute(Object parameter)
        {
            StoryEvent cSE = parameter as StoryEvent;

            IList eventCollection = FindObjectCollection(cSE);

            if (eventCollection == null)
            {
                // Something went wrong
                return;
            }
            else
            {
                // Remove references to this Event in all Blox

                foreach (Scene sEI in cSE.Scenes)
                {
                    sEI.StoryEvents.Remove(cSE);
                }

                foreach (Character cEI in cSE.Characters)
                {
                    cEI.StoryEvents.Remove(cSE);
                }

                foreach (Location lEI in cSE.Locations)
                {
                    lEI.StoryEvents.Remove(cSE);
                }

                // Remove Blox from its DataChild
                eventCollection.Remove(cSE);
            }
        }

        bool CanDeleteEventExecute(Object parameter)
        {
            if (parameter is StoryEvent)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public ICommand DeleteEvent
        {
            get
            {
                return new RelayCommand<Object>(parameter => DeleteEventExecute(parameter), parameter => CanDeleteEventExecute(parameter));
            }
        }

        #endregion

        #region DeleteFolder

        void DeleteFolderExecute(Object parameter)
        {
            DataChild cF = parameter as DataChild;

            IList folderCollection = FindObjectCollection(cF);

            // TOOD: Check here that Root will not become empty!!!
            // I don't think it is allowed.

            if (folderCollection == null)
            {
                
                return;
            }
            else
            {
                // There are some, probably stupid, iteration loops below. 
                // There are probably better ways to do it

                // Remove all Blox from the DataChild
                IEnumerable<Blox> bC = cF.Children.OfType<Blox>();
                while (bC.Count<Blox>() > 0)
                {
                    DeleteBloxExecute(bC.First<Blox>());
                    bC = cF.Children.OfType<Blox>();
                }

                // Remove all StoryEvent from the DataChild
                IEnumerable<StoryEvent> seC = cF.Children.OfType<StoryEvent>();
                while (seC.Count<StoryEvent>() > 0)
                {
                    DeleteEventExecute(seC.First<StoryEvent>());
                    seC = cF.Children.OfType<StoryEvent>();
                }

                // Recursively remove all DataChild from the DataChild
                IEnumerable<DataChild> dcC = cF.Children.OfType<DataChild>();
                while (dcC.Count<DataChild>() > 0)
                {
                    DeleteFolderExecute(dcC.First<DataChild>());
                    dcC = cF.Children.OfType<DataChild>();
                }  

                // Remove the DataChild
                folderCollection.Remove(cF);
            }
        }

        bool CanDeleteFolderExecute(Object parameter)
        {
            if (parameter is DataChild)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public ICommand DeleteFolder
        {
            get
            {
                return new RelayCommand<Object>(parameter => DeleteFolderExecute(parameter), parameter => CanDeleteFolderExecute(parameter));
            }
        }

        #endregion

        #region RemoveBloxFromEvent

        void RemoveBloxFromEventExecute(Object parameter)
        {
            StoryEvent sE = BeingEdited as StoryEvent;

            sE.Scenes.Remove(parameter as Scene);
            sE.Characters.Remove(parameter as Character);
            sE.Locations.Remove(parameter as Location);

            // Now we remove link to this event in the Blox that had it
            Blox b = parameter as Blox;
            b.StoryEvents.Remove(sE);
        }

        bool CanRemoveBloxFromEventExecute(Object parameter)
        {
            if (parameter is Blox) return true;
            return false;
        }

        public ICommand RemoveBloxFromEvent
        {
            get
            {
                return new RelayCommand<Object>(parameter => RemoveBloxFromEventExecute(parameter), parameter => CanRemoveBloxFromEventExecute(parameter));
            }
        }
   
        
        #endregion

        #region RemoveEventFromBlox

        void RemoveEventFromBloxExecute(Object parameter)
        {
            StoryEvent sE = parameter as StoryEvent;

            Blox b = BeingEdited as Blox;
            b.StoryEvents.Remove(sE);

            sE.Scenes.Remove(b as Scene);
            sE.Characters.Remove(b as Character);
            sE.Locations.Remove(b as Location);
        }

        bool CanRemoveEventFromBloxExecute(Object parameter)
        {
            if (parameter is StoryEvent) return true;
            return false;
        }

        public ICommand RemoveEventFromBlox
        {
            get
            {
                return new RelayCommand<Object>(parameter => RemoveEventFromBloxExecute(parameter), parameter => CanRemoveEventFromBloxExecute(parameter));
            }
        }


        #endregion

        #region EditScene

        void EditSceneExecute(Object parameter)
        {
            if (parameter != null)
            {
                Scene scene = parameter as Scene;

                _isModified = true;
                scene.IsSelected = true;
                ExpandToTop(scene);
                
                EditScenePage nextPage = new EditScenePage( scene, this );
                EditFrame.NavigationService.Navigate(nextPage);

            }
        }

        bool CanEditSceneExecute(Object parameter)
        {
            if (parameter is Scene)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public ICommand EditScene
        {
            get
            {
                return new RelayCommand<Object>(parameter => EditSceneExecute(parameter), parameter => CanEditSceneExecute(parameter));
            }
        }

        #endregion

        #region EditCharacter

        void EditCharacterExecute(Object parameter)
        {
            if (parameter != null)
            {
                Character character = parameter as Character;

                _isModified = true;
                character.IsSelected = true;
                ExpandToTop(character);

                EditCharacterPage nextPage = new EditCharacterPage(character, this);
                EditFrame.NavigationService.Navigate(nextPage);

            }
        }

        bool CanEditCharacterExecute(Object parameter)
        {
            if (parameter is Character)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public ICommand EditCharacter
        {
            get
            {
                return new RelayCommand<Object>(parameter => EditCharacterExecute(parameter), parameter => CanEditCharacterExecute(parameter));
            }
        }

        #endregion

        #region EditLocation

        void EditLocationExecute(Object parameter)
        {
            if (parameter != null)
            {
                Location location = parameter as Location;

                _isModified = true;
                location.IsSelected = true;
                ExpandToTop(location);

                EditLocationPage nextPage = new EditLocationPage( location, this );
                EditFrame.NavigationService.Navigate(nextPage);

            }
        }

        bool CanEditLocationExecute(Object parameter)
        {
            if (parameter is Location)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public ICommand EditLocation
        {
            get
            {
                return new RelayCommand<Object>(parameter => EditLocationExecute(parameter), parameter => CanEditLocationExecute(parameter));
            }
        }

        #endregion

        #region EditEvent

        void EditEventExecute(Object parameter)
        {
            if (parameter != null)
            {
                StoryEvent sEvent = parameter as StoryEvent;

                _isModified = true;
                sEvent.IsSelected = true;
                ExpandToTop(sEvent);

                EditEventPage nextPage = new EditEventPage(parameter as StoryEvent, this);
                EditFrame.NavigationService.Navigate(nextPage);

            }
        }

        bool CanEditEventExecute(Object parameter)
        {
            if (parameter is StoryEvent)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public ICommand EditEvent
        {
            get
            {
                return new RelayCommand<Object>(parameter => EditEventExecute(parameter), parameter => CanEditEventExecute(parameter));
            }
        }

        #endregion

        #region GoToStart

        void GoToStartExecute()
        {
            StartPage nextPage = new StartPage(this);
            EditFrame.NavigationService.Navigate(nextPage);
        }

        bool CanGoToStartExecute()
        {
            if (CurrentProject == null) return false;
            return true;
        }

        public ICommand GoToStart
        {
            get
            {
                return new RelayCommand(GoToStartExecute, CanGoToStartExecute);
            }
        }


        #endregion

        #region SelectStartTime

        void SelectStartTimeExecute(Object parameter)
        {
        }

        bool CanSelectStartTimeExecute(Object parameter)
        {
            return true;
        }

        public ICommand SelectStartTime
        {
            get
            {
                return new RelayCommand<Object>(parameter => SelectStartTimeExecute(parameter), parameter => CanSelectStartTimeExecute(parameter));
            }
        }

        #endregion

        #region RenameFolder

        void RenameFolderExecute(Object parameter)
        {

            // Find the parameter
            DataChild curChild = parameter as DataChild;

            // TODO: We are about to change edit view, possibly should we ask if this is ok!

            // Instantiate the dialog box
            AddObjectDialog dlg = new AddObjectDialog();

            // Instantiate the corresponding ModelView
            AddObjectDialogModelView mvDlg = new AddObjectDialogModelView("Byt namn på katalog", "Skriv in nytt namn på katalogen:", curChild.FolderName);

            // Set Datacontext of dialog to ModelView:
            dlg.DataContext = mvDlg;

            // Open the dialog box modally 
            dlg.ShowDialog();

            // Process data entered by user if dialog box is accepted
            if (dlg.DialogResult == true)
            {
                curChild.FolderName = mvDlg.NewObjectName;
                _isModified = true;
            }
        }

        bool CanRenameFolderExecute(Object parameter)
        {
            if (parameter is DataChild)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public ICommand RenameFolder
        {
            get
            {
                return new RelayCommand<Object>(parameter => RenameFolderExecute(parameter), parameter => CanRenameFolderExecute(parameter));
            }
        }

        #endregion

        #region NewProject
        
        void NewProjectExecute()
        {
            if (!CloseAndSavePrompt("Nytt projekt", "skapar ett nytt projekt")) return;

            var fileDialog = new System.Windows.Forms.SaveFileDialog();

            fileDialog.Filter = "Writers Blox projektfil (*.wbx)|*.wbx";

            var result = fileDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                // Instantiate the dialog box
                AddObjectDialog dlg = new AddObjectDialog();

                // Instantiate the corresponding ModelView
                AddObjectDialogModelView mvDlg = new AddObjectDialogModelView("Skapa nytt projekt", "Ge projektet en titel:", "Projektitel");

                // Set Datacontext of dialog to ModelView:
                dlg.DataContext = mvDlg;

                // Open the dialog box modally 
                dlg.ShowDialog();

                // Process data entered by user if dialog box is accepted
                if (dlg.DialogResult == true)
                {
                    CurrentProject = new ProjectInformation();

                    CurrentProject.Path = fileDialog.FileName;
                    CurrentProject.CreatedDate = DateTime.Now.ToString();
                    if (mvDlg.NewObjectName == "")
                    {
                        CurrentProject.Title = Path.GetFileNameWithoutExtension(CurrentProject.Path);
                    }
                    else
                    {
                        CurrentProject.Title = mvDlg.NewObjectName;
                    }
                    Root.Clear();
                    Root.Add(new DataChild(CurrentProject.Title));
                    _isModified = false;
                    ArmRemoveNavigationHistory = true;
                    StartPage nextPage = new StartPage(this);
                    EditFrame.NavigationService.Navigate(nextPage);

                    AddProjectToRecentList(CurrentProject);

                }
            }
        }

        bool CanNewProjectExecute()
        {
            return true;
        }

        public ICommand NewProject
        {
            get
            {
                return new RelayCommand(NewProjectExecute, CanNewProjectExecute);
            }
        }

        #endregion

        #region SaveProject

        void SaveProjectExecute()
        {
            try
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.NewLineOnAttributes = true;
                settings.NewLineChars = "\r\n";

                XmlWriter writer = XmlWriter.Create(CurrentProject.Path, settings);

                writer.WriteStartElement("WritersBloxData");

                XmlSerializer serializer = new XmlSerializer(typeof(ProjectInformation));
                serializer.Serialize(writer, CurrentProject);
                serializer = new XmlSerializer(typeof(ObservableCollection<DataChild>));
                serializer.Serialize(writer, Root);

                writer.WriteEndElement();

                writer.Close();
            }
            catch (Exception ex)
            {
                DumpException(ex);
            }


            _isModified = false;

        }

        public static void DumpException(Exception ex)
        {
            Console.WriteLine("--------- Outer Exception Data ---------");
            WriteExceptionInfo(ex);
            ex = ex.InnerException;
            if (null != ex)
            {
                Console.WriteLine("--------- Inner Exception Data ---------");
                WriteExceptionInfo(ex);
                ex = ex.InnerException;
            }
        }
        public static void WriteExceptionInfo(Exception ex)
        {
            Console.WriteLine("Message: {0}", ex.Message);
            Console.WriteLine("Exception Type: {0}", ex.GetType().FullName);
            Console.WriteLine("Source: {0}", ex.Source);
            Console.WriteLine("StrackTrace: {0}", ex.StackTrace);
            Console.WriteLine("TargetSite: {0}", ex.TargetSite);
        }




        bool CanSaveProjectExecute()
        {
            if (CurrentProject == null) return false;

            if (Root.Count == 0)
            {
                return false;
            }
            return true;
        }

        public ICommand SaveProject
        {
            get
            {
                return new RelayCommand(SaveProjectExecute, CanSaveProjectExecute);
            }

        }

        #endregion

        #region OpenProject

        void OpenProjectExecute()
        {
            if (!CloseAndSavePrompt("Öppna projekt", "öppnar ett nytt projekt")) return;

            //if (_isModified)
            //{
            //    // Prompt for discarding all data
            //    if (MessageBox.Show(MainWindow, "Det här kommer att radera allt osparad information Är du säker att du vill öppna projektet?", "Öppna projekt - Osparad information", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            //    {
            //        return;
            //    }
            //}

            var fileDialog = new System.Windows.Forms.OpenFileDialog();

            fileDialog.CheckFileExists = true;

            fileDialog.Filter = "Writers Blox projektfil (*.wbx)|*.wbx";

            var result = fileDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                OpenProjectFileExecute(fileDialog.FileName);
            }
        }

        bool CanOpenProjectExecute()
        {
            return true;
        }

        public ICommand OpenProject
        {
            get
            {
                return new RelayCommand(OpenProjectExecute, CanOpenProjectExecute);
            }
        }


        #endregion

        #region CloseProject

        void CloseProjectExecute()
        {
            if (!CloseAndSavePrompt("Stänga projekt", "stänger det")) return;
            //if (_isModified)
            //{
            //    // Prompt for discarding all data
            //    if (MessageBox.Show(MainWindow, "Projektet innehåller osparad information. Är du säker på att du vill stänga det?", "Stänga projekt", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            //    {
            //        return;
            //    }
            //}

            CurrentProject = null;

            Root.Clear();

            _isModified = false;
            ArmRemoveNavigationHistory = true;
            SplashPage nextPage = new SplashPage(this);
            EditFrame.NavigationService.Navigate(nextPage);

        }

        bool CanCloseProjectExecute()
        {
            return true;
        }

        public ICommand CloseProject
        {
            get
            {
                return new RelayCommand(CloseProjectExecute, CanCloseProjectExecute);
            }
        }


        #endregion

        #region OpenProjectFile

        void OpenProjectFileExecute(Object parameter)
        {
            if (parameter is string)
            {
                string path = parameter as string;

                if (path.Count() == 0) return;

                if (!File.Exists(path))
                {
                    MessageBox.Show(MainWindow, "Projekftilen du försöker öppna existerar inte.", "Fel vid läsning av projektfil", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreWhitespace = true;

                XmlReader reader = XmlReader.Create(path, settings);

                reader.ReadStartElement("WritersBloxData");

                System.Xml.Serialization.XmlSerializer serializer =
                    new System.Xml.Serialization.XmlSerializer(typeof(ProjectInformation));
                CurrentProject = new ProjectInformation();
                CurrentProject = serializer.Deserialize(reader) as ProjectInformation;

                AddProjectToRecentList(CurrentProject);

                serializer = new System.Xml.Serialization.XmlSerializer(typeof(ObservableCollection<DataChild>));
                Root = serializer.Deserialize(reader) as ObservableCollection<DataChild>;

                reader.ReadEndElement();

                OnPropertyChanged("Root");

                reader.Close();

                // Now we rehydrate all links
                foreach (DataChild dc in Root)
                {
                    dc.Rehydrate(FindGUIDObject);
                }

                CurrentProject.Path = path;

                ArmRemoveNavigationHistory = true;
                StartPage nextPage = new StartPage(this);
                EditFrame.NavigationService.Navigate(nextPage);

                _isModified = false;
            }

        }

        bool CanOpenProjectFileExecute(Object parameter)
        {
            return true;
        }

        public ICommand OpenProjectFile
        {
            get
            {
                return new RelayCommand<Object>(parameter => OpenProjectFileExecute(parameter), parameter => CanOpenProjectFileExecute(parameter));
            }
        }


        #endregion

        #region PopulateProject

        void PopulateProjectExecute()
        {
            // Poulate with dummy data

            Root.Add(new DataChild("My Scenes"));

            Scene samSandraMeetScene = new Scene("Samuel möter Sandra för första gången", "I denna scen kommer Sam åkande i en taxi till Eskils hus. Han är trött efter resan. Innanför dörren står Sandra och byter om.");
            Root[0].Children.Add(samSandraMeetScene);
            
            Root[0].Children.Add(new Scene("Samuel blir utskälld av Rune", "Rune skriker så spottet flyger genom rummet."));
            Root[0].Children.Add(new Scene("Eskil motionerar med Jon", "Eskil försöker övertyga Jon om att hjälpa honom med att få tag på utredningsmaterialet."));
            Root[0].Children.Add(new Scene("Samuel bär Eskil ut ur bunkern", "Eskil är nära medvetenlöshet, Sam bär honom på axeln och får bråttom innan bovarna kommer ned med hissen."));

            Root.Add(new DataChild("My Characters"));

            Character sam = new Character("Samuel Axellson", "Sam", "Kriminalinspektör", "Sam är huvudpersonen.");
            Root[1].Children.Add(sam);

            Root[1].Children.Add(new Character("Rune Andreasson", "", "Kriminalkommisarie", "Rune är Samuels gubbsjuka och elaka gruppchef."));
            Root[1].Children.Add(new DataChild("The Dahls"));

            DataChild lC = (DataChild)Root[1].Children[7];

            _beingEdited = (Character)Root[1].Children[5];

            Character sandra = new Character("Sandra Dahl", "Suzzy North", "", "Sandra är Eskils dotter och Samuels flickvän.");
            lC.Children.Add(sandra);

            lC.Children.Add(new Character("Eskil Dahl", "", "Polisoverintendent", "Eskil är polis i Bergen. Han är Sandras far. När hans fru dog blev det svårt att hantera..."));

            Root.Add(new DataChild("My Locations"));
            Root[2].Children.Add(new Location("Samuels lägenhet", "Samuels lägenhet är en två i Sumpan. Han är ganska nöjd med den men skulle vilja ha bättre isolering till grannarna.", "Jorden", "Sverige", "Sundbyberg", "Fredsgatan 6", "17.96569219413868", "59.36746771416884"));
            Root[2].Children.Add(new Location("Bunkern", "Bunkern är en småbåtshamn insprängd i berget med en hiss som går upp till huset på klippan. Det finns två celler och ett kommunikationsrum samt en verkstad.", "Jorden", "Norge", "Turöya", "", "", ""));

            Root.Add(new DataChild("My Events"));
            StoryEvent samBorn = new StoryEvent("The birth of Sam", "Sameul was born on a cold semptember morning");
            sam.StoryEvents.Add(samBorn);
            samBorn.Characters.Add(sam);
            Root[3].Children.Add(samBorn);

            StoryEvent sandraDies = new StoryEvent("Sandra overdose", "This is the event when Sandra dies from an overdose.");
            sandra.StoryEvents.Add(sandraDies);
            sandraDies.Characters.Add(sandra);
            Root[3].Children.Add(sandraDies);

            StoryEvent samSandraMeet = new StoryEvent("Sam and Sandra first meet", "This is the event when they met, him confused, she seminaked.");
            samSandraMeetScene.StoryEvents.Add(samSandraMeet);
            samSandraMeet.Scenes.Add(samSandraMeetScene);
            Root[3].Children.Add(samSandraMeet);

            CurrentProject = new ProjectInformation();
            CurrentProject.Path = "E:\\testFile.wbx";
            CurrentProject.Title = "Lille skutts äventyr";
            CurrentProject.Description = "Lille skutt hoppar, leker och älskar bland kullar och träd. Alltid ängslig, alltid på väg bort.";

            ArmRemoveNavigationHistory = true;
            StartPage nextPage = new StartPage(this);
            EditFrame.NavigationService.Navigate(nextPage);

        }

        bool CanPopulateProjectExecute()
        {
            if (Root.Count == 0)
            {
                return true;
            }
            return false;
        }

        public ICommand PopulateProject
        {
            get
            {
                return new RelayCommand(PopulateProjectExecute, CanPopulateProjectExecute);
            }
        }

        #endregion

        #region ZoomIn

        void ZoomInExecute()
        {
            _timelineGrid.Zoom(double.PositiveInfinity, 0.7);
            //SecPerPix /= 2;
        }

        bool CanZoomInExecute()
        {
            return true;
        }

        public ICommand ZoomIn
        {
            get
            {
                return new RelayCommand(ZoomInExecute, CanZoomInExecute);
            }
        }

        #endregion

        #region ZoomOut

        void ZoomOutExecute()
        {
            _timelineGrid.Zoom(double.PositiveInfinity, 1.3);
            //SecPerPix *= 2;
        }

        bool CanZoomOutExecute()
        {
            return true;
        }

        public ICommand ZoomOut
        {
            get
            {
                return new RelayCommand(ZoomOutExecute, CanZoomOutExecute);
            }
        }

        #endregion

        #region TestGrid

        void TestGridExecute()
        {        }

        bool CanTestGridExecute()
        {
            return true;
        }

        public ICommand TestGrid
        {
            get
            {
                return new RelayCommand(TestGridExecute, CanTestGridExecute);
            }
        }

        #endregion

        #region AddImage

        void AddImageExecute(Object parameter)
        {
            var fileDialog = new System.Windows.Forms.OpenFileDialog();

            fileDialog.ReadOnlyChecked = true;

            fileDialog.Filter = "Alla bildtyper|*.jpg;*.png;*.bmp|JPEG-Bilder (*.jpg)|*.jpg|PNG-Bilder (*.png)|*.png|Bitmap-bilder (*.bmp)|*.bmp";
            
            var result = fileDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                SerializableBitmapImage src = new SerializableBitmapImage(new Uri(fileDialog.FileName, UriKind.Absolute));

                Blox selBlox = parameter as Blox;
                selBlox.Images.Add(src);

            }

        }

        bool CanAddImageExecute(Object parameter)
        {
            return (BeingEdited != null);
        }

        public ICommand AddImage
        {
            get
            {
                return new RelayCommand<Object>(parameter => AddImageExecute(parameter), parameter => CanAddImageExecute(parameter));

            }
        }

        #endregion

        #region RelinkImage

        void RelinkImageExecute(Object parameter)
        {
            if (parameter == null) return;

            if (!(parameter is SerializableBitmapImage)) return;

            SerializableBitmapImage img = parameter as SerializableBitmapImage;

            var fileDialog = new System.Windows.Forms.OpenFileDialog();

            fileDialog.ReadOnlyChecked = true;
            fileDialog.CheckFileExists = true;

            fileDialog.Filter = "Alla bildtyper|*.jpg;*.png;*.bmp|JPEG-Bilder (*.jpg)|*.jpg|PNG-Bilder (*.png)|*.png|Bitmap-bilder (*.bmp)|*.bmp";

            fileDialog.FileName = img.Image.UriSource.OriginalString;

            var result = fileDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                img.RelinkImage(new Uri(fileDialog.FileName, UriKind.Absolute));
            }

            // Here we should find the relative path difference and see if we can apply
            // it to all broken links. This would require a list of all broken links
            // Which is simple by traversing all Blox in the hunt for them
            // It would be quit impressive!

        }

        bool CanRelinkImageExecute(Object parameter)
        {
            if (parameter == null) return false;

            if (!(parameter is SerializableBitmapImage)) return false;

            SerializableBitmapImage img = parameter as SerializableBitmapImage;

            return img.BrokenLink;
        }

        public ICommand RelinkImage
        {
            get
            {
                return new RelayCommand<Object>(parameter => RelinkImageExecute(parameter), parameter => CanRelinkImageExecute(parameter));

            }
        }

        #endregion

        #region LargerImages

        void LargerImagesExecute(Object parameter)
        {
            if (BeingEdited.GetType().IsSubclassOf(typeof(Blox)))
            {
                Blox bxEd = BeingEdited as Blox;

                if (bxEd.ImageSize == ImageSizes.eMediumImage) bxEd.ImageSize = ImageSizes.eLargeImage;
                else if (bxEd.ImageSize == ImageSizes.eSmallImage) bxEd.ImageSize = ImageSizes.eMediumImage;
            }
        }

        bool CanLargerImagesExecute(Object parameter)
        {
            if (BeingEdited == null) return false;

            if (BeingEdited.GetType().IsSubclassOf(typeof(Blox)))
            {
                Blox bxEd = BeingEdited as Blox;

                if (bxEd.Images.Count == 0) return false;

                if (bxEd.ImageSize != ImageSizes.eLargeImage) return true;
            }
            return false;
        }

        public ICommand LargerImages
        {
            get
            {
                return new RelayCommand<Object>(parameter => LargerImagesExecute(parameter), parameter => CanLargerImagesExecute(parameter));
            }
        }

        #endregion

        #region SmallerImages

        void SmallerImagesExecute()
        {
            if (BeingEdited.GetType().IsSubclassOf(typeof(Blox)))
            {
                Blox bxEd = BeingEdited as Blox;

                if (bxEd.ImageSize == ImageSizes.eMediumImage) bxEd.ImageSize = ImageSizes.eSmallImage;
                else if (bxEd.ImageSize == ImageSizes.eLargeImage) bxEd.ImageSize = ImageSizes.eMediumImage;
            }
        }

        bool CanSmallerImagesExecute()
        {
            if (BeingEdited == null) return false;

            if (BeingEdited.GetType().IsSubclassOf(typeof(Blox)))
            {
                Blox bxEd = BeingEdited as Blox;

                if (bxEd.Images.Count == 0) return false;

                if (bxEd.ImageSize != ImageSizes.eSmallImage) return true;
            }
            return false;
        }

        public ICommand SmallerImages
        {
            get
            {
                return new RelayCommand(SmallerImagesExecute, CanSmallerImagesExecute);
            }
        }

        #endregion

        #region RemoveImage

        void RemoveImageExecute(Object parameter)
        {
            if (BeingEdited.GetType().IsSubclassOf(typeof(Blox)))
            {
                Blox bxEd = BeingEdited as Blox;

                if (parameter is SerializableBitmapImage)
                {
                    bxEd.Images.Remove(parameter as SerializableBitmapImage);
                }
            }
        }

        bool CanRemoveImageExecute(Object parameter)
        {
            if (BeingEdited == null) return false;

            if (BeingEdited.GetType().IsSubclassOf(typeof(Blox)))
            {
                if (parameter is SerializableBitmapImage) return true;
            }
            return false;
        }

        public ICommand RemoveImage
        {
            get
            {
                return new RelayCommand<Object>(parameter => RemoveImageExecute(parameter), parameter => CanRemoveImageExecute(parameter));
            }
        }

        #endregion

        #region ViewImage

        void ViewImageExecute(Object parameter)
        {
            if (BeingEdited.GetType().IsSubclassOf(typeof(Blox)))
            {
                Blox bxEd = BeingEdited as Blox;

                if (parameter is SerializableBitmapImage)
                {
                    ImageCollectionViewer viewer = new ImageCollectionViewer();
                    ImageViewerModelView modelView = new ImageViewerModelView();
                    modelView.SelectedImage = parameter as SerializableBitmapImage;

                    modelView.Images = bxEd.Images;
                    viewer.DataContext = modelView;

                    viewer.ShowDialog();

                }
            }
        }

        bool CanViewImageExecute(Object parameter)
        {
            if (BeingEdited == null) return false;

            if (BeingEdited.GetType().IsSubclassOf(typeof(Blox)))
            {
                if (parameter is SerializableBitmapImage) return true;
            }
            return false;
        }

        public ICommand ViewImage
        {
            get
            {
                return new RelayCommand<Object>(parameter => ViewImageExecute(parameter), parameter => CanViewImageExecute(parameter));
            }
        }

        #endregion

        #region SetProfileImage

        void SetProfileImageExecute(Object parameter)
        {
            if (BeingEdited.GetType().IsSubclassOf(typeof(Blox)))
            {
                Blox bxEd = BeingEdited as Blox;

                if (parameter is SerializableBitmapImage)
                {
                    bxEd.ProfileImage = parameter as SerializableBitmapImage;
                }
            }
        }

        bool CanSetProfileImageExecute(Object parameter)
        {
            if (BeingEdited == null) return false;

            if (BeingEdited.GetType().IsSubclassOf(typeof(Blox)))
            {
                if (parameter is SerializableBitmapImage) return true;
            }
            return false;
        }

        public ICommand SetProfileImage
        {
            get
            {
                return new RelayCommand<Object>(parameter => SetProfileImageExecute(parameter), parameter => CanSetProfileImageExecute(parameter));
            }
        }

        #endregion

        #region LaunchGE

        void LaunchGEExecute()
        {
            // Check that BeingEdited is a location.
            if (!(BeingEdited is Location)) return;
            Location cLoc = BeingEdited as Location;

            String geApp = GetPathAssociatedWithFileExtension(".kml");

            if (geApp == null)
            {
                return;
            }

           // This line is a desperate fix for something that must have changed in retrieveing 
           // a registry key (or a change in the key itself). An additional space was added.
            geApp = geApp.Substring(0, geApp.Length - 1);

            // Check if any long/lat field is empty, in that case launce GE without kml-file
            if ((cLoc.Longitude == "") || (cLoc.Latitude == ""))
            {
               Process.Start(geApp);
               return;
            }

            // Convert string format to decimals
            double dLong = Convert.ToDouble(cLoc.Longitude, CultureInfo.CreateSpecificCulture("en-GB"));
            double dLat = Convert.ToDouble(cLoc.Latitude, CultureInfo.CreateSpecificCulture("en-GB"));

            GeoCoordinate gePLace = new GeoCoordinate(dLat, dLong);
//            GeoCoordinate boraBora = new GeoCoordinate(-16.443118, -151.752044);

            String kmlFile = GeneratePlacemarkKmlFile(gePLace, cLoc.Headline);

            Process.Start(geApp, "\"" + kmlFile + "\"");

        }

        bool CanLaunchGEExecute()
        {
            return _geInstalled;
        }

        public ICommand LaunchGE
        {
            get
            {
                return new RelayCommand(LaunchGEExecute, CanLaunchGEExecute);
            }
        }

        #endregion

        #region ParseGE

        void ParseGEExecute()
        {
            // Check that BeingEdited is a location.
            if (!(BeingEdited is Location)) return;
            Location cLoc = BeingEdited as Location;

            String geKml = Clipboard.GetText();
            // convert string to stream
            byte[] byteArray = Encoding.UTF8.GetBytes(geKml);
            //byte[] byteArray = Encoding.ASCII.GetBytes(contents);
            MemoryStream geStream = new MemoryStream(byteArray);

            if (geStream == null)
            {
                MessageBox.Show(MainWindow, "Det gick inte att skapa en tillfällig minneström för data från Google Earth.", "Internt fel vid inläsning av Google Earth data", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Assembly a = Assembly.GetExecutingAssembly();
            Stream stream = a.GetManifestResourceStream("WritersBlox.Schemas.kml21.xsd");
            
            if (stream == null)
            {
                MessageBox.Show(MainWindow, "Det gick inte att läsa schemafilen.", "Internt fel vid inläsning av Google Earth data", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            XmlSchema x = XmlSchema.Read(stream,
                new ValidationEventHandler(SchemaValidationEventHandler));

            // String schema = @"..\..\Schemas\kml21.xsd";
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            
            // settings.Schemas.Add("http://earth.google.com/kml/2.1", schema);
            
            settings.Schemas.Add(x);
            try
            {

                using (XmlReader reader = XmlReader.Create(geStream, settings))
                {
                    // Move to the hire-date element.

                    reader.MoveToContent();
                    reader.ReadToDescendant("Placemark");
                    reader.ReadToDescendant("Point");
                    reader.ReadToDescendant("coordinates");
                    String coord = reader.ReadElementContentAsString();
                    int c1 = coord.IndexOf(",");
                    int c2 = coord.IndexOf(",", c1 + 1);
                    cLoc.Longitude = coord.Substring(0, c1);
                    cLoc.Latitude = coord.Substring(c1 + 1, c2 - c1 - 1);
                }

            }

            catch (XmlException)
            {
                MessageBox.Show(MainWindow, "Platsen är inte korrekt kopierad från Google Earth.", "Fel vid läsning av kopierad information från GE", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        bool CanParseGEExecute()
        {
            return _geInstalled;
        }

        public ICommand ParseGE
        {
            get
            {
                return new RelayCommand(ParseGEExecute, CanParseGEExecute);
            }
        }

        public void SchemaValidationEventHandler(Object sender, ValidationEventArgs e)
        {
        }

        #endregion

        #region DebugCommand

        void DebugCommandExecute(Object parameter)
        {
            if (Root.Count > 0)
            {
                if (Root[0].Children[0] is DataChild)
                {
                    int n = Root[0].Children.Count;
                    int m = n;

                }
            }
        }

        bool CanDebugCommandExecute(Object parameter)
        {
            return true;
        }

        public ICommand DebugCommand
        {
            get
            {
                return new RelayCommand<Object>(parameter => DebugCommandExecute(parameter), parameter => CanDebugCommandExecute(parameter));
            }
        }

        #endregion

        #endregion

        #region Methods

        #region SetSplashScreen

        public void SetSplashScreen()
        {
            SplashPage nextPage = new SplashPage(this);
            EditFrame.NavigationService.Navigate(nextPage);
        }

        #endregion

        #region FindObjectCollection

        private IList FindObjectCollection(Object findMe)
        {
            // The theory is that one Blox only has one single place in the tree
            foreach (DataChild dC in Root)
            {
                IList childList = dC.FindObjectCollection(findMe);
                if (childList != null) return childList;
            }

            // If nothing was found, perhaps findMe lies on the root 
            // collection (which is another type of collection then what
            // FindObjectCollection is looking for. I.e. we check it here

            if (Root.Contains(findMe)) return Root; 
            
            return null;
        }

        #endregion

        #region FindObjectOwner

        private DataChild FindObjectOwner(Object findMe)
        {
            // Object may be an DataChild that may be owned by the Root
            // but Root is no DataChild so we return null.
            if (Root.Contains(findMe)) return null;

            // The theory is that one Blox only has one single place in the tree
            foreach (DataChild dC in Root)
            {
                if (dC.Children.Contains(findMe)) return dC;

                DataChild child = dC.FindObjectOwner(findMe);
                if (child != null) return child;
            }
            return null;
        }

        #endregion

        #region FindGUIDObject

        public WBReferencedObject FindGUIDObject(Guid id)
        {
            // Searches Root for the object with Guid
            foreach (DataChild dC in Root)
            {
                WBReferencedObject ro = dC.FindGuid(id);
                if (ro != null) return ro;
            }
            return null;
        }

        #endregion
        
        #region ExpandToTop

        private void ExpandToTop(Object expandMe)
        {
            DataChild dC = FindObjectOwner(expandMe);
            while (dC != null)
            {
                dC.IsExpanded = true;
                dC = FindObjectOwner( dC );
            }
        }

        #endregion

        #region PrepareObjectSelectionCollection

        ObservableCollection<DataChild> PrepareObjectSelectionCollection(Type filterType)
        {
            ObservableCollection<DataChild> nDCC = new ObservableCollection<DataChild>();

            foreach (DataChild tDC in Root)
            {
                // Create new DataChild with same FolderName but with the empty list
                DataChild nC = new DataChild(tDC.FolderName);

                // Add new DataChild to collection
                nDCC.Add(nC);

                // Ask DataChild to populate its collection with filterType and sub-DataChild objects (recursively)
                nC.PopulateWithFilter(tDC as DataChild, filterType);
            }
            return nDCC;
        }

        #endregion
        
        #region DragDrop

        bool IDragSource.CanStartDrag(IDragInfo dragInfo)
        {
            return true;
        }

        void IDragSource.StartDrag(IDragInfo dragInfo)
        {
            var itemCount = dragInfo.SourceItems.Cast<object>().Count();

            if (itemCount == 0) return;

            if (itemCount == 1)
            {
                dragInfo.Data = dragInfo.SourceItems.Cast<object>().First();
            }
            else if (itemCount > 1)
            {
                dragInfo.Data = TypeUtilities.CreateDynamicallyTypedList(dragInfo.SourceItems);
            }

            dragInfo.Effects = (dragInfo.Data != null) ?
                                 DragDropEffects.Copy | DragDropEffects.Move :
                                 DragDropEffects.None;


            // TODO: Make sure it is impossible to do an recursive move, 
            // i.e. move a folder onto one of its children

            // To have performance for each dragging we want to mark all descendants to the 
            // dragged DataChild as invalid for dropping. This requires a new field in all these 
            // Object. First it must be reset for all objects and then set on the descendants of
            // the dragged DataChild

            // It is only relevant for moving folders, i.e. DataChild
            if (dragInfo.Data.GetType() == typeof(DataChild))
            {
                // Validate all (clean up from last drop action)
                foreach (DataChild dCI in Root)
                {
                    dCI.InvalidateAscendantsDrop(false);
                }

                // Now, invalidate all ascendants of this DataChild
                DataChild dC = dragInfo.Data as DataChild;
                dC.InvalidateAscendantsDrop(true);

            }
        }

        void IDragSource.Dropped(IDropInfo dropInfo)
        {
        }

        void IDragSource.DragCancelled()
        {
        }


        void IDropTarget.DragOver(IDropInfo dropInfo)
        {

            // Dispaly data tracker
            if (dropInfo.InsertPosition == RelativeInsertPosition.BeforeTargetItem)
            {
                Message = "BeforeTargetItem";
            }
            else if (dropInfo.InsertPosition == RelativeInsertPosition.AfterTargetItem)
            {
                Message = "AfterTargetItem";
            }
            else if (dropInfo.InsertPosition == RelativeInsertPosition.TargetItemCenter)
            {
                Message = "TargetItemCenter";
            }

            Message2 = dropInfo.TargetCollection.GetType().ToString();

            // Disable moves to the same object:
            if (dropInfo.TargetItem == dropInfo.DragInfo.SourceItem)
            {
                return;
            }

            // Check if the DataChild that is being dragged in not droped on
            // Ascendants of itself
            if (dropInfo.DragInfo.SourceItem.GetType() == typeof(DataChild))
            {
                if (dropInfo.TargetItem == null)
                {
                    return;
                }

                if (dropInfo.TargetItem.GetType() == typeof(DataChild))
                {
                    //MessageBox.Show(MainWindow, "Got to drag over a DataChild");
                    DataChild dC = dropInfo.TargetItem as DataChild;
                    if (dC.InvalidDrop) return;
                }


                if (dropInfo.TargetItem.GetType().IsSubclassOf(typeof(WBObjectViewModel)))
                {
                    // MessageBox.Show(MainWindow, "Got to drag over a Blox");
                    WBObjectViewModel b = dropInfo.TargetItem as WBObjectViewModel;
                    if (b.InvalidDrop) return;
                }
            }

            // Hit it off with all allowed combinations!!!

            // Scene on the event Scenes collection
            if (dropInfo.Data.GetType() == typeof(Scene))
            {
                // if (dropInfo.TargetCollection.GetType() == typeof(WBObjectList<Scene>))
                if (dropInfo.TargetCollection.GetType() == typeof(ItemCollection))
                {
                    dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                    dropInfo.Effects = DragDropEffects.Copy;
                    return;
                }
            }

            // Character on the event Characters collection
            if (dropInfo.Data.GetType() == typeof(Character))
            {
                if (dropInfo.TargetCollection.GetType() == typeof(WBObjectList<Character>))
                {
                    dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                    dropInfo.Effects = DragDropEffects.Copy;
                    return;
                }
            }

            // Location on the event Locations collection
            if (dropInfo.Data.GetType() == typeof(Location))
            {
                if (dropInfo.TargetCollection.GetType() == typeof(WBObjectList<Location>))
                {
                    dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                    dropInfo.Effects = DragDropEffects.Copy;
                    return;
                }
            }

            // Event on the Blox event collection
            if (dropInfo.Data.GetType() == typeof(StoryEvent))
            {
                if (dropInfo.TargetCollection.GetType() == typeof(WBObjectList<StoryEvent>))
                {
                    dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                    dropInfo.Effects = DragDropEffects.Copy;
                    return;
                }
            }

            // Check that we are within the Main tree, it is either a ObservableCollection of DataChild (the root)
            // or a CompositeCollection, if not we terminate

            if ((dropInfo.TargetCollection.GetType() != typeof(ObservableCollection<DataChild>)) &&
                (dropInfo.TargetCollection.GetType() != typeof(System.Windows.Data.CompositeCollection)))
            {
                return;
            }


            // Check if droped on root space of main tree, i.e. targetItem == null
            if (dropInfo.TargetItem == null)
            {
                // Only DataChilds are allowed to be droped on root of Main tree
                if (dropInfo.Data.GetType() == typeof(DataChild))
                {
                    dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                    dropInfo.Effects = DragDropEffects.Move;
                    return;
                }
                return;
            }

            // Move Objects within the Main tree view onto folders
            if (dropInfo.TargetItem.GetType() == typeof(DataChild))
            {
                if ((dropInfo.InsertPosition == RelativeInsertPosition.BeforeTargetItem) ||
                    (dropInfo.InsertPosition == RelativeInsertPosition.AfterTargetItem))
                {
                    // If Target DataChild is on root only sources of DataChild is allowed
                    if (Root.Contains(dropInfo.TargetItem))
                    {
                        if (dropInfo.Data is DataChild)
                        {
                            dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                            dropInfo.Effects = DragDropEffects.Move;
                            return;
                        }
                        else
                        {
                            return;
                        }

                    }
                    else
                    {
                        dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                        dropInfo.Effects = DragDropEffects.Move;
                        return;
                    }

                }
                else 
                {
                    dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                    dropInfo.Effects = DragDropEffects.Move;
                    return;
                }
            }

            // Move Objects within the Main tree view onto objects in folders
            if (dropInfo.TargetCollection.GetType() == typeof(System.Windows.Data.CompositeCollection))
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                dropInfo.Effects = DragDropEffects.Move;
                return;
            }
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            // Hit it off with all allowed combinations!!!

            // Scene on the event Scenes collection
            if (dropInfo.Data.GetType() == typeof(Scene))
            {
                if (dropInfo.TargetCollection.GetType() == typeof(WBObjectList<Scene>))
                {
                    Scene selectedScene = dropInfo.Data as Scene;
                    StoryEvent sE = BeingEdited as StoryEvent;
                    sE.Scenes.Add(selectedScene);
                    selectedScene.StoryEvents.Add(sE);
                    return;
                }
            }

            // Character on the event Characters collection
            if (dropInfo.Data.GetType() == typeof(Character))
            {
                if (dropInfo.TargetCollection.GetType() == typeof(WBObjectList<Character>))
                {
                    Character selectedCharacter = dropInfo.Data as Character;
                    StoryEvent sE = BeingEdited as StoryEvent;
                    sE.Characters.Add(selectedCharacter);
                    selectedCharacter.StoryEvents.Add(sE);
                    return;
                }
            }

            // Location on the event Locations collection
            if (dropInfo.Data.GetType() == typeof(Location))
            {
                if (dropInfo.TargetCollection.GetType() == typeof(WBObjectList<Location>))
                {
                    Location selectedLocation = dropInfo.Data as Location;
                    StoryEvent sE = BeingEdited as StoryEvent;
                    sE.Locations.Add(selectedLocation);
                    selectedLocation.StoryEvents.Add(sE);
                    return;
                }
            }

            // Event on the Blox event collection
            if (dropInfo.Data.GetType() == typeof(StoryEvent))
            {
                if (dropInfo.TargetCollection.GetType() == typeof(WBObjectList<StoryEvent>))
                {
                    StoryEvent selectedEvent = dropInfo.Data as StoryEvent;
                    Blox cB = BeingEdited as Blox;
                    cB.StoryEvents.Add(selectedEvent);
                    switch (BeingEdited.GetType().ToString())
                    {
                        case "WritersBlox.Scene":
                            selectedEvent.Scenes.Add(cB as Scene);
                            break;
                        case "WritersBlox.Character":
                            selectedEvent.Characters.Add(cB as Character);
                            break;
                        case "WritersBlox.Location":
                            selectedEvent.Locations.Add(cB as Location);
                            break;
                    }
                    return;
                }
            }

            // Check that we are within the Main tree, it is either a ObservableCollection of DataChild (the root)
            // or a CompositeCollection, if not we terminate

            if ((dropInfo.TargetCollection.GetType() != typeof(ObservableCollection<DataChild>)) &&
                (dropInfo.TargetCollection.GetType() != typeof(System.Windows.Data.CompositeCollection)))
            {
                return;
            }

            // Check if droped on root empty space of main tree, i.e. targetItem == null
            if (dropInfo.TargetItem == null)
            {
                // Only DataChilds are allowed to be droped on root of Main tree
                if (dropInfo.Data.GetType() == typeof(DataChild))
                {
                    // Move DataChild to Root at insertion point

                    // Two cases...

                    if (Root.Contains(dropInfo.Data))
                    {
                        // 1: if source DataChild is in Root, i.e. from root to root, i.e same collection
                        
                        // If source and destination indices are the same (or source is dest + 1) 
                        // the user is trying to move to the same index, do not move at all
                        if ((dropInfo.DragInfo.SourceIndex == dropInfo.InsertIndex) ||
                            (dropInfo.DragInfo.SourceIndex + 1 == dropInfo.InsertIndex))
                        {
                            return;
                        }

                        // If source has bigger index, remove first and add later:
                        if (dropInfo.DragInfo.SourceIndex > dropInfo.InsertIndex)
                        {
                            Root.RemoveAt(dropInfo.DragInfo.SourceIndex);
                            Root.Insert(dropInfo.InsertIndex, dropInfo.Data as DataChild);
                            return;
                        }

                        // If source has samller index, add first and remove later:
                        if (dropInfo.DragInfo.SourceIndex < dropInfo.InsertIndex)
                        {
                            Root.Insert(dropInfo.InsertIndex, dropInfo.Data as DataChild);
                            Root.RemoveAt(dropInfo.DragInfo.SourceIndex);
                            return;
                        }

                    }
                    else
                    {
                        // 2: if source DataChild is not in root
                        CompositeCollection sourceColl = dropInfo.DragInfo.SourceCollection as CompositeCollection;
                        sourceColl.RemoveAt(dropInfo.DragInfo.SourceIndex + 5); // The first five items are collections.
                        Root.Insert(dropInfo.InsertIndex, dropInfo.Data as DataChild);
                        return;
                    }
                    return;
                }
                return;
            }


            // Move Objects within the Main tree view onto folders
            if (dropInfo.TargetItem.GetType() == typeof(DataChild))
            {
                if ((dropInfo.InsertPosition == RelativeInsertPosition.BeforeTargetItem) ||
                    (dropInfo.InsertPosition == RelativeInsertPosition.AfterTargetItem))
                {
                    // Move to before the Target DataChild

                    // If Target DataChild is on root only sources of DataChild is allowed
                    if (Root.Contains(dropInfo.TargetItem))
                    {
                        if (dropInfo.Data is DataChild)
                        {
                            // Perform move!
                            // Two Cases:
                            if (Root.Contains(dropInfo.Data))
                            {
                                // 1: Source DataChild is on Root, i.e. move DataChild Root to Root

                                // If source and destination indices are the same (or source is dest + 1) 
                                // the user is trying to move to the same index, do not move at all
                                if ((dropInfo.DragInfo.SourceIndex == dropInfo.InsertIndex) ||
                                    (dropInfo.DragInfo.SourceIndex + 1 == dropInfo.InsertIndex))
                                {
                                    return;
                                }

                                // If source has bigger index, remove first and add later:
                                if (dropInfo.DragInfo.SourceIndex > dropInfo.InsertIndex)
                                {
                                    Root.RemoveAt(dropInfo.DragInfo.SourceIndex);
                                    Root.Insert(dropInfo.InsertIndex, dropInfo.Data as DataChild);
                                    return;
                                }

                                // If source has samller index, add first and remove later:
                                if (dropInfo.DragInfo.SourceIndex < dropInfo.InsertIndex)
                                {
                                    Root.Insert(dropInfo.InsertIndex, dropInfo.Data as DataChild);
                                    Root.RemoveAt(dropInfo.DragInfo.SourceIndex);
                                    return;
                                }
                            }
                            else
                            {
                                // 2: Source DataChild is not on Root, i.e. move DataChild from other DataChild to Root
                                CompositeCollection sourceColl = dropInfo.DragInfo.SourceCollection as CompositeCollection;
                                sourceColl.RemoveAt(dropInfo.DragInfo.SourceIndex + 5); // The first five items are collections.
                                Root.Insert(dropInfo.InsertIndex, dropInfo.Data as DataChild);
                                return;
                            }
                            return;
                        }
                        else
                        {
                            return;
                        }

                    }
                    else
                    {
                        // Perform move!

                        // Target DataChild is not on root, but source may be

                        // Two Cases:
                        if (Root.Contains(dropInfo.Data))
                        {
                            // 1: Source DataChild is on Root, i.e. move DataChild on Root to Other DataChild
                            CompositeCollection targetColl = dropInfo.TargetCollection as CompositeCollection;
                            Root.RemoveAt(dropInfo.DragInfo.SourceIndex);
                            targetColl.Insert(dropInfo.InsertIndex + 5, dropInfo.Data as DataChild);
                            return;
                        }
                        else
                        {
                            // 2: Source DataChild is not on Root, i.e. move DataChild from one DataChild to Other DataChild
                            CompositeCollection sourceColl = dropInfo.DragInfo.SourceCollection as CompositeCollection;
                            CompositeCollection targetColl = dropInfo.TargetCollection as CompositeCollection;

                            // Is the source and target collection the same?
                            if (dropInfo.DragInfo.SourceCollection == dropInfo.TargetCollection)
                            {
                                // Yes, handle special cases to not fuck up indices.

                                // If source and destination are the same (or source is dest + 1) 
                                // the user is trying to move to the same index, do not move at all
                                if ((dropInfo.DragInfo.SourceIndex == dropInfo.InsertIndex) ||
                                    (dropInfo.DragInfo.SourceIndex + 1 == dropInfo.InsertIndex))
                                {
                                    return;
                                }

                                // If source has bigger index, remove first and add later:
                                if (dropInfo.DragInfo.SourceIndex > dropInfo.InsertIndex)
                                {
                                    sourceColl.RemoveAt(dropInfo.DragInfo.SourceIndex + 5);
                                    targetColl.Insert(dropInfo.InsertIndex + 5, dropInfo.Data);
                                    return;
                                }

                                // If source has samller index, add first and remove later:
                                if (dropInfo.DragInfo.SourceIndex < dropInfo.InsertIndex)
                                {
                                    targetColl.Insert(dropInfo.InsertIndex + 5, dropInfo.Data);
                                    sourceColl.RemoveAt(dropInfo.DragInfo.SourceIndex + 5);
                                    return;
                                }
                            }
                            else
                            {
                                // No, no special handling required
                                sourceColl.RemoveAt(dropInfo.DragInfo.SourceIndex + 5); // The first five items are collections.
                                targetColl.Insert(dropInfo.InsertIndex + 5, dropInfo.Data);
                                return;
                            }
                        }
                        return;
                    }
                    
                }
                else
                {
                    // This case is move onto a DataChild, it is always allowed and never in the same collection.

                    // Perform move!
                    // Two cases:
                    if (Root.Contains(dropInfo.Data))
                    {
                        // 1: Source object is from the Root
                        DataChild tDC = dropInfo.TargetItem as DataChild;
                        Root.RemoveAt(dropInfo.DragInfo.SourceIndex);
                        tDC.Children.Insert(5, dropInfo.Data);
                        return;
                    }
                    else
                    {
                        // 2: Source object is not from the Root
                        CompositeCollection sourceColl = dropInfo.DragInfo.SourceCollection as CompositeCollection;
                        DataChild tDC = dropInfo.TargetItem as DataChild;
                        sourceColl.RemoveAt(dropInfo.DragInfo.SourceIndex + 5);
                        tDC.Children.Insert(5, dropInfo.Data);
                        return;
                    }
                }
            }

            // Check that the drop object is part of a CompositeCollection (i.e. the contents of a DataCHild)
            if (dropInfo.TargetCollection.GetType() == typeof(System.Windows.Data.CompositeCollection))
            {

                // Two cases:
                if (Root.Contains(dropInfo.Data))
                {
                    // 1: source object is in root
                    CompositeCollection targetColl = dropInfo.TargetCollection as CompositeCollection;
                    Root.RemoveAt(dropInfo.DragInfo.SourceIndex + 5); // The first five items are collections.
                    targetColl.Insert(dropInfo.InsertIndex + 5, dropInfo.Data);
                    return;
                }
                else
                {
                    // 2: Source object is not in root
                    CompositeCollection sourceColl = dropInfo.DragInfo.SourceCollection as CompositeCollection;
                    CompositeCollection targetColl = dropInfo.TargetCollection as CompositeCollection;

                    // Is the source and target collection the same?
                    if (dropInfo.DragInfo.SourceCollection == dropInfo.TargetCollection)
                    {
                        // Yes, handle special cases to not fuck up indices.

                        // If source and destination are the same (or source is dest + 1) 
                        // the user is trying to move to the same index, do not move at all
                        if ((dropInfo.DragInfo.SourceIndex == dropInfo.InsertIndex) ||
                            (dropInfo.DragInfo.SourceIndex + 1 == dropInfo.InsertIndex))
                        {
                            return;
                        }

                        // If source has bigger index, remove first and add later:
                        if (dropInfo.DragInfo.SourceIndex > dropInfo.InsertIndex)
                        {
                            sourceColl.RemoveAt(dropInfo.DragInfo.SourceIndex + 5);
                            targetColl.Insert(dropInfo.InsertIndex + 5, dropInfo.Data);
                            return;
                        }

                        // If source has samller index, add first and remove later:
                        if (dropInfo.DragInfo.SourceIndex < dropInfo.InsertIndex)
                        {
                            targetColl.Insert(dropInfo.InsertIndex + 5, dropInfo.Data);
                            sourceColl.RemoveAt(dropInfo.DragInfo.SourceIndex + 5);
                            return;
                        }
                    }
                    else
                    {
                        // No, no special handling required
                        sourceColl.RemoveAt(dropInfo.DragInfo.SourceIndex + 5); // The first five items are collections.
                        targetColl.Insert(dropInfo.InsertIndex + 5, dropInfo.Data);
                        return;
                    }
                }
            }
        }

        #endregion

        #region AddProjectToRecentList

        public void AddProjectToRecentList(ProjectInformation pi)
        {

            int piIndex = FindRecentFile(pi.Path);

            if (piIndex >= 0)
            {
                RecentList.RemoveAt(piIndex);
                RecentList.Insert(0, pi);
            }
            else
            {
                RecentList.Insert(0, pi);
                while (RecentList.Count > 15)
                {
                    RecentList.RemoveAt(RecentList.Count - 1);
                }
            }
        }

        #endregion

        #region FindRecentFile

        private int FindRecentFile( string path )
        {
            foreach (ProjectInformation pi in RecentList)
            {
                if (pi.Path == path) return RecentList.IndexOf(pi);
            }
            return -1;
        }

        #endregion

        #region SaveRecentList

        private void SaveRecentList()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = true;
            settings.NewLineChars = "\r\n";

            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appPath = Path.Combine(appDataPath, "WritersBlox");
            string filePath = Path.Combine(appPath, "WBRecentList.xml");

            XmlWriter writer = XmlWriter.Create(filePath, settings);

            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<ProjectInformation>));
            serializer.Serialize(writer, RecentList);

            writer.Close();
        }

        #endregion

        #region LoadRecentList

        public void LoadRecentList()
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            // Check if appdata folder exists,
            // If not: Create it and return (no file to load)
            // If it exitsts, does it contain a file called "WBRecentList.xml?
            // Load file

            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appPath = Path.Combine(appDataPath, "WritersBlox");

            if (!Directory.Exists(appPath))
            {
                Directory.CreateDirectory(appPath);
                return;
            }

            string filePath = Path.Combine(appPath, "WBRecentList.xml");

            if (File.Exists(filePath))
            {
                XmlReader reader = XmlReader.Create(filePath, settings);

                System.Xml.Serialization.XmlSerializer serializer =
                    new System.Xml.Serialization.XmlSerializer(typeof(ObservableCollection<ProjectInformation>));
                RecentList = serializer.Deserialize(reader) as ObservableCollection<ProjectInformation>;

                OnPropertyChanged("RecentList");

                reader.Close();
            }
        }

        #endregion

        #region LoadPersonNames

        public void LoadPersonNames()
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            
            Uri uri = new Uri("/Data/players.xml", UriKind.Relative);
            // Uri uri = new Uri("pack://application:,,,/Data/players.xml", UriKind.Absolute);

            StreamResourceInfo info = Application.GetContentStream(uri );

            if (info.Stream != null)
            {
                XmlReader reader = XmlReader.Create(info.Stream, settings);

                XmlSerializer serializer = new XmlSerializer(typeof(List<Player>));
                _listOfPersons = serializer.Deserialize(reader) as List<Player>;

                reader.Close();
            }
            else
            {
                MessageBox.Show(MainWindow, "Filen med namn: players.xml, kunde inte läsas", "Fel vid läsning av namnfil", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region GeInstalled

        public void GeInstalled() 
        {

            Message = Assembly.GetExecutingAssembly().Location;


            String geApp = GetPathAssociatedWithFileExtension(".kml");

            if (geApp == null)
            {
                _geInstalled = false;
            }
            else
            {
                _geInstalled = true;
            }
        }

	    #endregion

        #region GetPathAssociatedWithFileExtension

        static Regex pathArgumentsRegex = new Regex(@" (%\d+)|(""%\d+"")", RegexOptions.ExplicitCapture);
        static string GetPathAssociatedWithFileExtension(string extension)
        {
            RegistryKey extensionKey = Registry.ClassesRoot.OpenSubKey(extension);
            if (extensionKey != null)
            {
                object applicationName = extensionKey.GetValue(string.Empty);
                if (applicationName != null)
                {
                    RegistryKey commandKey = Registry.ClassesRoot.OpenSubKey(applicationName.ToString() + @"\shell\open\command");
                    if (commandKey != null)
                    {
                        object command = commandKey.GetValue(string.Empty);
                        if (command != null)
                        {

                           string temp = command.ToString();

                           string regExp = pathArgumentsRegex.Replace(command.ToString(), "");

                           return regExp;

                        }
                    }
                }
            }
            return null;
        }
		 
    	#endregion

        #region GeneratePlacemarkKmlFile

        private String GeneratePlacemarkKmlFile(GeoCoordinate coord, String name)
        {

            String fileName = KmlTempFile();
            
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName))
            {
                string temp;
                file.WriteLine(@"<?xml version=""1.0"" encoding=""UTF-8""?>");
                file.WriteLine(@"<kml xmlns=""http://earth.google.com/kml/2.0"">");
                file.WriteLine(@"   <Placemark>");
                temp = "    <name>" + name + "</name>";
                file.WriteLine(temp);
                file.WriteLine(@"   <Point>");
                temp = "      <coordinates>" + coord.Longitude.ToString(CultureInfo.CreateSpecificCulture("en-GB")) + "," + coord.Latitude.ToString(CultureInfo.CreateSpecificCulture("en-GB")) + "</coordinates>";
                file.WriteLine(temp);
                file.WriteLine(@"   </Point>");
                file.WriteLine(@"   </Placemark>");
                file.WriteLine(@"</kml>");
            }

            return fileName;
        }

        private string KmlTempFile()
        {
            return Path.GetTempPath() + "writersblox.kml";
        }
		 
	    #endregion

        #region RemoveKmlFile

        private void RemoveKmlFile()
        {
            File.Delete(KmlTempFile());
        }

        #endregion

        #region CanClose

        public bool CanClose()
        {
            if (!CloseAndSavePrompt("Avsluta Writers Blox", "avslutar Writers Blox")) return true;

            // Clean up application
            SaveRecentList();
            RemoveKmlFile();

            return false;
        }

        #endregion

        #region CloseAndSavePrompt
        
        private bool CloseAndSavePrompt( string actionHeadline, string closeAction )
        {
            if (_isModified)
            {
                // Prompt for discarding all data
                MessageBoxResult result = MessageBox.Show(MainWindow, "Projektet innehåller osparad information. Vill du spara innan du " + closeAction + "?", actionHeadline, MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                switch (result)
                {
                    case MessageBoxResult.Yes :
                        if (CanSaveProjectExecute()) SaveProjectExecute();
                        return true;
                    case MessageBoxResult.No :
                        return true;
                    case MessageBoxResult.Cancel:
                        return false;
                }
            }
            return true;
        }

        #endregion

        #endregion

    }
}










using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MicroMvvm;

namespace WritersBlox
{
    public class SelectObjectDialogViewModel : ViewModelBase
    {
        #region Declarations

        private string _dialogTitle;
        private string _promptText;
        ObservableCollection<DataChild> _data;
        private Object _selectedObject;

        #endregion

        #region Constructor

        public SelectObjectDialogViewModel() 
        {
            DialogTitle = "Objektväljare";
            PromptText = "Välj objekt";
        }

        public SelectObjectDialogViewModel(string dialogTitle, string promptText, ObservableCollection<DataChild> data)
        {
            DialogTitle = dialogTitle;
            PromptText = promptText;
            _data = data;
        }

        #endregion

        #region Properties

        public string DialogTitle
        {
            get { return _dialogTitle; }
            set
            {
                _dialogTitle = value;
                OnPropertyChanged("DialogTitle");
            }
        }

        public string PromptText
        {
            get { return _promptText; }
            set
            {
                _promptText = value;
                OnPropertyChanged("PromptText");
            }
        }

        public ObservableCollection<DataChild> Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public Object SelectedObject
        {
            get { return _selectedObject; }
            set { _selectedObject = value; }
        }

        #endregion

        #region Commands

        #region SelectObject

        void SelectObjectExecute(Object parameter)
        {
            SelectedObject = parameter;
        }

        bool CanSelectObjectExecute(Object parameter)
        {
            // Tree only consists of filterd object (that we can select) and DataChilds
            if (parameter == null) return false;

            //if (parameter.GetType().ToString() == "WritersBlox.DataChild")
            if (parameter is DataChild)
            {
                return false;
            }

            return true;
        }

        public ICommand SelectObject
        {
            get
            {
                return new RelayCommand<Object>(parameter => SelectObjectExecute(parameter), parameter => CanSelectObjectExecute(parameter));
            }
        }

        #endregion
        
        
        #endregion

    }
}

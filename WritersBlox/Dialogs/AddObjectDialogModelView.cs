using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WritersBlox
{
    public class AddObjectDialogModelView : ViewModelBase
    {
        #region Declarations

        private string _dialogTitle;
        private string _promptText;
        private string _newObjectName;

        #endregion

        #region Constructors

        public AddObjectDialogModelView()
        {
            DialogTitle = "Nytt object";
            PromptText = "Skriv en rubrik för objektet:";
            NewObjectName = "Objektets namn";
        }

        public AddObjectDialogModelView(string dialogTitle, string promptText, string newObjectName)
        {
            DialogTitle = dialogTitle;
            PromptText = promptText;
            NewObjectName = newObjectName;
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

        public string NewObjectName
        {
            get { return _newObjectName; }
            set 
            {
                _newObjectName = value;
                OnPropertyChanged("NewObjectName");
            }
        }

        #endregion

    }
}

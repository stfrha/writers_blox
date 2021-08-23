using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WritersBlox
{
    [Serializable()]
    public class ProjectInformation : ViewModelBase
    {
        #region Declarations

        private string _path;
        private string _title;
        private string _description;
        private string _createdDate;

        #endregion

        #region Constructors

        public ProjectInformation()
        {
        }

        public ProjectInformation(string path, string title, string createdDate, string description)
        {
            Path = path;
            Title = title;
            CreatedDate = createdDate;
            Description = description;
        }

        #endregion

        #region Properties

        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                OnPropertyChanged("Path");
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged("Title");
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged("Description");
            }
        }

        public string CreatedDate
        {
            get { return _createdDate; }
            set
            {
                _createdDate = value;
                OnPropertyChanged("CreatedDate");
            }
        }

        #endregion

    }
}

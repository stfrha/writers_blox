using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WritersBlox 
{

    #region Types

    public enum TimeMode { eNoTime, eSingleTime, eStartEndTime };

    #endregion


    [Serializable]
    public class StoryEvent : WBReferencedObject
    {

        #region Declarations

        private string _headline;
        private string _description;
        private TimeMode _eventMode;
        private DateTime _startTime;
        private DateTime _endTime;

        private WBObjectList<Scene> _eventScenes = new WBObjectList<Scene>();
        private WBObjectList<Character> _eventCharacters = new WBObjectList<Character>();
        private WBObjectList<Location> _eventLocations = new WBObjectList<Location>();

        #endregion

        #region Constructors

        public StoryEvent()
        {
            Headline = "Unkown";
            Description = "Beskrivning...";
            EventMode = TimeMode.eStartEndTime;
            StartTime = new DateTime( 2015, 6, 15);
            EndTime = new DateTime(2015, 6, 16);
        }

        public StoryEvent(string headline, string description) : base(true)
        {
            Headline = headline;
            Description = description;
            EventMode = TimeMode.eStartEndTime;
            StartTime = new DateTime(2015, 6, 15);
            EndTime = new DateTime(2015, 6, 16);
        }

        #endregion

        #region Properties

        public string Headline
        {
            get { return _headline; }
            set
            {
                _headline = value;
                OnPropertyChanged("Headline");
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

        public DateTime StartTime
        {
            get { return _startTime; }
            set
            {
                _startTime = value;
                OnPropertyChanged("StartTime");
            }
        }

        public DateTime EndTime
        {
            get { return _endTime; }
            set
            {
                _endTime = value;
                OnPropertyChanged("EndTime");
            }
        }

        public TimeMode EventMode
        {
            get { return _eventMode; }
            set
            {
                _eventMode = value;
                OnPropertyChanged("EventMode");
            }
        }

        public WBObjectList<Scene> Scenes
        {
            get { return _eventScenes; }
            set
            {
                _eventScenes = value;
            }
        }

        public WBObjectList<Character> Characters
        {
            get { return _eventCharacters; }
            set
            {
                _eventCharacters = value;
            }
        }

        public WBObjectList<Location> Locations
        {
            get { return _eventLocations; }
            set
            {
                _eventLocations = value;
            }
        }
        
        #endregion

        #region Methods

        public void Rehydrate(Func<Guid, WBReferencedObject> FindGUIDObject)
        {
            foreach (Guid id in Scenes.DehydratedList)
            {
                WBReferencedObject se = FindGUIDObject(id);
                Scenes.Add(se as Scene);
            }
            foreach (Guid id in Characters.DehydratedList)
            {
                WBReferencedObject se = FindGUIDObject(id);
                Characters.Add(se as Character);
            }
            foreach (Guid id in Locations.DehydratedList)
            {
                WBReferencedObject se = FindGUIDObject(id);
                Locations.Add(se as Location);
            }
        }
        
        #endregion
    }
}

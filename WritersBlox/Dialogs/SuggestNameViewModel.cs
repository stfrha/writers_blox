using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WritersBlox
{

    #region Types

    public enum NameSuggestedState { eNoSelect, eGenderSelected, eCountrySelected, eDecadeSelected };

    #endregion


    public class SuggestNameViewModel : ViewModelBase
    {
        #region Declarations

        private ObservableCollection<string> _genderList = new ObservableCollection<string>();
        private ObservableCollection<string> _countryList = new ObservableCollection<string>();
        private ObservableCollection<string> _decadeList = new ObservableCollection<string>();
        private ObservableCollection<string> _nameList = new ObservableCollection<string>();
        private ObservableCollection<string> _surnameList = new ObservableCollection<string>();
        private List<Player> _listOfPersons;
        private NameSuggestedState _viewState;
        private string _selectedGender;
        private string _selectedCountry;
        private string _selectedDecade;
        private string _selectedName;
        private string _selectedSurname;
        private string _selectedFullname;

        #endregion

        #region Constructor

        public SuggestNameViewModel(List<Player> listOfPlayers)
        {
            _listOfPersons = listOfPlayers;
            _viewState = NameSuggestedState.eNoSelect;
        }

        #endregion

        #region Properties

        public string SelectedGender
        {
            get { return _selectedGender; }
            set
            {
                _selectedGender = value;
                OnPropertyChanged("SelectedGender");
            }
        }

        public string SelectedCountry
        {
            get { return _selectedCountry; }
            set
            {
                _selectedCountry = value;
                OnPropertyChanged("SelectedCountry");
            }
        }

        public string SelectedDecade
        {
            get { return _selectedDecade; }
            set
            {
                _selectedDecade = value;
                OnPropertyChanged("SelectedDecade");
            }
        }

        public string SelectedName
        {
            get { return _selectedName; }
            set
            {
                _selectedName = value;
                OnPropertyChanged("SelectedName");
            }
        }

        public string SelectedSurname
        {
            get { return _selectedSurname; }
            set
            {
                _selectedSurname = value;
                OnPropertyChanged("SelectedSurname");
            }
        }

        public string SelectedFullname
        {
            get { return _selectedFullname; }
            set
            {
                _selectedFullname = value;
                OnPropertyChanged("SelectedFullname");
            }
        }

        public ObservableCollection<string> GenderList
        {
            get { return _genderList; }
            set { _genderList = value; }
        }

        public ObservableCollection<string> CountryList
        {
            get { return _countryList; }
            set { _countryList = value; }
        }

        public ObservableCollection<string> DecadeList
        {
            get { return _decadeList; }
            set { _decadeList = value; }
        }

        public ObservableCollection<string> NameList
        {
            get { return _nameList; }
            set { _nameList = value; }
        }

        public ObservableCollection<string> SurnameList
        {
            get { return _surnameList; }
            set { _surnameList = value; }
        }



        #endregion

        #region Methods

        public void InitialiseData()
        {
            BuildGenderList();
        }

        void BuildGenderList()
        {
            List<string> tl = new List<string>();

            foreach (Player p in _listOfPersons)
            {
                string gender = p.Gender;
                if (!tl.Contains(gender)) tl.Add(gender);
            }
            
            tl.Sort();

            foreach (string s in tl)
            {
                GenderList.Add(s);
            }
        }

        void BuildCountryList(string gender)
        {
            List<string> tl = new List<string>();

            foreach (Player p in _listOfPersons)
            {
                if (p.Gender == gender) 
                {
                    string country = p.Country;
                    if (!tl.Contains(country)) tl.Add(country);
                }
            }

            tl.Sort();

            foreach (string s in tl)
            {
                CountryList.Add(s);
            }
        }

        void BuildDecadeList(string gender, string country)
        {
            List<string> tl = new List<string>();

            foreach (Player p in _listOfPersons)
            {
                if ((p.Gender == gender) && (p.Country == country))
                {

                    string decade = GetDecade(p.BDay);
                    if (!tl.Contains(decade)) tl.Add(decade);
                }
            }

            tl.Sort();

            foreach (string s in tl)
            {
                DecadeList.Add(s);
            }
        }

        string GetDecade( string bDay )
        {
            return bDay.Substring(0, 3) + "0";
        }

        void BuildNameLists(string gender, string country, string decade)
        {
            {
                List<string> tnl = new List<string>();
                List<string> tsl = new List<string>();

                foreach (Player p in _listOfPersons)
                {
                    if ((p.Gender == gender) && (p.Country == country) && (GetDecade(p.BDay) == decade))
                    {
                        string name;
                        if (p.Title.IndexOf(" ") >= 0) name = p.Title.Substring(0, p.Title.IndexOf(" "));
                        else name = p.Title;
                        if (!tnl.Contains(name)) tnl.Add(name);

                        string surname;
                        if (p.Title.IndexOf(" ") >= 0)
                        {
                            surname = p.Title.Substring(p.Title.IndexOf(" ") + 1, p.Title.Count() - p.Title.IndexOf(" ") - 1);
                            if (!tsl.Contains(surname)) tsl.Add(surname);
                        }
                    }
                }

                int rndIndex;

                while (tnl.Count > 0)
                {
                    rndIndex = RandomRange(tnl.Count);
                    NameList.Add(tnl[rndIndex]);
                    tnl.RemoveAt(rndIndex);
                }

                while (tsl.Count > 0)
                {
                    rndIndex = RandomRange(tsl.Count);
                    SurnameList.Add(tsl[rndIndex]);
                    tsl.RemoveAt(rndIndex);
                }
            }
        }

        private int RandomRange(int range)
        {
            Random rnd = new Random();
            double scale = rnd.NextDouble() * (double)range;
            return (int) scale;
        }


        public void SetSelectedGender()
        {
            if (_viewState == NameSuggestedState.eGenderSelected)
            {
                CountryList.Clear();
            }

            if (_viewState == NameSuggestedState.eCountrySelected)
            {
                CountryList.Clear();
                DecadeList.Clear();
            }

            if (_viewState == NameSuggestedState.eDecadeSelected)
            {
                CountryList.Clear();
                DecadeList.Clear();
                NameList.Clear();
                SurnameList.Clear();
            }

            _viewState = NameSuggestedState.eGenderSelected;

            BuildCountryList(SelectedGender);
        }

        public void SetSelectedCountry()
        {
            if (_viewState == NameSuggestedState.eCountrySelected)
            {
                DecadeList.Clear();
            }

            if (_viewState == NameSuggestedState.eDecadeSelected)
            {
                DecadeList.Clear();
                NameList.Clear();
                SurnameList.Clear();
            }

            _viewState = NameSuggestedState.eCountrySelected;

            BuildDecadeList(SelectedGender, SelectedCountry);
        }

        public void SetSelectedDecade()
        {
            if (_viewState == NameSuggestedState.eDecadeSelected)
            {
                NameList.Clear();
                SurnameList.Clear();
            }

            _viewState = NameSuggestedState.eDecadeSelected;

            BuildNameLists(SelectedGender, SelectedCountry, SelectedDecade);
        }

        public void SetSelectedName()
        {
            SelectedFullname = SelectedName + " " + SelectedSurname;
        }

        #endregion
    }
}

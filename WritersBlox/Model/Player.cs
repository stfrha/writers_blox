using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WritersBlox
{
    public class Player : IEquatable<Player>

    {

        public Player()
        {
            Name = "";
            Title = "";
            BDay = "";
            Country = "";
            Gender = "";
        }

        public Player(string name, string title, string bday, string country, string gender)
        {
            Name = name;
            Title = title;
            BDay = bday;
            Country = country;
            Gender = gender;
        }

        [XmlIgnore]
        public string Name { get; set; }
        
        public string Title { get; set; }
        public string BDay { get; set; }
        public string Country { get; set; }
        public string Gender { get; set; }

        public bool Equals(Player other)
        {
            if (other == null)
                return false;

            if ((this.Name == other.Name) && (this.Title == other.Title) && (this.BDay == other.BDay) && (this.Country == other.Country) && (this.Gender == other.Gender))
                return true;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return (this.Name + this.Title + this.BDay + this.Country + this.Gender).GetHashCode();
        }

    }    
}

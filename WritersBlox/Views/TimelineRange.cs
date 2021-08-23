using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace WritersBlox.Views
{
    class TimelineRange
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    [ValueConversion(typeof(String), typeof(TimelineRange))]
    public class StringToTimelineRangeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (!(value is String))
                throw new InvalidOperationException("FRHA comment: The source must be a string");
            if (targetType != typeof(String))
                throw new InvalidOperationException("FRHA comment: The target must be a TimelineRange");
            string sV = value as string;
            string sTstr = sV.Substring(0, sV.IndexOf(";"));
            string eTstr = sV.Substring(sV.IndexOf(";") + 1);

            if (sTstr == sV)
                throw new InvalidOperationException("FRHA comment: DateTime separator character ';' is missing.");
            if (sTstr == "")
                throw new InvalidOperationException("FRHA comment: First DateTime sub-string is empty.");
            if (eTstr == "")
                throw new InvalidOperationException("FRHA comment: Second DateTime sub-string is empty.");

            TimelineRange range = new TimelineRange();
            
            DateTime t = new DateTime();
            bool fail = false;

            if (DateTime.TryParse(sTstr, out t))
            {
                range.StartTime = t;
            }
            else fail = true;

            if (DateTime.TryParse(eTstr, out t))
            {
                range.EndTime = t;
            }
            else fail = true;

            if (fail) return DependencyProperty.UnsetValue;

            return range;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}


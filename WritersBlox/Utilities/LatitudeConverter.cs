using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WritersBlox
{
    [ValueConversion(typeof(double?), typeof(string))]
    public class LatitudeConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double angle = 0.0;

            if (value is string)
            {
                if (!double.TryParse(value as string, NumberStyles.Float, CultureInfo.InvariantCulture, out angle))
                {
                    return value;
                }

            }
            else if (value is double || value is double?)
            {
                angle = (double)value;

            }
            else
            {
                return value.ToString();
            }

            bool isNegative = angle < 0;
            if (isNegative) angle = -angle;

            double degrees = Math.Truncate(angle);
            double remainder = (angle - degrees) * 60.0;
            double minutes = Math.Truncate(remainder);
            double seconds = (remainder - minutes) * 60.0;

            string result = degrees.ToString("##0", culture.NumberFormat) + "° " +
                            minutes.ToString("#0", culture.NumberFormat) + "' " +
                            seconds.ToString("#0.00", culture.NumberFormat) + "\" ";

            // The parameter contains "NS" for Latitudes and "EW" for Longitudes.
            if (parameter != null)
            {
                result += ((string)parameter).Substring((isNegative ? 1 : 0), 1);
            }
            else
            {
                result = (isNegative ? "-" : string.Empty) + result;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = value as string;

            if (string.IsNullOrEmpty(strValue))
            {
                return null;
            }

            double adjustForSign = 1.0;
            if (strValue.IndexOf("-") >= 0)
            {
                adjustForSign = -1.0;
                strValue = strValue.Substring(strValue.IndexOf("-") + 1);
            }

            // Parse the value in the field.  It's in three parts:  Degrees, minutes & seconds
            int degreeSymbol = strValue.IndexOf("°");
            int minuteSymbol = strValue.IndexOf("'");
            int secondSymbol = strValue.IndexOf('"');

            string degrees = null, minutes = null, seconds = null;
            double angle, d, m, s;

            if (degreeSymbol < 0)
            {
                if (double.TryParse(strValue, NumberStyles.Number, culture.NumberFormat, out angle))
                {
                    return angle;
                }
                else
                {
                    return value;
                }

            }
            else
            {
                degrees = strValue.Substring(0, degreeSymbol);

                if (minuteSymbol >= 0)
                {
                    minutes = strValue.Substring(degreeSymbol + 2, minuteSymbol - degreeSymbol - 2);
                }
                if (secondSymbol < 0)
                {
                    seconds = "0" + culture.NumberFormat.NumberDecimalSeparator + "0";
                }
                else
                {
                    seconds = strValue.Substring(minuteSymbol + 2, secondSymbol - minuteSymbol - 2);
                }
            }

            if (!double.TryParse(degrees, NumberStyles.Integer, culture.NumberFormat, out d)) return value;
            if (!double.TryParse(minutes, NumberStyles.Integer, culture.NumberFormat, out m)) return value;
            if (!double.TryParse(seconds, NumberStyles.Float, culture.NumberFormat, out s)) return value;
            angle = d + m / 60.0 + s / 3600.0;

            if (parameter != null)
            {
                if (strValue.Contains(((string)parameter).Substring(1, 1)))
                {
                    angle = -angle;
                }
            }
            else
            {
                angle *= adjustForSign;
            }
            return angle;
        }
        #endregion
    }
}
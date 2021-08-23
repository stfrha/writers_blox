using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WritersBlox
{
    class ImageSizeEnumDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(double))
                throw new InvalidOperationException("FRHA: The target is not a double, when converting image sizes!");
            
            ImageSizes isV = (ImageSizes) value;

            if (isV == ImageSizes.eSmallImage) return 60;
            if (isV == ImageSizes.eMediumImage) return 120;
            return 220;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}

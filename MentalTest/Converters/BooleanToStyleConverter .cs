using Xamarin.Forms;
using System;
using System.Globalization;

namespace MentalTest.Converters
{
    public class BooleanToStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var styles = parameter.ToString().Split(',');
            return value != null ? Application.Current.Resources[styles[1]] as Style : Application.Current.Resources[styles[0]] as Style;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

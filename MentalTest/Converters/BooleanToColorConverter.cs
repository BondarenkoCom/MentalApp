using System;
using Xamarin.Forms;
using System.Globalization;

namespace MentalTest.Converters
{
    public class BooleanToColorConverter : IValueConverter
    {
        public Color TrueColor { get; set; } = Color.Orange;
        public Color FalseColor { get; set; } = Color.White;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == parameter ? TrueColor : FalseColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}

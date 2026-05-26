using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Satur8.UI.Converters
{
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type t, object p, CultureInfo c)
            => value is string s && !string.IsNullOrEmpty(s)
                ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object v, Type t, object p, CultureInfo c)
            => throw new NotSupportedException();
    }
}

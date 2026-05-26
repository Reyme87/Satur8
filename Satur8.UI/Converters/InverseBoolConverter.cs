using System.Globalization;
using System.Windows.Data;

namespace Satur8.UI.Converters
{
    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type t, object p, CultureInfo c)
            => value is bool b && !b;

        public object ConvertBack(object v, Type t, object p, CultureInfo c)
            => v is bool b && !b;
    }
}

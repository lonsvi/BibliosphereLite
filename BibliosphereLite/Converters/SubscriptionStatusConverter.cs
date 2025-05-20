using System;
using System.Windows.Data;

namespace BibliosphereLite.Converters
{
    public class SubscriptionStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isActive = (bool)value;
            return isActive ? "Активен" : "Завершён";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
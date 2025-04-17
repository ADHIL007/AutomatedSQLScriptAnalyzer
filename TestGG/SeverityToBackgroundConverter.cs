using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TestGG
{
    public class SeverityToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string severity = value as string;

            switch (severity)
            {
                case "Error":
                    return new SolidColorBrush(Colors.Red);
                case "Performance":
                    return new SolidColorBrush(Colors.Yellow);
                case "Security":
                    return new SolidColorBrush(Colors.Blue);
                default:
                    return new SolidColorBrush(Colors.Gray);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null; // One-way binding, no need to implement this method.
        }
    }
}
using System;
using System.Globalization;
using System.Windows.Data;

namespace TimePunchIntroPlayer.Converters
{
    public class DoubleToTimespanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return TimeSpan.FromMilliseconds((double)value).ToString(@"hh\:mm\:ss");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

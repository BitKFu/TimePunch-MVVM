using System;
using Windows.UI.Xaml.Data;
using Microsoft.UI.Xaml.Data;

namespace TimePunchIntroPlayerUWP.Converters
{
    public class DoubleToTimespanConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return TimeSpan.FromMilliseconds((double)value).ToString(@"hh\:mm\:ss");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

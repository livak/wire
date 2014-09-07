using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Wire
{
    public class ResultToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                int quantity = (int)value;

                byte n = (byte)((255 / Math.Pow(quantity + 1, 0.2)));

                return new SolidColorBrush(Color.FromRgb(n, n, n));
            }

            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
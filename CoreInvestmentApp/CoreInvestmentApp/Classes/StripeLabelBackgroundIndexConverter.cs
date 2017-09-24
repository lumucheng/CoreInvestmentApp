using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CoreInvestmentApp.Classes
{
    public class StripeLabelBackgroundIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null) return Color.White;
            var index = ((ListView)parameter).ItemsSource.Cast<object>().ToList().IndexOf(value);
            return index % 2 == 0 ? Color.Black : Color.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

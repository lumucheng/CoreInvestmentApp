using System;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace CoreInvestmentApp.Classes
{
	public class StripedBackgroundIndexConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null || parameter == null) return Color.White;
			var index = ((ListView)parameter).ItemsSource.Cast<object>().ToList().IndexOf(value);
            return index % 2 == 0 ? Color.FromHex("#E5E5E5") : Color.FromHex("CEE6E5");
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

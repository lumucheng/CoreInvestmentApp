using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CoreInvestmentApp.Menu
{
    public class MenuListView : ListView
    {
        public MenuListView()
        {
            List<MenuItem> data = new MenuListData();
            ItemsSource = data;
            VerticalOptions = LayoutOptions.FillAndExpand;
            BackgroundColor = Color.Transparent;

            var cell = new DataTemplate(typeof(ImageCell));
            cell.SetBinding(TextCell.TextProperty, "Title");
            cell.SetBinding(ImageCell.ImageSourceProperty, "IconSource");
            cell.SetValue(TextCell.TextColorProperty, Color.White);

            ItemTemplate = cell;
            SelectedItem = data[0];

            Footer = new Label(); 
        }
    }
}

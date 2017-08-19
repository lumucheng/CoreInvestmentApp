using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CoreInvestmentApp.SliderPages
{
    public class ContractsPage : ContentPage
    {
        public ContractsPage()
        {
            Title = "Contracts";
            Icon = "Contracts.png";

            var layout = new StackLayout
            {
                Spacing = 0,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            Button add = new Button
            {
                HorizontalOptions = LayoutOptions.End,
                BackgroundColor = Xamarin.Forms.Color.White,
                Text = "ADD",
                TextColor = Xamarin.Forms.Color.Maroon,
            };
            add.Clicked += OnButtonClicked;

            layout.Children.Add(add);
            Content = layout;
        }

        void OnButtonClicked(object sender, EventArgs e)
        {
           
        }
    }
}

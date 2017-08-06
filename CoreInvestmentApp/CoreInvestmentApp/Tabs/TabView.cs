using System;
using Xamarin.Forms;

namespace CoreInvestmentApp.Tabs
{
    public class TabView : ContentView
    {
        public TabView()
        {
            BackgroundColor = Color.White;

            var label = new Label
            {
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Color.Black
            };

            label.SetBinding(Label.TextProperty, "Title");
            this.SetBinding(ContentView.BackgroundColorProperty, "Background");

            Content = new StackLayout
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Children = {
                    label
                }
            };
        }
    }
}


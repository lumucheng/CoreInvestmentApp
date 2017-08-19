using CoreInvestmentApp.Tabs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CoreInvestmentApp.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DetailedStockPage : ContentPage
	{
        View _tabs;
        RelativeLayout relativeLayout;
        SwitcherPageViewModel viewModel;

        public DetailedStockPage()
        {
            viewModel = new SwitcherPageViewModel();
            BindingContext = viewModel;

            StackLayout stackLayout = new StackLayout
            {
                BackgroundColor = Color.FromHex("#09b2c9"),
                Orientation = StackOrientation.Vertical,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(0, 0, 0, 0)
                
            };

            stackLayout.Children.Add(new Label { Text = "AAPL", TextColor = Color.White, FontSize = 24, FontAttributes = FontAttributes.Bold, Margin = new Thickness(15, 10, 0, 0)});
            stackLayout.Children.Add(new Label { Text = "Apple Inc.", TextColor = Color.White, FontSize = 16, Margin = new Thickness(15, 10, 0, 0)});

            relativeLayout = new RelativeLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.StartAndExpand
            };

            var pagesCarousel = CreatePagesCarousel();
            _tabs = CreateTabs();

            var tabsHeight = 50;
            relativeLayout.Children.Add(_tabs,
                Constraint.RelativeToParent((parent) => { return parent.X; }),
                Constraint.RelativeToParent((parent) => { return parent.Y - 80; }),
                Constraint.RelativeToParent(parent => parent.Width),
                Constraint.Constant(tabsHeight)
            );

            relativeLayout.Children.Add(pagesCarousel,
                Constraint.Constant(0),
                Constraint.RelativeToParent((parent) => { return parent.Y + tabsHeight - 80; }),
                Constraint.RelativeToParent((parent) => { return parent.Width; }),
                Constraint.RelativeToView(_tabs, (parent, sibling) => { return parent.Height - (sibling.Height); })
            );
            stackLayout.Children.Add(relativeLayout);
            Content = stackLayout;
        }

        CarouselLayout CreatePagesCarousel()
        {
            var carousel = new CarouselLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                IndicatorStyle = CarouselLayout.IndicatorStyleEnum.Tabs,
                ItemTemplate = new DataTemplate(typeof(ContentView))
            };
            carousel.SetBinding(CarouselLayout.ItemsSourceProperty, "Pages");
            carousel.SetBinding(CarouselLayout.SelectedItemProperty, "CurrentPage", BindingMode.TwoWay);

            return carousel;
        }

        View CreatePagerIndicatorContainer()
        {
            return new StackLayout
            {
                Children = { CreatePagerIndicators() }
            };
        }

        View CreatePagerIndicators()
        {
            var pagerIndicator = new PagerIndicatorDots() { DotSize = 5, DotColor = Color.Black };
            pagerIndicator.SetBinding(PagerIndicatorDots.ItemsSourceProperty, "Pages");
            pagerIndicator.SetBinding(PagerIndicatorDots.SelectedItemProperty, "CurrentPage");
            return pagerIndicator;
        }

        View CreateTabsContainer()
        {
            return new StackLayout
            {
                Children = { CreateTabs() }
            };
        }

        View CreateTabs()
        {
            var pagerIndicator = new PagerIndicatorTabs() { HorizontalOptions = LayoutOptions.FillAndExpand };
            pagerIndicator.RowDefinitions.Add(new RowDefinition() { Height = 50 });
            pagerIndicator.SetBinding(PagerIndicatorTabs.ItemsSourceProperty, "Pages");
            pagerIndicator.SetBinding(PagerIndicatorTabs.SelectedItemProperty, "CurrentPage");

            return pagerIndicator;
        }
    }

    public class SpacingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var items = value as IEnumerable<HomeViewModel>;

            var collection = new ColumnDefinitionCollection();
            foreach (var item in items)
            {
                collection.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            }
            return collection;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
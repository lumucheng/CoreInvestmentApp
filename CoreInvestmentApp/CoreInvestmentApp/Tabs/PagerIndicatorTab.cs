﻿using System;
using Xamarin.Forms;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace CoreInvestmentApp.Tabs
{
    public class PagerIndicatorTabs : Grid
    {
        int _selectedIndex;

        public Color DotColor { get; set; }
        public double DotSize { get; set; }

        public PagerIndicatorTabs()
        {
            HorizontalOptions = LayoutOptions.CenterAndExpand;
            VerticalOptions = LayoutOptions.Center;
            DotColor = Color.Black;
            BackgroundColor = Color.FromHex("89C4F4");

            var assembly = typeof(PagerIndicatorTabs).GetTypeInfo().Assembly;
            foreach (var res in assembly.GetManifestResourceNames())
                System.Diagnostics.Debug.WriteLine("found resource: " + res);
        }

        void CreateTabs()
        {

            if (Children != null && Children.Count > 0) Children.Clear();

            foreach (var item in ItemsSource)
            {

                var index = Children.Count;
                var tab = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Padding = new Thickness(7),
                };

                string title;
                string img;

                if (index == 0)
                {
                    title = "INFO";
                    img = "document.png";
                }
                else if (index == 1)
                {
                    title = "ASSESS";
                    img = "book.png";
                }
                else
                {
                    title = "VALUE";
                    img = "chart.png";
                }

                tab.Children.Add(new Image { Source = img, HeightRequest = 25 });
                tab.Children.Add(new Label { Text = title, FontSize = 12, FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.CenterAndExpand, TextColor = Color.White });

                var tgr = new TapGestureRecognizer();
                tgr.Command = new Command(() =>
                {
                    SelectedItem = ItemsSource[index];
                });
                tab.GestureRecognizers.Add(tgr);
                Children.Add(tab, index, 0);
            }
        }

        public static BindableProperty ItemsSourceProperty =
            BindableProperty.Create(
                nameof(ItemsSource),
                typeof(IList),
                typeof(PagerIndicatorTabs),
                null,
                BindingMode.OneWay,
                propertyChanging: (bindable, oldValue, newValue) =>
                {
                    ((PagerIndicatorTabs)bindable).ItemsSourceChanging();
                },
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    ((PagerIndicatorTabs)bindable).ItemsSourceChanged();
                }
        );

        public IList ItemsSource
        {
            get
            {
                return (IList)GetValue(ItemsSourceProperty);
            }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        public static BindableProperty SelectedItemProperty =
            BindableProperty.Create(
                nameof(SelectedItem),
                typeof(object),
                typeof(PagerIndicatorTabs),
                null,
                BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    ((PagerIndicatorTabs)bindable).SelectedItemChanged();
                }
        );

        public object SelectedItem
        {
            get
            {
                return GetValue(SelectedItemProperty);
            }
            set
            {
                SetValue(SelectedItemProperty, value);
            }
        }

        void ItemsSourceChanging()
        {
            if (ItemsSource != null)
                _selectedIndex = ItemsSource.IndexOf(SelectedItem);
        }

        void ItemsSourceChanged()
        {
            if (ItemsSource == null) return;

            this.ColumnDefinitions.Clear();
            foreach (var item in ItemsSource)
            {
                this.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            }

            CreateTabs();
        }

        void SelectedItemChanged()
        {

            var selectedIndex = ItemsSource.IndexOf(SelectedItem);
            var pagerIndicators = Children.Cast<StackLayout>().ToList();

            foreach (var pi in pagerIndicators)
            {
                UnselectTab(pi);
            }

            if (selectedIndex > -1)
            {
                SelectTab(pagerIndicators[selectedIndex]);
            }
        }

        static void UnselectTab(StackLayout tab)
        {
            tab.Opacity = 0.5;
        }

        static void SelectTab(StackLayout tab)
        {
            tab.Opacity = 1.0;
        }
    }
}



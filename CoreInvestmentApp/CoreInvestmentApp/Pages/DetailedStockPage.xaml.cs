using CoreInvestmentApp.Classes;
using CoreInvestmentApp.Model;
using CoreInvestmentApp.Tabs;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;

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
        Stock stock;

        public DetailedStockPage(Stock stock)
        {
            this.stock = stock;
            GetDetailedInfoAsync();
        }

        private async void GetDetailedInfoAsync()
        {
            string query = string.Format(Util.IntrinioAPIUrl +
                    "/data_point?identifier={0}&item=long_description,adj_close_price,volume,52_week_high,52_week_low,sector,marketcap,basiceps,epsgrowth,debttoequity", stock.StockIdentifier.Ticker);
            HttpClient client = Util.GetAuthHttpClient();
            var uri = new Uri(query);

            var response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                JObject jsonObject = JObject.Parse(json);
                JArray data = (JArray)jsonObject["data"];

                foreach (JToken token in data)
                {
                    string item = token["item"].ToString();
                    string value = token["value"].ToString();

                    if (item == "long_description")
                    {
                        stock.Description = value;
                    }
                    else if (item == "adj_close_price")
                    {
                        decimal adjClosePrice;
                        if (Decimal.TryParse(value, out adjClosePrice))
                        {
                            stock.AdjClosePrice = adjClosePrice;
                        }
                    }
                    else if (item == "volume")
                    {
                        decimal volume;
                        if (Decimal.TryParse(value, out volume))
                        {
                            stock.Volume = volume;
                        }
                    }
                    else if (item == "52_week_high")
                    {
                        decimal fiftyTwoWeekHigh;
                        if (Decimal.TryParse(value, out fiftyTwoWeekHigh))
                        {
                            stock.FiftyTwoWeekHigh = fiftyTwoWeekHigh;
                        }
                    }
                    else if (item == "52_week_low")
                    {
                        decimal fiftyTwoWeekLow;
                        if (Decimal.TryParse(value, out fiftyTwoWeekLow))
                        {
                            stock.FiftyTwoWeekLow = fiftyTwoWeekLow;   
                        }
                    }
                    else if (item == "sector")
                    {
                        stock.Sector = value;
                    }
                    else if (item == "marketcap")
                    {
                        decimal marketCap;
                        if (Decimal.TryParse(value, out marketCap))
                        {
                            stock.MarketCap = marketCap;
                        }
                    }
                    else if (item == "basiceps")
                    {
                        decimal basicEps;
                        if (Decimal.TryParse(value, out basicEps))
                        {
                            stock.BasicEps = basicEps;
                        }
                    }
                    else if (item == "epsgrowth")
                    {
                        decimal epsGrowth;
                        if (Decimal.TryParse(value, out epsGrowth))
                        {
                            stock.EpsGrowth = epsGrowth;
                        }
                    }
                    else if (item == "debttoequity")
                    {
                        decimal debtToEquity;
                        if (Decimal.TryParse(value, out debtToEquity))
                        {
                            stock.DebtToEquity = debtToEquity;
                        }
                    }
                }

                GetHistoricalEPS();
            }
        }

        private async void GetHistoricalEPS()
        {
            DateTime currentDate = DateTime.Now;
            DateTime previousDate = currentDate.AddYears(-10);

            string query = string.Format(Util.IntrinioAPIUrl +
                   "/historical_data?identifier={0}&item=basiceps&type=FY&start_date={1}-01-01&end_date={2}-01-01",
                   stock.StockIdentifier.Ticker, previousDate.Year, currentDate.Year);
            HttpClient client = Util.GetAuthHttpClient();
            var uri = new Uri(query);

            var response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                JObject jsonObject = JObject.Parse(json);
                JArray data = (JArray)jsonObject["data"];
                List<EarningPerShare> epsList = new List<EarningPerShare>();

                foreach (JToken token in data)
                {
                    string date = token["date"].ToString();
                    string value = token["value"].ToString();

                    if (value.Length > 0 && value != "null")
                    {
                        EarningPerShare eps = new EarningPerShare();
                        eps.DateStr = date;
                        eps.ValueStr = value;

                        epsList.Add(eps);
                    }
                }

                stock.EpsList = epsList;

                GetFreeCashFlow();
            }
        }

        private async void GetFreeCashFlow()
        {
            DateTime currentDate = DateTime.Now;
            DateTime previousDate = currentDate.AddYears(-10);

            string query = string.Format(Util.IntrinioAPIUrl +
                   "/historical_data?identifier={0}&item=netcashfromoperatingactivities&type=FY&start_date={1}-01-01&end_date={2}-01-01",
                   stock.StockIdentifier.Ticker, previousDate.Year, currentDate.Year);
            HttpClient client = Util.GetAuthHttpClient();
            var uri = new Uri(query);

            var response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                JObject jsonObject = JObject.Parse(json);
                JArray data = (JArray)jsonObject["data"];
                List<FreeCashFlow> cashFlowList = new List<FreeCashFlow>();

                foreach (JToken token in data)
                {
                    string date = token["date"].ToString();
                    string value = token["value"].ToString();

                    if (value.Length > 0 && value != "null")
                    {
                        FreeCashFlow fcf = new FreeCashFlow();
                        fcf.DateStr = date;
                        fcf.ValueStr = value;

                        cashFlowList.Add(fcf);
                    }
                }

                stock.CashFlowList = cashFlowList;

                GetDebtToEquity();
            }
        }

        private async void GetDebtToEquity()
        {
            DateTime currentDate = DateTime.Now;
            DateTime previousDate = currentDate.AddYears(-10);

            string query = string.Format(Util.IntrinioAPIUrl +
                   "/historical_data?identifier={0}&item=debttoequity&type=FY&start_date={1}-01-01&end_date={2}-01-01",
                   stock.StockIdentifier.Ticker, previousDate.Year, currentDate.Year);
            HttpClient client = Util.GetAuthHttpClient();
            var uri = new Uri(query);

            var response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                JObject jsonObject = JObject.Parse(json);
                JArray data = (JArray)jsonObject["data"];
                List<DebtToEquity> debtToEquityList = new List<DebtToEquity>();

                foreach (JToken token in data)
                {
                    string date = token["date"].ToString();
                    string value = token["value"].ToString();

                    if (value.Length > 0 && value != "null")
                    {
                        DebtToEquity debtToEqu = new DebtToEquity();
                        debtToEqu.DateStr = date;
                        debtToEqu.ValueStr = value;

                        debtToEquityList.Add(debtToEqu);
                    }
                }

                stock.DebtToEquityList = debtToEquityList;

                GetReturnOnEquity();
            }
        }

        private async void GetReturnOnEquity()
        {
            DateTime currentDate = DateTime.Now;
            DateTime previousDate = currentDate.AddYears(-10);

            string query = string.Format(Util.IntrinioAPIUrl +
                   "/historical_data?identifier={0}&item=roe&type=FY&start_date={1}-01-01&end_date={2}-01-01",
                   stock.StockIdentifier.Ticker, previousDate.Year, currentDate.Year);
            HttpClient client = Util.GetAuthHttpClient();
            var uri = new Uri(query);

            var response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                JObject jsonObject = JObject.Parse(json);
                JArray data = (JArray)jsonObject["data"];
                List<ReturnOnEquity> roeList = new List<ReturnOnEquity>();

                foreach (JToken token in data)
                {
                    string date = token["date"].ToString();
                    string value = token["value"].ToString();

                    if (value.Length > 0 && value != "null")
                    {
                        ReturnOnEquity returnToEquity = new ReturnOnEquity();
                        returnToEquity.DateStr = date;
                        returnToEquity.ValueStr = value;

                        roeList.Add(returnToEquity);
                    }
                }

                stock.ReturnToEquityList = roeList;
                GetReturnOnAsset();
            }
        }

        private async void GetReturnOnAsset()
        {
            DateTime currentDate = DateTime.Now;
            DateTime previousDate = currentDate.AddYears(-10);

            string query = string.Format(Util.IntrinioAPIUrl +
                   "/historical_data?identifier={0}&item=roa&type=FY&start_date={1}-01-01&end_date={2}-01-01",
                   stock.StockIdentifier.Ticker, previousDate.Year, currentDate.Year);
            HttpClient client = Util.GetAuthHttpClient();
            var uri = new Uri(query);

            var response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                JObject jsonObject = JObject.Parse(json);
                JArray data = (JArray)jsonObject["data"];
                List<ReturnOnAsset> roaList = new List<ReturnOnAsset>();

                foreach (JToken token in data)
                {
                    string date = token["date"].ToString();
                    string value = token["value"].ToString();

                    if (value.Length > 0 && value != "null")
                    {
                        ReturnOnAsset returnToAssest = new ReturnOnAsset();
                        returnToAssest.DateStr = date;
                        returnToAssest.ValueStr = value;

                        roaList.Add(returnToAssest);
                    }
                }

                stock.ReturnToAssetList = roaList;
                LayoutInterface();
            }
        }

        private void LayoutInterface()
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

            stackLayout.Children.Add(new Label { Text = stock.StockIdentifier.Ticker, TextColor = Color.White, FontSize = 24, FontAttributes = FontAttributes.Bold, Margin = new Thickness(15, 10, 0, 0) });
            stackLayout.Children.Add(new Label { Text = stock.StockIdentifier.Name, TextColor = Color.White, FontSize = 16, Margin = new Thickness(15, 10, 0, 0) });

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
                ItemTemplate = new DataTemplate(typeof(ContentView)),
                Stock = stock
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
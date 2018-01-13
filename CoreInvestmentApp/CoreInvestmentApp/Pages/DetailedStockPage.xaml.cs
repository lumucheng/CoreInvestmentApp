using Acr.UserDialogs;
using CoreInvestmentApp.Classes;
using CoreInvestmentApp.Model;
using CoreInvestmentApp.Tabs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FormsCommunityToolkit.Effects;

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
            Title = "Info";
            this.stock = stock;

            ToolbarItems.Add(new ToolbarItem
            {
                Text = "Add to Portfolio",
                Command = new Command(() => Navigation.PushModalAsync(new NavigationPage(new AddPortfolioPage(stock))
                {
                    BarBackgroundColor = Color.FromHex("4B77BE"),
                    BarTextColor = Color.White
                }))
            });

            if (!stock.UserManualEntry) // && DateTime.Now > stock.LastUpdated.AddHours(2))
            {
				UserDialogs.Instance.ShowLoading("Loading..", MaskType.Black);
				GetDetailedInfoAsync().ContinueWith((task) =>
				{
					UserDialogs.Instance.HideLoading();
				});
            }
            else 
            {
                LayoutInterface();
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Send<string>("refresh", "refresh");
        }

        private async Task GetDetailedInfoAsync()
        {
            string query = query = string.Format(Util.IntrinioAPIUrl +
                    "/data_point?identifier={0}&item=long_description,adj_close_price,volume,52_week_high,52_week_low,sector,marketcap,basiceps,epsgrowth,debttoequity,cashdividendspershare,dividendyield,bookvaluepershare,pricetobook,pricetoearnings,currentratio", stock.StockIdentifier.Ticker);

            HttpClient client = Util.GetAuthHttpClient();
            var uri = new Uri(query);

            if (Util.IsNetworkAvailable())
            {

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
                        else if (item == "cashdividendspershare")
                        {
                            decimal dividend;
                            if (Decimal.TryParse(value, out dividend))
                            {
                                stock.Dividend = dividend;
                            }
                        }
                        else if (item == "dividendyield")
                        {
                            decimal dividendYield;
                            if (Decimal.TryParse(value, out dividendYield))
                            {
                                stock.DividendYield = dividendYield;
                            }
                        }
                        else if (item == "pricetobook")
                        {
                            decimal priceToBook;
                            if (Decimal.TryParse(value, out priceToBook))
                            {
                                stock.PriceToBook = priceToBook;
                            }
                        }
                        else if (item == "bookvaluepershare")
                        {
                            decimal bookValuePerShare;
                            if (Decimal.TryParse(value, out bookValuePerShare))
                            {
                                stock.BookValuePerShare = bookValuePerShare;
                            }
                        }
                        else if (item == "pricetoearnings")
                        {
                            decimal priceToEarnings;
                            if (Decimal.TryParse(value, out priceToEarnings))
                            {
                                stock.PriceToEarnings = priceToEarnings;
                            }
                        }
                        else if (item == "currentratio")
                        {
                            decimal currentRatio;
                            if (Decimal.TryParse(value, out currentRatio))
                            {
                                stock.CurrentRatio = currentRatio;
                            }
                        }
                    }

                    stock.LastUpdated = DateTime.Now;
                    await GetHistoricalEPS();
                }
            }
            else
            {
                UserDialogs.Instance.Alert("Please ensure you have a working connection", "Error", "OK");
            }
        }

        private async Task GetHistoricalEPS()
        {
            DateTime currentDate = DateTime.Now;
            DateTime previousDate = currentDate.AddYears(-6);
            List<EarningPerShare> SortedList = stock.EpsList.OrderByDescending(o => o.Date).ToList();
            EarningPerShare latestEps = SortedList.FirstOrDefault();

            if (SortedList.Count == 0 || latestEps.Date.AddYears(1) < currentDate.Date)
            {
                string query = string.Format(Util.IntrinioAPIUrl +
                    "/historical_data?identifier={0}&item=dilutedeps&frequency=yearly&start_date={1}-01-01&end_date={2}-{3}-{4}",
                    stock.StockIdentifier.Ticker, previousDate.Year, currentDate.Year, currentDate.Month, currentDate.Day);
                HttpClient client = Util.GetAuthHttpClient();
                var uri = new Uri(query);

                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    JObject jsonObject = JObject.Parse(json);
                    JArray data = (JArray)jsonObject["data"];
                    List<EarningPerShare> epsList = new List<EarningPerShare>();
                    int index = 0;
                    DateTime latestDate = DateTime.Now;

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

                        if (index == 0)
                        {
                            latestDate = Convert.ToDateTime(date);
                            index++;
                        }
                    }

                    var query1 = string.Format(Util.IntrinioAPIUrl +
                        "/historical_data?identifier={0}&item=dilutedeps",
                    stock.StockIdentifier.Ticker, previousDate.Year, currentDate.Year, currentDate.Month, currentDate.Day);
                    var uri1 = new Uri(query1);

                    var response1 = await client.GetAsync(uri1);
                    if (response1.IsSuccessStatusCode)
                    {
                        json = await response1.Content.ReadAsStringAsync();
                        jsonObject = JObject.Parse(json);
                        data = (JArray)jsonObject["data"];

                        foreach (JToken token in data)
                        {
                            string date = token["date"].ToString();
                            string value = token["value"].ToString();

                            if (value.Length > 0 && value != "null")
                            {
                                DateTime dt = Convert.ToDateTime(date);

                                if (dt.Year > latestDate.Year)
                                {
                                    EarningPerShare eps = new EarningPerShare();
                                    eps.DateStr = date;
                                    eps.ValueStr = value;

                                    epsList.Add(eps);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }

                    stock.EpsList = epsList;
                }
            }

            await GetFreeCashFlow();
        }

        private async Task GetFreeCashFlow()
        {
            DateTime currentDate = DateTime.Now;
            DateTime previousDate = currentDate.AddYears(-6);
            List<FreeCashFlow> SortedList = stock.CashFlowList.OrderByDescending(o => o.Date).ToList();
            FreeCashFlow latestCashFlow = SortedList.FirstOrDefault();

            if (SortedList.Count == 0 || latestCashFlow.Date.AddMonths(1) < currentDate.Date)
            {
                string query = string.Format(Util.IntrinioAPIUrl +
                   "/historical_data?identifier={0}&item=netcashfromoperatingactivities&start_date={1}-01-01&end_date={2}-01-01",
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
                }
            }

            await GetDebtToEquity();
        }

        private async Task GetDebtToEquity()
        {
            DateTime currentDate = DateTime.Now;
            DateTime previousDate = currentDate.AddYears(-6);
            List<DebtToEquity> SortedList = stock.DebtToEquityList.OrderByDescending(o => o.Date).ToList();
            DebtToEquity latestDebtToEquity = SortedList.FirstOrDefault();

            if (SortedList.Count == 0 || latestDebtToEquity.Date.AddYears(1) < currentDate.Date)
            {
                string query = string.Format(Util.IntrinioAPIUrl +
                   "/historical_data?identifier={0}&item=debttoequity&start_date={1}-01-01&end_date={2}-01-01",
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
                }
            }

            await GetReturnOnEquity();
        }

        private async Task GetReturnOnEquity()
        {
            DateTime currentDate = DateTime.Now;
            DateTime previousDate = currentDate.AddYears(-6);
            List<ReturnOnEquity> SortedList = stock.ReturnToEquityList.OrderByDescending(o => o.Date).ToList();
            ReturnOnEquity latestReturnOnEquity = SortedList.FirstOrDefault();

            if (SortedList.Count == 0 || latestReturnOnEquity.Date.AddYears(1) < currentDate.Date)
            {
                string query = string.Format(Util.IntrinioAPIUrl +
                   "/historical_data?identifier={0}&item=roe&start_date={1}-01-01&end_date={2}-01-01",
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
                }
            }

            await GetReturnOnAsset();
        }

        private async Task GetReturnOnAsset()
        {
            DateTime currentDate = DateTime.Now;
            DateTime previousDate = currentDate.AddYears(-6);
            List<ReturnOnAsset> SortedList = stock.ReturnToAssetList.OrderByDescending(o => o.Date).ToList();
            ReturnOnAsset latestReturnOnAsset = SortedList.FirstOrDefault();

            if (SortedList.Count == 0 || latestReturnOnAsset.Date.AddYears(1) < currentDate.Date)
            {
                string query = string.Format(Util.IntrinioAPIUrl +
                   "/historical_data?identifier={0}&item=roa&start_date={1}-01-01&end_date={2}-01-01",
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
                }
            }

            await GetDividend();
        }

        private async Task GetDividend()
        {
            DateTime currentDate = DateTime.Now;
            DateTime previousDate = currentDate.AddYears(-6);
            List<Dividend> SortedList = stock.DividendList.OrderByDescending(o => o.Date).ToList();
            Dividend latestDividend = SortedList.FirstOrDefault();

            if (SortedList.Count == 0 || latestDividend.Date.AddYears(1) < currentDate.Date)
            {
                string query = string.Format(Util.IntrinioAPIUrl +
                   "/historical_data?identifier={0}&item=dividend&start_date={1}-01-01&end_date={2}-01-01",
                   stock.StockIdentifier.Ticker, previousDate.Year, currentDate.Year);
                HttpClient client = Util.GetAuthHttpClient();
                var uri = new Uri(query);

                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    JObject jsonObject = JObject.Parse(json);
                    JArray data = (JArray)jsonObject["data"];
                    List<Dividend> dividendList = new List<Dividend>();

                    foreach (JToken token in data)
                    {
                        string date = token["date"].ToString();
                        string value = token["value"].ToString();

                        if (value.Length > 0 && value != "null")
                        {
                            Dividend dividend = new Dividend();
                            dividend.DateStr = date;
                            dividend.ValueStr = value;

                            dividendList.Add(dividend);
                        }
                    }

                    stock.DividendList = dividendList;
                }
            }

            await GetBookValue();
        }

        private async Task GetBookValue()
        {
            DateTime currentDate = DateTime.Now;
            DateTime previousDate = currentDate.AddYears(-6);
            List<BookValue> SortedList = stock.BookValueList.OrderByDescending(o => o.Date).ToList();
            BookValue latestBookValue = SortedList.FirstOrDefault();

            if (SortedList.Count == 0 || latestBookValue.Date.AddYears(1) < currentDate.Date)
            {
                string query = string.Format(Util.IntrinioAPIUrl +
                   "/historical_data?identifier={0}&item=bookvaluepershare&start_date={1}-01-01&end_date={2}-01-01",
                   stock.StockIdentifier.Ticker, previousDate.Year, currentDate.Year);
                HttpClient client = Util.GetAuthHttpClient();
                var uri = new Uri(query);

                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    JObject jsonObject = JObject.Parse(json);
                    JArray data = (JArray)jsonObject["data"];
                    List<BookValue> bookValueList = new List<BookValue>();

                    foreach (JToken token in data)
                    {
                        string date = token["date"].ToString();
                        string value = token["value"].ToString();

                        if (value.Length > 0 && value != "null")
                        {
                            BookValue bookValue = new BookValue();
                            bookValue.DateStr = date;
                            bookValue.ValueStr = value;

                            bookValueList.Add(bookValue);
                        }
                    }

                    stock.BookValueList = bookValueList;
                }
            }

            LayoutInterface();
        }

        private void LayoutInterface()
        {
            viewModel = new SwitcherPageViewModel();
            BindingContext = viewModel;

            StackLayout stackLayout = new StackLayout
            {
				BackgroundColor = Color.FromHex("#89C4F4"),
				Orientation = StackOrientation.Vertical,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(0, 0, 0, 0)
            };

            Grid grid = new Grid();
            ColumnDefinition col1 = new ColumnDefinition();
            col1.Width = new GridLength(73, GridUnitType.Star);
            ColumnDefinition col2 = new ColumnDefinition();
            col2.Width = new GridLength(27, GridUnitType.Star);

            grid.ColumnDefinitions.Add(col1);
            grid.ColumnDefinitions.Add(col2);

            Label LabelTicker = new Label { Text = stock.StockIdentifier.Ticker, TextColor = Color.White, FontSize = 14, FontAttributes = FontAttributes.Bold, Margin = new Thickness(15, 10, 0, 0) };
            Label LabelIdentifier = new Label { Text = stock.StockIdentifier.Name, TextColor = Color.White, FontSize = 12, FontAttributes = FontAttributes.Bold, Margin = new Thickness(15, 10, 0, 0) };
            Label LabelClose = new Label { Text = "Price: " + Util.FormatNumberToCurrency(stock.AdjClosePrice, CURRENCY_TYPE.USD), TextColor = Color.White, FontSize = 12, FontAttributes = FontAttributes.Bold, Margin = new Thickness(15, 10, 0, 0) };

            StackLayout leftLayout = new StackLayout
            {
                BackgroundColor = Color.FromHex("#89C4F4"),
                Orientation = StackOrientation.Vertical,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(0, 0, 0, 0),
                HeightRequest = 105
            };

            leftLayout.Children.Add(LabelTicker);
            leftLayout.Children.Add(LabelIdentifier);
            leftLayout.Children.Add(LabelClose);

            Label LabelFiftyTwo = new Label { Text = "52-week", FontAttributes = FontAttributes.Bold, TextColor = Color.White, FontSize = 14, Margin = new Thickness(10, 5, 0, 5) };

            Label LabelFiftyTwoHigh = new Label { Text = "Hi: " + stock.FiftyTwoWeekHighString, FontAttributes = FontAttributes.Bold, TextColor = Color.FromHex("#27ae60"), FontSize = 12, Margin = new Thickness(10, 5, 0, 5) };
            LabelFiftyTwoHigh.Effects.Add(new SizeFontToFitEffect());

            Label LabelFiftyTwoLow = new Label { Text = "Lo: " + stock.FiftyTwoWeekLowString, FontAttributes = FontAttributes.Bold, TextColor = Color.FromHex("FF0000"), FontSize = 12, Margin = new Thickness(10, 5, 0, 5) };
            LabelFiftyTwoLow.Effects.Add(new SizeFontToFitEffect());

            StackLayout rightLayout = new StackLayout
            {
                BackgroundColor = Color.FromHex("#89C4F4"),
                Orientation = StackOrientation.Vertical,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(0, 0, 0, 0) ,
                HeightRequest = 105
            };

            rightLayout.Children.Add(LabelFiftyTwo);
            rightLayout.Children.Add(LabelFiftyTwoHigh);
            rightLayout.Children.Add(LabelFiftyTwoLow);

            grid.Children.Add(leftLayout, 0, 0);
            grid.Children.Add(rightLayout, 1, 0);

            stackLayout.Children.Add(grid);

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
                Constraint.RelativeToParent((parent) => { return parent.Y - 105; }),
                Constraint.RelativeToParent(parent => parent.Width),
                Constraint.Constant(tabsHeight)
            );

            relativeLayout.Children.Add(pagesCarousel,
                Constraint.Constant(0),
                Constraint.RelativeToParent((parent) => { return parent.Y + tabsHeight - 105; }),
                Constraint.RelativeToParent((parent) => { return parent.Width; }),
                Constraint.RelativeToView(_tabs, (parent, sibling) => { return parent.Height - (sibling.Height); })
            );
            stackLayout.Children.Add(relativeLayout);
            Content = stackLayout;

            // Save stock
            Util.SaveStockToDB(stock);
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
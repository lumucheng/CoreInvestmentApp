using Acr.UserDialogs;
using CoreInvestmentApp.Classes;
using CoreInvestmentApp.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Realms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CoreInvestmentApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WatchlistPage : ContentPage
    {
        public ObservableCollection<Stock> StockList { get; set; }

        public WatchlistPage()
        {
            InitializeComponent();
            StockList = new ObservableCollection<Stock>();

			var addItem = new ToolbarItem
			{
				Text = "Add"
			};

			addItem.Clicked += (object sender, System.EventArgs e) =>
			{
                HandleAddButton();
			};

            ToolbarItems.Add(addItem);

            MessagingCenter.Subscribe<string>("refresh", "refresh", (sender) => {
                LoadTickersFromDBAsync();
            });

            LoadTickersFromDBAsync();
        }

        private async void HandleAddButton()
        {
            // check accessrights first
            if (Util.AccessRights == "full" || 
                Util.AccessRights == "guest" && StockList.Count < 3 ||
                Util.AccessRights == "expired" && StockList.Count < 3)
            {
                var action = await DisplayActionSheet("Selection", "Cancel", null, "Search Stock", "Manual Add");

                if (action == "Search Stock")
                {
                    await Navigation.PushModalAsync(new NavigationPage(new SearchPage())
                    {
                        BarBackgroundColor = Color.FromHex("#4B77BE"),
                        BarTextColor = Color.White
                    });
                }
                else if (action == "Manual Add")
                {
                    await Navigation.PushModalAsync(new NavigationPage(new AddManualStock())
                    {
                        BarBackgroundColor = Color.FromHex("#4B77BE"),
                        BarTextColor = Color.White
                    });
                }
            }
            else
            {
                string message = "You have reached the max limit of stock valuation. Please subscribe to our app account to have unlimited access.";
                bool result = await DisplayAlert("Error", message, "Subscribe", "Cancel");

                if (result) {
                    Uri url = new Uri("http://coreinvest.me/appsubscribe.php");
                    Device.OpenUri(url);
                }
            }
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                Stock stock = (Stock)e.SelectedItem;
                Navigation.PushAsync(new DetailedStockPage(stock));
                ((ListView)sender).SelectedItem = null;
            }    
        }

        private void LoadTickersFromDBAsync()
        {
            var vRealmDb = Realm.GetInstance();
            var vAllStock = vRealmDb.All<RealmStockJson>();

            Dictionary<string, Stock> stockDictionary = new Dictionary<string, Stock>();
            string identifierQuery = "";

            if (vAllStock.Count() == 0)
            {
                StockListView.ItemsSource = null;
				StockListView.IsRefreshing = false;
				StockListView.EndRefresh();
            }
            else
            {
                foreach (RealmStockJson stockJson in vAllStock)
                {
                    Stock stock = JsonConvert.DeserializeObject<Stock>(stockJson.JsonObjStr);
                    StockIdentifier stockIdentifier = stock.StockIdentifier;

                    if (!stock.UserManualEntry)
                    {
                        identifierQuery += stockIdentifier.Ticker + ",";
                    }
                    stockDictionary.Add(stockIdentifier.Ticker, stock);
                }

                UserDialogs.Instance.ShowLoading("Getting data..", MaskType.Black);
                LoadTickersFromAPI(stockDictionary, identifierQuery).ContinueWith((task) =>
                {
                    UserDialogs.Instance.HideLoading();
                });
            }
        }

        private async Task LoadTickersFromAPI(Dictionary<string, Stock> stockDictionary, string identifierQuery)
        {
            string query = string.Format(Util.IntrinioAPIUrl +
                    "/data_point?identifier={0}&item=close_price,company_url", identifierQuery);
            HttpClient client = Util.GetAuthHttpClient();
            var uri = new Uri(query);

            if (stockDictionary.Count() > 0)
            {
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
                            string identifier = token["identifier"].ToString();
                            string item = token["item"].ToString();
                            string value = token["value"].ToString();

                            Stock stock = stockDictionary[identifier];

                            if (item == "close_price")
                            {
                                decimal currentValue;
                                if (Decimal.TryParse(value, out currentValue))
                                {
                                    stock.CurrentValue = currentValue;
                                }
                            }
                            else if (item == "company_url")
                            {
                                stock.ImageUrl = string.Format("https://logo.clearbit.com/{0}", value);
                            }
                        }

                        List<Stock> list = stockDictionary.Values.ToList();
                        list = list.OrderByDescending(s => s.InvestorConfidence)
                                             .ThenByDescending(s => s.CurrentValue)
                                             .ToList();
                        StockList = new ObservableCollection<Stock>(list);
                        StockListView.ItemsSource = null;
                        StockListView.ItemsSource = StockList;
                    }
                    else
                    {
						List<Stock> list = stockDictionary.Values.ToList();
						list = list.OrderByDescending(s => s.InvestorConfidence)
											 .ThenByDescending(s => s.CurrentValue)
											 .ToList();
						StockList = new ObservableCollection<Stock>(list);
						StockListView.ItemsSource = null;
						StockListView.ItemsSource = StockList;
                    }
                }
                else
                {
                    UserDialogs.Instance.Alert("Please ensure you have a working connection", "Error", "OK");
                }
            }
        }

        private void Handle_ItemTapped(object sender, System.EventArgs e)
        {
            Navigation.PushModalAsync(new NavigationPage(new SearchPage()));
            StockListView.SelectedItem = null;
        }

        private void OnDelete(object sender, System.EventArgs e)
        {
            var menuItem = (MenuItem)sender;
            Stock stock = (Stock)menuItem.CommandParameter;
			var vRealmDb = Realm.GetInstance();

			var realmJsonObj = vRealmDb.All<RealmStockJson>()
										   .Where(s => s.StockTicker == stock.StockIdentifier.Ticker)
										   .FirstOrDefault();
			vRealmDb.Write(() =>
		    {
                vRealmDb.Remove(realmJsonObj);   
		    });

            LoadTickersFromDBAsync();
        }

        private void StockListView_Refreshing(object sender, EventArgs e)
        {
            LoadTickersFromDBAsync();
            StockListView.IsRefreshing = false;
            StockListView.EndRefresh();
        }
    }
}
using Acr.UserDialogs;
using CoreInvestmentApp.Classes;
using CoreInvestmentApp.Model;
using CoreInvestmentApp.ViewModel;
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
				Navigation.PushModalAsync(new NavigationPage(new SearchPage())
                {
                    BarBackgroundColor = Color.FromHex("#4B77BE"),
                    BarTextColor = Color.White
                });
			};

            ToolbarItems.Add(addItem);

            MessagingCenter.Subscribe<string>("refresh", "refresh", (sender) => {
                LoadTickersFromDBAsync();
            });

            LoadTickersFromDBAsync();
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
                    identifierQuery += stockIdentifier.Ticker + ",";

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

                    StockList = new ObservableCollection<Stock>(stockDictionary.Values.ToList());
                    StockListView.ItemsSource = null;
                    StockListView.ItemsSource = StockList;
                }
                else
                {
                    UserDialogs.Instance.Alert("Something went wrong with the network", "Error", "OK");
                }
            }
            else
            {
                UserDialogs.Instance.Alert("Please ensure you have a working connection", "Error", "OK");
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
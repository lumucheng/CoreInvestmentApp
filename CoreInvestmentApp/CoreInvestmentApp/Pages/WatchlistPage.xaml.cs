using CoreInvestmentApp.Classes;
using CoreInvestmentApp.Model;
using CoreInvestmentApp.ViewModel;
using Newtonsoft.Json.Linq;
using Realms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CoreInvestmentApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WatchlistPage : ContentPage
    {
        public static ObservableCollection<Stock> StockList { get; set; }
        
        public WatchlistPage()
        {
            InitializeComponent();
            // BindingContext = new StockViewModel();

            StockList = new ObservableCollection<Stock>();
            BindingContext = StockList;
            this.Appearing += Page_Appearing;
        }

        private void Page_Appearing(object sender, EventArgs e)
        {
            LoadTickersFromDBAsync();
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Stock stock = (Stock)e.SelectedItem;
            Navigation.PushAsync(new DetailedStockPage(stock));
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new NavigationPage(new SearchPage()));
        }

        private async Task LoadTickersFromDBAsync()
        {
            var vRealmDb = Realm.GetInstance();
            var vAllStockIdentifier = vRealmDb.All<StockIdentifier>();
            Dictionary<string, Stock> stockDictionary = new Dictionary<string, Stock>();
            string identifierQuery = "";

            if (vAllStockIdentifier.Count() > StockList.Count)
            {
                foreach (StockIdentifier stockIdentifier in vAllStockIdentifier)
                {
                    identifierQuery += stockIdentifier.Ticker + ",";

                    Stock stock = new Stock();
                    stock.StockIdentifier = stockIdentifier;
                    stockDictionary.Add(stockIdentifier.Ticker, stock);
                }

                string query = string.Format(Util.IntrinioAPIUrl +
                    "/data_point?identifier={0}&item=revenuegrowth,dividend,average_daily_volume,close_price,company_url", identifierQuery);
                HttpClient client = Util.GetAuthHttpClient();
                var uri = new Uri(query);

                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    JObject jsonObject = JObject.Parse(json);
                    JArray data = (JArray)jsonObject["data"];
                    ObservableCollection<Stock> list = StockList;

                    foreach (JToken token in data)
                    {
                        string identifier = token["identifier"].ToString();
                        string item = token["item"].ToString();
                        string value = token["value"].ToString();

                        Stock stock = stockDictionary[identifier];

                        if (item == "revenuegrowth")
                        {
                            stock.Growth = value;
                        }
                        else if (item == "dividend")
                        {
                            stock.Dividend = value;
                        }
                        else if (item == "average_daily_volume")
                        {
                            stock.Average = value;
                        }
                        else if (item == "close_price")
                        {
                            stock.CurrentValue = value;
                        }
                        else if (item == "company_url")
                        {
                            stock.ImageUrl = string.Format("https://logo.clearbit.com/{0}", value);
                        }
                    }

                    StockListView.ItemsSource = new ObservableCollection<Stock>(stockDictionary.Values.ToList());
                }
                else
                {
                    //TODO: Handle error here
                }
            }
        }
    }
}
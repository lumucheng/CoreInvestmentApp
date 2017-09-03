using CoreInvestmentApp.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Newtonsoft.Json;
using CoreInvestmentApp.Model;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;
using Realms;

namespace CoreInvestmentApp.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SearchPage : ContentPage
	{
        private ObservableCollection<StockIdentifier> identifierList = new ObservableCollection<StockIdentifier>();
        public ObservableCollection<StockIdentifier> IdentifierList
        {
            get 
            {
                return identifierList;
            }
            set 
            {
                identifierList = value;
            }
        }
         
		public SearchPage ()
		{
			InitializeComponent ();
            Title = "Search";

			ToolbarItems.Add(new ToolbarItem
			{
				Text = "Done",
				Command = new Command(() => Navigation.PopModalAsync()),
			});
		}

        private async Task Entry_Completed(object sender, EventArgs e)
        {
            string query = EntrySearch.Text;
            query = query.Trim();

            HttpClient client = Util.GetAuthHttpClient();
            var uri = new Uri(string.Format(Util.IntrinioAPIUrl + "/companies?query={0}", query));

            var response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                JObject jsonObject = JObject.Parse(json);
                JArray data = (JArray)jsonObject["data"];
                IdentifierList = JsonConvert.DeserializeObject<ObservableCollection<StockIdentifier>>(data.ToString());
                StockIdentifierListView.ItemsSource = IdentifierList;
            }
            else 
            {
                //TODO: Handle error here
            }
        }

        private void StockIdentifierListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var vRealmDb = Realm.GetInstance();
            StockIdentifier stockIdentifier = (StockIdentifier)e.SelectedItem;
            Stock stock = new Stock();
            stock.StockIdentifier = stockIdentifier;

            string jsonObj = JsonConvert.SerializeObject(stock);
            RealmStockJson stockJson = new RealmStockJson();
            stockJson.JsonObjStr = jsonObj;
            stockJson.StockTicker = stockIdentifier.Ticker;

            vRealmDb.Write(() => {
                vRealmDb.Add(stockJson, true);
            });

            Navigation.PopModalAsync();
        }
    }
}
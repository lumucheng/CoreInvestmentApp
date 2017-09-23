using Acr.UserDialogs;
using CoreInvestmentApp.Classes;
using CoreInvestmentApp.Model;
using Newtonsoft.Json.Linq;
using OxyPlot;
using OxyPlot.Series;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CoreInvestmentApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PortfolioPage : ContentPage
    {
        List<PortfolioStock> portfolioList;
        public PlotModel PortfolioModel { get; set; }

        public PortfolioPage()
        {
            Title = "Portfolio";
            InitializeComponent();

            if (Device.RuntimePlatform == Device.Android)
            {
                StackLay.VerticalOptions = LayoutOptions.FillAndExpand;
			}
            else if (Device.RuntimePlatform == Device.iOS)
            {
                StackLay.VerticalOptions = LayoutOptions.StartAndExpand;
            }

            this.Appearing += Page_Appearing;
        }

        private void Page_Appearing(object sender, EventArgs e)
        {
            Init();
        }

        private void Init()
        {
            UserDialogs.Instance.ShowLoading("Getting data..", MaskType.Black);
            GetDataFromAPI().ContinueWith((task) =>
            {
                UserDialogs.Instance.HideLoading();
            });
        }

        private async Task GetDataFromAPI()
        {
            string identifierQuery = "";
            var vRealmDb = Realm.GetInstance();
            var vAllPortfolio = vRealmDb.All<PortfolioStock>();
            Dictionary<string, PortfolioStock> portfolioDictionary = new Dictionary<string, PortfolioStock>();

            portfolioList = vAllPortfolio.ToList();

            if (portfolioList.Count > 0)
            {
				foreach (PortfolioStock portfolio in portfolioList)
				{
					identifierQuery += portfolio.StockTicker + ",";
					portfolioDictionary[portfolio.StockTicker] = portfolio;
				}

				string query = string.Format(Util.IntrinioAPIUrl +
						"/data_point?identifier={0}&item=close_price,company_url", identifierQuery);
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
						string identifier = token["identifier"].ToString();
						string item = token["item"].ToString();
						string value = token["value"].ToString();

						foreach (PortfolioStock portfolio in portfolioList)
						{
							if (portfolio.StockTicker == identifier)
							{
								if (item == "close_price")
								{
									decimal currentValue;
									if (Decimal.TryParse(value, out currentValue))
									{
										portfolio.CurrentValue = currentValue;
									}
								}
							}
						}
					}

					InitLayout();
				}
				else
				{
					UserDialogs.Instance.Alert("Something went wrong with the network", "Error", "OK");
				}
            }
            else
            {
                ListViewPortfolio.ItemsSource = null;
                LabelPortfolioPercent.Text = "";
                LabelPortfolioValue.Text = "";
                LabelTotalCapital.Text = "";
                PortfolioModel = null;
                PlotPortfolio.BindingContext = null;
            }
        }

        private void InitLayout()
        {
            PlotPortfolio.BindingContext = null;
            decimal totalProfitLoss = 0.0M;
            decimal totalCapital = 0.0M;
            decimal totalCurrentValue = 0.0M;

            foreach(PortfolioStock portfolio in portfolioList)
            {
                string purchaseString = string.Format("{0}.{1}", portfolio.PurchasePriceBeforeDecimalPoint, portfolio.PurchasePriceAfterDecimalPoint);
                decimal value = portfolio.Quantity * Decimal.Parse(purchaseString);
                decimal currentValue = portfolio.Quantity * portfolio.CurrentValue;
                decimal profitLoss = currentValue - value;

                totalCurrentValue += currentValue;
                totalProfitLoss += profitLoss;
                totalCapital += value;
            }

            LabelPortfolioValue.Text = Util.FormatNumberToCurrency(totalProfitLoss, CURRENCY_TYPE.USD);
            decimal percent = totalCurrentValue / totalCapital;

            if (percent < 1.0M)
            {
                LabelPortfolioPercent.Text = "-" + Util.FormatNumberToPercent(1 - percent);
                LabelPortfolioPercent.TextColor = Color.FromHex("FF0000");
                LabelPortfolioValue.TextColor = Color.FromHex("FF0000");
            }
            else
            {
                LabelPortfolioPercent.Text = Util.FormatNumberToPercent(percent - 1);
                LabelPortfolioPercent.TextColor = Color.FromHex("38DE40");
                LabelPortfolioValue.TextColor = Color.FromHex("38DE40");
            }

            LabelTotalCapital.Text = "Total Capital: " + Util.FormatNumberToCurrency(totalCapital, CURRENCY_TYPE.USD);
            
            ListViewPortfolio.ItemsSource = portfolioList;
            CreatePieChart();
            PlotPortfolio.BindingContext = this;
        }

        private void CreatePieChart()
        {
            Dictionary<string, decimal> dictionaryPortfolio = new Dictionary<string, decimal>();
            decimal total = 0.0M;

            var model = new PlotModel { Title = "" };

            var ps = new PieSeries
            {
                StrokeThickness = 2.0,
                InsideLabelPosition = 0.5,
                OutsideLabelFormat = "",
                TickHorizontalLength = 0.0,
                TickRadialLength = 0.0,
                AngleSpan = 360,
                StartAngle = 0
            };

            foreach (PortfolioStock portfolio in portfolioList)
            {
                string purchaseString = string.Format("{0}.{1}", portfolio.PurchasePriceBeforeDecimalPoint, portfolio.PurchasePriceAfterDecimalPoint);
                decimal value = portfolio.Quantity * Decimal.Parse(purchaseString);
                string key = portfolio.StockTicker;

                if (dictionaryPortfolio.ContainsKey(key))
                {
                    dictionaryPortfolio[key] += value;
                }
                else
                {
                    dictionaryPortfolio[key] = value;
                }

                total += value;
            }

            foreach (string key in dictionaryPortfolio.Keys)
            {
                double value = (double)dictionaryPortfolio[key];
                double percent = (value / (double)total) * 100;
                string label = string.Format("{0}\n{1}%", key, Math.Round(percent, 2));
                ps.Slices.Add(new PieSlice(label, value));
            }

            model.Series.Add(ps);
            this.PortfolioModel = model;
        }

        private void ListViewPortfolio_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
			if (e.SelectedItem != null)
			{
                PortfolioStock portfolio = (PortfolioStock)e.SelectedItem;
                Navigation.PushModalAsync(new NavigationPage(new EditPortfolioPage(portfolio)));
				((ListView)sender).SelectedItem = null;
			}
        }

        private void MenuItem_Clicked(object sender, EventArgs e)
        {
            var menuItem = (MenuItem)sender;
            PortfolioStock stockPortfolio = (PortfolioStock)menuItem.CommandParameter;
            var vRealmDb = Realm.GetInstance();

            var realmJsonObj = vRealmDb.All<PortfolioStock>()
                                           .Where(p => p.PortfolioID == stockPortfolio.PortfolioID)
                                           .FirstOrDefault();

            using (var transaction = vRealmDb.BeginWrite())
            {
                vRealmDb.Remove(realmJsonObj);
                transaction.Commit();
            }

            Init();
        }
    }
}
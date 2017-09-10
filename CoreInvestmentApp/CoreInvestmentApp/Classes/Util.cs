using CoreInvestmentApp.Model;
using Newtonsoft.Json;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CoreInvestmentApp.Classes
{
    public enum CURRENCY_TYPE
    {
        DOLLAR_SIGN,
        USD,
        SGD
    }

    public static class Util
    {
        private static string IntrinioID = "773bd894834c1d90100edc4002b8ddc9";
        private static string IntrinioPassword = "e6614f242403f487ebac88401a931fdc";
        public static string IntrinioAPIUrl = "https://api.intrinio.com";

        private static string GetBasicAuth()
        {
            string auth = string.Format("{0}:{1}", IntrinioID, IntrinioPassword);
            string authBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(auth));
            return authBase64;
        }

        public static HttpClient GetAuthHttpClient()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", GetBasicAuth());
            return client;
        }

        public static string FormatNumberEnglishUnits(decimal n)
        {
            if (n < 1000)
                return n.ToString();

            if (n < 10000)
                return String.Format("{0:#,.##}K", n - 5);

            if (n < 100000)
                return String.Format("{0:#,.#}K", n - 50);

            if (n < 1000000)
                return String.Format("{0:#,.}K", n - 500);

            if (n < 10000000)
                return String.Format("{0:#,,.##}M", n - 5000);

            if (n < 100000000)
                return String.Format("{0:#,,.#}M", n - 50000);

            if (n < 1000000000)
                return String.Format("{0:#,,.}M", n - 500000);

            return String.Format("{0:#,,,.##}B", n - 5000000);
        }

        public static string FormatNumberToCurrency(decimal num, CURRENCY_TYPE type)
        {
            string sign = "";
            if (type == CURRENCY_TYPE.DOLLAR_SIGN)
            {
                sign = "$";
            }
            else if (type == CURRENCY_TYPE.USD)
            {
                sign = "USD";
            }
            else if (type == CURRENCY_TYPE.SGD)
            {
                sign = "SGD";
            }
            return sign + num.ToString("0.##");
        }

        public static string FormatNumberToPercent(decimal num)
        {
            return String.Format("{0:P2}", num);
        }
        
        public static void SaveStockToDB(Stock stock)
        {
            var vRealmDb = Realm.GetInstance();

            RealmStockJson stockJson = new RealmStockJson();
            stockJson.StockTicker = stock.Ticker;
            stockJson.JsonObjStr = JsonConvert.SerializeObject(stock);

            vRealmDb.Write(() =>
            {
                vRealmDb.Add(stockJson, true);
            });
        }
    }
}

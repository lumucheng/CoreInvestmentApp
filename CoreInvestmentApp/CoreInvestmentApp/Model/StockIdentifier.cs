using System;
using Newtonsoft.Json;

namespace CoreInvestmentApp.Model
{
    public class StockIdentifier
    {
		[JsonProperty("ticker")]
		public string Ticker { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("lei")]
		public string Lei { get; set; }

		[JsonProperty("cik")]
		public string Cik { get; set; }

		[JsonProperty("latest_filing_date")]
		public string LatestFillingDate { get; set; }

        public StockIdentifier()
        {
            
        }
    }
}

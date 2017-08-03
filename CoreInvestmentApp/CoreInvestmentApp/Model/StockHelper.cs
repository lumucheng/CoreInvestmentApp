using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreInvestmentApp.Model
{
    public class StockHelper
    {
        public static ObservableCollection<Stock> StockList { get; set; }

        public string ImageUrl { get; set; }
        public string CurrentValue { get; set; }
        public string Growth { get; set; }
        public string Dividend { get; set; }
        public string Average { get; set; }

        static StockHelper()
        {
            StockList = new ObservableCollection<Stock>();
            StockList.Add(new Stock
            {
                Name = "FB",
                Category = "News",
                ImageUrl = "https://cdn4.iconfinder.com/data/icons/miu-gloss-social/60/facebook-512.png",
                CurrentValue = "USD116.92",
                Growth = "USD38.75",
                Dividend = "USD0.00",
                Average = "USD14.85"
            });

            StockList.Add(new Stock
            {
                Name = "GNTX",
                Category = "News",
                ImageUrl = "https://weekherald.com/logos/gentex-co-logo.jpg",
                CurrentValue = "USD19.92",
                Growth = "USD18.00",
                Dividend = "USD5.00",
                Average = "USD5.16"
            });

            StockList.Add(new Stock
            {
                Name = "MDLZ",
                Category = "News",
                ImageUrl = "https://pbs.twimg.com/profile_images/476066629000785921/6D3DK65E_400x400.jpeg",
                CurrentValue = "USD44.72",
                Growth = "USD7.95",
                Dividend = "USD30.99",
                Average = "USD14.13"
            });

            StockList.Add(new Stock
            {
                Name = "MMM",
                Category = "News",
                ImageUrl = "https://media.licdn.com/mpr/mpr/shrink_200_200/AAEAAQAAAAAAAAerAAAAJGNmYTQ1MmFkLTExYjMtNDY3MC1iMDkyLTJmYjc2ODBiNDM2MQ.png",
                CurrentValue = "USD178.08",
                Growth = "USD45.48",
                Dividend = "USD148.00",
                Average = "USD14.97"
            });

            StockList.Add(new Stock
            {
                Name = "NKE",
                Category = "News",
                ImageUrl = "https://s-media-cache-ak0.pinimg.com/736x/93/fa/6c/93fa6c564347ce8f7cec4b18b689d2f2--nike-womens-shoes-nike-free-shoes.jpg",
                CurrentValue = "USD51.02",
                Growth = "USD29.77",
                Dividend = "USD16.00",
                Average = "USD5.82"
            });
        }
    }
}

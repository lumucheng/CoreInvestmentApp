using CoreInvestmentApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreInvestmentApp.ViewModel
{
    public class StockViewModel
    {
        public ObservableCollection<Stock> StockList { get; set; }

        public StockViewModel()
        {
            StockList = StockHelper.StockList;
        }
    }
}

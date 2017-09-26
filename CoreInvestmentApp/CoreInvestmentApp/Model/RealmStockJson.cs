using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Realms;

namespace CoreInvestmentApp.Model
{
    public class RealmStockJson : RealmObject
    {
        [PrimaryKey]
        public string StockTicker { get; set; }

        public string JsonObjStr { get; set; }

        public bool UserEnteredStock { get; set; }
    }
}

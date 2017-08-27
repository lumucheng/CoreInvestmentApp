using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreInvestmentApp.Model
{
    public class Dividend
    {
        private string dateStr;
        private string valueStr;
        private DateTime date;
        private decimal value;

        public string DateStr
        {
            get { return dateStr; }
            set { dateStr = value; }
        }

        public string ValueStr
        {
            get { return valueStr; }
            set { valueStr = value; }
        }

        public DateTime Date
        {
            get { return Convert.ToDateTime(dateStr); }
        }

        public decimal Value
        {
            get { return Decimal.Parse(valueStr); }
        }
    }
}

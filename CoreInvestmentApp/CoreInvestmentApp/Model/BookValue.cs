using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreInvestmentApp.Model
{
    public class BookValue
    {
        private string dateStr;
        private string valueStr;
        private DateTime date;
        private double value;

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

        public double Value
        {
            get { return Math.Round(Double.Parse(valueStr), 2); }
        }
    }
}

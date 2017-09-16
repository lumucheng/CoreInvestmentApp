using System;
using Realms;

namespace CoreInvestmentApp.Model
{
    public class User : RealmObject
    {
        [PrimaryKey]
        public string Email { get; set; }

        public string P { get; set; }

        public User()
        {
        }
    }
}

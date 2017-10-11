
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;

namespace StartFinance.Models
{
    public class ShoppingList
    {
        [PrimaryKey,AutoIncrement]
        public int ID { get; set; }

        [Unique]
        public string NameOfItem { get; set; }

        [NotNull]
        public double PriceQuoted { get; set; }

        [NotNull]
        public string ShoppingDate { get; set; }
    }
}

using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartFinance.Models
{
    public class ContactDetails
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [NotNull]
        public string FirstName { get; set; }

        [NotNull]
        public string LastName { get; set; }

        [NotNull]
        public int Phone { get; set; }
    }
}

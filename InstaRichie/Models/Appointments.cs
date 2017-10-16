using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;

namespace StartFinance.Models
{
    public class Appointments
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [Unique]
        public string AptDescription { get; set; }

        [NotNull]
        public string AptDate { get; set; }

        [NotNull]
        public int AptStartTime{ get; set; }

        [NotNull]
        public int AptEndTime { get; set; }

    }
}
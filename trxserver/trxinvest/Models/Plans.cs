using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace trxinvest.Models
{
    public class Plans
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public float DailyIncome { get; set; }
        public long Days { get; set; }
    }
}

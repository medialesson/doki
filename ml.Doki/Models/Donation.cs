using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ml.Doki.Models
{
    public class Donation
    {
        public string FullName { get; set; }

        public decimal Amount { get; set; }

        public DateTime DonatedAt { get; set; } = DateTime.Now;
    }
}

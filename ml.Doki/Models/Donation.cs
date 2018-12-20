using ml.Doki.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ml.Doki.Models
{
    public class Donation
    {
        public string FullName { get; set; }

        private decimal _amount;
        public decimal Amount
        {
            get => decimal.Parse(_amount.ToString(), CultureInfo.CurrentUICulture);
            set => _amount = value;
        }

        public DateTime DonatedAt { get; set; } = DateTime.Now;
    }
}

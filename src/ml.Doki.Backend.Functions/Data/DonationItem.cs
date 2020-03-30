using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ml.Doki.Backend.Functions.Data
{
    public class DonationItem : TableEntity
    {
        public DonationItem()
        {
            RowKey = Guid.NewGuid().ToString();
            PartitionKey = "doki_donations";
            Timestamp = DateTime.Now;
        }

        public string FullName { get; set; }
        
        // Azure Table Storage doesn't support decimal entities
        [IgnoreProperty]
        public decimal Amount
        {
            get => decimal.Parse(AmountString, NumberStyles.Currency, new CultureInfo(CultureName));
            set => this.AmountString = value.ToString(new CultureInfo(CultureName));
        }

        public string AmountString { get; set; }

        public string CultureName { get; set; } = Environment.GetEnvironmentVariable("Locale") ?? CultureInfo.InvariantCulture.Name;

        public DateTime DonatedAt { get; set; } = DateTime.Now;
    }
}

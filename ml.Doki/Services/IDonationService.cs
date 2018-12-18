using ml.Doki.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ml.Doki.Services
{
    public interface IDonationService
    {
        public Task DonateAsync(Donation donation);

        public Task<IList<Donation>> GetAllDonationsAsync();
    }
}

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
        Task DonateAsync(Donation donation);

        Task<IList<Donation>> GetAllDonationsAsync();
    }
}

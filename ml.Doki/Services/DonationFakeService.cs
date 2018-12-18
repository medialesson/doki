using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ml.Doki.Models;

namespace ml.Doki.Services
{
    public class DonationFakeService : IDonationService
    {
        private List<Donation> _donations = new List<Donation>()
        {
            new Donation
            {
                FullName = "Samuel Oechsler",
                Amount = 3.50m,
                DonatedAt = DateTime.Now
            },
            new Donation
            {
                FullName = "Samuel Oechsler",
                Amount = 4.30m,
                DonatedAt = DateTime.Now
            },
            new Donation
            {
                FullName = "Melissa Mast",
                Amount = 5,
                DonatedAt = DateTime.Now
            },
            new Donation
            {
                FullName = "Gino Messmer",
                Amount = 100,
                DonatedAt = DateTime.Now
            },
        };

        public async Task DonateAsync(Donation donation)
        {
            await Task.Run(() =>
            {
                _donations.Add(donation);
            });
        }

        public async Task<IList<Donation>> GetAllDonationsAsync()
        {
            return await Task.FromResult(_donations.OrderByDescending(x => x.DonatedAt).ToList());
        }
    }
}

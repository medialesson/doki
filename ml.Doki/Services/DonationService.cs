using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ml.Doki.Helpers;
using ml.Doki.Models;

namespace ml.Doki.Services
{
    public class DonationService : IDonationService
    {
        DonationFakeService fakeService;
        DonationRemoteService remoteService;

        public DonationService()
        {
            fakeService = Singleton<DonationFakeService>.Instance;
            remoteService = Singleton<DonationRemoteService>.Instance;
        }

        private IDonationService CurrentService
        {
            get
            {
                if (Singleton<Settings>.Instance.IsApiEnabled)
                    return remoteService;
                return fakeService;
            }
        }
    
        public Task DonateAsync(Donation donation)
        {
            return CurrentService.DonateAsync(donation);
        }

        public async Task<IList<Donation>> GetAllDonationsAsync()
        {
            return await CurrentService.GetAllDonationsAsync();
        }
    }
}

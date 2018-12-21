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
        DonationLocalService localService;
        DonationRemoteService remoteService;

        public DonationService()
        {
            localService = Singleton<DonationLocalService>.Instance;
            remoteService = Singleton<DonationRemoteService>.Instance;
        }

        private IDonationService CurrentService
        {
            get
            {
                if (Singleton<Settings>.Instance.IsApiEnabled)
                    return remoteService;
                return localService;
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

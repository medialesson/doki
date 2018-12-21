using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AppCenter.Analytics;
using ml.Doki.Models;
using Newtonsoft.Json;
using Windows.Storage;

namespace ml.Doki.Services
{
    public class DonationLocalService : IDonationService
    {
        public async Task DonateAsync(Donation donation)
        {
            StorageFolder localStorage = ApplicationData.Current.LocalFolder;
            StorageFile donationStorageFile = null;
            try
            {
                donationStorageFile = await localStorage.GetFileAsync("donations.json");
            }
            catch (Exception)
            {
                donationStorageFile = await localStorage.CreateFileAsync("donations.json");
            }

            var donations = await GetAllDonationsAsync();
            donations.Add(donation);

            var donationsData = JsonConvert.SerializeObject(donations);
            await FileIO.WriteTextAsync(donationStorageFile, donationsData);

            Analytics.TrackEvent("DonationLocalService.CommitDonation");
        }

        public async Task<IList<Donation>> GetAllDonationsAsync()
        {
            StorageFolder localStorage = ApplicationData.Current.LocalFolder;
            StorageFile donationStorageFile = null;

            Analytics.TrackEvent("DonationLocalService.GetDonations");

            try
            {
                donationStorageFile = await localStorage.GetFileAsync("donations.json");

                var donationData = await FileIO.ReadTextAsync(donationStorageFile);
                return JsonConvert.DeserializeObject<List<Donation>>(donationData).OrderByDescending(x => x.DonatedAt).ToList();
            }
            catch (Exception)
            {
                return new List<Donation>();
            }
        }
    }
}

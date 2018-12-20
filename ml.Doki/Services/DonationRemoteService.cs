using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ml.Doki.Helpers;
using ml.Doki.Models;
using Newtonsoft.Json;

namespace ml.Doki.Services
{
    public class DonationRemoteService : IDonationService
    {
        public async Task DonateAsync(Donation donation)
        {
            await Singleton<Settings>.Instance.InitializeAsync();
            if (!Singleton<Settings>.Instance.IsApiEnabled)
                return;

            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, Singleton<Settings>.Instance.RemotePostEndpoint);
                request.Content = new StringContent(JsonConvert.SerializeObject(donation));

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
            }
        }

        public async Task<IList<Donation>> GetAllDonationsAsync()
        {
            await Singleton<Settings>.Instance.InitializeAsync();
            if (!Singleton<Settings>.Instance.IsApiEnabled)
                return new List<Donation>();

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(Singleton<Settings>.Instance.RemoteGetEndpoint);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Donation>>(responseString);
            }
        }
    }
}

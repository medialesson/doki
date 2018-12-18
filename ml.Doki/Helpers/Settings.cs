using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace ml.Doki.Helpers
{
    public class Settings
    {
        public string AboutText { get; private set; }

        public string ApplicationCultureName { get; private set; }


        public async Task InitializeAsync()
        {
            AboutText = await ApplicationData.Current.LocalSettings.ReadAsync<string>(nameof(AboutText));
            ApplicationCultureName = await ApplicationData.Current.LocalSettings.ReadAsync<string>(nameof(ApplicationCultureName)) ?? "en-us";
        }

        public async Task SetAboutTextAsync(string text)
        {
            await ApplicationData.Current.LocalSettings.SaveAsync<string>(nameof(AboutText), text);
            await InitializeAsync();
        }

        public async Task SetApplicationCultureNameAsync(string name)
        {
            await ApplicationData.Current.LocalSettings.SaveAsync<string>(nameof(ApplicationCultureName), name);
            await InitializeAsync();
        }

        public async Task<IEnumerable<string>> GetAllAvailableCultureNamesAsync()
        {
            return await Task.FromResult(new List<string>()
            {
                "en-us",
                "de-de"
            });
        }
    }
}

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

        public CultureInfo ApplicationCultureInfo => new CultureInfo(ApplicationCultureName);


        public string AppCenterId { get; private set; }

        private bool _isApiEnabled;
        public bool IsApiEnabled
        {
            get
            {
                return _isApiEnabled && !string.IsNullOrEmpty(RemoteGetEndpoint) && !string.IsNullOrEmpty(RemotePostEndpoint);
            }
            set
            {
                _isApiEnabled = value;
            }
        }

        public string RemoteGetEndpoint { get; private set; }

        public string RemotePostEndpoint { get; private set; }


        public async Task InitializeAsync()
        {
            AboutText = await ApplicationData.Current.LocalSettings.ReadAsync<string>(nameof(AboutText));
            ApplicationCultureName = await ApplicationData.Current.LocalSettings.ReadAsync<string>(nameof(ApplicationCultureName)) ?? "en-us";


            AppCenterId = await ApplicationData.Current.LocalSettings.ReadAsync<string>(nameof(AppCenterId));

            IsApiEnabled = await ApplicationData.Current.LocalSettings.ReadAsync<bool>(nameof(IsApiEnabled));
            RemoteGetEndpoint = await ApplicationData.Current.LocalSettings.ReadAsync<string>(nameof(RemoteGetEndpoint));
            RemotePostEndpoint = await ApplicationData.Current.LocalSettings.ReadAsync<string>(nameof(RemotePostEndpoint));
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

        public async Task SetAppCenterIdAsync(string id)
        {
            await ApplicationData.Current.LocalSettings.SaveAsync<string>(nameof(AppCenterId), id);
            await InitializeAsync();
        }

        public async Task SetApiIsEnabledAsync(bool isEnabled)
        {
            await ApplicationData.Current.LocalSettings.SaveAsync<bool>(nameof(IsApiEnabled), isEnabled);
            await InitializeAsync();
        }

        public async Task<IEnumerable<string>> GetAllAvailableCultureNamesAsync()
        {
            return await Task.FromResult(new List<string>()
            {
                "en-US",
                "de-DE"
            });
        }

        public async Task SetRemoteEndpointsAsync(string remoteGetEndpoint, string remotePostEndpoint)
        {
            await ApplicationData.Current.LocalSettings.SaveAsync<string>(nameof(RemoteGetEndpoint), remoteGetEndpoint);
            await ApplicationData.Current.LocalSettings.SaveAsync<string>(nameof(RemotePostEndpoint), remotePostEndpoint);
            await InitializeAsync();
        }
    }
}

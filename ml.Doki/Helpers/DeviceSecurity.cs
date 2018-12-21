using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials.UI;
using Windows.UI.Popups;

namespace ml.Doki.Helpers
{
    public class DeviceSecurity
    {
        public static async Task<bool> ChallengeWindowsHelloAsync()
        {
            if (await UserConsentVerifier.CheckAvailabilityAsync() == UserConsentVerifierAvailability.Available)
            {
                var consentResult = await UserConsentVerifier.RequestVerificationAsync("DeviceSecurity_WindowsHelloRequest/Description".GetLocalized());

                if (consentResult == UserConsentVerificationResult.Verified)
                {
                    return true;
                }
            }


            // Fallback
            var dialog = new MessageDialog("DeviceSecurity_ChallengeWindowsHelloError/Description".GetLocalized(),
                "DeviceSecurity_ChallengeWindowsHelloError/Title".GetLocalized());

            await dialog.ShowAsync();
            return false;
        }
    }
}

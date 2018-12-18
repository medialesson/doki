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
                var consentResult = await UserConsentVerifier.RequestVerificationAsync("Let's make sure you're authorized to make changes");

                if (consentResult == UserConsentVerificationResult.Verified)
                {
                    return true;
                }
            }


            // Fallback
            var dialog = new MessageDialog("Make sure Windows Hello has been already set up first.", "You are not authorized");

            await dialog.ShowAsync();
            return false;
        }
    }
}

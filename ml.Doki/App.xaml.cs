using System;

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

using ml.Doki.Helpers;
using ml.Doki.Services;

using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace ml.Doki
{
    public sealed partial class App : Application
    {
        private Lazy<ActivationService> _activationService;

        private ActivationService ActivationService
        {
            get { return _activationService.Value; }
        }

        public App()
        {
            InitializeComponent();

            // Deferred execution until used. Check https://msdn.microsoft.com/library/dd642331(v=vs.110).aspx for further info on Lazy<T> class.
            _activationService = new Lazy<ActivationService>(CreateActivationService);
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            await Singleton<Settings>.Instance.InitializeAsync();
            await Singleton<ContactService>.Instance.PromptForPermissionsAsync();

            if (!args.PrelaunchActivated)
            {
                await ActivationService.ActivateAsync(args);
            }

            // Init app center
            var appCenterId = Singleton<Settings>.Instance.AppCenterId;
            if (string.IsNullOrEmpty(appCenterId))
            {
                AppCenter.Start(appCenterId, typeof(Analytics), typeof(Crashes));
            }
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            await ActivationService.ActivateAsync(args);
        }

        private ActivationService CreateActivationService()
        {
            return new ActivationService(this, typeof(Views.PivotPage));
        }
    }
}

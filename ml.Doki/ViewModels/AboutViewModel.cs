using System;
using System.Windows.Input;
using Microsoft.Toolkit.Uwp.UI.Controls;
using ml.Doki.Helpers;
using Windows.System;
using Windows.UI.Xaml.Controls;
using ml.Doki.Views;
using Microsoft.AppCenter.Analytics;

namespace ml.Doki.ViewModels
{
    public class AboutViewModel : Observable
    {
        #region Properties
        private string _aboutText;
        public string AboutText
        {
            get { return _aboutText; }
            set { Set(ref _aboutText, value); }
        }

        public string AppDisplayName { get; set; }

        public string AppVersion { get; set; }

        public string AppDescription { get; set; }
        #endregion

        #region Commands
        public ICommand LoadCommand { get; }

        public ICommand MarkdownLinkClickedCommand { get; }

        public ICommand OpenConfigurationsCommand { get; }
        #endregion

        public AboutViewModel()
        {
            AppDisplayName = "AppDisplayName".GetLocalized();
            AppVersion = Settings.GetAppVersion();
            AppDescription = "AppDescription".GetLocalized();

            LoadCommand = new RelayCommand(Load);
            OpenConfigurationsCommand = new RelayCommand(OpenConfigurations);
            MarkdownLinkClickedCommand = new RelayCommand<LinkClickedEventArgs>(MarkdownLinkClicked);

            LoadCommand.Execute(null);
        }

        public async void Load()
        {
            await Singleton<Settings>.Instance.InitializeAsync();
            this.AboutText = Singleton<Settings>.Instance.AboutText;
        }

        private async void MarkdownLinkClicked(LinkClickedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri(args.Link));
        }



        public async void OpenConfigurations()
        {
#if !DEBUG || !NOAUTH
            if (await DeviceSecurity.ChallengeWindowsHelloAsync())
            {
#endif
                var configurationPage = new ConfigurationPage();
                var dialog = new ContentDialog
                {
                    Content = configurationPage,

                    PrimaryButtonText = "DonatePage_OpenConfigurationsDialog/PrimaryButtonText".GetLocalized(),
                    PrimaryButtonCommand = configurationPage.ViewModel.SaveCommand,

                    CloseButtonText = "DonatePage_OpenConfigurationsDialog/CloseButtonText".GetLocalized(),

                    DefaultButton = ContentDialogButton.Primary
                };

                await dialog.ShowAsync();

                Analytics.TrackEvent("Donate.OpenConfiguration");
#if !DEBUG || !NOAUTH
            }
            else
            {
                Analytics.TrackEvent("Donate.OpenConfigurationUnauthorized");
            }
#endif
        }
    }
}

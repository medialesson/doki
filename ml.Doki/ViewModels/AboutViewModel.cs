using System;
using System.Windows.Input;
using Microsoft.Toolkit.Uwp.UI.Controls;
using ml.Doki.Helpers;
using Windows.System;

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
        #endregion

        #region Commands
        public ICommand LoadCommand { get; }

        public ICommand MarkdownLinkClickedCommand { get; }
        #endregion

        public AboutViewModel()
        {
            LoadCommand = new RelayCommand(Load);
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
    }
}

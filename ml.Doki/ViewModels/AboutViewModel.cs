using System;
using System.Windows.Input;
using ml.Doki.Helpers;

namespace ml.Doki.ViewModels
{
    public class AboutViewModel : Observable
    {
        private string _aboutText;
        public string AboutText
        {
            get { return _aboutText; }
            set { Set(ref _aboutText, value); }
        }

        public ICommand LoadCommand { get; }

        public AboutViewModel()
        {
            LoadCommand = new RelayCommand(Load);
            LoadCommand.Execute(null);
        }

        public async void Load()
        {
            await Singleton<Settings>.Instance.InitializeAsync();
            this.AboutText = Singleton<Settings>.Instance.AboutText;
        }
    }
}

using System;
using System.Windows.Input;
using ml.Doki.Helpers;
using Windows.UI.Popups;

namespace ml.Doki.ViewModels
{
    public class ConfigurationViewModel : Observable
    {
        private string _aboutText;
        public string AboutText
        {
            get => _aboutText;
            set => Set(ref _aboutText, value);
        }


        public ICommand LoadCommand { get; }

        public ICommand SaveCommand { get; }


        public ConfigurationViewModel()
        {
            LoadCommand = new RelayCommand(Load);
            SaveCommand = new RelayCommand(Save);

            LoadCommand.Execute(null);
        }

        public async void Load()
        {
            await Singleton<Settings>.Instance.InitializeAsync();
            AboutText = Singleton<Settings>.Instance.AboutText;
        }

        public async void Save()
        {
            await Singleton<Settings>.Instance.SetAboutTextAsync(this.AboutText);

            await new MessageDialog("Settings were applied.").ShowAsync();
        }
    }
}

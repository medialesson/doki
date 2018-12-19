using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ml.Doki.Helpers;
using Windows.ApplicationModel.Resources;
using Windows.UI.Popups;

namespace ml.Doki.ViewModels
{
    public class ConfigurationViewModel : Observable
    {
        #region Resource
        private ResourceLoader Resource { get; }
        #endregion

        #region Properties
        private string _aboutText;
        public string AboutText
        {
            get => _aboutText;
            set => Set(ref _aboutText, value);
        }

        
        private ObservableCollection<string> _availableLocales;
        public ObservableCollection<string> AvailableLocales
        {
            get => _availableLocales;
            set => Set(ref _availableLocales, value);
        }


        private string _selectedCurrencyLocale;
        public string SelectedCurrencyLocale
        {
            get => _selectedCurrencyLocale;
            set => Set(ref _selectedCurrencyLocale, value);
        }


        private string _appCenterId;
        public string AppCenterId
        {
            get => _appCenterId;
            set => Set(ref _appCenterId, value);
        }
        #endregion

        #region Commands
        public ICommand LoadCommand { get; }

        public ICommand SaveCommand { get; }
        #endregion

        public ConfigurationViewModel()
        {
            // Set resource manager
            Resource = ResourceLoader.GetForCurrentView();

            // Set commands
            LoadCommand = new RelayCommand(Load);
            SaveCommand = new RelayCommand(Save);

            LoadCommand.Execute(null);
        }

        public async void Load()
        {
            await Singleton<Settings>.Instance.InitializeAsync();

            AboutText = Singleton<Settings>.Instance.AboutText;
            AvailableLocales = new ObservableCollection<string>(await Singleton<Settings>.Instance.GetAllAvailableCultureNamesAsync());
            SelectedCurrencyLocale = Singleton<Settings>.Instance.ApplicationCultureName;

            AppCenterId = Singleton<Settings>.Instance.AppCenterId;
        }

        public async void Save()
        {
            await Singleton<Settings>.Instance.SetAboutTextAsync(this.AboutText);
            await Singleton<Settings>.Instance.SetApplicationCultureNameAsync(this.SelectedCurrencyLocale);

            // TODO: Is this legit?
            Singleton<DonateViewModel>.Instance.FetchCurrencySymbol();
            Singleton<DonateViewModel>.Instance.FetchCurrencyPlaceholder();
            Singleton<OverviewViewModel>.Instance.LoadCommand.Execute(null);
            Singleton<AboutViewModel>.Instance.LoadCommand.Execute(null);

            await Singleton<Settings>.Instance.SetAppCenterIdAsync(this.AppCenterId);

            await new MessageDialog(Resource.GetString("ConfigurationPage_SaveDialog/Description")).ShowAsync();
        }
    }
}

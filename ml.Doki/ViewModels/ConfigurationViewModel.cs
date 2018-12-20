﻿using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ml.Doki.Helpers;
using Windows.ApplicationModel.Resources;
using Windows.UI.Popups;

namespace ml.Doki.ViewModels
{
    public class ConfigurationViewModel : Observable
    {
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

        private string _remoteGetEndpoint;
        public string RemoteGetEndpoint
        {
            get => _remoteGetEndpoint;
            set => Set(ref _remoteGetEndpoint, value);
        }


        private string _remotePostEndpoint;
        public string RemotePostEndpoint
        {
            get => _remotePostEndpoint;
            set => Set(ref _remotePostEndpoint, value);
        }


        public ICommand LoadCommand { get; }

        public ICommand SaveCommand { get; }
        #endregion

        public ConfigurationViewModel()
        {
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
            RemoteGetEndpoint = Singleton<Settings>.Instance.RemoteGetEndpoint;
            RemotePostEndpoint = Singleton<Settings>.Instance.RemotePostEndpoint;
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
            await Singleton<Settings>.Instance.SetRemoteEndpointsAsync(this.RemoteGetEndpoint, this.RemotePostEndpoint);

            await new MessageDialog("ConfigurationPage_SaveDialog/Description".GetLocalized()).ShowAsync();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using Humanizer.Localisation;
using Microsoft.AppCenter.Analytics;
using ml.Doki.Helpers;
using ml.Doki.Models;
using ml.Doki.Services;
using ml.Doki.Views;
using Windows.ApplicationModel.Contacts;
using Windows.ApplicationModel.Resources;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace ml.Doki.ViewModels
{
    public class DonateViewModel : Observable
    {
        #region Properties
        private ImageSource _currentDonatorImageSource;
        public ImageSource CurrentDonatorImageSource
        {
            get => _currentDonatorImageSource;
            set => Set(ref _currentDonatorImageSource, value);
        }


        private string _currentDonationAmount;
        public string CurrentDonationAmount
        {
            get => _currentDonationAmount;
            set => Set(ref _currentDonationAmount, value);
        }


        private string _currentDonationName;
        public string CurrentDonationName
        {
            get => _currentDonationName;
            set
            {
                Set(ref _currentDonationName, value);
                AssignAvatarByName(value);
            }
        }


        private ObservableCollection<string> _donationNameAutoSuggestList;
        public ObservableCollection<string> DonationNameAutoSuggestList
        {
            get => _donationNameAutoSuggestList;
            set => Set(ref _donationNameAutoSuggestList, value);
        }


        private string _currencyPlaceholder;
        public string CurrencyPlaceholder
        {
            get => _currencyPlaceholder;
            set => Set(ref _currencyPlaceholder, value);
        }


        private string _currencySymbol;
        public string CurrencySymbol
        {
            get => _currencySymbol;
            set => Set(ref _currencySymbol, value);
        }


        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => Set(ref _isLoading, value);
        }


        private bool _isDonationButtonVisible = true;
        public bool IsDonationButtonVisible
        {
            get => _isDonationButtonVisible;
            set => Set(ref _isDonationButtonVisible, value);
        }
        #endregion

        #region Commands

        public ICommand DonateCommand { get; }

        public ICommand FetchAvatarCommand { get; }

        public ICommand FetchAverageDonationAmountCommand { get; }

        public ICommand ChooseFromContactsCommand { get; }

        public ICommand OpenConfigurationsCommand { get; }

        public ICommand PopulateAutoSuggestNamesCommand { get; }

        public ICommand FocusNextElementCommand { get; }

        public ICommand FetchCurrencySymbolCommand { get; }
        #endregion

        public DonateViewModel()
        {
            // Set properties
            DonationNameAutoSuggestList = new ObservableCollection<string>();
            FetchCurrencyPlaceholder();
            FetchCurrencySymbol();

            // Set commands
            DonateCommand = new RelayCommand(Donate);
            FetchAvatarCommand = new RelayCommand<string>(AssignAvatarByName);
            ChooseFromContactsCommand = new RelayCommand(ChooseFromContacts);
            OpenConfigurationsCommand = new RelayCommand(OpenConfigurations);
            PopulateAutoSuggestNamesCommand = new RelayCommand(PopulateAutoSuggestNames);
            FocusNextElementCommand = new RelayCommand(FocusNextElement);
            FetchAverageDonationAmountCommand = new RelayCommand(AssignAverageDonationAmount);
        }

        public async void Donate()
        {
            // Do validation
            if (!IsInputValid())
            {
                var dialog = new MessageDialog("DonatePage_DonateValidationDialog/Description".GetLocalized(),
                    "DonatePage_DonateValidationDialog/Title".GetLocalized());
                await dialog.ShowAsync();
                return;
            }

            try
            {
                IsLoading = true;

                // Donate
                await Singleton<DonationService>.Instance.DonateAsync(new Donation
                {
                    FullName = CurrentDonationName,
                    Amount = decimal.Parse(CurrentDonationAmount, NumberStyles.Currency, Singleton<Settings>.Instance.ApplicationCultureInfo),
                    DonatedAt = DateTime.Now
                });

                // Refresh overview view model in background
                Singleton<OverviewViewModel>.Instance.LoadCommand.Execute(null);

                // Clear input
                ClearInput();
                IsLoading = false;

                // Show confirmation
                var confirmationDialog = new ContentDialog
                {
                    Content = new DonationConfirmationPage(),

                    PrimaryButtonText = "DonatePage_DonateConfirmationDialog/PrimaryButtonText".GetLocalized(),
                    DefaultButton = ContentDialogButton.Primary
                };

                await confirmationDialog.ShowAsync();

                // Navigate to overview page
                Singleton<PivotViewModel>.Instance.SelectOverviewPivot();
            }
            catch(HttpRequestException ex)
            {
                await new MessageDialog("DonatePage_CommitException/Content".GetLocalized()).ShowAsync();

                Analytics.TrackEvent("Donate.CommitException", new Dictionary<string, string>
                {
                    {"message", ex.Message}
                });
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool IsInputValid()
        {
            decimal amount;

            bool isNameValid = !string.IsNullOrEmpty(CurrentDonationName);
            bool isAmountValid = decimal.TryParse(CurrentDonationAmount,
                NumberStyles.Currency, CultureInfo.CurrentCulture,
                out amount);

            return isNameValid && isAmountValid && amount > 0.01m;
        }

        private async void AssignAvatarByName(string name)
        {
            // Check input
            if (!string.IsNullOrEmpty(name))
            {
                // Get contact
                Contact contact = await Singleton<ContactService>.Instance.GetContactByDisplayNameAsync(name);
                if (contact != null)
                {
                    // Load image
                    var image = await Singleton<ContactService>.Instance.LoadContactAvatarToBitmapAsync(contact);

                    // Assign to image source
                    CurrentDonatorImageSource = image;
                    return;
                }
            }

            // In any other case - reset the image
            CurrentDonatorImageSource = null;
        }

        private async void AssignViewByContact(Contact contact)
        {
            CurrentDonationName = contact.DisplayName;
            CurrentDonatorImageSource = await Singleton<ContactService>.Instance.LoadContactAvatarToBitmapAsync(contact);
        }

        private async void ChooseFromContacts()
        {
            if (IsLoading)
                return;

            var contact = await Singleton<ContactService>.Instance.PromptUserForContactAsync();

            if(contact != null)
                AssignViewByContact(contact);

            Analytics.TrackEvent("Donate.ChooseContact");
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

        public async void PopulateAutoSuggestNames()
        {
            DonationNameAutoSuggestList.Clear();

            if(CurrentDonationName.Length >= 3)
            {
                var list = (await Singleton<ContactService>.Instance.SearchContactsByNameAsync(CurrentDonationName, true)).ToList();
                list.ForEach(c => DonationNameAutoSuggestList.Add(c.DisplayName));
            }
        }

        private void FocusNextElement()
        {
            FocusManager.TryMoveFocus(FocusNavigationDirection.Next);
        }

        private void AssignAverageDonationAmount()
        {
            var locale = Singleton<Settings>.Instance.ApplicationCultureName;
            var cultureInfo = new CultureInfo(locale);

            var currentMonthDonations = Singleton<OverviewViewModel>.Instance.DonatorsPerMonth.FirstOrDefault();
            if (currentMonthDonations != null)
            {
                var averageMonthDonation = currentMonthDonations.ToList().Average(d => decimal.Parse(d.TotalAmount.ToString(), CultureInfo.CurrentUICulture));
                CurrentDonationAmount = averageMonthDonation.ToString("F2", cultureInfo);
            }
            else
            {
                CurrentDonationAmount = CurrencyPlaceholder;
            }

            Analytics.TrackEvent("Donate.AssignAverageAmount");
        }

        public void FetchCurrencyPlaceholder()
        {
            var locale = Singleton<Settings>.Instance.ApplicationCultureName;
            var cultureInfo = new CultureInfo(locale);

            CurrencyPlaceholder = 1.ToString("F2", cultureInfo);
        }

        public void FetchCurrencySymbol()
        {
            var locale = Singleton<Settings>.Instance.ApplicationCultureName;
            var cultureInfo = new CultureInfo(locale);

            CurrencySymbol = cultureInfo.NumberFormat.CurrencySymbol;
        }

        public void ClearInput()
        {
            CurrentDonationName = string.Empty;
            CurrentDonationAmount = string.Empty;
            CurrentDonatorImageSource = null;
        }
    }
}

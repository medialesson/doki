using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using Microsoft.AppCenter.Analytics;
using ml.Doki.Helpers;
using ml.Doki.Models;
using ml.Doki.Services;
using ml.Doki.Views;
using Windows.ApplicationModel.Contacts;
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
        #endregion

        #region Commands

        public ICommand DonateCommand { get; }

        public ICommand FetchAvatarCommand { get; }

        public ICommand ChooseFromContactsCommand { get; }

        public ICommand OpenConfigurationsCommand { get; }

        public ICommand PopulateAutoSuggestNamesCommand { get; }

        public ICommand FocusNextElementCommand { get; }

        #endregion

        public DonateViewModel()
        {
            // Set properties
            DonationNameAutoSuggestList = new ObservableCollection<string>();

            // Set commands
            DonateCommand = new RelayCommand(Donate);
            FetchAvatarCommand = new RelayCommand<string>(AssignAvatarByName);
            ChooseFromContactsCommand = new RelayCommand(ChooseFromContacts);
            OpenConfigurationsCommand = new RelayCommand(OpenConfigurations);
            PopulateAutoSuggestNamesCommand = new RelayCommand(PopulateAutoSuggestNames);
            FocusNextElementCommand = new RelayCommand(FocusNextElement);
        }

        public async void Donate()
        {
            // Do validation
            if (!IsInputValid())
            {
                var dialog = new MessageDialog("Please check all required fields and try again.", "Your input is not valid");
                await dialog.ShowAsync();
                return;
            }

            // Donate
            await Singleton<DonationFakeService>.Instance.DonateAsync(new Donation
            {
                FullName = CurrentDonationName,
                Amount = decimal.Parse(CurrentDonationAmount, NumberStyles.Currency, CultureInfo.CurrentCulture),
                DonatedAt = DateTime.Now
            });

            // Refresh overview view model in background
            Singleton<OverviewViewModel>.Instance.LoadCommand.Execute(null);

            // Clear input
            ClearInput();

            // Show confirmation
            var confirmationDialog = new ContentDialog
            {
                Content = new DonationConfirmationPage(),

                PrimaryButtonText = "Dismiss",
                DefaultButton = ContentDialogButton.Primary
            };

            await confirmationDialog.ShowAsync();
        }

        private bool IsInputValid()
        {
            decimal amount;

            bool isNameValid = !string.IsNullOrEmpty(CurrentDonationName);
            bool isAmountValid = decimal.TryParse(CurrentDonationAmount,
                NumberStyles.Currency, CultureInfo.CurrentCulture,
                out amount);

            return isNameValid && isAmountValid;
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
            var contact = await Singleton<ContactService>.Instance.PromptUserForContactAsync();

            if(contact != null)
                AssignViewByContact(contact);
        }

        public async void OpenConfigurations()
        {
            if (await DeviceSecurity.ChallengeWindowsHelloAsync())
            {
                var configurationPage = new ConfigurationPage();
                var dialog = new ContentDialog
                {
                    Content = configurationPage,

                    PrimaryButtonText = "Apply",
                    PrimaryButtonCommand = configurationPage.ViewModel.SaveCommand,

                    CloseButtonText = "Cancel",

                    DefaultButton = ContentDialogButton.Primary
                };

                await dialog.ShowAsync();
            }
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

        public void ClearInput()
        {
            CurrentDonationName = string.Empty;
            CurrentDonationAmount = string.Empty;
            CurrentDonatorImageSource = null;
        }
    }
}

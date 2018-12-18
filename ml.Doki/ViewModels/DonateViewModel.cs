using System;
using System.Windows.Input;
using ml.Doki.Helpers;
using ml.Doki.Models;
using ml.Doki.Services;
using Windows.ApplicationModel.Contacts;
using Windows.UI.Xaml.Controls;
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
        #endregion 

        #region Commands

        public ICommand DonateCommand { get; }

        public ICommand FetchAvatarCommand { get; }

        #endregion

        public DonateViewModel()
        {
            // Set commands
            DonateCommand = new RelayCommand(Donate);
            FetchAvatarCommand = new RelayCommand<string>(AssignAvatarByName);
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

        public async void Donate()
        {
            // Do validation

            // Donate
            await Singleton<DonationFakeService>.Instance.DonateAsync(new Donation
            {
                FullName = CurrentDonationName,
                Amount = decimal.Parse(CurrentDonationAmount),
                DonatedAt = DateTime.Now
            });

            // Refresh overview view model in background
            Singleton<OverviewViewModel>.Instance.LoadCommand.Execute(null);

            // Clear input
            ClearInput();
        }

        public void ClearInput()
        {
            CurrentDonationName = string.Empty;
            CurrentDonationAmount = string.Empty;
            CurrentDonatorImageSource = null;
        }
    }
}

using System;
using System.Windows.Input;
using ml.Doki.Helpers;
using ml.Doki.Models;
using ml.Doki.Services;

namespace ml.Doki.ViewModels
{
    public class DonateViewModel : Observable
    {
        #region Properties
        private string _currentDonationAmount;
        public string CurrentDonationAmount { get => _currentDonationAmount; set => Set(ref _currentDonationAmount, value); }


        private string _currentDonationName;
        public string CurrentDonationName { get => _currentDonationName; set => Set(ref _currentDonationName, value); }
        #endregion 

        #region Commands
        public ICommand DonateCommand { get; }
        #endregion

        public DonateViewModel()
        {
            // Set commands
            DonateCommand = new RelayCommand(Donate);
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
            Singleton<OverviewViewModel>.Instance.LoadCommand.Execute(null);
        }
    }
}

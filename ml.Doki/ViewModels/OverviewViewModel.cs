using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ml.Doki.Helpers;
using ml.Doki.Models;
using ml.Doki.Models.Grouping;
using ml.Doki.Services;
using ml.Doki.Views;
using Windows.ApplicationModel.Contacts;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace ml.Doki.ViewModels
{
    public class OverviewViewModel : Observable
    {
        #region Properties
        private ObservableCollection<Donator> _donators;
        public ObservableCollection<Donator> Donators { get => _donators; set => Set(ref _donators, value); }


        private Donator _selectedDonator;
        public Donator SelectedDonator { get => _selectedDonator; set => Set(ref _selectedDonator, value); }


        private ObservableCollection<DonatorsPerMonthGroup> _donatorsPerMonth;
        public ObservableCollection<DonatorsPerMonthGroup> DonatorsPerMonth
        {
            get => _donatorsPerMonth;
            set => Set(ref _donatorsPerMonth, value);
        }


        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => Set(ref _isLoading, value);
        }
        #endregion

        #region Commands
        public ICommand LoadCommand { get; }

        public ICommand SelectDonatorCommand { get; set; }
        #endregion

        public OverviewViewModel()
        {
            // Set properties
            Donators = new ObservableCollection<Donator>();
            DonatorsPerMonth = new ObservableCollection<DonatorsPerMonthGroup>();

            // Set commands
            LoadCommand = new RelayCommand(Load);
            SelectDonatorCommand = new RelayCommand<ItemClickEventArgs>(SelectDonator);

            LoadCommand.Execute(null);
        }

        private async void Load()
        {
            IsLoading = true;

            // Clear all previous donations
            DonatorsPerMonth.Clear();

            // Load donations
            var donations = await Singleton<DonationFakeService>.Instance.GetAllDonationsAsync();

            // Get the donations created during this year
            donations = donations.Where(d => d.DonatedAt.Year == DateTime.Now.Year).ToList();

            // Group by month
            var monthlyDonations = donations.GroupBy(d => d.DonatedAt.Month, (key, list) => new DonationsPerMonthGroup(key, list));

            // Loop through each month
            foreach(var month in monthlyDonations)
            {
                var currentMonthDonators = new List<Donator>();

                // Select all donations of the current month
                var nameGroups = month.ToList().GroupBy(d => d.FullName);

                // Select donator with most donations during the current month
                var trendingDonation = nameGroups.OrderByDescending(d => d.Count()).FirstOrDefault().ToList().FirstOrDefault();

                // Loop through each donation during the current month
                foreach (var name in nameGroups)
                {
                    // Map to donators based on groups
                    var donator = new Donator
                    {
                        FullName = name.Key,
                        TotalAmount = name.Sum(x => x.Amount),
                    };

                    // Load image
                    Contact contact = await Singleton<ContactService>.Instance.GetContactByDisplayNameAsync(donator.FullName);
                    if(contact != null)
                    {
                        donator.AvatarSource = await Singleton<ContactService>.Instance.LoadContactAvatarToBitmapAsync(contact);
                    }
                    
                    // Reward for trending donator
                    donator.Rewards.IsTrending = trendingDonation.FullName == name.Key;

                    // Add to list
                    currentMonthDonators.Add(donator);
                }

                // Select donator with most valuable donations during the current month
                var mostValuableDonation = currentMonthDonators.ToList().OrderByDescending(d => d.TotalAmount).FirstOrDefault();
                mostValuableDonation.Rewards.IsMostValuable = true;

                // Sort by total amount
                currentMonthDonators = currentMonthDonators.OrderByDescending(x => x.TotalAmount).ToList();
                DonatorsPerMonth.Add(new DonatorsPerMonthGroup(month.Key, currentMonthDonators));

                IsLoading = false;
            }
        }

        public async void SelectDonator(ItemClickEventArgs args)
        {
            // Assign donator
            SelectedDonator = args.ClickedItem as Donator;
            if (SelectedDonator == null)
                return;

            // Initialize details page
            var page = new DonatorDetailPage();
            page.ViewModel.Donations = new ObservableCollection<Donation>();

            // Get all donations by person
            var donations = (await Singleton<DonationFakeService>.Instance.GetAllDonationsAsync())
                .Where(d => d.FullName == SelectedDonator.FullName);
            donations.ToList().ForEach(d => page.ViewModel.Donations.Add(d));

            page.ViewModel.Donator = new Donator
            {
                FullName = SelectedDonator.FullName,
                AvatarSource = SelectedDonator.AvatarSource,
                TotalAmount = donations.Sum(x => x.Amount)
            };


            var dialog = new ContentDialog
            {
                Content = page,

                PrimaryButtonText = "OverviewPage_SelectDonatorDialog/PrimaryButtonText".GetLocalized(),
                PrimaryButtonCommand = new RelayCommand(() => SelectedDonator = null),
                DefaultButton = ContentDialogButton.Primary
            };

            await dialog.ShowAsync();
        }
    }
}

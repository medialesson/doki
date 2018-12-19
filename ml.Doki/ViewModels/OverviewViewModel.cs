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
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace ml.Doki.ViewModels
{
    public class OverviewViewModel : Observable
    {
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


        public ICommand LoadCommand { get; }

        public ICommand SelectDonatorCommand { get; set; }


        public OverviewViewModel()
        {
            // Set properties
            Donators = new ObservableCollection<Donator>();
            DonatorsPerMonth = new ObservableCollection<DonatorsPerMonthGroup>();

            // Set commands
            LoadCommand = new RelayCommand(Load);
            SelectDonatorCommand = new RelayCommand(SelectDonator);

            LoadCommand.Execute(null);
        }

        private async void Load()
        {
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
                // Loop through each donation during the current month
                var nameGroups = month.ToList().GroupBy(d => d.FullName);
                foreach (var name in nameGroups)
                {
                    // Map to donators based on groups
                    var donator = new Donator
                    {
                        FullName = name.Key,
                        TotalAmount = name.Sum(x => x.Amount)
                    };

                    // Load image
                    Contact contact = await Singleton<ContactService>.Instance.GetContactByDisplayNameAsync(donator.FullName);
                    if(contact != null)
                    {
                        donator.AvatarSource = await Singleton<ContactService>.Instance.LoadContactAvatarToBitmapAsync(contact);
                    }

                    // Add to list
                    currentMonthDonators.Add(donator);
                }

                // Sort by total amount
                currentMonthDonators = currentMonthDonators.OrderByDescending(x => x.TotalAmount).ToList();

                DonatorsPerMonth.Add(new DonatorsPerMonthGroup(month.Key, currentMonthDonators));
            }
        }

        public async void SelectDonator()
        {
            if (SelectedDonator == null)
                return;


            var page = new DonatorDetailPage();
            page.ViewModel.Donator = SelectedDonator;

            page.ViewModel.Donations = new ObservableCollection<Donation>((
                await Singleton<DonationFakeService>.Instance.GetAllDonationsAsync())
                    .Where(d => d.FullName == SelectedDonator.FullName));

            var dialog = new ContentDialog
            {
                Content = page,

                PrimaryButtonText = "Close",
                DefaultButton = ContentDialogButton.Primary
            };

            await dialog.ShowAsync();
        }
    }
}

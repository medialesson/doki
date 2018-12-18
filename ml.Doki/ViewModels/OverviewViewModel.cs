using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ml.Doki.Helpers;
using ml.Doki.Models;
using ml.Doki.Models.Grouping;
using ml.Doki.Services;

namespace ml.Doki.ViewModels
{
    public class OverviewViewModel : Observable
    {
        private ObservableCollection<Donator> _donators;
        public ObservableCollection<Donator> Donators { get => _donators; set => Set(ref _donators, value); }


        private ObservableCollection<DonatorsPerMonthGroup> _donatorsPerMonth;
        public ObservableCollection<DonatorsPerMonthGroup> DonatorsPerMonth
        {
            get => _donatorsPerMonth;
            set => Set(ref _donatorsPerMonth, value);
        }


        public ICommand LoadCommand { get; }

        public OverviewViewModel()
        {
            // Set properties
            Donators = new ObservableCollection<Donator>();
            DonatorsPerMonth = new ObservableCollection<DonatorsPerMonthGroup>();

            // Set commands
            LoadCommand = new RelayCommand(Load);
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

                    // Add to list
                    currentMonthDonators.Add(donator);
                }

                DonatorsPerMonth.Add(new DonatorsPerMonthGroup(month.Key, currentMonthDonators));
            }
        }
    }
}

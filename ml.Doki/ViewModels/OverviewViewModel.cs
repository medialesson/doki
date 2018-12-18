using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ml.Doki.Helpers;
using ml.Doki.Models;
using ml.Doki.Services;

namespace ml.Doki.ViewModels
{
    public class OverviewViewModel : Observable
    {
        private ObservableCollection<Donator> _donators;

        public ObservableCollection<Donator> Donators { get => _donators; set => Set(ref _donators, value); }


        public ICommand LoadCommand { get; }

        public OverviewViewModel()
        {
            // Set properties
            Donators = new ObservableCollection<Donator>();

            // Set commands
            LoadCommand = new RelayCommand(Load);
            LoadCommand.Execute(null);
        }

        private async void Load()
        {
            // Clear all previous donations
            Donators.Clear();

            // Load donations
            var donations = await Singleton<DonationFakeService>.Instance.GetAllDonationsAsync();

            // Group by name
            var nameGroups = donations.GroupBy(d => d.FullName);
            foreach(var name in nameGroups)
            {
                // Map to donators based on groups
                var donator = new Donator
                {
                    FullName = name.Key,
                    TotalAmount = name.Sum(x => x.Amount)
                };

                // Load image

                // Add to list
                Donators.Add(donator);
            }
        }
    }
}

using System;
using System.Collections.ObjectModel;
using ml.Doki.Helpers;
using ml.Doki.Models;

namespace ml.Doki.ViewModels
{
    public class DonatorDetailViewModel : Observable
    {
        #region Properties

        private Donator _donator;

        public Donator Donator { get => _donator; set => Set(ref _donator, value); }


        private ObservableCollection<Donation> _donations;

        public ObservableCollection<Donation> Donations { get => _donations; set => Set(ref _donations, value); }

        #endregion

        public DonatorDetailViewModel()
        {
            _donations = new ObservableCollection<Donation>();
        }
    }
}

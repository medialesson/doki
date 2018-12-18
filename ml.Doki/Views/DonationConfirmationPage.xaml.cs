using System;

using ml.Doki.ViewModels;

using Windows.UI.Xaml.Controls;

namespace ml.Doki.Views
{
    public sealed partial class DonationConfirmationPage : Page
    {
        public DonationConfirmationViewModel ViewModel { get; } = new DonationConfirmationViewModel();

        public DonationConfirmationPage()
        {
            InitializeComponent();
        }
    }
}

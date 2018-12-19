using System;

using ml.Doki.ViewModels;

using Windows.UI.Xaml.Controls;

namespace ml.Doki.Views
{
    public sealed partial class DonatorDetailPage : Page
    {
        public DonatorDetailViewModel ViewModel { get; } = new DonatorDetailViewModel();

        public DonatorDetailPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}

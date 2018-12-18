using System;

using ml.Doki.ViewModels;

using Windows.UI.Xaml.Controls;

namespace ml.Doki.Views
{
    public sealed partial class OverviewPage : Page
    {
        public OverviewViewModel ViewModel { get; } = new OverviewViewModel();

        public OverviewPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}

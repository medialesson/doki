using System;
using ml.Doki.Helpers;
using ml.Doki.ViewModels;

using Windows.UI.Xaml.Controls;

namespace ml.Doki.Views
{
    public sealed partial class OverviewPage : Page
    {
        public OverviewViewModel ViewModel { get; } = Singleton<OverviewViewModel>.Instance;

        public OverviewPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}

using System;
using ml.Doki.Helpers;
using ml.Doki.ViewModels;

using Windows.UI.Xaml.Controls;

namespace ml.Doki.Views
{
    public sealed partial class DonatePage : Page
    {
        public DonateViewModel ViewModel { get; } = Singleton<DonateViewModel>.Instance;

        public DonatePage()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}

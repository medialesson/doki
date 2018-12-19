using System;
using ml.Doki.Helpers;
using ml.Doki.ViewModels;

using Windows.UI.Xaml.Controls;

namespace ml.Doki.Views
{
    public sealed partial class AboutPage : Page
    {
        public AboutViewModel ViewModel { get; } = Singleton<AboutViewModel>.Instance;

        public AboutPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}

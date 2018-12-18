using System;

using ml.Doki.ViewModels;

using Windows.UI.Xaml.Controls;

namespace ml.Doki.Views
{
    public sealed partial class ConfigurationPage : Page
    {
        public ConfigurationViewModel ViewModel { get; } = new ConfigurationViewModel();

        public ConfigurationPage()
        {
            InitializeComponent();
        }
    }
}

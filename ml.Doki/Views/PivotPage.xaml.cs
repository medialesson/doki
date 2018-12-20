using System;
using System.Threading.Tasks;
using ml.Doki.Helpers;
using ml.Doki.ViewModels;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ml.Doki.Views
{
    public sealed partial class PivotPage : Page
    {
        public PivotViewModel ViewModel { get; } = Singleton<PivotViewModel>.Instance;

        public PivotPage()
        {
            // We use NavigationCacheMode.Required to keep track the selected item on navigation. For further information see the following links.
            // https://msdn.microsoft.com/en-us/library/windows/apps/xaml/windows.ui.xaml.controls.page.navigationcachemode.aspx
            // https://msdn.microsoft.com/en-us/library/windows/apps/xaml/Hh771188.aspx

            NavigationCacheMode = NavigationCacheMode.Required;
            DataContext = ViewModel;
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await Task.CompletedTask;
        }
    }
}

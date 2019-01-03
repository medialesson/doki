using System;

using ml.Doki.ViewModels;

using Windows.UI.Xaml.Controls;

namespace ml.Doki.Views
{
    public sealed partial class ExpensesPage : Page
    {
        public ExpensesViewModel ViewModel { get; } = new ExpensesViewModel();

        public ExpensesPage()
        {
            InitializeComponent();
        }
    }
}

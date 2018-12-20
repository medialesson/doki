using System;

using ml.Doki.Helpers;

namespace ml.Doki.ViewModels
{
    public class PivotViewModel : Observable
    {
        public const int OverviewPagePivotIndex = 1;

        #region Properties
        private int _selectedPivotIndex;

        public int SelectedPivotIndex { get => _selectedPivotIndex; set => Set(ref _selectedPivotIndex, value); }
        #endregion

        public PivotViewModel()
        {
        }

        public void SelectOverviewPivot()
        {
            SelectedPivotIndex = OverviewPagePivotIndex;
        }
    }
}

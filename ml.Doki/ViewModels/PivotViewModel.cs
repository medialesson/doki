using System;

using ml.Doki.Helpers;

namespace ml.Doki.ViewModels
{
    public class PivotViewModel : Observable
    {
        #region Properties
        private int _selectedPivotIndex;

        public int SelectedPivotIndex { get => _selectedPivotIndex; set => Set(ref _selectedPivotIndex, value); }
        #endregion

        public PivotViewModel()
        {
        }
    }
}

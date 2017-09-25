using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dcx.Plus.Localization;
using Dcx.Plus.Repository.Modules.$Product$.Contracts.DataItems;
using Dcx.Plus.UI.WPF.FW.Shell.Infrastructure;
using Dcx.Plus.UI.WPF.FW.Shell.Interfaces;

namespace Dcx.Plus.UI.WPF.Modules.$Product$.Windows.$Dialog$.Regions.Detail
{
    public class DetailViewModel : RegionViewModel<$Product$$Item$DataItem> 
    {
		#region Members

		private bool _isInReadOnlyMode = false;

		#endregion Members
		
		#region Construction
		
	    public DetailViewModel(IViewModelBaseServices baseServices): base(baseServices)
        {
            DisplayName = GlobalLocalizer.Singleton.Global_strDetails.Translation;
        }
		
		#endregion Construction

		#region Navigation
		
		public void OnNavigatedTo($Product$$Item$DataItem dataItem)
        {
            DataItem = dataItem;
			IsReadOnly = _isInReadOnlyMode;

			OnPropertyChanged(() => IsNewItem);
			DataItemObserver.RegisterHandler(x => x.HasChanges, $item$ => OnPropertyChanged(() => IsNewItem));
		}

		#endregion Navigation
    }
}

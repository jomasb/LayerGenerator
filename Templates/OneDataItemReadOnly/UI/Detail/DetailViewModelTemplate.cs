using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dcx.Plus.Repository.Modules.$Product$.Contracts.DataItems;
using Dcx.Plus.UI.WPF.FW.Shell.Infrastructure;
using Dcx.Plus.UI.WPF.FW.Shell.Interfaces;

namespace Dcx.Plus.UI.WPF.Modules.$Product$.Windows.$Dialog$.Regions.Detail
{
    public class DetailViewModel : RegionViewModel<$Product$$Item$DataItem> : base(baseServices)
    {
	    #region Construction
		
	    public DetailViewModel(IViewModelBaseServices baseServices)
        {
            DisplayName = GlobalLocalizer.Singleton.Global_strDetails.Translation;
        }
		
		#endregion Construction

		#region Navigation
		
		public void OnNavigatedTo($Product$$Item$DataItem dataItem)
        {
            DataItem = dataItem;
        }
		
		#endregion Navigation
    }
}

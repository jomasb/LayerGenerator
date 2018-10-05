using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dcx.Plus.Localization;
using Dcx.Plus.Repository.FW.Contracts;
using Dcx.Plus.Repository.Modules.$Product$.Contracts;
using Dcx.Plus.Repository.Modules.$Product$.DataItems;
using Dcx.Plus.UI.WPF.FW.Shell.Infrastructure.ViewModels;
using Dcx.Plus.UI.WPF.FW.Shell.Interfaces;

namespace Dcx.Plus.UI.WPF.Modules.$Product$.Windows.$Dialog$.Regions.Detail
{
	/// <summary>
	/// Definition of the view model for the $item$ detail view.
	/// </summary>
	/// <seealso cref="Dcx.Plus.UI.WPF.FW.Shell.Infrastructure.RegionViewModel{$Product$$Item$DataItem}" />
    public class $Item$DetailViewModel : RegionViewModel<$Product$$Item$DataItem>, ILazyLoadingHandler
    {
		#region Members

		private readonly I$Product$$Dialog$Repository _repository;
		private bool _isInReadOnlyMode = false;
		$specialContent1$

		#endregion Members
		
		#region Construction
		
		/// <summary>
		/// Initializes a new instance of the <see cref="$Item$DetailViewModel"/> class.
		/// </summary>
		/// <param name="baseServices">The base services.</param>
	    public $Item$DetailViewModel(IViewModelBaseServices baseServices, I$Product$$Dialog$Repository repository): base(baseServices)
        {
			_repository = repository;
            DisplayName = GlobalLocalizer.Singleton.Global_strDetails.Translation;
			$specialContent2$
        }
		
		#endregion Construction
		
		#region Properties
		
		$specialContent3$
		
		#endregion Properties

		#region Navigation
		
		/// <summary>
		/// Called when [navigated to].
		/// </summary>
		/// <param name="dataItem">The data item.</param>
		public void OnNavigatedTo($Product$$Item$DataItem dataItem)
        {
            DataItem = dataItem;
			IsReadOnly = _isInReadOnlyMode;

			$specialContent4$
			
			OnPropertyChanged(() => IsNewItem);
			DataItemObserver.RegisterHandler(x => x.HasChanges, $item$ => OnPropertyChanged(() => IsNewItem));
		}

		#endregion Navigation
		
		public void OnRegisteredLazyCollectionLoaded(object sender)
		{
			$specialContent5$
		}

		public void OnRegisteredLazyCollectionException(object sender, Exception exception)
		{
			
		}

		$specialContent6$
    }	
}

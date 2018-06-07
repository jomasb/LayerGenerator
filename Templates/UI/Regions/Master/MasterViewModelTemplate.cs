using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using Dcx.Plus.Infrastructure.Contracts.Application;
using Dcx.Plus.Localization;
using Dcx.Plus.Repository.FW;
using Dcx.Plus.Repository.FW.Collections;
using Dcx.Plus.Repository.FW.Contracts;
using Dcx.Plus.Repository.FW.DataItems;
using Dcx.Plus.Repository.Modules.$Product$.Contracts;
using Dcx.Plus.Repository.Modules.$Product$.DataItems;
using Dcx.Plus.UI.WPF.FW.Shell.Infrastructure;
using Dcx.Plus.UI.WPF.FW.Shell.Infrastructure.Filter;
using Dcx.Plus.UI.WPF.FW.Shell.Infrastructure.ViewModels;
using Dcx.Plus.UI.WPF.FW.Shell.Interfaces;
using Dcx.Plus.UI.WPF.Modules.$Product$.Windows.$Dialog$.Infrastructure;
using Dcx.Plus.Localization.Modules.$Product$;
using Dcx.Plus.UI.WPF.FW.Shell.Commands;
using Dcx.Plus.UI.WPF.FW.Shell.Extensions;
using Dcx.Plus.UI.WPF.FW.Shell.Infrastructure.Selection;

namespace Dcx.Plus.UI.WPF.Modules.$Product$.Windows.$Dialog$.Regions.Master
{
	/// <summary>
	/// Definition of the view model for the $Dialog$ master view.
	/// </summary>
	/// <seealso cref="MasterViewModelBase"/>
	/// <author></author>
	/// <company>abat+ GmbH</company>
	/// <date></date>
	public class $Dialog$MasterViewModel : MasterViewModelBase, ILazyLoadingHandler$specialContent5$
	{
		#region Members
		
		$specialContent1$
		
		#endregion Members
		
		#region Construction
		
		/// <summary>
		/// Initializes a new instance of the <see cref="$Dialog$MasterViewModel"/> class.
		/// </summary>
		/// <param name="baseServices">The base services.</param>
		public $Dialog$MasterViewModel(
			IViewModelBaseServices baseServices,
			I$Product$$Dialog$Repository $product$$Dialog$Repository)
			: base(baseServices)
		{
			_$product$$Dialog$Repository = $product$$Dialog$Repository;
			Initialize();
		}

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		/// <exception cref="System.NotImplementedException"></exception>
		private void Initialize()
		{
			DisplayName = $Product$Localizer.Singleton.$Product$$Dialog$_lbl$Item$s.Translation;
			$specialContent2$
		}
		
		#endregion Construction
		
		#region Properties
		
		$specialContent3$
		
		#endregion Properties
		
		#region Methods

		/// <summary>
		/// Updates the display name.
		/// </summary>
		private void UpdateDisplayName()
		{
			DisplayName = $Product$Localizer.Singleton.$Product$$Dialog$_lbl$Item$s.Translation + " (" + FilterCollection.Count + ")";
		}
		
		$specialContent4$
		
		#endregion Methods

		#region Command Handler
		
		$specialContent6$
		
		/// <summary>
		/// Refreshes the command executed.
		/// </summary>
		private async void RefreshCommandExecuted()
		{
			await Refresh();
		}
		
		/// <summary>
		/// Refreshes this instance.
		/// </summary>
		/// <returns></returns>
		private async Task Refresh()
		{
			IsGlobalBusy = true;
			IsInitialLoadingCompleted = false;
			RaiseCanExecuteChanged();
			$Product$$Item$DataItemsList.Reload();
			await $Product$$Item$DataItemsList.CurrentLoadingTask;
		}
		
		public bool HasAnyErrors
	    {
	        get
	        {
	            var activeItem = Selected$Item$DataItem as PlusNotifyDataErrorInfoBase;
	            return activeItem != null && activeItem.HasErrors || BaseServices.ValidationService.HasAnyErrors;
	        }
	    }
		
		/// <summary>
        /// Selectings the active item command execute.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private async void SelectingActiveItemCommandExecuted(CancelableSelectionArgs<object> args)
	    {
            // If the user tries to select another item while there are any changes, we either cancel
            // the selection or allow selection and eigher save or discard the changes
            if (IsDirty && (($Product$$Item$DataItem)args.Value).State != DataItemState.New)
            {
                args.Cancel = true;

                var result = await BaseServices.PopupDialogService.ShowSaveQuestionAsync();

                if (result == NlsMessageResult.YES)
                    if (HasAnyErrors)
                    {
                        await BaseServices.PopupDialogService.ShowCheckYourInputMessageAsync();
                    }
                    else
                    {
                        var saveResponse = await SaveCommandExecuted();
                        if (saveResponse)
                            Selected$Item$DataItem = ($Product$$Item$DataItem)args.Value;
                    }

                if (result == NlsMessageResult.NO)
                {
                    args.Cancel = false;
                    await CancelCommandExecuted();
                    Selected$Item$DataItem = ($Product$$Item$DataItem)args.Value;
                }

                if (result == NlsMessageResult.CANCEL) args.Cancel = true;
            }
        }
		
		#endregion Command Handler
		
		#region Navigation
		
		/// <summary>
		/// Resets the navigation.
		/// </summary>
		private void ResetNavigation()
		{
			$specialContent9$
			NavigationService.Unload(RegionNames.DetailRegion);
		}

		$specialContent7$
		
		#endregion Navigation
		
		#region FilterSourceProvider Interface
		
		$specialContent8$

		#endregion
	}
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Data;
using Dcx.Plus.Infrastructure.Contracts.Application;
using Dcx.Plus.Localization;
using Dcx.Plus.Repository.FW.Collections;
using Dcx.Plus.Repository.FW.Contracts;
using Dcx.Plus.Repository.Modules.$Product$.Contracts;
using Dcx.Plus.Repository.Modules.$Product$.DataItems;
using Dcx.Plus.UI.WPF.FW.Shell.Infrastructure;
using Dcx.Plus.UI.WPF.FW.Shell.Infrastructure.Filter;
using Dcx.Plus.UI.WPF.FW.Shell.Infrastructure.ViewModels;
using Dcx.Plus.UI.WPF.FW.Shell.Interfaces;
using Dcx.Plus.UI.WPF.Modules.$Product$.Windows.$Dialog$.Infrastructure;
using Dcx.Plus.Localization.Modules.$Product$;
using Dcx.Plus.Repository.FW.DataItems;
using Dcx.Plus.UI.WPF.FW.Shell.Commands;
using Dcx.Plus.UI.WPF.FW.Shell.Extensions;
using Dcx.Plus.UI.WPF.FW.Shell.Infrastructure.Selection;

namespace Dcx.Plus.UI.WPF.Modules.$Product$.Windows.$Dialog$.Regions.Master
{
	/// <summary>
	/// Definition of the view model for the master view.
	/// </summary>
	/// <seealso cref="MasterViewModelMultiEditBase"/>
	/// <author></author>
	/// <company>abat+ GmbH</company>
	/// <date></date>
	public class $Dialog$MasterViewModel : MasterViewModelBase, ILazyLoadingHandler, IFilterSourceProvider<$Product$$MasterDataItem$DataItem>
	{
		#region Members
		
		private readonly I$Product$$Dialog$Repository _$product$$Dialog$Repository;
		private PlusLazyLoadingAsyncObservableCollection<$Product$$MasterDataItem$DataItem> _$product$$MasterDataItem$DataItemsList;
		private PlusStateDataItem _activeItem;
		private PropertyObserver<PlusLazyLoadingAsyncObservableCollection<$Product$$MasterDataItem$DataItem>> _isDirtyObserver;
		
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
			DisplayName = $Product$Localizer.Singleton.$Product$$Dialog$_lbl$MasterDataItem$s.Translation;
			CommandService.SubscribeAsyncCommand(GlobalCommandNames.SaveCommand, SaveCommandExecuted, SaveCommandCanExecute);
			CommandService.SubscribeAsyncCommand(GlobalCommandNames.CancelCommand, CancelCommandExecuted, CancelCommandCanExecute);
			CommandService.SubscribeAsyncCommand(GlobalCommandNames.CopyCommand, CopyCommandExecuted, CopyCommandCanExecute);
			CommandService.SubscribeAsyncCommand(GlobalCommandNames.DeleteCommand, DeleteCommandExecuted, DeleteCommandCanExecute);
			CommandService.SubscribeCommand(GlobalCommandNames.RefreshCommand, RefreshCommandExecuted, RefreshCommandCanExecute);
			CommandService.SubscribeAsyncCommand(GlobalCommandNames.AddCommand, AddCommandExecuted, AddCommandCanExecute);
			CommandService.SubscribeCommand(GlobalCommandNames.OpenSettingsDialogCommand, OpenSettingsDialogCommandExecuted);
			
			SelectingActiveItemCommand = new PlusCommand<CancelableSelectionArgs<object>>(SelectingActiveItemCommandExecuted);
			
			$Product$$MasterDataItem$DataItemsList = _$product$$Dialog$Repository.Get$MasterDataItem$s(CreateNewCallContext());
			$Product$$MasterDataItem$DataItemsList.RegisterLoadingHandler(this);
			$Product$$MasterDataItem$DataItemsList.Load();

			FilterCollection = (CollectionView)CollectionViewSource.GetDefaultView($Product$$MasterDataItem$DataItemsList.LoadedItems);
			(FilterCollection as ICollectionView).CollectionChanged += (sender, args) =>
			{
				UpdateDisplayName();
			};
		}
		
		#endregion Construction
		
		#region Properties
		
		/// <summary>
		/// Gets or sets the data items list.
		/// </summary>
		/// <value>
		/// Data items list.
		/// </value>
		public PlusLazyLoadingAsyncObservableCollection<$Product$$MasterDataItem$DataItem> $Product$$MasterDataItem$DataItemsList
		{
			get
			{
				return _$product$$MasterDataItem$DataItemsList;
			}
			set
			{
				Set(ref _$product$$MasterDataItem$DataItemsList, value);
			}
		}
		
		/// <summary>
		/// Gets or sets the selected data item.
		/// </summary>
		/// <value>
		/// The selected data item.
		/// </value>
		public PlusStateDataItem ActiveItem
		{
			get
			{
				return _activeItem;
			}
			set
			{
				if (Set(ref _activeItem, value))
				{
					if (_activeItem != null)
					{
						if (_activeItem is $Product$$MasterDataItem$DataItem)
						{
							NavigateTo$MasterDataItem$Detail(($Product$$MasterDataItem$DataItem)_activeItem);
						}
						if (_activeItem is $Product$$ChildDataItem$DataItem)
						{
							NavigateToVersionDetail(($Product$$ChildDataItem$DataItem)_activeItem);
						}
					}
					else
					{
						ResetNavigation();
					}
				}

				RaiseCanExecuteChanged();
			}
		}
		
		#endregion Properties
		
		#region Methods

		/// <summary>
		/// Updates the display name.
		/// </summary>
		private void UpdateDisplayName()
		{
			DisplayName = $Product$Localizer.Singleton.$Product$$Dialog$_lbl$MasterDataItem$s.Translation + " (" + FilterCollection.Count + ")";
		}
		
		/// <summary>
		/// Called when [loaded].
		/// </summary>
		/// <param name="sender">The sender.</param>
		public void OnRegisteredLazyCollectionLoaded(object sender)
		{
			if (sender is ILazyLoadingObservableCollection<$Product$$MasterDataItem$DataItem>)
			{
				IsInitialLoadingCompleted = true;
				On$Product$$MasterDataItem$DataItemListLoaded();
			}

			IsBusy = false;
			RaiseCanExecuteChanged();
		}
		
		/// <summary>
		/// Called when [$Product$$MasterDataItem$DataItemList loaded].
		/// </summary>
		private void On$Product$$MasterDataItem$DataItemListLoaded()
		{
			_isDirtyObserver = new PropertyObserver<PlusLazyLoadingAsyncObservableCollection<$Product$$MasterDataItem$DataItem>>($Product$$MasterDataItem$DataItemsList);
			_isDirtyObserver.RegisterHandler(x => x.HasAnyChanges, OnCollectionHasAnyChanges);

			RaiseSourceCollectionChanged($Product$$MasterDataItem$DataItemsList);

			ResetNavigation();

			if ($Product$$MasterDataItem$DataItemsList.Count > 0)
			{
				ActiveItem = null;
				ActiveItem = FilterCollection.GetItemAt(0) as $Product$$MasterDataItem$DataItem;
			}
		}
		
		/// <summary>
		/// Called when [has changed changed].
		/// </summary>
		private void OnCollectionHasAnyChanges(PlusLazyLoadingAsyncObservableCollection<$Product$$MasterDataItem$DataItem> collection)
		{
			IsDirty = collection.HasAnyChanges;
			RaiseCanExecuteChanged();
		}

		/// <summary>
		/// Called when [loading exception].
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="exception">The exception.</param>
		public void OnRegisteredLazyCollectionException(object sender, Exception exception)
		{
			IsBusy = false;
			IsInitialLoadingCompleted = false;
			RaiseCanExecuteChanged();
		}
		
		#endregion Methods

		#region Command Handler
		
		private async Task AddCommandExecuted()
		{
			IsBusy = true;
			CallResponse<$Product$$MasterDataItem$DataItem> response = await _$product$$Dialog$Repository.AddNew$MasterDataItem$Async(CreateNewCallContext(true), $Product$$MasterDataItem$DataItemsList);
			if (response.IsSuccess)
			{
				$Product$$MasterDataItem$DataItem new$Product$$MasterDataItem$DataItem = response.Result;
				ActiveItem = new$Product$$MasterDataItem$DataItem;
				new$Product$$MasterDataItem$DataItem.HasAnyChanges = true;
				new$Product$$MasterDataItem$DataItem.PropertyChanged += New$Product$$MasterDataItem$DataItem_PropertyChanged;
				RaiseCanExecuteChanged();
			}
			IsBusy = false;
		}
				
		private async Task CopyCommandExecuted()
		{
			IsBusy = true;
			if (ActiveItem != null)
			{
				CallResponse<$Product$$MasterDataItem$DataItem> response = await _$product$$Dialog$Repository.Clone$MasterDataItem$Async(CreateNewCallContext(true), ($Product$$MasterDataItem$DataItem)ActiveItem, $Product$$MasterDataItem$DataItemsList);
				if (response.IsSuccess)
				{
					$Product$$MasterDataItem$DataItem new$Product$$MasterDataItem$DataItem = response.Result;
					ActiveItem = new$Product$$MasterDataItem$DataItem;
					new$Product$$MasterDataItem$DataItem.HasAnyChanges = true;
					new$Product$$MasterDataItem$DataItem.PropertyChanged += New$Product$$MasterDataItem$DataItem_PropertyChanged;
					RaiseCanExecuteChanged();
				}
			}
			IsBusy = false;
		}
		
		private async Task CancelCommandExecuted()
		{
			await $Product$$MasterDataItem$DataItemsList.Rollback();
			$Product$$MasterDataItem$DataItemsList.HasAnyChanges = false;
			RaiseCanExecuteChanged();
			BaseServices.EventAggregator.Publish(GlobalEventNames.CancelExecuted);
		}
		
		private async Task DeleteCommandExecuted()
		{
			if (ShowDeletingMessage() == NlsMessageResult.YES)
			{
				IsBusy = true;
				if (ActiveItem is $Product$$MasterDataItem$DataItem)
				{
					CallResponse<bool> response = await _$product$$Dialog$Repository.Delete$MasterDataItem$Async(CreateNewCallContext(true), ($Product$$MasterDataItem$DataItem)ActiveItem, $Product$$MasterDataItem$DataItemsList);
					if (response.IsSuccess)
					{
						if ($Product$$MasterDataItem$DataItemsList.Count > 0)
						{
							ActiveItem = null;
							ActiveItem = FilterCollection.GetItemAt(0) as $Product$$MasterDataItem$DataItem;
						}
					}
					else
					{
						ResetNavigation();
					}
				}
				if (ActiveItem is $Product$$ChildDataItem$DataItem)
				{
					//await _$product$$Dialog$Repository.DeleteVersionAsync(CreateNewCallContext(true), ($Product$$ChildDataItem$DataItem)ActiveItem, parent);
				}

				RaiseCanExecuteChanged();
				IsBusy = false;
			}
		}
		
		public override bool SaveCommandCanExecute()
	    {
	        return !IsInReadOnlyMode && IsDirty && !IsBusy;
        }
		
		private async Task<bool> SaveCommandExecuted()
		{
			IsBusy = true;
			RaiseCanExecuteChanged();
			bool result = false;

			if (ActiveItem is $Product$$MasterDataItem$DataItem)
			{
				var response = await _$product$$Dialog$Repository.Save$MasterDataItem$Async(CreateNewCallContext(true), ($Product$$MasterDataItem$DataItem)ActiveItem);
				if (response.IsSuccess)
				{
					$Product$$MasterDataItem$DataItemsList.Accept();
				}

				result = response.IsSuccess;
			}
			if (ActiveItem is $Product$$ChildDataItem$DataItem)
			{
				var response = await _$product$$Dialog$Repository.Save$ChildDataItem$Async(CreateNewCallContext(true), ($Product$$ChildDataItem$DataItem)ActiveItem);
				if (response.IsSuccess)
				{
					(($Product$$MasterDataItem$DataItem)ActiveItem.Parent.Parent).Accept();
				}

				result = response.IsSuccess;
			}

			IsBusy = false;
			RaiseCanExecuteChanged();
			
			return result;
		}

		private void New$Product$$MasterDataItem$DataItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			RaiseCanExecuteChanged();
		}

		/// <summary>
		/// Refreshes this instance.
		/// </summary>
		/// <returns></returns>
		private async Task Refresh()
		{
			IsBusy = true;
			IsInitialLoadingCompleted = false;
			RaiseCanExecuteChanged();
			$Product$$MasterDataItem$DataItemsList.Reload();
			await $Product$$MasterDataItem$DataItemsList.CurrentLoadingTask;
		}

		/// <summary>
		/// Resets the navigation.
		/// </summary>
		private void ResetNavigation()
		{
			BaseServices.NavigationService.Unload(RegionNames.DetailRegion);
			
			if (ActiveItem != null)
			{
				ActiveItem.PropertyChanged -= New$Product$$MasterDataItem$DataItem_PropertyChanged;
			}
		}

		/// <summary>
		/// Refreshes the command executed.
		/// </summary>
		private async void RefreshCommandExecuted()
		{
			await Refresh();
		}
		
		/// <summary>
	    /// Selectings the active item command execute.
	    /// </summary>
	    /// <param name="args">The arguments.</param>
	    private async void SelectingActiveItemCommandExecuted(CancelableSelectionArgs<object> args)
	    {
		    // If the user tries to select another item while there are any changes, we either cancel
		    // the selection or allow selection and eigher save or discard the changes
		    if (IsDirty && ActiveItem != args.Value)
		    {
			    args.Cancel = true;

			    var result = await BaseServices.PopupDialogService.ShowSaveQuestionAsync();

			    if (result == NlsMessageResult.YES)
			    {
					//if (HasAnyErrors)
					//{
					//	await BaseServices.PopupDialogService.ShowCheckYourInputMessageAsync();
					//}
					//else
					//{
					    var saveResponse = await SaveCommandExecuted();
					    if (saveResponse)
						    ActiveItem = (PlusStateDataItem)args.Value;
				    //}
			    }
			    if (result == NlsMessageResult.NO)
			    {
				    args.Cancel = false;
				    await CancelCommandExecuted();
				    ActiveItem = ($Product$DataItem)args.Value;
			    }
			    if (result == NlsMessageResult.CANCEL)
			    {
				    args.Cancel = true;
			    }
		    }
	    }

		#endregion Command Handler
		
		#region Navigation

		/// <summary>
		/// Navigates to .
		/// </summary>
		/// <param name="activeItem">The active item.</param>
		private void NavigateTo$MasterDataItem$Detail($Product$$MasterDataItem$DataItem activeItem)
		{
			BaseServices.NavigationService.Navigate(RegionNames.DetailRegion, ViewNames.$MasterDataItem$DetailView,
				ParameterNames.SelectedDataItem, activeItem);
		}

		/// <summary>
		/// Navigates to .
		/// </summary>
		/// <param name="activeItem">The active item.</param>
		private void NavigateToVersionDetail($Product$$ChildDataItem$DataItem activeItem)
		{
			BaseServices.NavigationService.Navigate(RegionNames.DetailRegion, ViewNames.VersionDetailView,
				ParameterNames.SelectedDataItem, activeItem);
		}

		#endregion Navigation
		
		#region FilterSourceProvider Interface

		/// <summary>
		/// Raises the source collection changed.
		/// </summary>
		/// <param name="sourceCollection">The source collection.</param>
		private void RaiseSourceCollectionChanged(IList<$Product$$MasterDataItem$DataItem> sourceCollection)
		{
			if (SourceCollectionChanged != null)
			{
				SourceCollectionChanged.Invoke(this, new SourceCollectionChangedEventArgs<$Product$$MasterDataItem$DataItem>()
				{
					SourceCollection = sourceCollection
				});
			}
		}

		/// <summary>
		/// Occurs when [source collection changed].
		/// </summary>
		public event EventHandler<SourceCollectionChangedEventArgs<$Product$$MasterDataItem$DataItem>> SourceCollectionChanged;

		#endregion
	}
}
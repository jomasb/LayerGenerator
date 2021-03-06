﻿using System;
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
	public class $Dialog$MasterViewModel : MasterViewModelBase, ILazyLoadingHandler, IFilterSourceProvider<$Product$$Item$DataItem>
	{
		#region Members
		
		private readonly I$Product$$Dialog$Repository _$product$$Dialog$Repository;
		private PlusLazyLoadingAsyncObservableCollection<$Product$$Item$DataItem> _$product$$Item$DataItemsList;
		private $Product$$Item$DataItem _selected$Item$DataItem;
		private PropertyObserver<PlusLazyLoadingAsyncObservableCollection<$Product$$Item$DataItem>> _isDirtyObserver;
		
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
			CommandService.SubscribeAsyncCommand(GlobalCommandNames.SaveCommand, SaveCommandExecuted, SaveCommandCanExecute);
			CommandService.SubscribeAsyncCommand(GlobalCommandNames.CancelCommand, CancelCommandExecuted, CancelCommandCanExecute);
			CommandService.SubscribeAsyncCommand(GlobalCommandNames.CopyCommand, CopyCommandExecuted, CopyCommandCanExecute);
			CommandService.SubscribeAsyncCommand(GlobalCommandNames.DeleteCommand, DeleteCommandExecuted, DeleteCommandCanExecute);
			CommandService.SubscribeCommand(GlobalCommandNames.RefreshCommand, RefreshCommandExecuted, RefreshCommandCanExecute);
			CommandService.SubscribeAsyncCommand(GlobalCommandNames.AddCommand, AddCommandExecuted, AddCommandCanExecute);
			CommandService.SubscribeCommand(GlobalCommandNames.OpenSettingsDialogCommand, OpenSettingsDialogCommandExecuted);
			
			SelectingActiveItemCommand = new PlusCommand<CancelableSelectionArgs<object>>(SelectingActiveItemCommandExecuted);
			
			$Product$$Item$DataItemsList = _$product$$Dialog$Repository.Get$Item$s(CreateNewCallContext());
			$Product$$Item$DataItemsList.RegisterLoadingHandler(this);
			$Product$$Item$DataItemsList.Load();

			FilterCollection = (CollectionView)CollectionViewSource.GetDefaultView($Product$$Item$DataItemsList.LoadedItems);
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
		public PlusLazyLoadingAsyncObservableCollection<$Product$$Item$DataItem> $Product$$Item$DataItemsList
		{
			get
			{
				return _$product$$Item$DataItemsList;
			}
			set
			{
				Set(ref _$product$$Item$DataItemsList, value);
			}
		}
		
		/// <summary>
		/// Gets or sets the selected data item.
		/// </summary>
		/// <value>
		/// The selected data item.
		/// </value>
		public $Product$$Item$DataItem Selected$Item$DataItem
		{
			get
			{
				return _selected$Item$DataItem;
			}
			set
			{
				if (Set(ref _selected$Item$DataItem, value))
				{
					if (_selected$Item$DataItem != null)
					{
						NavigateToDetail(_selected$Item$DataItem);
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
			DisplayName = $Product$Localizer.Singleton.$Product$$Dialog$_lbl$Item$s.Translation + " (" + FilterCollection.Count + ")";
		}
		
		/// <summary>
		/// Called when [loaded].
		/// </summary>
		/// <param name="sender">The sender.</param>
		public void OnRegisteredLazyCollectionLoaded(object sender)
		{
			if (sender is ILazyLoadingObservableCollection<$Product$$Item$DataItem>)
			{
				IsInitialLoadingCompleted = true;
				On$Product$$Item$DataItemListLoaded();
			}

			IsBusy = false;
			RaiseCanExecuteChanged();
		}
		
		/// <summary>
		/// Called when [$Product$$Item$DataItemList loaded].
		/// </summary>
		private void On$Product$$Item$DataItemListLoaded()
		{
			_isDirtyObserver = new PropertyObserver<PlusLazyLoadingAsyncObservableCollection<$Product$$Item$DataItem>>($Product$$Item$DataItemsList);
			_isDirtyObserver.RegisterHandler(x => x.HasAnyChanges, OnCollectionHasAnyChanges);

			RaiseSourceCollectionChanged($Product$$Item$DataItemsList);

			ResetNavigation();

			if ($Product$$Item$DataItemsList.Count > 0)
			{
				Selected$Item$DataItem = null;
				Selected$Item$DataItem = FilterCollection.GetItemAt(0) as $Product$$Item$DataItem;
			}
		}
		
		/// <summary>
		/// Called when [has changed changed].
		/// </summary>
		private void OnCollectionHasAnyChanges(PlusLazyLoadingAsyncObservableCollection<$Product$$Item$DataItem> collection)
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
			CallResponse<$Product$$Item$DataItem> response = await _$product$$Dialog$Repository.AddNew$Item$Async(CreateNewCallContext(true), $Product$$Item$DataItemsList);
			if (response.IsSuccess)
			{
				$Product$$Item$DataItem new$Product$$Item$DataItem = response.Result;
				Selected$Item$DataItem = new$Product$$Item$DataItem;
				new$Product$$Item$DataItem.HasAnyChanges = true;
				new$Product$$Item$DataItem.PropertyChanged += New$Product$$Item$DataItem_PropertyChanged;
				RaiseCanExecuteChanged();
			}
			IsBusy = false;
		}
				
		private async Task CopyCommandExecuted()
		{
			IsBusy = true;
			if (Selected$Item$DataItem != null)
			{
				CallResponse<$Product$$Item$DataItem> response = await _$product$$Dialog$Repository.Clone$Item$Async(CreateNewCallContext(true), Selected$Item$DataItem, $Product$$Item$DataItemsList);
				if (response.IsSuccess)
				{
					$Product$$Item$DataItem new$Product$$Item$DataItem = response.Result;
					Selected$Item$DataItem = new$Product$$Item$DataItem;
					new$Product$$Item$DataItem.HasAnyChanges = true;
					new$Product$$Item$DataItem.PropertyChanged += New$Product$$Item$DataItem_PropertyChanged;
					RaiseCanExecuteChanged();
				}
			}
			IsBusy = false;
		}
		
		private async Task CancelCommandExecuted()
		{
			await $Product$$Item$DataItemsList.Rollback();
			$Product$$Item$DataItemsList.HasAnyChanges = false;
			RaiseCanExecuteChanged();
			BaseServices.EventAggregator.Publish(GlobalEventNames.CancelExecuted);
		}
		
		private async Task DeleteCommandExecuted()
		{
			if (ShowDeletingMessage() == NlsMessageResult.YES)
			{
				IsBusy = true;
				CallResponse<bool> response = await _$product$$Dialog$Repository.Delete$Item$Async(CreateNewCallContext(true), Selected$Item$DataItem, $Item$DataItemsList);
				if (response.IsSuccess)
				{
					if ($Item$DataItemsList.Count > 0)
					{
						Selected$Item$DataItem = null;
						Selected$Item$DataItem = FilterCollection.GetItemAt(0) as $Product$$Item$DataItem;
					}
				}
				else
				{
					ResetNavigation();
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

			var response = await _$product$$Dialog$Repository.Save$Item$Async(CreateNewCallContext(true), Selected$Item$DataItem);
			if (response.IsSuccess)
			{
                $Item$DataItemsList.Accept();
			}
			IsBusy = false;
			RaiseCanExecuteChanged();
			
			return response.IsSuccess;
		}

		private void New$Product$$Item$DataItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
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
			$Product$$Item$DataItemsList.Reload();
			await $Product$$Item$DataItemsList.CurrentLoadingTask;
		}

		/// <summary>
		/// Resets the navigation.
		/// </summary>
		private void ResetNavigation()
		{
			BaseServices.NavigationService.Unload(RegionNames.DetailRegion);
			
			if (Selected$Item$DataItem != null)
			{
				Selected$Item$DataItem.PropertyChanged -= New$Product$$Item$DataItem_PropertyChanged;
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
	        // If the user tries to select another item while there are any Changes We either Cancel
	        // the Selection or allow selection and eigher save or discard the changes

	        if (IsDirty && Selected$Item$DataItem != null && !Equals(Selected$Item$DataItem, args.Value))
	        {
	            args.Cancel = true;

	            NlsMessageResult result = ShowSaveChangesMessage();

	            if (result == NlsMessageResult.YES)
	            {
	                if (Selected$Item$DataItem.HasErrors)
	                {
	                    await BaseServices.PopupDialogService.ShowCheckYourInputMessageAsync();
	                }
	                else
	                {
	                    bool saveResponse = await SaveCommandExecuted();
	                    // if save succeeded then change selection
	                    if (saveResponse)
	                        Selected$Item$DataItem = ($Product$$Item$DataItem)args.Value;
	                }
	            }
	            if (result == NlsMessageResult.NO)
	            {
	                args.Cancel = false;
	                await CancelCommandExecuted();
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
		private void NavigateToDetail($Product$$Item$DataItem activeItem)
		{
			BaseServices.NavigationService.Navigate(RegionNames.DetailRegion, ViewNames.DetailView,
				ParameterNames.Selected$Item$DataItem, activeItem);
		}

		#endregion Navigation
		
		#region FilterSourceProvider Interface

		/// <summary>
		/// Raises the source collection changed.
		/// </summary>
		/// <param name="sourceCollection">The source collection.</param>
		private void RaiseSourceCollectionChanged(IList<$Product$$Item$DataItem> sourceCollection)
		{
			if (SourceCollectionChanged != null)
			{
				SourceCollectionChanged.Invoke(this, new SourceCollectionChangedEventArgs<$Product$$Item$DataItem>()
				{
					SourceCollection = sourceCollection
				});
			}
		}

		/// <summary>
		/// Occurs when [source collection changed].
		/// </summary>
		public event EventHandler<SourceCollectionChangedEventArgs<$Product$$Item$DataItem>> SourceCollectionChanged;

		#endregion
	}
}
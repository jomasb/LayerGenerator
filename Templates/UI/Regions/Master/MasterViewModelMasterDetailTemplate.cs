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
using Dcx.Plus.Repository.FW;
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
	public class $Dialog$MasterViewModel : MasterViewModelBase, ILazyLoadingHandler, IFilterSourceProvider<$Product$$ChildDataItem$DataItem>, IFilterSourceProvider<$Product$$MasterDataItem$DataItem>
	{
		#region Members
		
		private readonly I$Product$$Dialog$Repository _$product$$Dialog$Repository;
		private PlusLazyLoadingAsyncObservableCollection<$Product$$MasterDataItem$DataItem> _$product$$MasterDataItem$DataItemsList;
		private $Product$$MasterDataItem$DataItem _selected$MasterDataItem$DataItem;
		private $Product$$ChildDataItem$DataItem _selected$ChildDataItem$DataItem;
		private PropertyObserver<PlusLazyLoadingAsyncObservableCollection<$Product$$MasterDataItem$DataItem>> _isDirtyObserver;
		private CollectionView _secondFilterCollection;
		
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
		    CommandService.SubscribeCommand(GlobalCommandNames.OpenSecondSettingsDialogCommand, OpenSecondSettingsDialogCommandExecuted);

            Add$ChildDataItem$Command = CommandService.GetCommand(CommandNames.Add$ChildDataItem$Command);
		    Add$ChildDataItem$Command.CanExecuteWhileActive = false;
		    Delete$ChildDataItem$Command = CommandService.GetCommand(CommandNames.Delete$ChildDataItem$Command);
		    Delete$ChildDataItem$Command.CanExecuteWhileActive = false;
            CommandService.SubscribeAsyncCommand(CommandNames.Add$ChildDataItem$Command, Add$ChildDataItem$CommandExecuted, Add$ChildDataItem$CommandCanExecute);
		    CommandService.SubscribeAsyncCommand(CommandNames.Delete$ChildDataItem$Command, Delete$ChildDataItem$CommandExecuted, Delete$ChildDataItem$CommandCanExecute);

            SelectingActiveItemCommand = new PlusCommand<CancelableSelectionArgs<object>>(SelectingActiveItemCommandExecute);

			$Product$$MasterDataItem$DataItemsList = _$product$$Dialog$Repository.Get$MasterDataItem$s(CreateNewCallContext(true));
			$Product$$MasterDataItem$DataItemsList.RegisterLoadingHandler(this);

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
		public $Product$$MasterDataItem$DataItem Selected$MasterDataItem$DataItem
		{
			get
			{
				return _selected$MasterDataItem$DataItem;
			}
			set
			{
				Set(ref _selected$MasterDataItem$DataItem, value);

				if (_selected$MasterDataItem$DataItem != null)
				{
				    _selected$MasterDataItem$DataItem.$ChildDataItem$s.RegisterLoadingHandler(this);
                    SecondFilterCollection = (CollectionView)CollectionViewSource.GetDefaultView(_selected$MasterDataItem$DataItem.$ChildDataItem$s.LoadedItems);
					NavigateTo$MasterDataItem$Detail(_selected$MasterDataItem$DataItem);
				}
				else
				{
					ResetNavigation();
				}

				RaiseCanExecuteChanged();
			}
		}

		public $Product$$ChildDataItem$DataItem Selected$ChildDataItem$DataItem
		{
			get
			{
				return _selected$ChildDataItem$DataItem;
			}
			set
			{
				Set(ref _selected$ChildDataItem$DataItem, value);

				if (_selected$ChildDataItem$DataItem != null)
				{
					NavigateTo$ChildDataItem$Detail(_selected$ChildDataItem$DataItem);
				}
				else
				{
					ResetNavigation();
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
		    if (sender is ILazyLoadingObservableCollection<$Product$DivisionDataItem>)
		    {
		        On$Product$$MasterDataItem$DataItemListLoaded();
		    }

            if (sender is ILazyLoadingObservableCollection<$Product$$MasterDataItem$DataItem>)
			{
				IsInitialLoadingCompleted = true;
				On$Product$$MasterDataItem$DataItemListLoaded();
			}

			if (sender is ILazyLoadingObservableCollection<$Product$$ChildDataItem$DataItem>)
			{
				On$Product$$ChildDataItem$DataItemListLoaded();
			}

			IsGlobalBusy = false;
			RaiseCanExecuteChanged();
		}

		private void On$Product$$ChildDataItem$DataItemListLoaded()
		{
			On$ChildDataItem$SourceCollectionChanged(new SourceCollectionChangedEventArgs<$Product$$ChildDataItem$DataItem>()
			{
				SourceCollection = Selected$MasterDataItem$DataItem.$ChildDataItem$s.LoadedItems
			});
		}

		/// <summary>
		/// Called when [$Product$DataItemList loaded].
		/// </summary>
		private void On$Product$$MasterDataItem$DataItemListLoaded()
		{
			_isDirtyObserver = new PropertyObserver<PlusLazyLoadingAsyncObservableCollection<$Product$$MasterDataItem$DataItem>>($Product$$MasterDataItem$DataItemsList);
			_isDirtyObserver.RegisterHandler(x => x.HasAnyChanges, OnCollectionHasAnyChanges);

			On$MasterDataItem$SourceCollectionChanged(new SourceCollectionChangedEventArgs<$Product$$MasterDataItem$DataItem>()
			{
				SourceCollection = $Product$$MasterDataItem$DataItemsList.LoadedItems
			});

			ResetNavigation();

			if ($Product$$MasterDataItem$DataItemsList.Count > 0)
			{
				Selected$MasterDataItem$DataItem = null;
				Selected$MasterDataItem$DataItem = FilterCollection.GetItemAt(0) as $Product$$MasterDataItem$DataItem;
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
			IsGlobalBusy = false;
			IsInitialLoadingCompleted = false;
			RaiseCanExecuteChanged();
		}

		public CollectionView SecondFilterCollection
		{
			get
			{
				return _secondFilterCollection;
			}
			set
			{
				Set(ref _secondFilterCollection, value);
			}
		}

        #endregion Methods

        #region Command Handler

	    public IPlusCommand Add$ChildDataItem$Command { get; set; }

	    public IPlusCommand Delete$ChildDataItem$Command { get; set; }

	    private bool Delete$ChildDataItem$CommandCanExecute()
	    {
	        return !IsInReadOnlyMode && !IsBusy && IsInitialLoadingCompleted && Selected$MasterDataItem$DataItem != null && Selected$ChildDataItem$DataItem != null;
	    }

	    private async Task Delete$ChildDataItem$CommandExecuted()
	    {
	        if (ShowDeletingMessage() == NlsMessageResult.YES)
	        {
	            IsGlobalBusy = true;
	            Selected$MasterDataItem$DataItem.$ChildDataItem$s.Remove(Selected$ChildDataItem$DataItem);

                Selected$MasterDataItem$DataItem.HasAnyChanges = true;
	            if (SecondFilterCollection.Count > 0)
	            {
	                Selected$ChildDataItem$DataItem = SecondFilterCollection.GetItemAt(0) as $Product$$ChildDataItem$DataItem;
	            }
                RaiseCanExecuteChanged();
	            IsGlobalBusy = false;
	        }
        }

	    private bool Add$ChildDataItem$CommandCanExecute()
	    {
	        return !IsInReadOnlyMode && !IsBusy && IsInitialLoadingCompleted && Selected$MasterDataItem$DataItem != null;
	    }

	    private async Task Add$ChildDataItem$CommandExecuted()
	    {
	        IsGlobalBusy = true;
	        CallResponse<$Product$$ChildDataItem$DataItem> response = await _$product$$Dialog$Repository.AddNew$ChildDataItem$Async(CreateNewCallContext(true), Selected$MasterDataItem$DataItem.$ChildDataItem$s);
	        if (response.IsSuccess)
	        {
	            Selected$MasterDataItem$DataItem.HasAnyChanges = true;
	            Selected$ChildDataItem$DataItem = response.Result;
	            Selected$ChildDataItem$DataItem.PropertyChanged += New$Product$DataItem_PropertyChanged;
            }
	        IsGlobalBusy = false;
        }

	    public override bool AddCommandCanExecute()
	    {
	        return base.AddCommandCanExecute() && IsInitialLoadingCompleted;
	    }

	    private async Task AddCommandExecuted()
		{
			IsGlobalBusy = true;
			CallResponse<$Product$$MasterDataItem$DataItem> response = await _$product$$Dialog$Repository.AddNew$MasterDataItem$Async(CreateNewCallContext(true), $Product$$MasterDataItem$DataItemsList);
			if (response.IsSuccess)
			{
				$Product$$MasterDataItem$DataItem new$Product$DataItem = response.Result;
				Selected$MasterDataItem$DataItem = new$Product$DataItem;
				new$Product$DataItem.HasAnyChanges = true;
				new$Product$DataItem.PropertyChanged += New$Product$DataItem_PropertyChanged;
				RaiseCanExecuteChanged();
			}
			IsGlobalBusy = false;
		}

	    public override bool CopyCommandCanExecute()
	    {
	        return base.CopyCommandCanExecute() && IsInitialLoadingCompleted;
	    }

	    private async Task CopyCommandExecuted()
		{
			IsGlobalBusy = true;
			if (Selected$MasterDataItem$DataItem != null)
			{
				CallResponse<$Product$$MasterDataItem$DataItem> response = await _$product$$Dialog$Repository.Clone$MasterDataItem$Async(CreateNewCallContext(true), Selected$MasterDataItem$DataItem, $Product$$MasterDataItem$DataItemsList);
				if (response.IsSuccess)
				{
					$Product$$MasterDataItem$DataItem new$Product$DataItem = response.Result;
					Selected$MasterDataItem$DataItem = new$Product$DataItem;
					new$Product$DataItem.HasAnyChanges = true;
					new$Product$DataItem.PropertyChanged += New$Product$DataItem_PropertyChanged;
					RaiseCanExecuteChanged();
				}
			}
			IsGlobalBusy = false;
		}

	    public override bool CancelCommandCanExecute()
	    {
	        return base.CancelCommandCanExecute() && IsInitialLoadingCompleted;
	    }

	    private async Task CancelCommandExecuted()
		{
			await $Product$$MasterDataItem$DataItemsList.Rollback();
			$Product$$MasterDataItem$DataItemsList.HasAnyChanges = false;
			RaiseCanExecuteChanged();
			BaseServices.EventAggregator.Publish(GlobalEventNames.CancelExecuted);
		}

	    public override bool DeleteCommandCanExecute()
	    {
	        return base.DeleteCommandCanExecute() && IsInitialLoadingCompleted;
	    }

	    private async Task DeleteCommandExecuted()
		{
			if (ShowDeletingMessage() == NlsMessageResult.YES)
			{
				IsGlobalBusy = true;
				CallResponse<bool> response = await _$product$$Dialog$Repository.Delete$MasterDataItem$Async(CreateNewCallContext(true), Selected$MasterDataItem$DataItem, $Product$$MasterDataItem$DataItemsList);
				if (response.IsSuccess)
				{
				    $Product$$MasterDataItem$DataItemsList.Remove(Selected$MasterDataItem$DataItem);
                    $Product$$MasterDataItem$DataItemsList.AcceptDeep();

					if ($Product$$MasterDataItem$DataItemsList.Count > 0)
					{
						Selected$MasterDataItem$DataItem = null;
						Selected$MasterDataItem$DataItem = FilterCollection.GetItemAt(0) as $Product$$MasterDataItem$DataItem;
					}
				}
				else
				{
					ResetNavigation();
				}

				RaiseCanExecuteChanged();
				IsGlobalBusy = false;
			}
		}

	    public override bool SaveCommandCanExecute()
	    {
	        return base.SaveCommandCanExecute() && IsInitialLoadingCompleted;
	    }

	    private async Task<bool> SaveCommandExecuted()
		{
			IsGlobalBusy = true;
			RaiseCanExecuteChanged();

			CallResponse<$Product$$MasterDataItem$DataItem> response = await _$product$$Dialog$Repository.Save$MasterDataItem$Async(CreateNewCallContext(true), Selected$MasterDataItem$DataItem);
			if (response.IsSuccess)
			{
			}
			IsGlobalBusy = false;
			RaiseCanExecuteChanged();
			
			return response.IsSuccess;
		}

		private void New$Product$DataItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
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
			await $Product$$MasterDataItem$$DataItemsList.CurrentLoadingTask;
			}
			
			
			await $Product$$Item$DataItemsList.CurrentLoadingTask;
		}

		/// <summary>
		/// Resets the navigation.
		/// </summary>
		private void ResetNavigation()
		{
			BaseServices.NavigationService.Unload(RegionNames.DetailRegion);
			
			if (Selected$MasterDataItem$DataItem != null)
			{
				Selected$MasterDataItem$DataItem.PropertyChanged -= New$Product$DataItem_PropertyChanged;
			}
		}

	    protected override bool RefreshCommandCanExecute()
	    {
	        return base.RefreshCommandCanExecute() && IsInitialLoadingCompleted;
	    }

	    /// <summary>
		/// Refreshes the command executed.
		/// </summary>
		private async void RefreshCommandExecuted()
		{
			await Refresh();
		}

	    /// <summary>
	    /// Gets a value indicating whether this instance has any errors.
	    /// </summary>
	    /// <value>
	    ///   <c>true</c> if this instance has any errors; otherwise, <c>false</c>.
	    /// </value>
	    public bool HasAnyErrors
	    {
	        get
	        {
	            var activeItem = Selected$MasterDataItem$DataItem as PlusNotifyDataErrorInfoBase;
	            return activeItem != null && activeItem.HasErrors || BaseServices.ValidationService.HasAnyErrors;
	        }
	    }

        /// <summary>
        /// Selectings the active item command execute.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private async void SelectingActiveItemCommandExecute(CancelableSelectionArgs<object> args)
	    {
            // If the user tries to select another item while there are any changes, we either cancel
            // the selection or allow selection and eigher save or discard the changes
            if (IsDirty && (($Product$$MasterDataItem$DataItem)args.Value).State != DataItemState.New)
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
                            Selected$MasterDataItem$DataItem = ($Product$$MasterDataItem$DataItem)args.Value;
                    }

                if (result == NlsMessageResult.NO)
                {
                    args.Cancel = false;
                    await CancelCommandExecuted();
                    Selected$MasterDataItem$DataItem = ($Product$$MasterDataItem$DataItem)args.Value;
                }

                if (result == NlsMessageResult.CANCEL) args.Cancel = true;
            }
        }

        #endregion Command Handler

        #region Navigation

        /// <summary>
        /// Navigates to $MasterDataItem$.
        /// </summary>
        /// <param name="activeItem">The active item.</param>
        private void NavigateTo$MasterDataItem$Detail($Product$$MasterDataItem$DataItem activeItem)
		{
			BaseServices.NavigationService.Navigate(RegionNames.DetailRegion, ViewNames.$MasterDataItem$DetailView,
				ParameterNames.Selected$MasterDataItem$DataItem, activeItem);
		}

		/// <summary>
		/// Navigates to $ChildDataItem$.
		/// </summary>
		/// <param name="activeItem">The active item.</param>
		private void NavigateTo$ChildDataItem$Detail($Product$$ChildDataItem$DataItem activeItem)
		{
			BaseServices.NavigationService.Navigate(RegionNames.DetailRegion, ViewNames.$ChildDataItem$DetailView,
				ParameterNames.Selected$MasterDataItem$DataItem, activeItem);
		}

		#endregion Navigation

		#region FilterSourceProvider Interface

		/// <summary>
		/// The component action source collection changed
		/// </summary>
		private EventHandler<SourceCollectionChangedEventArgs<$Product$$MasterDataItem$DataItem>> _$masterDataItem$SourceCollectionChanged;

		/// <summary>
		/// The component cell assignment source collection a changed
		/// </summary>
		private EventHandler<SourceCollectionChangedEventArgs<$Product$$ChildDataItem$DataItem>> _$childDataItem$SourceCollectionChanged;


		/// <summary>
		/// Occurs when [source collection changed].
		/// </summary>
		event EventHandler<SourceCollectionChangedEventArgs<$Product$$MasterDataItem$DataItem>> IFilterSourceProvider<$Product$$MasterDataItem$DataItem>.SourceCollectionChanged
		{
			add
			{
				_$masterDataItem$SourceCollectionChanged += value;

			}
			remove
			{
				if (_$masterDataItem$SourceCollectionChanged != null)
					_$masterDataItem$SourceCollectionChanged -= value;
			}
		}

		/// <summary>
		/// Occurs when [source collection changed].
		/// </summary>
		event EventHandler<SourceCollectionChangedEventArgs<$Product$$ChildDataItem$DataItem>> IFilterSourceProvider<$Product$$ChildDataItem$DataItem>.SourceCollectionChanged
		{
			add
			{
				_$childDataItem$SourceCollectionChanged += value;

			}
			remove
			{
				if (_$childDataItem$SourceCollectionChanged != null)
					_$childDataItem$SourceCollectionChanged -= value;
			}
		}

		/// <summary>
		/// Raises the <see cref="E:SiteSourceCollectionChanged" /> event.
		/// </summary>
		/// <param name="e">The <see cref="SourceCollectionChangedEventArgs{$Product$$MasterDataItem$DataItem}"/> instance containing the event data.</param>
		protected void On$MasterDataItem$SourceCollectionChanged(SourceCollectionChangedEventArgs<$Product$$MasterDataItem$DataItem> e)
		{
			var handler = _$masterDataItem$SourceCollectionChanged;
			if (handler != null)
				handler(this, e);
		}

		/// <summary>
		/// Raises the <see cref="E:ComponentCellAssignmentSourceCollectionChanged" /> event.
		/// </summary>
		/// <param name="e">The <see cref="SourceCollectionChangedEventArgs{$Product$$ChildDataItem$DataItem}"/> instance containing the event data.</param>
		protected void On$ChildDataItem$SourceCollectionChanged(SourceCollectionChangedEventArgs<$Product$$ChildDataItem$DataItem> e)
		{
			var handler = _$childDataItem$SourceCollectionChanged;
			if (handler != null)
				handler(this, e);
		}

		#endregion
	}
}
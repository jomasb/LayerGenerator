using System.Threading.Tasks;
using Dcx.Plus.Localization;
using Dcx.Plus.Localization.Modules.$Product$;
using Dcx.Plus.Repository.FW;
using Dcx.Plus.Repository.Modules.$Product$.Contracts;
using Dcx.Plus.Repository.Modules.$Product$.DataItems;
using Dcx.Plus.UI.WPF.FW.Shell.Commands;
using Dcx.Plus.UI.WPF.FW.Shell.Extensions;
using Dcx.Plus.UI.WPF.FW.Shell.Infrastructure;
using Dcx.Plus.UI.WPF.FW.Shell.Interfaces;
using Dcx.Plus.UI.WPF.Modules.$Product$.Windows.$Dialog$.Infrastructure;

namespace Dcx.Plus.UI.WPF.Modules.$Product$.Windows.$Dialog$.Regions.Detail
{
	/// <summary>
	/// Definition of the view model for the version detail view.
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
	        ActivateVersionCommand = PlusCommand.FromAsyncHandler(ActivateVersionCommandExecute, ActivateVersionCommandCanExecute);
	        ActivateVersionCommand.CanExecuteWhileActive = false;
	        BaseServices.EventAggregator.Subscribe<bool>(GlobalEventNames.BusyStateChanged, isbusy => IsBusy = isbusy);
			$specialContent2$
        }
		
		#endregion Construction

	    #region Properties
		
		$specialContent3$

		/// <summary>
	    /// Gets the activate version command.
	    /// </summary>
	    /// <value>The activate version command.</value>
	    public PlusCommand ActivateVersionCommand
	    {
		    get;
		    private set;
	    }

	    #endregion

	    #region Commands

	    /// <summary>
	    /// Activates the version command can execute.
	    /// </summary>
	    /// <returns></returns>
	    private bool ActivateVersionCommandCanExecute()
	    {
		    if (DataItem != null)
		    {
			    return !DataItem.HasAnyChanges && DataItem.State != DataItemState.New;
		    }
		    return false;
	    }

	    /// <summary>
	    /// Activates the version command execute.
	    /// </summary>
	    /// <returns></returns>
	    private async Task ActivateVersionCommandExecute()
	    {
		    if (DataItem.IsActive)
		    {
			    await _repository.Deactivate$Item$Async(
				    BaseServices.CallContextCreator.CreateContext(),
				    DataItem);
		    }
		    else
		    {
			    await _repository.Activate$Item$Async(
				    BaseServices.CallContextCreator.CreateContext(),
				    DataItem);
		    }

		    RaiseCanExecuteChanged();
		    OnPropertyChanged(() => ActivateVersionCommandDisplayName);
		    BaseServices.EventAggregator.Publish(EventNames.VersionActivationChanged, true);
	    }

	    /// <summary>
	    /// Gets the display name of the activate version command.
	    /// </summary>
	    /// <value>
	    /// The display name of the activate version command.
	    /// </value>
	    public string ActivateVersionCommandDisplayName
	    {
		    get
		    {
			    string commandName = string.Empty;

			    if (DataItem != null)
			    {
				    commandName = DataItem.IsActive
					    ? $Product$Localizer.Singleton.$Product$$Dialog$_btnDeactivateVersion.Translation
						: $Product$Localizer.Singleton.$Product$$Dialog$_btnActivateVersion.Translation;
			    }

			    return commandName;
		    }
	    }

	    /// <summary>
	    /// Raises the can execute changed.
	    /// </summary>
	    private void RaiseCanExecuteChanged()
	    {
		    CommandService.RaiseCanExecuteChanged();
		    ActivateVersionCommand.RaiseCanExecuteChanged();
	    }

	    #endregion

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
	        OnPropertyChanged(() => ActivateVersionCommandDisplayName);
			DataItemObserver.RegisterHandler(x => x.HasChanges, version => OnPropertyChanged(() => IsNewItem));
	        RaiseCanExecuteChanged();
		}

		#endregion Navigation
		
		public void OnRegisteredLazyCollectionLoaded(object sender)
		{
			$specialContent5$
		}

		public void OnRegisteredLazyCollectionException(object sender, Exception exception)
		{
			
		}
    }
}

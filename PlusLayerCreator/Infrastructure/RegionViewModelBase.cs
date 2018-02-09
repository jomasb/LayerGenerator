using System;
using Prism;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

namespace PlusLayerCreator.Infrastructure
{
    public abstract class RegionViewModelBase : BindableBase, INavigationAware, IActiveAware, IConfirmNavigationRequest
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly INavigationService _navigationService;
        private string _displayName;
        private bool _isActive;


        public RegionViewModelBase(INavigationService navigationService, IEventAggregator eventAggregator)
        {
            _navigationService = navigationService;
            _eventAggregator = eventAggregator;
            DisplayName = this.ToString();
        }


        public IEventAggregator EventAggregator
        {
            get { return _eventAggregator; }
        }

        public INavigationService NavigationService
        {
            get { return _navigationService; }
        }

        public string DisplayName
        {
            get { return _displayName; }
            set { SetProperty(ref _displayName, value); }
        }

        public bool IsActive
        {
            get { return _isActive; }
            set { SetProperty(ref _isActive, value); }
        }

        public event EventHandler IsActiveChanged;

        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        public virtual bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public virtual void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            continuationCallback(true);
        }
    }
}
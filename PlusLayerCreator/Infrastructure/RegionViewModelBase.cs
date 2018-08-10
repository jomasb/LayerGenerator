using System;
using Prism;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

namespace PlusLayerCreator.Infrastructure
{
    public abstract class RegionViewModelBase : BindableBase, IActiveAware, IConfirmNavigationRequest
    {
        private string _displayName;
        private bool _isActive;


        public RegionViewModelBase(INavigationService navigationService, IEventAggregator eventAggregator)
        {
            NavigationService = navigationService;
            EventAggregator = eventAggregator;
            DisplayName = ToString();
        }


        public IEventAggregator EventAggregator { get; }

        public INavigationService NavigationService { get; }

        public string DisplayName
        {
            get => _displayName;
            set => SetProperty(ref _displayName, value);
        }

        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }

        public event EventHandler IsActiveChanged;

        public virtual void ConfirmNavigationRequest(NavigationContext navigationContext,
            Action<bool> continuationCallback)
        {
            continuationCallback(true);
        }

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
    }
}
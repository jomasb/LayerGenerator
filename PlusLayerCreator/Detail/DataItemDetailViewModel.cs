using PlusLayerCreator.Infrastructure;
using PlusLayerCreator.Items;
using Prism.Events;
using Prism.Regions;

namespace PlusLayerCreator.Detail
{
    public class DataItemDetailViewModel : RegionViewModelBase
    {
        private ConfigurationItem _dataItem;

        public DataItemDetailViewModel(INavigationService navigationService, IEventAggregator eventAggregator) : base(
            navigationService, eventAggregator)
        {
        }

        public ConfigurationItem DataItem
        {
            get => _dataItem;
            set => SetProperty(ref _dataItem, value);
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            DataItem = navigationContext.Parameters[ParameterNames.SelectedItem] as ConfigurationItem;
        }
    }
}
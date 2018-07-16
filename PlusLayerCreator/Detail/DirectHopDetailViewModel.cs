using PlusLayerCreator.Infrastructure;
using PlusLayerCreator.Items;
using Prism.Events;
using Prism.Regions;

namespace PlusLayerCreator.Detail
{
    public class DirectHopDetailViewModel : RegionViewModelBase
    {
        private DirectHopItem _dataItem;

        public DirectHopDetailViewModel(INavigationService navigationService, IEventAggregator eventAggregator) : base(
            navigationService, eventAggregator)
        {
        }

        public DirectHopItem DataItem
        {
            get => _dataItem;
            set => SetProperty(ref _dataItem, value);
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            DataItem = navigationContext.Parameters[ParameterNames.SelectedItem] as DirectHopItem;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlusLayerCreator.Infrastructure;
using PlusLayerCreator.Items;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

namespace PlusLayerCreator.Detail
{
    public class DataItemDetailViewModel : RegionViewModelBase
    {

        private ConfigurationItem _dataItem;
        public ConfigurationItem DataItem
        {
            get { return _dataItem; }
            set { SetProperty(ref _dataItem, value); }
        }

        public DataItemDetailViewModel(INavigationService navigationService, IEventAggregator eventAggregator) : base(navigationService, eventAggregator)
        {
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            DataItem = navigationContext.Parameters[ParameterNames.SelectedItem] as ConfigurationItem;
        }
    }
}

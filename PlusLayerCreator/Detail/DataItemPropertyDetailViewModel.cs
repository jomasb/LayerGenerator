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
    public class DataItemPropertyDetailViewModel : RegionViewModelBase
    {
        private ConfigurationProperty _property;
        public ConfigurationProperty Property
        {
            get { return _property; }
            set { SetProperty(ref _property, value); }
        }

        public List<string> Types { get; set; }

        public List<string> FilterTypes { get; set; }


        public DataItemPropertyDetailViewModel(INavigationService navigationService, IEventAggregator eventAggregator) : base(navigationService, eventAggregator)
        {
            Types = new List<string>();
            Types.Add("string");
            Types.Add("int");
            Types.Add("bool");
            Types.Add("DateTime");

            FilterTypes = new List<string>();
            FilterTypes.Add("TextBox");
            FilterTypes.Add("ComboBox");
            FilterTypes.Add("CheckBox");
            FilterTypes.Add("DateTimePicker");
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            Property = navigationContext.Parameters[ParameterNames.SelectedItem] as ConfigurationProperty;
        }
    }
}

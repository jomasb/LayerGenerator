using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace PlusLayerCreator.Items
{
    [DataContract]
    public class DirectHopItem : ItemBase
    {
        
        private string _controllerHandle;
        private string _dialogName;
        private string _localizationKey;
        private string _product;
        private int _order;

        [DataMember]
        public string ControllerHandle
        {
            get => _controllerHandle;
            set => SetProperty(ref _controllerHandle, value);
        }

        [DataMember]
        public string DialogName
        {
            get => _dialogName;
            set => SetProperty(ref _dialogName, value);
        }

        [DataMember]
        public string LocalizationKey
        {
            get => _localizationKey;
            set => SetProperty(ref _localizationKey, value);
        }

        [DataMember]
        public string Product
        {
            get => _product;
            set => SetProperty(ref _product, value);
        }

        [DataMember]
        public int Order
        {
            get => _order;
            set => SetProperty(ref _order, value);
        }
    }
}
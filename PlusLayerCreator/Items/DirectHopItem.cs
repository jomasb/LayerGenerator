namespace PlusLayerCreator.Items
{
    public class DirectHopItem : ItemBase
    {
        
        private string _controllerHandle;
        private string _dialogName;
        private string _localizationKey;
        private string _product;
        private int _order;

        
        public string ControllerHandle
        {
            get => _controllerHandle;
            set => SetProperty(ref _controllerHandle, value);
        }

        
        public string DialogName
        {
            get => _dialogName;
            set => SetProperty(ref _dialogName, value);
        }

        
        public string LocalizationKey
        {
            get => _localizationKey;
            set => SetProperty(ref _localizationKey, value);
        }

        
        public string Product
        {
            get => _product;
            set => SetProperty(ref _product, value);
        }

        
        public int Order
        {
            get => _order;
            set => SetProperty(ref _order, value);
        }
    }
}
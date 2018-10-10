namespace PlusLayerCreator.Items
{
    public class ConfigurationProperty : ItemBase
    {
        private int _order;
        private string _name;
        private string _translationDe;
        private string _translationEn;
        private string _type;
        private string _comboBoxItem;
        private string _filterPropertyType;
        private bool _isFilterProperty;
        private bool _isKey;
        private bool _isVisibleInGrid;
        private bool _isRequired;
        private bool _isReadOnly;
        private string _length;
        private bool _shouldLazyLoad;
        private string _messageField;
        private string _messageDataType = "*";

        
        public string Name
		{
			get
			{
				return _name;
			}

			set
			{
				if (SetProperty(ref _name, value))
				{
					TranslationEn = _name;
				}
			}
		}

		
        public int Order
        {
            get => _order;
            set => SetProperty(ref _order, value);
        }

        
        public string TranslationDe
        {
            get => _translationDe;
            set => SetProperty(ref _translationDe, value);
        }

        
        public string TranslationEn
        {
            get => _translationEn;
            set => SetProperty(ref _translationEn, value);
        }

        
        public string Type
        {
            get => _type;
			set
			{
				if (SetProperty(ref _type, value))
				{
					if (_type == "DataItem")
					{
						Length = string.Empty;
						IsReadOnly = false;
						IsRequired = false;
						IsKey = false;
						IsFilterProperty = false;
						ShouldLazyLoad = false;
						MessageField = string.Empty;
					}

					if (_type == "bool")
					{
						Length = "1";
					}
				}
			}
		}

        
        public string ComboBoxItem
		{
            get => _comboBoxItem;
            set => SetProperty(ref _comboBoxItem, value);
        }

        
        public string Length
        {
            get => _length;
            set => SetProperty(ref _length, value);
        }

        
        public bool IsReadOnly
        {
            get => _isReadOnly;
            set => SetProperty(ref _isReadOnly, value);
        }

        
        public bool IsRequired
        {
            get => _isRequired;
            set => SetProperty(ref _isRequired, value);
        }

        
        public bool IsKey
        {
            get => _isKey;
            set => SetProperty(ref _isKey, value);
        }

        
        public bool IsVisibleInGrid
        {
            get => _isVisibleInGrid;
            set => SetProperty(ref _isVisibleInGrid, value);
        }

        
        public bool IsFilterProperty
        {
            get => _isFilterProperty;
            set => SetProperty(ref _isFilterProperty, value);
        }

        
        public string FilterPropertyType
        {
            get => _filterPropertyType;
            set => SetProperty(ref _filterPropertyType, value);
        }

        
        public bool ShouldLazyLoad
        {
            get => _shouldLazyLoad;
            set => SetProperty(ref _shouldLazyLoad, value);
        }
		
        
        public string MessageField
        {
            get => _messageField;
            set => SetProperty(ref _messageField, value);
        }

        
        public string MessageDataType
		{
            get => _messageDataType;
            set => SetProperty(ref _messageDataType, value);
        }
    }
}
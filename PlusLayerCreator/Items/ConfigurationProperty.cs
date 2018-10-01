using System.Runtime.Serialization;

namespace PlusLayerCreator.Items
{
    [DataContract]
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

        [DataMember]
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        [DataMember]
        public int Order
        {
            get => _order;
            set => SetProperty(ref _order, value);
        }

        [DataMember]
        public string TranslationDe
        {
            get => _translationDe;
            set => SetProperty(ref _translationDe, value);
        }

        [DataMember]
        public string TranslationEn
        {
            get => _translationEn;
            set => SetProperty(ref _translationEn, value);
        }

        [DataMember]
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
				}
			}
		}

        [DataMember]
        public string ComboBoxItem
		{
            get => _comboBoxItem;
            set => SetProperty(ref _comboBoxItem, value);
        }

        [DataMember]
        public string Length
        {
            get => _length;
            set => SetProperty(ref _length, value);
        }

        [DataMember]
        public bool IsReadOnly
        {
            get => _isReadOnly;
            set => SetProperty(ref _isReadOnly, value);
        }

        [DataMember]
        public bool IsRequired
        {
            get => _isRequired;
            set => SetProperty(ref _isRequired, value);
        }

        [DataMember]
        public bool IsKey
        {
            get => _isKey;
            set => SetProperty(ref _isKey, value);
        }

        [DataMember]
        public bool IsVisibleInGrid
        {
            get => _isVisibleInGrid;
            set => SetProperty(ref _isVisibleInGrid, value);
        }

        [DataMember]
        public bool IsFilterProperty
        {
            get => _isFilterProperty;
            set => SetProperty(ref _isFilterProperty, value);
        }

        [DataMember]
        public string FilterPropertyType
        {
            get => _filterPropertyType;
            set => SetProperty(ref _filterPropertyType, value);
        }

        [DataMember]
        public bool ShouldLazyLoad
        {
            get => _shouldLazyLoad;
            set => SetProperty(ref _shouldLazyLoad, value);
        }
		
        [DataMember]
        public string MessageField
        {
            get => _messageField;
            set => SetProperty(ref _messageField, value);
        }

        [DataMember]
        public string MessageDataType
		{
            get => _messageDataType;
            set => SetProperty(ref _messageDataType, value);
        }
    }
}
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace PlusLayerCreator.Items
{
    [DataContract]
    public class ConfigurationItem : ItemBase
    {
        private bool _canSort;
        private bool _canEditMultiple;
        private bool _canEdit;
        private int _order;
        private string _name;
        private string _translation;
        private string _parent;
        private bool _isPreFilterItem;
        private bool _canDelete;
        private bool _canClone;
        private ObservableCollection<ConfigurationProperty> _properties;
        private string _tableCountProperty;
        private string _transactionCodeWrite;
        private string _transactionCodeRead;
        private string _repRplWrite;
        private string _repRplRead;
        private string _server;

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
        public string Translation
        {
            get => _translation;
            set => SetProperty(ref _translation, value);
        }

        [DataMember]
        public string Parent
        {
            get => _parent;
            set => SetProperty(ref _parent, value);
        }

        [DataMember]
        public bool IsPreFilterItem
        {
            get => _isPreFilterItem;
            set
            {
                SetProperty(ref _isPreFilterItem, value);
                if (value)
                {
                    CanEdit = false;
                    CanEditMultiple = false;
                    CanClone = false;
                    CanDelete = false;
                    CanSort = false;
                }
            }
        }

        [DataMember]
        public bool CanEdit
        {
            get => _canEdit;
            set
            {
                if (value == false && CanEditMultiple)
                    return;

                SetProperty(ref _canEdit, value); ;
            }
        }

        [DataMember]
        public bool CanEditMultiple
        {
            get => _canEditMultiple;
            set
            {
                SetProperty(ref _canEditMultiple, value);
                if (value && !CanEdit)
                    CanEdit = true;
            }
        }

        [DataMember]
        public bool CanDelete
        {
            get => _canDelete;
            set => SetProperty(ref _canDelete, value);
        }

        [DataMember]
        public bool CanClone
        {
            get => _canClone;
            set => SetProperty(ref _canClone, value);
        }

        [DataMember]
        public bool CanSort
        {
            get => _canSort;
            set
            {
                SetProperty(ref _canSort, value);
                if (value && !CanEditMultiple)
                {
                    CanEditMultiple = true;
                }

                if (Properties != null && Properties.Count(t => t.Name == "Sequence") == 0)
                {
                    foreach (ConfigurationProperty property in Properties)
                    {
                        property.Order++;
                    }
                    Properties.Add(new ConfigurationProperty()
                    {
                        Name = "Sequence",
                        FilterPropertyType = null,
                        IsFilterProperty = false,
                        IsKey = false,
                        IsReadOnly = false,
                        IsRequired = true,
                        TranslationDe = "Reihenfolge",
                        Type = "int"
                    });
                }
            }
        }

        [DataMember]
        public string Server
        {
            get => _server;
            set => SetProperty(ref _server, value);
        }

        [DataMember]
        public string RepRplRead
        {
            get => _repRplRead;
            set => SetProperty(ref _repRplRead, value);
        }

        [DataMember]
        public string RepRplWrite
        {
            get => _repRplWrite;
            set => SetProperty(ref _repRplWrite, value);
        }

        [DataMember]
        public string TransactionCodeRead
        {
            get => _transactionCodeRead;
            set => SetProperty(ref _transactionCodeRead, value);
        }

        [DataMember]
        public string TransactionCodeWrite
        {
            get => _transactionCodeWrite;
            set => SetProperty(ref _transactionCodeWrite, value);
        }

        [DataMember]
        public string TableCountProperty
        {
            get => _tableCountProperty;
            set => SetProperty(ref _tableCountProperty, value);
        }

        [DataMember]
        public ObservableCollection<ConfigurationProperty> Properties
        {
            get => _properties;
            set => SetProperty(ref _properties, value);
        }
    }
}
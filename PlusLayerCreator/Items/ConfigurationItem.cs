using System.Collections.ObjectModel;
using System.Linq;

namespace PlusLayerCreator.Items
{
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
        private bool _isDetailComboBoxItem;
        private bool _canDelete;
        private bool _canClone;
        private ObservableCollection<ConfigurationProperty> _properties;
        private string _transactionCodeWrite = "RD";
        private string _transactionCodeRead = "MR";
	    private string _reqRplWrite = "REQ-RPL-2";
		private string _reqRplRead = "REQ-RPL-1";
        private string _sys;
	    private string _requirement;
        private string _server;
	    private string _table;
	    private string _tableDescription;

		
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        
        public int Order
        {
            get => _order;
            set => SetProperty(ref _order, value);
        }

        
        public string Translation
        {
            get => _translation;
            set => SetProperty(ref _translation, value);
        }

        
        public string Parent
        {
            get => _parent;
            set => SetProperty(ref _parent, value);
        }

        
        public bool IsPreFilterItem
        {
            get => _isPreFilterItem;
            set
            {
                SetProperty(ref _isPreFilterItem, value);
                if (value)
                {
	                IsDetailComboBoxItem = false;
	                CanEditMultiple = false;
	                CanEdit = false;
                    CanClone = false;
                    CanDelete = false;
                    CanSort = false;

	                if (Properties != null && !Properties.Any())
	                {
		                Properties.Add(new ConfigurationProperty()
		                {
							Order = 0,
			                Name = "Id",
			                FilterPropertyType = null,
			                IsFilterProperty = false,
			                IsKey = true,
			                IsReadOnly = false,
			                IsRequired = true,
			                TranslationDe = "Id",
			                Type = "string"
		                });
		                Properties.Add(new ConfigurationProperty()
		                {
							Order = 1,
			                Name = "Description",
			                FilterPropertyType = null,
			                IsFilterProperty = false,
			                IsKey = false,
			                IsReadOnly = false,
			                IsRequired = false,
			                TranslationDe = "Beschreibung",
			                Type = "string"
		                });
	                }
				}
            }
        }

        
        public bool IsDetailComboBoxItem
		{
            get => _isDetailComboBoxItem;
            set
            {
                SetProperty(ref _isDetailComboBoxItem, value);
                if (value)
                {
	                IsPreFilterItem = false;
	                CanEditMultiple = false;
					CanEdit = false;
                    CanClone = false;
                    CanDelete = false;
                    CanSort = false;

	                if (Properties != null && !Properties.Any())
	                {
		                Properties.Add(new ConfigurationProperty()
		                {
			                Order = 0,
			                Name = "Id",
			                FilterPropertyType = null,
			                IsFilterProperty = false,
			                IsKey = true,
			                IsReadOnly = false,
			                IsRequired = true,
			                TranslationDe = "Id",
			                Type = "string"
		                });
		                Properties.Add(new ConfigurationProperty()
		                {
			                Order = 1,
			                Name = "Description",
			                FilterPropertyType = null,
			                IsFilterProperty = false,
			                IsKey = false,
			                IsReadOnly = false,
			                IsRequired = false,
			                TranslationDe = "Beschreibung",
			                Type = "string"
		                });
	                }
				}
            }
        }

        
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

        
        public bool CanDelete
        {
            get => _canDelete;
            set => SetProperty(ref _canDelete, value);
        }

        
        public bool CanClone
        {
            get => _canClone;
            set => SetProperty(ref _canClone, value);
        }

        
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
                        Type = "int",
						Length = "3",
						MessageDataType = "*",
						MessageField = "REIHENFOLGE"
                    });
                }
            }
        }

        
        public string Requirement
        {
            get => _requirement;
            set => SetProperty(ref _requirement, value);
        }

        
        public string Sys
        {
            get => _sys;
            set => SetProperty(ref _sys, value);
        }

        
        public string Server
        {
            get => _server;
            set => SetProperty(ref _server, value);
        }

        
        public string Table
        {
            get => _table;
            set => SetProperty(ref _table, value);
        }

        
        public string TableDescription
		{
            get => _tableDescription;
            set => SetProperty(ref _tableDescription, value);
        }

        
        public string ReqRplRead
        {
            get => _reqRplRead;
            set => SetProperty(ref _reqRplRead, value);
        }

        
        public string ReqRplWrite
        {
            get => _reqRplWrite;
            set => SetProperty(ref _reqRplWrite, value);
        }

        
        public string TransactionCodeRead
        {
            get => _transactionCodeRead;
            set => SetProperty(ref _transactionCodeRead, value);
        }

        
        public string TransactionCodeWrite
        {
            get => _transactionCodeWrite;
            set => SetProperty(ref _transactionCodeWrite, value);
        }

        
        public ObservableCollection<ConfigurationProperty> Properties
        {
            get => _properties;
            set => SetProperty(ref _properties, value);
        }
    }
}
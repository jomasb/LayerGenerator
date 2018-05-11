using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using PlusLayerCreator.Infrastructure;
using PlusLayerCreator.Items;
using Prism.Commands;
using Prism.Events;

namespace PlusLayerCreator.Configure
{
	public class ConfigureViewModel : RegionViewModelBase
    {
        #region Members

        private readonly INavigationService _navigationService;

        private ConfigurationItem _activeConfiguration;
		private ConfigurationItem _selectedItem;
		private ConfigurationProperty _selectedPropertyItem;
		private ObservableCollection<ConfigurationItem> _dataLayout;
		
		#region Settings
		
		private string _product;
		private string _dialogName;
		private string _controllerHandle;
		private string _dialogTranslationEnglish;
		private string _dialogTranslationGerman;
		
		private string _inputhPath = @"C:\Projects\LayerGenerator\Templates\";
		private string _outputPath = @"C:\Output\";

		private bool _isCreateDto = true;
		private bool _isCreateDtoFactory = true;
		private bool _isCreateGateway = true;
		private bool _isCreateBusinessService = true;
		private bool _isUseBusinessServiceWithoutBo;
		private bool _isCreateDataItem = true;
		private bool _isCreateDataItemFactory = true;
		private bool _isCreateRepositoryDtoFactory = true;
		private bool _isCreateRepository = true;
		private bool _isCreateUi = true;
		private bool _isCreateUiFilter = true;

		#endregion Settings

		#endregion Members

		#region Construction

        public ConfigureViewModel(INavigationService navigationService, IEventAggregator eventAggregator) : base(navigationService, eventAggregator)
        {
            _navigationService = navigationService;

            _dataLayout = new ObservableCollection<ConfigurationItem>();
            
            StartCommand = new DelegateCommand(StartExecuted);
            AddItemCommand = new DelegateCommand(AddItemCommandExecuted, AddItemCommandCanExecute);
            AddVersionCommand = new DelegateCommand(AddVersionCommandExecuted, AddVersionCommandCanExecute);
            DeleteItemCommand = new DelegateCommand(DeleteItemCommandExecuted, DeleteItemCommandCanExecute);
            AddItemPropertyCommand = new DelegateCommand(AddItemPropertyCommandExecuted, AddItemPropertyCommandCanExecute);
            DeleteItemPropertyCommand = new DelegateCommand(DeleteItemPropertyCommandExecuted, DeleteItemPropertyCommandCanExecute);
            ImportSettingsCommand = new DelegateCommand(ImportSettingsExecuted);
            ExportSettingsCommand = new DelegateCommand(ExportSettingsExecuted);
        }

        #endregion Construction

        #region Commands

        private void RaiseCanExecuteChanged()
		{
            AddItemCommand.RaiseCanExecuteChanged();
			AddVersionCommand.RaiseCanExecuteChanged();
			DeleteItemCommand.RaiseCanExecuteChanged();
			AddItemPropertyCommand.RaiseCanExecuteChanged();
			DeleteItemPropertyCommand.RaiseCanExecuteChanged();
		}

		#region Import/Export

		public readonly DataContractJsonSerializerSettings Settings =
			new DataContractJsonSerializerSettings
			{
				UseSimpleDictionaryFormat = true
			};

		private void ExportSettingsExecuted()
		{
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.Title = "Export settings";
			dialog.Filter = "Config files (*.json)|*.json";
			dialog.ShowDialog();

			if (dialog.FileName != string.Empty)
			{
				FileStream stream = new FileStream(dialog.FileName, FileMode.Create);
				using (var writer = JsonReaderWriterFactory.CreateJsonWriter(
					stream, Encoding.UTF8, true, true, "  "))
				{
					DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Configuration), Settings);
					ser.WriteObject(writer, GetConfiguration());
					writer.Flush();
				}
			}
		}

		private void ImportSettingsExecuted()
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Title = "Import settings";
			dialog.Filter = "Config files (*.json)|*.json";
			dialog.ShowDialog();

			if (dialog.FileName != string.Empty)
			{
				DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Configuration));
				FileStream stream = new FileStream(dialog.FileName, FileMode.Open);
				Configuration configuration = ser.ReadObject(stream) as Configuration;
				stream.Close();
				PropertyInfo[] properties = typeof(Configuration).GetProperties();
				foreach (var property in properties)
				{
					if (property.PropertyType == typeof(IList<ConfigurationItem>))
					{
						IList<ConfigurationItem> dataItems =
							property.GetValue(configuration) as IList<ConfigurationItem>;
						DataLayout.Clear();
						if (dataItems != null)
						{
							foreach (ConfigurationItem plusDataItem in dataItems)
							{
								DataLayout.Add(plusDataItem);
							}
						}
					}
					else
					{
						PropertyInfo propertyInfo = GetType().GetProperty(property.Name);
						if (propertyInfo != null)
						{
							propertyInfo.SetValue(this, property.GetValue(configuration));
						}
						else
						{
						    throw new Exception("Property not found.");
						}
					}
				}
			}
		}

        #endregion Import/Export

        
        private bool AddItemCommandCanExecute()
        {
            return true; //DataLayout.Count < 2;
        }

        private void AddItemCommandExecuted()
		{
		    var item = new ConfigurationItem() {Properties = new ObservableCollection<ConfigurationProperty>(), Order = DataLayout.Count};
            DataLayout.Add(item);
		    SelectedItem = item;
            RaiseCanExecuteChanged();
		}

	    private bool AddVersionCommandCanExecute()
	    {
	        return SelectedItem != null && DataLayout.Count < 2 && DataLayout.All(t => t.Name != null && !t.Name.EndsWith("Version"));
	    }

		private void AddVersionCommandExecuted()
		{
		    var item = new ConfigurationItem()
		    {
			    Name = SelectedItem.Name + "Version",
			    Translation = SelectedItem.Translation + "Version",
				CanClone = true,
				CanDelete = true,
				CanEdit = true,
				CanRead = true,
				Parent = SelectedItem.Name,
			    Properties = new ObservableCollection<ConfigurationProperty>()
			    {
				    new ConfigurationProperty()
				    {
					    IsKey = true,
						IsRequired = true,
					    Type = "int",
					    Name = "Version",
						Translation = "Version",
						Length = "2"
				    },
				    new ConfigurationProperty()
				    {
						Type = "bool",
					    Name = "IsActive",
					    Translation = "Aktiv",
					    Length = "2"
				    },
				    new ConfigurationProperty()
				    {
						Type = "string",
					    Name = "Description",
					    Translation = "Beschreibung",
					    Length = "60"
				    }
			    }
		    };
            DataLayout.Add(item);
		    SelectedItem = item;
            RaiseCanExecuteChanged();
		}

		private void DeleteItemCommandExecuted()
		{
			DataLayout.Remove(SelectedItem);
		    RaiseCanExecuteChanged();
        }

		private bool DeleteItemCommandCanExecute()
		{
			return SelectedItem != null;
		}

		private void AddItemPropertyCommandExecuted()
		{
		    var property = new ConfigurationProperty();
            ActiveConfiguration.Properties.Add(property);
		    SelectedPropertyItem = property;
		    RaiseCanExecuteChanged();
        }

		private bool AddItemPropertyCommandCanExecute()
		{
			return ActiveConfiguration != null;
		}

		private void DeleteItemPropertyCommandExecuted()
		{
			if (ActiveConfiguration != null && SelectedPropertyItem != null)
			{
			    ActiveConfiguration.Properties.Remove(SelectedPropertyItem);
			}
		    RaiseCanExecuteChanged();
        }

		private bool DeleteItemPropertyCommandCanExecute()
		{
			return ActiveConfiguration != null && SelectedPropertyItem != null;
		}

		private void StartExecuted()
		{
			GetTemplatesFromDisk();

			Configuration configuration = GetConfiguration();
			GatewayPart gatewayPart = new GatewayPart(configuration);
			BusinessServiceLocalPart businessServiceLocalPart = new BusinessServiceLocalPart(configuration);
			RepositoryPart repositoryPart = new RepositoryPart(configuration);
			UiPart uiPart = new UiPart(configuration);
			LocalizationPart localizationPart = new LocalizationPart(configuration);

			// Gateway
			if (!IsUseBusinessServiceWithoutBo)
			{
				if (IsCreateDto)
				{
					gatewayPart.CreateDto(true);
				}

				if (IsCreateDtoFactory)
				{
					gatewayPart.CreateDtoFactory();
				}

				if (IsCreateGateway)
				{
					gatewayPart.CreateGateway();
				}
			}

			// Business Service
			if (IsCreateBusinessService)
			{
				if (IsUseBusinessServiceWithoutBo)
				{
					gatewayPart.CreateDto(false);
					businessServiceLocalPart.CreateTandem();
					businessServiceLocalPart.CreateBusinessService(false);	
				}
				else
				{
					businessServiceLocalPart.CreateBusinessService(true);	
				}
			}
			
			// Repository
			if (IsCreateDataItemFactory)
			{
				repositoryPart.CreateDataItemFactory();
			}

			if (IsCreateRepositoryDtoFactory)
			{
				repositoryPart.CreateRepositoryDtoFactory();
			}

			if (IsCreateDataItem)
			{
				repositoryPart.CreateDataItem();
			}
			
			if (IsCreateRepository)
			{
				repositoryPart.CreateRepository();
			}

			// UI
			if (IsCreateUi)
			{
				uiPart.CreateUiInfrastructure();
				uiPart.CreateUiStatusbar();
			    uiPart.CreateUiToolbar();
                uiPart.CreateUi();
				uiPart.CreateUiFilter();
			    uiPart.CreateLauncherConfigEntry();
			    uiPart.CreatePlusDialogInfrastructure();
			}

			localizationPart.CreateLocalization();

		    RaiseCanExecuteChanged();

            MessageBox.Show("Done", "Info", MessageBoxButton.OK);
		}

		#endregion Commands

		#region Methods

		private Configuration GetConfiguration()
		{
			return new Configuration()
			{
				InputPath = InputPath,
				OutputPath = OutputPath,
				IsCreateDto = _isCreateDto,
				IsCreateDtoFactory = _isCreateDtoFactory,
				IsCreateGateway = _isCreateGateway,
				IsCreateBusinessService = _isCreateBusinessService,
				IsUseBusinessServiceWithoutBo = _isUseBusinessServiceWithoutBo,
				IsCreateDataItem = _isCreateDataItem,
				IsCreateDataItemFactory = _isCreateDataItemFactory,
				IsCreateRepositoryDtoFactory = _isCreateRepositoryDtoFactory,
				IsCreateRepository = _isCreateRepository,
				IsCreateUi = _isCreateUi,
				IsCreateUiFilter = _isCreateUiFilter,
				Product = _product,
				DialogName = _dialogName,
				DialogTranslationGerman = _dialogTranslationGerman,
				DialogTranslationEnglish = _dialogTranslationEnglish,
                ControllerHandle = _controllerHandle,
				DataLayout = _dataLayout.ToList()
			};
		}
		
		private void GetTemplatesFromDisk()
		{
		    Helpers.Configuration = GetConfiguration();

			Helpers.FilterChildViewModelTemplate = File.ReadAllText(InputPath + @"UI\Regions\Filter\FilterViewModelChildTemplate.txt");
			Helpers.FilterPropertyTemplate = File.ReadAllText(InputPath + @"UI\Regions\Filter\FilterProperty.txt");
			Helpers.FilterComboBoxPropertyTemplate = File.ReadAllText(InputPath + @"UI\Regions\Filter\FilterComboBox.txt");
			Helpers.FilterTextBoxXamlTemplate = File.ReadAllText(InputPath + @"UI\Regions\Filter\FilterTextBoxXaml.txt");
			Helpers.FilterComboBoxXamlTemplate = File.ReadAllText(InputPath + @"UI\Regions\Filter\FilterComboBoxXaml.txt");
			Helpers.FilterCheckBoxXamlTemplate = File.ReadAllText(InputPath + @"UI\Regions\Filter\FilterCheckBoxXaml.txt");
			Helpers.FilterDateTimePickerXamlTemplate = File.ReadAllText(InputPath + @"UI\Regions\Filter\FilterDateTimePickerXaml.txt");
		}
		
		#endregion Methods

		#region Properties

		#region Commands

		public DelegateCommand StartCommand { get; set; }
		public DelegateCommand ImportSettingsCommand { get; set; }
		public DelegateCommand ExportSettingsCommand
		{
			get;
			set;
		}
		public DelegateCommand AddItemCommand
		{
			get;
			set;
		}
		public DelegateCommand AddVersionCommand
		{
			get;
			set;
		}
		public DelegateCommand DeleteItemCommand
		{
			get;
			set;
		}
		public DelegateCommand AddItemPropertyCommand
		{
			get;
			set;
		}
		public DelegateCommand DeleteItemPropertyCommand
		{
			get;
			set;
		}

        #endregion Commands

        public ConfigurationItem ActiveConfiguration
        {
            get
            {
                return _activeConfiguration;
            }
            set
            {
                if (SetProperty(ref _activeConfiguration, value))
                {
                    RaiseCanExecuteChanged();
                }
            }
        }

        public ConfigurationItem SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                if (SetProperty(ref _selectedItem, value))
                {
                    if (_selectedItem != null)
                    {
                        ActiveConfiguration = SelectedItem;
                        NavigateToDataItemDetail();
                        SelectedPropertyItem = null;
                    }
                    else if (_selectedPropertyItem == null)
                    {
                        UnloadDetailRegion();
                    }

                    RaiseCanExecuteChanged();
                }
            }
        }

        public ConfigurationProperty SelectedPropertyItem
		{
			get
			{
				return _selectedPropertyItem;
			}
			set
			{
				if (SetProperty(ref _selectedPropertyItem, value))
				{
				    if (_selectedPropertyItem != null)
				    {
				        SelectedItem = null;
				        NavigateToDataItemPropertyDetail();
                    }
                    
                    RaiseCanExecuteChanged();
				}
			}
		}

		#region Settings

		public bool IsCreateDto
		{
			get
			{
				return _isCreateDto;
			}
			set
			{
				SetProperty(ref _isCreateDto, value);
			}
		}

		public bool IsCreateDtoFactory
		{
			get
			{
				return _isCreateDtoFactory;
			}
			set
			{
				SetProperty(ref _isCreateDtoFactory, value);
			}
		}

		public bool IsCreateGateway
		{
			get
			{
				return _isCreateGateway;
			}
			set
			{
				SetProperty(ref _isCreateGateway, value);
			}
		}

		public bool IsCreateBusinessService
		{
			get
			{
				return _isCreateBusinessService;
			}
			set
			{
				SetProperty(ref _isCreateBusinessService, value);
			}
		}

		public bool IsUseBusinessServiceWithoutBo
		{
			get
			{
				return _isUseBusinessServiceWithoutBo;
			}
			set
			{
				if (SetProperty(ref _isUseBusinessServiceWithoutBo, value))
				{
					if (value)
					{
						IsCreateDto = false;
						IsCreateDtoFactory = false;
						IsCreateGateway = false;
					}
				}
			}
		}

		public bool IsCreateDataItem
		{
			get
			{
				return _isCreateDataItem;
			}
			set
			{
				SetProperty(ref _isCreateDataItem, value);
			}
		}

		public bool IsCreateDataItemFactory
		{
			get
			{
				return _isCreateDataItemFactory;
			}
			set
			{
				SetProperty(ref _isCreateDataItemFactory, value);
			}
		}


		public bool IsCreateRepositoryDtoFactory
		{
			get
			{
				return _isCreateRepositoryDtoFactory;
			}
			set
			{
				SetProperty(ref _isCreateRepositoryDtoFactory, value);
			}
		}

		public bool IsCreateRepository
		{
			get
			{
				return _isCreateRepository;
			}
			set
			{
				SetProperty(ref _isCreateRepository, value);
			}
		}

		public bool IsCreateUi
		{
			get
			{
				return _isCreateUi;
			}
			set
			{
				SetProperty(ref _isCreateUi, value);
			}
		}

		public bool IsCreateUiFilter
		{
			get
			{
				return _isCreateUiFilter;
			}
			set
			{
				SetProperty(ref _isCreateUiFilter, value);
			}
		}

		#endregion Settings

		public ObservableCollection<ConfigurationItem> DataLayout
		{
			get
			{
				return _dataLayout;
			}
			set
			{
				SetProperty(ref _dataLayout, value);
			}
		}

		public string Product
		{
			get
			{
				return _product;
			}
			set
			{
				SetProperty(ref _product, value);
			}
		}

		public string DialogName
		{
			get
			{
				return _dialogName;
			}
			set
			{
				SetProperty(ref _dialogName, value);
			}
		}

		public string DialogTranslationGerman
		{
			get
			{
				return _dialogTranslationGerman;
			}
			set
			{
				SetProperty(ref _dialogTranslationGerman, value);
			}
		}

		public string DialogTranslationEnglish
		{
			get
			{
				return _dialogTranslationEnglish;
			}
			set
			{
				SetProperty(ref _dialogTranslationEnglish, value);
			}
		}

		public string ControllerHandle
        {
			get
			{
				return _controllerHandle;
			}
			set
			{
				SetProperty(ref _controllerHandle, value);
			}
		}

		public string InputPath
		{
			get
			{
				return _inputhPath;
			}
			set
			{
				SetProperty(ref _inputhPath, value);
			}
		}

		public string OutputPath
		{
			get
			{
				return _outputPath;
			}
			set
			{
				SetProperty(ref _outputPath, value);
			}
		}

		#endregion Properties

        public void UnloadDetailRegion()
        {
            _navigationService.Navigate(RegionNames.DetailRegion, ViewNames.EmptyView);
        }
        public void NavigateToDataItemDetail()
        {
            _navigationService.Navigate(RegionNames.DetailRegion, ViewNames.DataItemDetailView, ParameterNames.SelectedItem, SelectedItem);
        }
        public void NavigateToDataItemPropertyDetail()
        {
            _navigationService.Navigate(RegionNames.DetailRegion, ViewNames.DataItemPropertyDetailView, ParameterNames.SelectedItem, SelectedPropertyItem);
        }
    }
}

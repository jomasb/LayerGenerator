using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Microsoft.Win32;
using PlusLayerCreator.Infrastructure;
using PlusLayerCreator.Items;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;

namespace PlusLayerCreator.Configure
{
    public class ConfigureViewModel : RegionViewModelBase
    {
        #region Construction

        public ConfigureViewModel(INavigationService navigationService, IEventAggregator eventAggregator) : base(
            navigationService, eventAggregator)
        {
            _navigationService = navigationService;

            DataLayout = new ObservableCollection<ConfigurationItem>();
            DirectHops = new ObservableCollection<DirectHopItem>();
            ItemCollection = (CollectionView)CollectionViewSource.GetDefaultView(DataLayout);
            DirectHopCollection = (CollectionView)CollectionViewSource.GetDefaultView(DirectHops);

            StartCommand = new DelegateCommand(StartExecuted);
            AddItemCommand = new DelegateCommand(AddItemCommandExecuted, AddItemCommandCanExecute);
            AddDirectHopCommand = new DelegateCommand(AddDirectHopCommandExecuted, AddDirectHopCommandCanExecute);
            AddVersionCommand = new DelegateCommand(AddVersionCommandExecuted, AddVersionCommandCanExecute);
            DeleteItemCommand = new DelegateCommand(DeleteItemCommandExecuted, DeleteItemCommandCanExecute);
            DeleteDirectHopCommand = new DelegateCommand(DeleteDirectHopCommandExecuted, DeleteDirectHopCommandCanExecute);
            AddItemPropertyCommand =
                new DelegateCommand(AddItemPropertyCommandExecuted, AddItemPropertyCommandCanExecute);
            DeleteItemPropertyCommand =
                new DelegateCommand(DeleteItemPropertyCommandExecuted, DeleteItemPropertyCommandCanExecute);
            ImportSettingsCommand = new DelegateCommand(ImportSettingsExecuted);
            ExportSettingsCommand = new DelegateCommand(ExportSettingsExecuted);
            SortItemUpCommand = new DelegateCommand(SortItemUpCommandExecuted, SortItemUpCommandCanExecute);
            SortItemDownCommand = new DelegateCommand(SortItemDownCommandExecuted, SortItemDownCommandCanExecute);
            SortItemPropertyUpCommand = new DelegateCommand(SortItemPropertyUpCommandExecuted, SortItemPropertyUpCommandCanExecute);
            SortItemPropertyDownCommand = new DelegateCommand(SortItemPropertyDownCommandExecuted, SortItemPropertyDownCommandCanExecute);
	        SortDirectHopUpCommand = new DelegateCommand(SortDirectHopUpCommandExecuted, SortDirectHopUpCommandCanExecute);
	        SortDirectHopDownCommand = new DelegateCommand(SortDirectHopDownCommandExecuted, SortDirectHopDownCommandCanExecute);
        }


	    public CollectionView ItemCollection
		{
		    get => _itemCollection;
		    set => SetProperty(ref _itemCollection, value);
	    }
	    public CollectionView DirectHopCollection
		{
		    get => _directHopCollection;
		    set => SetProperty(ref _directHopCollection, value);
	    }
	    public CollectionView PropertyCollection
	    {
		    get => _propertyCollection;
		    set => SetProperty(ref _propertyCollection, value);
	    }

		#endregion Construction

		public void UnloadDetailRegion()
        {
            _navigationService.Navigate(RegionNames.DetailRegion, ViewNames.EmptyView);
        }

        public void NavigateToDataItemDetail()
        {
            _navigationService.Navigate(RegionNames.DetailRegion, ViewNames.DataItemDetailView,
                ParameterNames.SelectedItem, SelectedItem);
        }

        public void NavigateToDirectHopDetail()
        {
            _navigationService.Navigate(RegionNames.DetailRegion, ViewNames.DirectHopDetailView,
                ParameterNames.SelectedItem, SelectedDirectHop);
        }

        public void NavigateToDataItemPropertyDetail()
        {
	        var nParams = new NavigationParameters();
	        nParams.Add(ParameterNames.SelectedItem, SelectedPropertyItem);
	        nParams.Add(ParameterNames.DataLayout, DataLayout.Where(s => s.IsDetailComboBoxItem).Select(t => t.Name).ToList());

			_navigationService.Navigate(RegionNames.DetailRegion, ViewNames.DataItemPropertyDetailView,
				nParams);
        }

        #region Members

        private readonly INavigationService _navigationService;

        private ConfigurationItem _selectedItem;
        private DirectHopItem _selectedDirectHop;
        private ConfigurationProperty _selectedPropertyItem;
        private ObservableCollection<ConfigurationItem> _dataLayout;
        private ObservableCollection<DirectHopItem> _directHops;

        #region Settings

        private string _product;
        private string _dialogName;
        private string _controllerHandle;
        private string _dialogTranslationEnglish;
        private string _dialogTranslationGerman;

        private string _inputPath = @"C:\Projects\LayerGenerator\Templates\";
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

        #region Commands

        private void RaiseCanExecuteChanged()
        {
			//Item
	        AddItemCommand.RaiseCanExecuteChanged();
	        AddVersionCommand.RaiseCanExecuteChanged();
	        DeleteItemCommand.RaiseCanExecuteChanged();
	        SortItemDownCommand.RaiseCanExecuteChanged();
	        SortItemUpCommand.RaiseCanExecuteChanged();

			//Property
	        AddItemPropertyCommand.RaiseCanExecuteChanged();
	        DeleteItemPropertyCommand.RaiseCanExecuteChanged();
	        SortItemPropertyDownCommand.RaiseCanExecuteChanged();
	        SortItemPropertyUpCommand.RaiseCanExecuteChanged();

			//DirectHop
	        AddDirectHopCommand.RaiseCanExecuteChanged();
	        DeleteDirectHopCommand.RaiseCanExecuteChanged();
	        SortDirectHopDownCommand.RaiseCanExecuteChanged();
	        SortDirectHopUpCommand.RaiseCanExecuteChanged();
		}

		#region Import/Export

		private CollectionView _itemCollection;
		private CollectionView _propertyCollection;
		private CollectionView _directHopCollection;

		private void ExportSettingsExecuted()
		{
			var dialog = new SaveFileDialog();
			dialog.Title = "Export settings";
			dialog.Filter = "Config files (*.json)|*.json";
			dialog.ShowDialog();

			if (dialog.FileName != string.Empty)
			{
				Settings settings = new Settings();
				settings.Export(dialog.FileName, GetConfiguration());
			}
		}

		private void ImportSettingsExecuted()
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "Import settings";
            dialog.Filter = "Config files (*.json)|*.json";
            dialog.ShowDialog();

            if (dialog.FileName != string.Empty)
            {
	            Settings settings = new Settings();
	            WriteConfiguration(settings.Import(dialog.FileName));
			}
        }

        #endregion Import/Export


        private bool AddItemCommandCanExecute()
        {
            return true; //DataLayout.Count < 2;
        }

        private void AddItemCommandExecuted()
        {
            var item = new ConfigurationItem
            {
                Properties = new ObservableCollection<ConfigurationProperty>(),
                Order = DataLayout.Count
            };
            DataLayout.Add(item);
            SelectedItem = item;
            RaiseCanExecuteChanged();
        }

        private bool AddDirectHopCommandCanExecute()
        {
            return true; //DataLayout.Count < 2;
        }

        private void AddDirectHopCommandExecuted()
        {
            var item = new DirectHopItem
            {
                Order = DirectHops.Count
            };
            DirectHops.Add(item);
            SelectedDirectHop = item;
            RaiseCanExecuteChanged();
        }

        private bool AddVersionCommandCanExecute()
        {
            return SelectedItem != null && DataLayout.Count < 2 &&
                   DataLayout.All(t => t.Name != null && !t.Name.EndsWith("Version"));
        }

        private void AddVersionCommandExecuted()
        {
            var item = new ConfigurationItem
            {
                Name = SelectedItem.Name + "Version",
                Translation = SelectedItem.Translation + "Version",
                CanClone = true,
                CanDelete = true,
                CanEdit = true,
                Order = DataLayout.Count,
                Parent = SelectedItem.Name,
                Properties = new ObservableCollection<ConfigurationProperty>
                {
                    new ConfigurationProperty
                    {
                        Order = 0,
                        IsKey = true,
                        IsRequired = true,
                        Type = "int",
                        Name = "Version",
                        TranslationDe = "Version",
                        TranslationEn = "Version",
                        Length = "2"
                    },
                    new ConfigurationProperty
                    {
                        Order = 1,
                        Type = "bool",
                        Name = "IsActive",
                        TranslationDe = "Aktiv",
                        TranslationEn = "Active",
                        Length = "2"
                    },
                    new ConfigurationProperty
                    {
                        Order = 2,
                        Type = "string",
                        Name = "Description",
                        TranslationDe = "Beschreibung",
                        TranslationEn = "Description",
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
            int count = 0;
            foreach (ConfigurationItem item in DataLayout)
            {
                item.Order = count++;
            }
            RaiseCanExecuteChanged();
        }

        private bool DeleteItemCommandCanExecute()
        {
            return SelectedItem != null;
        }

        private void DeleteDirectHopCommandExecuted()
        {
            DirectHops.Remove(SelectedDirectHop);
            int count = 0;
            foreach (DirectHopItem item in DirectHops)
            {
                item.Order = count++;
            }
            RaiseCanExecuteChanged();
        }

        private bool DeleteDirectHopCommandCanExecute()
        {
            return SelectedDirectHop != null;
        }

        private void AddItemPropertyCommandExecuted()
        {
            var property = new ConfigurationProperty()
            {
                Order = SelectedItem.Properties.Count
            };
            SelectedItem.Properties.Add(property);
            SelectedPropertyItem = property;
            RaiseCanExecuteChanged();
        }

        private bool AddItemPropertyCommandCanExecute()
        {
            return SelectedItem != null;
        }

        private bool SortItemDownCommandCanExecute()
        {
            return SelectedItem != null &&
                   SelectedItem.Order < DataLayout.Max(t => t.Order);
        }

        private void SortItemDownCommandExecuted()
        {
            int order = SelectedItem.Order;
            ConfigurationItem item = DataLayout.First(t => t.Order == order + 1);
            item.Order = order;
            SelectedItem.Order++;
            ItemCollection.Refresh();
            RaiseCanExecuteChanged();
        }

        private bool SortItemUpCommandCanExecute()
        {
            return SelectedItem != null &&
                   SelectedItem.Order > DataLayout.Min(t => t.Order);
        }

        private void SortItemUpCommandExecuted()
        {
            int order = SelectedItem.Order;
            ConfigurationItem item = DataLayout.First(t => t.Order == order - 1);
            item.Order = order;
            SelectedItem.Order--;
            ItemCollection.Refresh();
            RaiseCanExecuteChanged();
        }
        private bool SortDirectHopDownCommandCanExecute()
        {
            return SelectedDirectHop != null &&
                   SelectedDirectHop.Order < DirectHops.Max(t => t.Order);
        }

        private void SortDirectHopDownCommandExecuted()
        {
            int order = SelectedDirectHop.Order;
            DirectHopItem directHop = DirectHops.First(t => t.Order == order + 1);
            directHop.Order = order;
            SelectedDirectHop.Order++;
            DirectHopCollection.Refresh();
            RaiseCanExecuteChanged();
        }

        private bool SortDirectHopUpCommandCanExecute()
        {
            return SelectedDirectHop != null &&
                   SelectedDirectHop.Order > DirectHops.Min(t => t.Order);
        }

        private void SortDirectHopUpCommandExecuted()
        {
            int order = SelectedDirectHop.Order;
	        DirectHopItem directHop = DirectHops.First(t => t.Order == order - 1);
            directHop.Order = order;
            SelectedDirectHop.Order--;
            DirectHopCollection.Refresh();
            RaiseCanExecuteChanged();
        }

        private bool SortItemPropertyDownCommandCanExecute()
        {
            return SelectedItem != null && SelectedItem.Properties != null && SelectedItem.Properties.Count > 0 &&
                   SelectedPropertyItem != null &&
                   SelectedPropertyItem.Order < SelectedItem.Properties.Max(t => t.Order);
        }

        private void SortItemPropertyDownCommandExecuted()
        {
            int order = SelectedPropertyItem.Order;
            ConfigurationProperty prop = SelectedItem.Properties.First(t => t.Order == order + 1);
            prop.Order = order;
            SelectedPropertyItem.Order++;
            PropertyCollection.Refresh();
            RaiseCanExecuteChanged();
        }

        private bool SortItemPropertyUpCommandCanExecute()
        {
            return SelectedItem != null && SelectedItem.Properties != null && SelectedItem.Properties.Count > 0 &&
                   SelectedPropertyItem != null &&
                   SelectedPropertyItem.Order > SelectedItem.Properties.Min(t => t.Order);
        }

        private void SortItemPropertyUpCommandExecuted()
        {
            int order = SelectedPropertyItem.Order;
            ConfigurationProperty prop = SelectedItem.Properties.First(t => t.Order == order -1);
            prop.Order = order;
            SelectedPropertyItem.Order--;
            PropertyCollection.Refresh();
            RaiseCanExecuteChanged();

        }

        private void DeleteItemPropertyCommandExecuted()
        {
            if (SelectedItem != null && SelectedPropertyItem != null)
            {
                SelectedItem.Properties.Remove(SelectedPropertyItem);
                int count = 0;
                foreach (ConfigurationProperty property in SelectedItem.Properties)
                {
                    property.Order = count++;
                }
            }
            RaiseCanExecuteChanged();
        }

        private bool DeleteItemPropertyCommandCanExecute()
        {
            return SelectedItem != null && SelectedPropertyItem != null;
        }

        private void StartExecuted()
        {
            GetTemplatesFromDisk();

            var configuration = GetConfiguration();
            var serverPart = new ServerPart(configuration);
            var gatewayPart = new GatewayPart(configuration);
            var businessServiceLocalPart = new BusinessServiceLocalPart(configuration);
            var repositoryPart = new RepositoryPart(configuration);
            var uiPart = new UiPart(configuration);
            var localizationPart = new LocalizationPart(configuration);

			//Server
	        serverPart.Create();

            // Gateway
            if (!IsUseBusinessServiceWithoutBo)
            {
                if (IsCreateDto) gatewayPart.CreateDto(true);

                if (IsCreateDtoFactory) gatewayPart.CreateDtoFactory();

                if (IsCreateGateway) gatewayPart.CreateGateway();
            }

            // Business Service
            if (IsCreateBusinessService)
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

            // Repository
            if (IsCreateDataItemFactory) repositoryPart.CreateDataItemFactory();

            if (IsCreateRepositoryDtoFactory) repositoryPart.CreateRepositoryDtoFactory();

            if (IsCreateDataItem) repositoryPart.CreateDataItem();

            if (IsCreateRepository) repositoryPart.CreateRepository();

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
            Configuration configuration = new Configuration
            {
                InputPath = _inputPath,
                OutputPath = _outputPath,
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
                DataLayout = _dataLayout,
                DirectHops = _directHops
			};

	        return configuration;
        }

        private void WriteConfiguration(Configuration configuration)
        {
			InputPath = configuration.InputPath;
	        OutputPath = configuration.OutputPath;
	        IsCreateDto = configuration.IsCreateDto;
	        IsCreateDtoFactory = configuration.IsCreateDtoFactory;
	        IsCreateGateway = configuration.IsCreateGateway;
	        IsCreateBusinessService = configuration.IsCreateBusinessService;
	        IsUseBusinessServiceWithoutBo = configuration.IsUseBusinessServiceWithoutBo;
	        IsCreateDataItem = configuration.IsCreateDataItem;
	        IsCreateDataItemFactory = configuration.IsCreateDataItemFactory;
	        IsCreateRepositoryDtoFactory = configuration.IsCreateRepositoryDtoFactory;
	        IsCreateRepository = configuration.IsCreateRepository;
	        IsCreateUi = configuration.IsCreateUi;
	        IsCreateUiFilter = configuration.IsCreateUiFilter;
	        Product = configuration.Product;
	        DialogName = configuration.DialogName;
	        DialogTranslationGerman = configuration.DialogTranslationGerman;
	        DialogTranslationEnglish = configuration.DialogTranslationEnglish;
	        ControllerHandle = configuration.ControllerHandle;
	        DataLayout = configuration.DataLayout;
	        DirectHops = configuration.DirectHops;
		}

		private void GetTemplatesFromDisk()
        {
            Helpers.Configuration = GetConfiguration();

            Helpers.FilterChildViewModelTemplate =
                File.ReadAllText(InputPath + @"UI\Regions\Filter\FilterViewModelChildTemplate.txt");
            Helpers.FilterPropertyTemplate = File.ReadAllText(InputPath + @"UI\Regions\Filter\FilterProperty.txt");
            Helpers.FilterComboBoxPropertyTemplate =
                File.ReadAllText(InputPath + @"UI\Regions\Filter\FilterComboBox.txt");
            Helpers.FilterTextBoxXamlTemplate =
                File.ReadAllText(InputPath + @"UI\Regions\Filter\FilterTextBoxXaml.txt");
            Helpers.FilterComboBoxXamlTemplate =
                File.ReadAllText(InputPath + @"UI\Regions\Filter\FilterComboBoxXaml.txt");
            Helpers.FilterCheckBoxXamlTemplate =
                File.ReadAllText(InputPath + @"UI\Regions\Filter\FilterCheckBoxXaml.txt");
            Helpers.FilterDateTimePickerXamlTemplate =
                File.ReadAllText(InputPath + @"UI\Regions\Filter\FilterDateTimePickerXaml.txt");
        }

        #endregion Methods

        #region Properties

        #region Commands

        public DelegateCommand StartCommand { get; set; }
        public DelegateCommand ImportSettingsCommand { get; set; }

        public DelegateCommand ExportSettingsCommand { get; set; }

        public DelegateCommand AddItemCommand { get; set; }
		public DelegateCommand AddDirectHopCommand { get; set; }

        public DelegateCommand SortItemUpCommand { get; set; }

        public DelegateCommand SortItemDownCommand { get; set; }


        public DelegateCommand AddVersionCommand { get; set; }

        public DelegateCommand DeleteItemCommand { get; set; }
		public DelegateCommand DeleteDirectHopCommand { get; set; }

        public DelegateCommand AddItemPropertyCommand { get; set; }

        public DelegateCommand SortItemPropertyUpCommand { get; set; }

        public DelegateCommand SortItemPropertyDownCommand { get; set; }

        public DelegateCommand DeleteItemPropertyCommand { get; set; }

        public DelegateCommand SortDirectHopUpCommand { get; set; }

        public DelegateCommand SortDirectHopDownCommand
        { get; set; }

        #endregion Commands

        public ConfigurationItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (SetProperty(ref _selectedItem, value))
                {
                    if (_selectedItem != null)
                    {
                        PropertyCollection = (CollectionView)CollectionViewSource.GetDefaultView(SelectedItem.Properties);
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

        public DirectHopItem SelectedDirectHop
        {
            get => _selectedDirectHop;
            set
            {
                if (SetProperty(ref _selectedDirectHop, value))
                {
                    if (_selectedDirectHop != null)
                    {
                        NavigateToDirectHopDetail();
                    }

                    RaiseCanExecuteChanged();
                }
            }
        }

        public ConfigurationProperty SelectedPropertyItem
        {
            get => _selectedPropertyItem;
            set
            {
                if (SetProperty(ref _selectedPropertyItem, value))
                {
                    if (_selectedPropertyItem != null)
                    {
                        //SelectedItem = null;
                        NavigateToDataItemPropertyDetail();
                    }

                    RaiseCanExecuteChanged();
                }
            }
        }

        #region Settings

        public bool IsCreateDto
        {
            get => _isCreateDto;
            set => SetProperty(ref _isCreateDto, value);
        }

        public bool IsCreateDtoFactory
        {
            get => _isCreateDtoFactory;
            set => SetProperty(ref _isCreateDtoFactory, value);
        }

        public bool IsCreateGateway
        {
            get => _isCreateGateway;
            set => SetProperty(ref _isCreateGateway, value);
        }

        public bool IsCreateBusinessService
        {
            get => _isCreateBusinessService;
            set => SetProperty(ref _isCreateBusinessService, value);
        }

        public bool IsUseBusinessServiceWithoutBo
        {
            get => _isUseBusinessServiceWithoutBo;
            set
            {
                if (SetProperty(ref _isUseBusinessServiceWithoutBo, value))
                    if (value)
                    {
                        IsCreateDto = false;
                        IsCreateDtoFactory = false;
                        IsCreateGateway = false;
                    }
            }
        }

        public bool IsCreateDataItem
        {
            get => _isCreateDataItem;
            set => SetProperty(ref _isCreateDataItem, value);
        }

        public bool IsCreateDataItemFactory
        {
            get => _isCreateDataItemFactory;
            set => SetProperty(ref _isCreateDataItemFactory, value);
        }


        public bool IsCreateRepositoryDtoFactory
        {
            get => _isCreateRepositoryDtoFactory;
            set => SetProperty(ref _isCreateRepositoryDtoFactory, value);
        }

        public bool IsCreateRepository
        {
            get => _isCreateRepository;
            set => SetProperty(ref _isCreateRepository, value);
        }

        public bool IsCreateUi
        {
            get => _isCreateUi;
            set => SetProperty(ref _isCreateUi, value);
        }

        public bool IsCreateUiFilter
        {
            get => _isCreateUiFilter;
            set => SetProperty(ref _isCreateUiFilter, value);
        }

        #endregion Settings

        public ObservableCollection<ConfigurationItem> DataLayout
        {
            get => _dataLayout;
            set => SetProperty(ref _dataLayout, value);
        }
        public ObservableCollection<DirectHopItem> DirectHops
        {
            get => _directHops;
            set => SetProperty(ref _directHops, value);
        }

        public string Product
        {
            get => _product;
            set => SetProperty(ref _product, value);
        }

        public string DialogName
        {
            get => _dialogName;
            set => SetProperty(ref _dialogName, value);
        }

        public string DialogTranslationGerman
        {
            get => _dialogTranslationGerman;
            set => SetProperty(ref _dialogTranslationGerman, value);
        }

        public string DialogTranslationEnglish
        {
            get => _dialogTranslationEnglish;
            set => SetProperty(ref _dialogTranslationEnglish, value);
        }

        public string ControllerHandle
        {
            get => _controllerHandle;
            set => SetProperty(ref _controllerHandle, value);
        }

        public string InputPath
        {
            get => _inputPath;
            set => SetProperty(ref _inputPath, value);
        }

        public string OutputPath
        {
            get => _outputPath;
            set => SetProperty(ref _outputPath, value);
        }

        #endregion Properties


    }
}
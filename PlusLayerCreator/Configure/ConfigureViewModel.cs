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
using PlusLayerCreator.Items;
using Prism.Commands;
using Prism.Mvvm;

namespace PlusLayerCreator.Configure
{
	public class ConfigureViewModel : BindableBase
	{
		#region Members

		private PlusDataItem _selectedItem;
		private PlusDataItemProperty _selectedPropertyItem;
		private TemplateMode _template;
		private ObservableCollection<PlusDataItem> _dataLayout;
		
		#region Settings
		
		private string _product;
		private string _dialogName;
		private string _dialogTranslationEnglish;
		private string _dialogTranslationGerman;
		
		private string _inputhPath = @"C:\Projects\LayerGenerator\Templates\";
		private string _commonPath = @"Common\";
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

		public ConfigureViewModel()
		{
			_dataLayout = new ObservableCollection<PlusDataItem>();

			//GenerateTempData();

			StartCommand = new DelegateCommand(StartExecuted);
			AddItemCommand = new DelegateCommand(AddItemCommandExecuted);
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
					if (property.PropertyType == typeof(IList<PlusDataItem>))
					{
						IList<PlusDataItem> dataItems =
							property.GetValue(configuration) as IList<PlusDataItem>;
						DataLayout.Clear();
						if (dataItems != null)
						{
							foreach (PlusDataItem plusDataItem in dataItems)
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
					}
				}
			}
		}

		#endregion Import/Export

		private void AddItemCommandExecuted()
		{
			DataLayout.Add(new PlusDataItem(){Properties = new ObservableCollection<PlusDataItemProperty>()});
		}

		private void DeleteItemCommandExecuted()
		{
			DataLayout.Remove(SelectedItem);
		}

		private bool DeleteItemCommandCanExecute()
		{
			return SelectedItem != null;
		}

		private void AddItemPropertyCommandExecuted()
		{
			SelectedItem.Properties.Add(new PlusDataItemProperty());
		}

		private bool AddItemPropertyCommandCanExecute()
		{
			return SelectedItem != null;
		}

		private void DeleteItemPropertyCommandExecuted()
		{
			if (SelectedPropertyItem != null && SelectedItem != null)
			{
				SelectedItem.Properties.Remove(SelectedPropertyItem);
			}
		}

		private bool DeleteItemPropertyCommandCanExecute()
		{
			return SelectedPropertyItem != null;
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
				uiPart.CreateUiToolbar();
				uiPart.CreateUi();
				uiPart.CreateUiFilter();
			}

			localizationPart.CreateLocalization();

			MessageBox.Show("Done", "Info", MessageBoxButton.OK);
		}

		#endregion Commands

		#region Methods

		private Configuration GetConfiguration()
		{
			return new Configuration()
			{
				Template = Template,
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
				DataLayout = _dataLayout.ToList()
			};
		}
		
		private void GetTemplatesFromDisk()
		{
			Helpers.Product = Product;
			Helpers.DialogName = DialogName;

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

		public PlusDataItem SelectedItem
		{
			get
			{
				return _selectedItem;
			}
			set
			{
				if (SetProperty(ref _selectedItem, value))
				{
					RaiseCanExecuteChanged();
				}
			}
		}

		public PlusDataItemProperty SelectedPropertyItem
		{
			get
			{
				return _selectedPropertyItem;
			}
			set
			{
				if (SetProperty(ref _selectedPropertyItem, value))
				{
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

		public TemplateMode Template
		{
			get
			{
				return _template;
			}
			set
			{
				SetProperty(ref _template, value);
			}
		}

		public IEnumerable<TemplateMode> TemplateModes
		{
			get
			{
				return Enum.GetValues(typeof(TemplateMode))
					.Cast<TemplateMode>();
			}
		}

		public ObservableCollection<PlusDataItem> DataLayout
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
	}
}

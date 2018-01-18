using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using Microsoft.Win32;
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
		private TemplateMode _selectedTemplateMode;
		private ObservableCollection<PlusDataItem> _dataLayout;
		
		#region Settings
		
		private string _product;
		//private string _item;
		private string _dialogName;
		private string _dialogTranslationEN;
		private string _dialogTranslationDE;
		private string _templateDirectory;

		private string _inputhPath = @"C:\Projects\LayerGenerator\Templates\";
		private string _commonPath = @"Common\";
		private string _outputPath = @"C:\Output\";

		private bool _isCreateDto = true;
		private bool _isCreateDtoFactory = true;
		private bool _isCreateGateway = true;
		private bool _isCreateBusinessService = true;
		private bool _isUseBusinessServiceWithoutBO;
		private bool _isCreateDataItem = true;
		private bool _isCreateDataItemFactory = true;
		private bool _isCreateRepositoryDtoFactory = true;
		private bool _isCreateRepository = true;
		private bool _isCreateUi = true;
		private bool _isCreateUiFilter = true;

		#endregion Settings

		#region Template

		private string _keyReadOnlyTemplate =
			" IsReadOnly=\"{Binding IsNewItem, Converter={StaticResource InvertBoolConverter}}\"";

		private string _readOnlyTemplate = " IsReadOnly=\"True\"";

		private string _isNumericTemplate = " IsNumeric=\"True\"";

		private readonly string _masterGridTemplate;
		private readonly string _filterChildViewModelTemplate;
		private readonly string _filterPropertyTemplate;
		private readonly string _filterComboBoxPropertyTemplate;
		private readonly string _filterTextBoxXamlTemplate;
		private readonly string _filterComboBoxXamlTemplate;
		private readonly string _filterCheckBoxXamlTemplate;
		private readonly string _filterDateTimePickerXamlTemplate;

		private readonly string _createGatewayDtoTemplateUpperPart;
		private readonly string _createGatewayDtoTemplateLowerPart;

		private readonly string _createRepositoryDtoTemplateUpperPart;
		private readonly string _createRepositoryDtoTemplateLowerPart;
		private readonly string _createDataItemTemplateUpperPart;
		private readonly string _createDataItemTemplateLowerPart;

		#endregion Template

		#endregion Members

		#region Construction

		public ConfigureViewModel()
		{
			_dataLayout = new ObservableCollection<PlusDataItem>();

			//GenerateTempData();

			_masterGridTemplate = File.ReadAllText(InputPath + _commonPath + @"UI\Regions\Master\MasterGrid.txt");
			_filterChildViewModelTemplate = File.ReadAllText(InputPath + _commonPath + @"UI\Regions\Filter\FilterViewModelChildTemplate.txt");
			_filterPropertyTemplate = File.ReadAllText(InputPath + _commonPath + @"UI\Regions\Filter\FilterProperty.txt");
			_filterComboBoxPropertyTemplate = File.ReadAllText(InputPath + _commonPath + @"UI\Regions\Filter\FilterComboBox.txt");
			_filterTextBoxXamlTemplate = File.ReadAllText(InputPath + _commonPath + @"UI\Regions\Filter\FilterTextBoxXaml.txt");
			_filterComboBoxXamlTemplate = File.ReadAllText(InputPath + _commonPath + @"UI\Regions\Filter\FilterComboBoxXaml.txt");
			_filterCheckBoxXamlTemplate = File.ReadAllText(InputPath + _commonPath + @"UI\Regions\Filter\FilterCheckBoxXaml.txt");
			_filterDateTimePickerXamlTemplate = File.ReadAllText(InputPath + _commonPath + @"UI\Regions\Filter\FilterDateTimePickerXaml.txt");

			_createDataItemTemplateUpperPart = File.ReadAllText(InputPath + _commonPath + @"Repository\CreateDataItemUpperPart.txt");
			_createDataItemTemplateLowerPart = File.ReadAllText(InputPath + _commonPath + @"Repository\CreateDataItemLowerPart.txt");
			_createRepositoryDtoTemplateUpperPart = File.ReadAllText(InputPath + _commonPath + @"Repository\CreateDtoUpperPart.txt");
			_createRepositoryDtoTemplateLowerPart = File.ReadAllText(InputPath + _commonPath + @"Repository\CreateDtoLowerPart.txt");

			_createGatewayDtoTemplateUpperPart = File.ReadAllText(InputPath + _commonPath + @"Gateway\CreateDtoUpperPart.txt");
			_createGatewayDtoTemplateLowerPart = File.ReadAllText(InputPath + _commonPath + @"Gateway\CreateDtoLowerPart.txt");

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

		#region Import/Export

		private void ExportSettingsExecuted()
		{
			Configuration configuration = new Configuration()
			{
				IsCreateDto = _isCreateDto,
				IsCreateDtoFactory = _isCreateDtoFactory,
				IsCreateGateway = _isCreateGateway,
				IsCreateBusinessService = _isCreateBusinessService,
				IsUseBusinessServiceWithoutBO = _isUseBusinessServiceWithoutBO,
				IsCreateDataItem = _isCreateDataItem,
				IsCreateDataItemFactory = _isCreateDataItemFactory,
				IsCreateRepositoryDtoFactory = _isCreateRepositoryDtoFactory,
				IsCreateRepository = _isCreateRepository,
				IsCreateUi = _isCreateUi,
				IsCreateUiFilter = _isCreateUiFilter,
				Product = _product,
				DialogName = _dialogName,
				DialogTranslationDE = _dialogTranslationDE,
				DialogTranslationEN = _dialogTranslationEN,
				DataLayout = _dataLayout.ToList()
			};

			SaveFileDialog dialog = new SaveFileDialog();
			dialog.Title = "Export settings";
			dialog.Filter = "Config files (*.cfg)|*.cfg";
			dialog.ShowDialog();

			if (dialog.FileName != string.Empty)
			{
				Helpers.ExportSettingsToFile(dialog.FileName, configuration);
			}
		}

		private void ImportSettingsExecuted()
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Title = "Import settings";
			dialog.Filter = "Config files (*.cfg)|*.cfg";
			dialog.ShowDialog();

			if (dialog.FileName != string.Empty)
			{
				Configuration configuration = Helpers.ImportSettingsFromFile(dialog.FileName);
				PropertyInfo[] properties = typeof(Configuration).GetProperties();
				foreach (var property in properties)
				{
					if (property.PropertyType == typeof(IList<PlusDataItem>))
					{
						IList<PlusDataItem> dataItems =
							property.GetValue(configuration) as IList<PlusDataItem>;
						DataLayout.Clear();
						foreach (PlusDataItem plusDataItem in dataItems)
						{
							DataLayout.Add(plusDataItem);
						}
					}
					else
					{
						PropertyInfo propertyInfo = this.GetType().GetProperty(property.Name);
						propertyInfo.SetValue(this, property.GetValue(configuration));
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
			_templateDirectory = InputPath + SelectedTemplateMode + "\\";

			// Gateway
			if (!IsUseBusinessServiceWithoutBO)
			{
				if (IsCreateDto)
				{
					CreateDto();
				}

				if (IsCreateDtoFactory)
				{
					CreateDtoFactory();
				}

				if (IsCreateGateway)
				{
					CreateGateway();
				}
			}


			// Business Service
			if (IsCreateBusinessService)
			{
				foreach (PlusDataItem dataItem in DataLayout)
				{
					CreateFile(_templateDirectory + @"Service\Contracts\IServiceTemplate.cs", OutputPath + @"Service\Contracts\I" + Product + dataItem.Name + "Service.cs", null, dataItem.Name);
					if (IsUseBusinessServiceWithoutBO)
					{
						CreateDto();
						CreateTandem();
						CreateFile(_templateDirectory + @"Service\ServiceNoBOTemplate.cs", OutputPath + @"Service\" + Product + dataItem.Name + "Service.cs", null, dataItem.Name);
					}
					else
					{
						CreateFile(_templateDirectory + @"Service\ServiceTemplate.cs", OutputPath + @"Service\" + Product + dataItem.Name + "Service.cs", null, dataItem.Name);
					}	
				}
			}


			// Repository
			if (IsCreateDataItemFactory)
			{
				CreateDataItemFactory();
			}

			if (IsCreateRepositoryDtoFactory)
			{
				CreateRepositoryDtoFactory();
			}

			if (IsCreateDataItem)
			{
				CreateDataItem();
			}
			
			if (IsCreateRepository)
			{
				CreateRepository();
			}

			// UI
			if (IsCreateUi)
			{
				CreateUi();
				CreateUiFilter();
			}

			CreateLocalization();

			MessageBox.Show("Done", "Info", MessageBoxButton.OK);
		}

		#endregion Commands

		#region Methods

		private void RaiseCanExecuteChanged()
		{
			DeleteItemCommand.RaiseCanExecuteChanged();
			AddItemPropertyCommand.RaiseCanExecuteChanged();
			DeleteItemPropertyCommand.RaiseCanExecuteChanged();
		}

		private void CreateTandem()
		{
			string converterMessageToBoContent = string.Empty;
			string converterBoToMessageContent = string.Empty;
			foreach (PlusDataItem dataItem in DataLayout)
			{
				foreach (PlusDataItemProperty plusDataObject in dataItem.Properties)
				{
					converterMessageToBoContent += "serviceMessage." + plusDataObject.Name + "(checkpointReference." +
					                               plusDataObject.Name + ", i);\r\n";
					converterBoToMessageContent += plusDataObject.Name + " = serviceMessage." + plusDataObject.Name + "(i),\r\n";
				}

				CreateFile(_templateDirectory + @"Service\Tandem\Converter.cs", OutputPath + @"Service\Tandem\" + Product + dataItem.Name + "Converter.cs", new[] { converterMessageToBoContent, converterBoToMessageContent }, dataItem.Name);
				CreateFile(_templateDirectory + @"Service\Tandem\ServerMapping.cs", OutputPath + @"Service\Tandem\" + Product + dataItem.Name + "ServerMapping.cs", null, dataItem.Name);
			}
		}

		#region Gateway

		private void CreateGateway()
		{
			Random rnd = new Random();
			string key = string.Empty;
			string identifier = string.Empty;
			string readOnlyMappingDto = string.Empty;
			string readOnlyMappingBo = string.Empty;
			string mock = string.Empty;

			foreach (PlusDataItem dataItem in DataLayout)
			{
				foreach (PlusDataItemProperty plusDataObject in dataItem.Properties.Where(t => t.IsKey))
				{
					key += Helpers.ToPascalCase(dataItem.Name) + "." + plusDataObject.Name + ", ";
					identifier += "x.Key." + plusDataObject.Name + ".Equals(dto." + plusDataObject.Name + ") &&";
					if (plusDataObject.Type == "string")
					{
						mock += Helpers.ToPascalCase(dataItem.Name) + "." + plusDataObject.Name + " = " + plusDataObject.Name + " + i,\r\n";
					}
					if (plusDataObject.Type == "int")
					{
						mock += Helpers.ToPascalCase(dataItem.Name) + "." + plusDataObject.Name + " = " +
						        rnd.Next(1, Helpers.GetMaxValue(plusDataObject.Length)) + ",\r\n";
					}
					if (plusDataObject.Type == "bool")
					{
						mock += Helpers.ToPascalCase(dataItem.Name) + "." + plusDataObject.Name + " = true,\r\n";
					}
					if (plusDataObject.Type == "DateTime")
					{
						mock += Helpers.ToPascalCase(dataItem.Name) + "." + plusDataObject.Name + " = new DateTime(2016, 12, 25),\r\n";
					}
					if (plusDataObject.IsReadOnly)
					{
						readOnlyMappingDto +=
							Helpers.ToPascalCase(dataItem.Name) + "Dto." + plusDataObject.Name + " = " + Helpers.ToPascalCase(dataItem.Name) + "." +
							plusDataObject.Name + ";\r\n";
						readOnlyMappingBo +=
							Helpers.ToPascalCase(dataItem.Name) + "." + plusDataObject.Name + " = " + Helpers.ToPascalCase(dataItem.Name) + "Dto." +
							plusDataObject.Name + ";\r\n";
					}
				}

				if (key.Length > 0)
				{
					key = key.Substring(0, key.Length - 2);
				}

				if (identifier.Length > 0)
				{
					identifier = identifier.Substring(0, identifier.Length - 3);
				}

				CreateFile(GetInputhPath(_templateDirectory + @"Gateway\Contracts\IGatewayTemplate.cs"),
					OutputPath + @"Gateway\Contracts\I" + Product + dataItem.Name + "Gateway.cs", null, dataItem.Name);
				CreateFile(GetInputhPath(_templateDirectory + @"Gateway\GatewayTemplate.cs"),
					OutputPath + @"Gateway\" + Product + dataItem.Name + "Gateway.cs",
					new[] { key, identifier, readOnlyMappingDto, readOnlyMappingBo }, dataItem.Name);
				CreateFile(GetInputhPath(_templateDirectory + @"Gateway\GatewayMockTemplate.cs"),
					OutputPath + @"Gateway\" + Product + dataItem.Name + "GatewayMock.cs", new[] { mock }, dataItem.Name);
			}
		}

		private void CreateDto()
		{
			string dtoContent = string.Empty;
			foreach (PlusDataItem dataItem in DataLayout)
			{
				foreach (PlusDataItemProperty plusDataObject in dataItem.Properties)
				{
					dtoContent += "public " + plusDataObject.Type + " " + plusDataObject.Name + " {get; set;}\r\n\r\n";
				}

				if (IsUseBusinessServiceWithoutBO)
				{
					CreateFile(GetInputhPath(_templateDirectory + @"Service\Dtos\DtoTemplate.cs"),
						OutputPath + @"Service\Dtos\" + Product + dataItem.Name + ".cs", new[] { dtoContent }, dataItem.Name);
				}
				else
				{
					CreateFile(GetInputhPath(_templateDirectory + @"Gateway\Dtos\DtoTemplate.cs"),
						OutputPath + @"Gateway\Dtos\" + Product + dataItem.Name + ".cs", new[] { dtoContent }, dataItem.Name);
				}
			}
		}

		private void CreateDtoFactory()
		{
			string factoryContent = string.Empty;
			foreach (PlusDataItem dataItem in DataLayout)
			{
				factoryContent += DoReplaces(_createGatewayDtoTemplateUpperPart + "\r\n", dataItem.Name);
				foreach (PlusDataItemProperty plusDataObject in dataItem.Properties)
				{
					if (plusDataObject.IsKey)
					{
						factoryContent += plusDataObject.Name + " = bo.Key." + plusDataObject.Name + ",\r\n";
					}
					else
					{
						factoryContent += plusDataObject.Name + " = bo." + plusDataObject.Name + ",\r\n";
					}
				}
				factoryContent += DoReplaces(_createGatewayDtoTemplateLowerPart, dataItem.Name);
			}

			string[] contentsDtoFactory = {
				factoryContent
			};

			CreateFile(GetInputhPath(_templateDirectory + @"Gateway\DtoFactoryTemplate.cs"), OutputPath + @"Gateway\" + Product + "DtoFactory.cs", contentsDtoFactory);
		}

		#endregion Gateway

		#region Localization

		private void CreateLocalization()
		{
			string lblEn = string.Empty;
			string lblEnOut = string.Empty;
			string lblDe = string.Empty;
			string lblDeOut = string.Empty;
			string strEn = string.Empty;
			string strEnOut = string.Empty;
			string strDe = string.Empty;
			string strDeOut = string.Empty;

			lblEn += Product + DialogName + "_lblCaption=" + Product + " - " + DialogTranslationEN + "\r\n";
			lblEnOut += Product + DialogName + "_lblCaption=@@" + Product + " - " + DialogTranslationEN + "\r\n";
			lblDe += Product + DialogName + "_lblCaption=" + Product + " - " + DialogTranslationDE + "\r\n";
			lblEnOut += Product + DialogName + "_lblCaption=@@" + Product + " - " + DialogTranslationDE + "\r\n";

			foreach (PlusDataItem dataItem in DataLayout)
			{
				lblEn += Product + DialogName + "_lbl" + dataItem.Name + "s=" + dataItem.Name + "s\r\n";
				lblEnOut += Product + DialogName + "_lbl" + dataItem.Name + "s=@@" + dataItem.Name + "s\r\n";
				lblDe += Product + DialogName + "_lbl" + dataItem.Name + "s=" + dataItem.Translation + "en\r\n";
				lblDeOut += Product + DialogName + "_lbl" + dataItem.Name + "s=@@" + dataItem.Translation + "en\r\n";

				foreach (PlusDataItemProperty plusDataObject in dataItem.Properties)
				{
					lblEn += Product + DialogName + "_lbl" + dataItem.Name + plusDataObject.Name + "=" + plusDataObject.Name + "\r\n";
					lblEnOut += Product + DialogName + "_lbl" + dataItem.Name + plusDataObject.Name + "=@@" + plusDataObject.Name + "\r\n";
					lblDe += Product + DialogName + "_lbl" + dataItem.Name + plusDataObject.Name + "=" + plusDataObject.Translation + "\r\n";
					lblDeOut += Product + DialogName + "_lbl" + dataItem.Name + plusDataObject.Name + "=@@" + plusDataObject.Translation + "\r\n";
				}
			}

			string[] languages = { "en", "fr", "de", "hu", "pt", "zh-CHS" };

			foreach (string language in languages)
			{
				string fileContent = File.ReadAllText(InputPath +_commonPath + @"Localization\localization.txt");
				fileContent = DoReplaces(fileContent, string.Empty);
				fileContent = fileContent.Replace("$language$", language);
				fileContent = fileContent.Replace("$date$", DateTime.Now.ToShortDateString());
				fileContent = fileContent.Replace("$Product$", Product);
				if (language == "en")
				{
					fileContent = fileContent.Replace("$KeysLabel$", lblEn);
					fileContent = fileContent.Replace("$KeysString$", strEn);
				}
				else if (language == "de")
				{
					fileContent = fileContent.Replace("$KeysLabel$", lblDe);
					fileContent = fileContent.Replace("$KeysString$", strDe);
				}
				else if (language == "fr" || language == "hu")
				{
					fileContent = fileContent.Replace("$KeysLabel$", lblDeOut);
					fileContent = fileContent.Replace("$KeysString$", strDeOut);
				}
				else
				{
					fileContent = fileContent.Replace("$KeysLabel$", lblEnOut);
					fileContent = fileContent.Replace("$KeysString$", strEnOut);
				}
				string languageExtension = language == "de" ? string.Empty : "." + language;
				FileInfo fileInfo = new FileInfo(OutputPath + @"Localization\Dcx.Plus.Localization.Modules." + Product + ".Nls" + languageExtension + ".txt");
				fileInfo.Directory.Create();
				File.WriteAllText(fileInfo.FullName, fileContent, Encoding.UTF8);				
			}

		}

		#endregion Localization

		#region UI

		private void CreateUiFilter()
		{
			string filterXamlContent = string.Empty;
			string filterMembersContent = string.Empty;
			string filterPredicatesContent = string.Empty;
			string filterPredicateResetContent = string.Empty;
			string filterMultiSelectorsInitializeContent = string.Empty;
			string filterPropertiesContent = string.Empty;


			//filterMembersContent
			foreach (PlusDataItem dataItem in DataLayout)
			{
				foreach (PlusDataItemProperty plusDataObject in dataItem.Properties)
				{
					if (plusDataObject.IsFilterProperty)
					{
						string filterSuffix = plusDataObject.Type == "string" ? string.Empty : ".ToString()";
						if (plusDataObject.Type == "string" || plusDataObject.Type == "int")
						{
							if (plusDataObject.FilterPropertyType == "TextBox")
							{
								filterMembersContent += "private string _" + Helpers.ToPascalCase(plusDataObject.Name) + ";\r\n";
								filterPropertiesContent += _filterPropertyTemplate.Replace(plusDataObject.Type, "string");
								filterPredicateResetContent += plusDataObject.Name + " = string.Empty;";
								filterPredicatesContent += ".StartsWith(x => x." + plusDataObject.Name + filterSuffix + ", x => x." +
								                           plusDataObject.Name + ")\r\n";
								filterXamlContent += DoReplaces(_filterTextBoxXamlTemplate);
							}

							if (plusDataObject.FilterPropertyType == "ComboBox")
							{
								filterMembersContent += "private MultiValueSelector<string> _" +
								                        Helpers.ToPascalCase(plusDataObject.Name) + "MultiSelector;\r\n";
								filterPropertiesContent += _filterComboBoxPropertyTemplate;
								filterPredicatesContent += ".IsContainedInList(x => x." + plusDataObject.Name + filterSuffix + ", x => x." +
								                           plusDataObject.Name + "MultiSelector.SelectedValues)\r\n";
								filterMultiSelectorsInitializeContent +=
									plusDataObject.Name + "MultiSelector = new MultiValueSelector<string>(SourceCollection.Select(x => x." +
									plusDataObject.Name + ".ToString()).Distinct(), RefreshCollectionView, AllValue);";
								filterXamlContent += DoReplaces(_filterComboBoxXamlTemplate);
							}
						}

						if (plusDataObject.Type == "bool")
						{
							filterMembersContent += "private " + plusDataObject.Type + "? _" + Helpers.ToPascalCase(plusDataObject.Name) +
							                        ";\r\n";
							filterPropertiesContent += _filterPropertyTemplate;
							filterPredicateResetContent += plusDataObject.Name + " = null;";
							filterPredicatesContent +=
								".IsEqual(x => x." + plusDataObject.Name + ", x => x." + plusDataObject.Name + ")\r\n";
							filterXamlContent += DoReplaces(_filterCheckBoxXamlTemplate);
						}

						if (plusDataObject.Type == "DateTime")
						{
							filterMembersContent += "private " + plusDataObject.Type + "? _" + Helpers.ToPascalCase(plusDataObject.Name) +
							                        ";\r\n";
							filterPropertiesContent += _filterPropertyTemplate;
							filterPredicateResetContent += plusDataObject.Name + " = null;";
							filterPredicatesContent +=
								".IsEqual(x => x." + plusDataObject.Name + ", x => x." + plusDataObject.Name + ")\r\n";
							filterXamlContent += DoReplaces(_filterDateTimePickerXamlTemplate);
						}

						filterMembersContent = filterMembersContent.Replace("$Name$", plusDataObject.Name)
							.Replace("$name$", Helpers.ToPascalCase(plusDataObject.Name)).Replace("$Type$", plusDataObject.Type);
						filterPropertiesContent = filterPropertiesContent.Replace("$Name$", plusDataObject.Name)
							.Replace("$name$", Helpers.ToPascalCase(plusDataObject.Name)).Replace("$Type$", plusDataObject.Type);
						filterPredicateResetContent = filterPredicateResetContent.Replace("$Name$", plusDataObject.Name)
							.Replace("$name$", Helpers.ToPascalCase(plusDataObject.Name)).Replace("$Type$", plusDataObject.Type);
						filterPredicatesContent = filterPredicatesContent.Replace("$Name$", plusDataObject.Name)
							.Replace("$name$", Helpers.ToPascalCase(plusDataObject.Name)).Replace("$Type$", plusDataObject.Type);
						filterXamlContent = filterXamlContent.Replace("$Name$", plusDataObject.Name)
							.Replace("$name$", Helpers.ToPascalCase(plusDataObject.Name)).Replace("$Type$", plusDataObject.Type);
					}
				}

				CreateFile(GetInputhPath(_templateDirectory) + @"UI\Filter\FilterViewTemplate.xaml",
					OutputPath + @"UI\Filter\FilterView.xaml", new[] {filterXamlContent});
				CreateFile(GetInputhPath(_templateDirectory) + @"UI\Filter\FilterViewTemplate.xaml.cs",
					OutputPath + @"UI\Filter\FilterView.xaml.cs");

				string fileContent = File.ReadAllText(InputPath + _commonPath + @"UI\Regions\Filter\FilterViewModelTemplate.cs");
				fileContent = DoReplaces(fileContent);
				fileContent = fileContent.Replace("$filterMembers$", filterMembersContent);
				fileContent = fileContent.Replace("$filterPredicates$", filterPredicatesContent);
				fileContent = fileContent.Replace("$filterPredicateReset$", filterPredicateResetContent);
				fileContent = fileContent.Replace("$filterMultiSelectorsInitialize$", filterMultiSelectorsInitializeContent);
				fileContent = fileContent.Replace("$filterProperties$", filterPropertiesContent);
			}
		}

		/// <summary>
		/// Creates the UI.
		/// </summary>
		private void CreateUi()
		{
			string masterViewContent = string.Empty;

			foreach (PlusDataItem dataItem in DataLayout)
			{
				//Detail
				string detailViewContent = string.Empty;
				string yesNoConverterString = ", Converter={StaticResource BoolToLocalizedYesNoConverterConverter}";

				detailViewContent += "<plus:PlusGroupBox Header=\"" + GetLocalizedString(dataItem.Name) + "s\">";
				detailViewContent += "    <StackPanel>";

				foreach (PlusDataItemProperty plusDataObject in dataItem.Properties)
				{
					detailViewContent += "        <plus:PlusFormRow Label=\"" + GetLocalizedString(plusDataObject.Name) + "\">\r\n";

					if (SelectedTemplateMode == TemplateMode.OneDataItemReadOnly)
					{
						if (plusDataObject.Type == "bool")
						{
							detailViewContent += "            <plus:PlusLabel Content=\"{Binding DataItem." + plusDataObject.Name +
							                     yesNoConverterString + "}\" />\r\n";
						}
						else
						{
							detailViewContent += "            <plus:PlusLabel Content=\"{Binding DataItem." + plusDataObject.Name +
							                     "}\" />\r\n";
						}
					}
					else
					{
						string addintionalInformation = string.Empty;
						if (plusDataObject.IsKey)
						{
							addintionalInformation += _keyReadOnlyTemplate;
						}
						else if (plusDataObject.IsReadOnly)
						{
							addintionalInformation += _readOnlyTemplate;
						}

						if (plusDataObject.Type == "bool")
						{
							detailViewContent += "            <plus:PlusCheckbox " + addintionalInformation +
							                     " IsChecked=\"{Binding DataItem." + plusDataObject.Name + "}\" />\r\n";
						}
						else
						{
							if (plusDataObject.Length != string.Empty)
							{
								addintionalInformation += " MaxLength=\"" + plusDataObject.Length + "\"";
							}
							if (plusDataObject.Type == "int")
							{
								addintionalInformation += _isNumericTemplate;
							}

							detailViewContent += "            <plus:PlusTextBox" + addintionalInformation + " Text=\"{Binding DataItem." +
							                     plusDataObject.Name +
							                     ", UpdateSourceTrigger=PropertyChanged}\" />\r\n";
						}
					}

					detailViewContent += "        </plus:PlusFormRow>\r\n";
				}

				detailViewContent +=
					"				<plus:PlusFormRow Label=\"{localization:Localize Key=Global_lblLupdTimestamp, Source=GlobalLocalizer}\">";
				detailViewContent += "				    <plus:PlusLabel Content=\"{Binding DataItem.LupdTimestamp}\" />";
				detailViewContent += "				</plus:PlusFormRow>";
				detailViewContent +=
					"				<plus:PlusFormRow Label=\"{localization:Localize Key=Global_lblLupdUser, Source=GlobalLocalizer}\">";
				detailViewContent += "				    <plus:PlusLabel Content=\"{Binding DataItem.LupdUser}\" />";
				detailViewContent += "				</plus:PlusFormRow>";

				detailViewContent += "    </StackPanel>";
				detailViewContent += "</plus:PlusGroupBox>";

				CreateFile(GetInputhPath(_templateDirectory + @"UI\Detail\DetailViewModelTemplate.cs"),
					OutputPath + @"UI\Detail\" + dataItem.Name + "DetailViewModel.cs", null, dataItem.Name);
				CreateFile(GetInputhPath(_templateDirectory + @"UI\Detail\DetailViewTemplate.xaml"),
					OutputPath + @"UI\Detail\" + dataItem.Name + "DetailView.xaml", new[] { detailViewContent }, dataItem.Name);
				CreateFile(GetInputhPath(_templateDirectory + @"UI\Detail\DetailViewTemplate.xaml.cs"),
					OutputPath + @"UI\Detail\" + dataItem.Name + "DetailView.xaml.cs", null, dataItem.Name);


				//Master
				masterViewContent += _masterGridTemplate;
				foreach (PlusDataItemProperty plusDataObject in dataItem.Properties)
				{
					if (plusDataObject.Type == "bool")
					{
						masterViewContent = masterViewContent.Replace("$specialContent$", "                <plus:PlusGridViewCheckColumn Width=\"*\" DataMemberBinding=\"{Binding " +
						                     plusDataObject.Name + "}\" Header=\"" + GetLocalizedString(plusDataObject.Name) + "\"/>\r\n");
					}
					else
					{
						masterViewContent = masterViewContent.Replace("$specialContent$", "                <plus:PlusGridViewTextColumnMinimal Width=\"*\" DataMemberBinding=\"{Binding " +
							plusDataObject.Name + "}\" Header=\"" + GetLocalizedString(plusDataObject.Name) + "\"/>\r\n");
					}
				}
			}
			CreateFile(GetInputhPath(_templateDirectory + @"UI\Master\MasterViewModelTemplate.cs"), OutputPath + @"UI\Master\" + DialogName + @"MasterViewModel.cs");
			CreateFile(GetInputhPath(InputPath + _commonPath + @"UI\Regions\Master\MasterViewTemplate.xaml"), OutputPath + @"UI\Master\" + DialogName + @"MasterView.xaml", new[] { masterViewContent });
			CreateFile(GetInputhPath(InputPath + _commonPath + @"UI\Regions\Master\MasterViewTemplate.xaml.cs"), OutputPath + @"UI\Master\" + DialogName + @"MasterView.xaml.cs");
		}

		#endregion UI

		#region Repository

		private void CreateRepository()
		{
			string content = string.Empty;
			string identifier = string.Empty;
			string readOnly = string.Empty;

			DirectoryInfo di = new DirectoryInfo(_templateDirectory + @"Repository");
			foreach (FileInfo fileInfo in di.GetFiles().Where(t => t.Name.ToLower().EndsWith("part.txt")))
			{
				foreach (PlusDataItem dataItem in DataLayout)
				{
					string templateText = DoReplaces(File.ReadAllText(fileInfo.Directory + "\\" + fileInfo.Name), dataItem.Name);
					if (fileInfo.Name.ToLower().Contains("save"))
					{
						foreach (PlusDataItemProperty plusDataObject in dataItem.Properties.Where(t => t.IsKey))
						{
							identifier += "x." + plusDataObject.Name + ".Equals(dto." + plusDataObject.Name + ") &&";
							if (plusDataObject.IsReadOnly)
							{
								readOnly +=
									Helpers.ToPascalCase(dataItem.Name) + "Dto." + plusDataObject.Name + " = " + Helpers.ToPascalCase(dataItem.Name) + "." +
									plusDataObject.Name + ";\r\n";
							}
						}

						content = content.Replace("$specialContent$", identifier);
						content = content.Replace("$specialContent2$", readOnly);
					}

					content += templateText + "\r\n\r\n";
				}

				content = content.Substring(0, content.Length - 8);
			}

			CreateFile(GetInputhPath(_templateDirectory + @"Repository\Contracts\IRepositoryTemplate.cs"), OutputPath + @"Repository\Contracts\I" + Product + DialogName + "Repository.cs", new[] { content });
			CreateFile(GetInputhPath(_templateDirectory + @"Repository\RepositoryTemplate.cs"), OutputPath + @"Repository\" + Product + DialogName + "Repository.cs", new[] { content });
		}

		private void CreateDataItem()
		{
			string dataItemContent = string.Empty;
			foreach (PlusDataItem dataItem in DataLayout)
			{
				foreach (PlusDataItemProperty plusDataObject in dataItem.Properties)
				{
					if (plusDataObject.IsRequired || plusDataObject.Length != string.Empty)
					{
						dataItemContent += "[";
						if (plusDataObject.IsRequired)
						{
							dataItemContent += "Required";
						}

						if (plusDataObject.IsRequired || plusDataObject.Length != string.Empty)
						{
							dataItemContent += ", ";
						}

						if (plusDataObject.Length != string.Empty)
						{
							if (plusDataObject.Type == "int")
							{
								dataItemContent += "NumericRange(0, " + Helpers.GetMaxValue(plusDataObject.Length) + ")";
							}
							if (plusDataObject.Type == "string")
							{
								dataItemContent += "MaxLenght(0, " + plusDataObject.Length + ")";
							}

						}
						dataItemContent += "]\r\n";
					}
					dataItemContent += "public " + plusDataObject.Type + " " + plusDataObject.Name + "\r\n" +
					                   "    {get\r\n" +
					                   "    {\r\n" +
					                   "        return Get<" + plusDataObject.Type + ">();\r\n" +
					                   "    }\r\n" +
					                   "    set\r\n" +
					                   "    {\r\n" +
					                   "        Set<" + plusDataObject.Type + ">(value);\r\n" +
					                   "    }}\r\n\r\n";
				}

				string[] contentsDataItem =
				{
					dataItemContent
				};

				CreateFile(GetInputhPath(_templateDirectory + @"Repository\DataItems\DataItemTemplate.cs"),
					OutputPath + @"Repository\DataItems\" + Product + dataItem.Name + "DataItem.cs", contentsDataItem, dataItem.Name);
			}
		}

		private void CreateRepositoryDtoFactory()
		{
			string factoryContent = string.Empty;
			foreach (PlusDataItem dataItem in DataLayout)
			{
				factoryContent += DoReplaces(_createRepositoryDtoTemplateUpperPart + "\r\n", dataItem.Name);
				foreach (PlusDataItemProperty plusDataObject in dataItem.Properties)
				{
					factoryContent += plusDataObject.Name + " = dataItem." + plusDataObject.Name + ",\r\n";
				}
				factoryContent += DoReplaces(_createRepositoryDtoTemplateLowerPart, dataItem.Name);
			}

			string[] contentsDtoFactory = {
				factoryContent
			};

			CreateFile(GetInputhPath(_templateDirectory + @"Repository\DtoFactoryTemplate.cs"), OutputPath + @"Repository\" + Product + "DtoFactory.cs",
				contentsDtoFactory);
		}

		private void CreateDataItemFactory()
		{
			string factoryContent = string.Empty;
			foreach (PlusDataItem dataItem in DataLayout)
			{
				factoryContent += DoReplaces(_createDataItemTemplateUpperPart + "\r\n", dataItem.Name);
				foreach (PlusDataItemProperty plusDataObject in dataItem.Properties)
				{
					factoryContent += plusDataObject.Name + " = dto." + plusDataObject.Name + ",\r\n";
				}
				factoryContent += DoReplaces(_createDataItemTemplateLowerPart, dataItem.Name);
			}

			string[] contentsDataItemFactory = {
				factoryContent
			};

			CreateFile(GetInputhPath(_templateDirectory + @"Repository\DataItemFactoryTemplate.cs"),
				OutputPath + @"Repository\" + Product + "DataItemFactory.cs", contentsDataItemFactory);
		}

		#endregion Repository

		#region Helpers

		private string GetLocalizedString(string input)
		{
			return "{localization:Localize Key=" + Product + DialogName + "_lbl" + input + ", Source=" + Product +
			       "Localizer}";
		}

		private void CreateFile(string input, string output, string[] contents = null, string item = "")
		{
			string fileContent = File.ReadAllText(input);

			fileContent = DoReplaces(fileContent, item);

			if (contents != null)
			{
				for (int i = 0; i < contents.Length; i++)
				{
					if (i == 0)
					{
						fileContent = fileContent.Replace("$specialContent$", contents[i]);
					}
					else
					{
						fileContent = fileContent.Replace("$specialContent" + i + "$", contents[i]);
					}
				}
			}

			FileInfo fileInfo = new FileInfo(output);
			fileInfo.Directory.Create();
			File.WriteAllText(fileInfo.FullName, fileContent);
		}

		private string DoReplaces(string input, string item = "")
		{
			input = input.Replace("$Product$", Product);
			input = input.Replace("$product$", Helpers.ToPascalCase(Product));
			input = input.Replace("$Item$", item);
			input = input.Replace("$item$", Helpers.ToPascalCase(item));
			input = input.Replace("$Dialog$", DialogName);
			input = input.Replace("$dialog$", Helpers.ToPascalCase(DialogName));

			return input;
		}

		private string GetInputhPath(string path)
		{
			if (File.Exists(path))
			{
				return path;
			}
			return path.Replace(SelectedTemplateMode.ToString(), "Common");
		}

		#endregion Helpers

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

		public bool IsUseBusinessServiceWithoutBO
		{
			get
			{
				return _isUseBusinessServiceWithoutBO;
			}
			set
			{
				SetProperty(ref _isUseBusinessServiceWithoutBO, value);
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

		public TemplateMode SelectedTemplateMode
		{
			get
			{
				return _selectedTemplateMode;
			}
			set
			{
				SetProperty(ref _selectedTemplateMode, value);
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

		public string DialogTranslationDE
		{
			get
			{
				return _dialogTranslationDE;
			}
			set
			{
				SetProperty(ref _dialogTranslationDE, value);
			}
		}

		public string DialogTranslationEN
		{
			get
			{
				return _dialogTranslationEN;
			}
			set
			{
				SetProperty(ref _dialogTranslationEN, value);
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

		#region Temp

		private void GenerateTempData()
		{
			Product = "Mst";
			//Item = "Site";
			DialogName = "AdministrationOfSites";

			PlusDataItem dataItem = new PlusDataItem()
			{
				Name = "Site",
				Translation = "Anlage",
				Properties = new ObservableCollection<PlusDataItemProperty>()
			};

			dataItem.Properties.Add(new PlusDataItemProperty()
			{
				IsRequired = true,
				Name = "Id",
				Translation = "Id",
				Type = "int",
				Length = "3",
				IsKey = true,
				IsFilterProperty = true,
				FilterPropertyType = "TextBox"
			});

			dataItem.Properties.Add(new PlusDataItemProperty()
			{
				IsRequired = false,
				Name = "Description",
				Translation = "Beschreibung",
				Type = "string",
				Length = "20",
				IsKey = false,
				IsFilterProperty = true,
				FilterPropertyType = "ComboBox"
			});

			DataLayout.Add(dataItem);
		}

		#endregion Temp
	}

	public enum TemplateMode
	{
		OneDataItemReadOnly,
		OneDataItemReadAndSave,
		OneDataItemMulti,
		OneDataItemEditMulti,
		TwoDataItemsSplittedReadOnly,
		//TwoDataItemsSplittedReadAndSave,
		OneDataItemWithVersion
	}
}

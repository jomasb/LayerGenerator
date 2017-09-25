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

		private PlusDataItemProperty _selectedItem;
		private bool _isCreateDto = true;
		private bool _isCreateDtoFactory = true;
		private bool _isCreateGateway = true;
		private bool _isCreateBusinessService = true;
		private bool _isCreateDataItem = true;
		private bool _isCreateDataItemFactory = true;
		private bool _isCreateRepositoryDtoFactory = true;
		private bool _isCreateRepository = true;
		private bool _isCreateUi = true;
		private bool _isCreateUiFilter = true;
		private TemplateMode _template;
		private string _product;
		private string _item;
		private string _dialogName;
		private string _templateDirectory;
		private TemplateMode _selectedTemplateMode;
		private ObservableCollection<PlusDataItemProperty> _dataLayout;
		private string _inputhPath = @"C:\Users\jmaurer\Desktop\Templates\";
		private string _commonPath = @"Common\";
		private string _outputPath = @"C:\Output\";

		private string _keyReadOnlyTemplate =
			" IsReadOnly=\"{Binding IsNewItem, Converter={StaticResource InvertBoolConverter}}\"";

		private string _readOnlyTemplate = " IsReadOnly=\"True\"";

		private string _isNumericTemplate = " IsNumeric=\"True\"";

		private string _filterPropertyTemplate = "public $Type$ $Name$\r\n" +
		                                         "{\r\n" +
		                                         "    get { return _$name$; }\r\n" +
		                                         "    set\r\n" +
		                                         "    {\r\n" +
		                                         "        Set(ref _$name$, value);\r\n" +
		                                         "        RefreshCollectionView();\r\n" +
		                                         "    }\r\n" +
		                                         "}\r\n\r\n";

		private string _filterComboBoxPropertyTemplate = "public MultiValueSelector<$Type$> $Name$MultiSelector\r\n" +
		                                        "{\r\n" +
												"    get { return _$name$MultiSelector; }\r\n" +
		                                        "    set\r\n" +
		                                        "    {\r\n" +
												"        Set(ref _$name$MultiSelector, value);\r\n" +
		                                        "    }\r\n" +
		                                        "}\r\n\r\n";

		private string _filterTextBoxXamlTemplate = "<plus:PlusFormRow Label=\"{localization:Localize Key=$Product$$Dialog$_lbl$Name$, Source=$Product$Localizer}\">\r\n" +
											"    <plus:PlusTextBoxButton Text=\"{Binding $Name$, UpdateSourceTrigger=PropertyChanged, Delay=300}\" />\r\n" +
											"</plus:PlusFormRow>\r\n\r\n";

		private string _filterComboBoxXamlTemplate = "<plus:PlusFormRow Label=\"{localization:Localize Key=$Product$$Dialog$_lbl$Name$, Source=$Product$Localizer}\">\r\n" +
											 "    <plus:PlusMultiComboBox\r\n" +
											 "        IsSynchronizedWithCurrentItem=\"True\"\r\n" +
											 "        ItemsSource=\"{Binding $Name$MultiSelector.Values, Mode=OneWay}\"\r\n" +
											 "        SelectedIndex=\"0\"\r\n" +
											 "        SelectedItemsSource=\"{Binding $Name$MultiSelector.SelectedValues}\" />\r\n" +
											 "</plus:PlusFormRow>\r\n\r\n";

		private string _filterCheckBoxXamlTemplate = "<plus:PlusFormRow>\r\n" +
											 "    <plus:PlusCheckBox\r\n" +
											 "        Content=\"$Name$\"\r\n" +
											 "						IsChecked=\"{Binding $Name$, Mode=TwoWay}\"\r\n" +
											 "						IsThreeState=\"True\" />\r\n" +
											 "</plus:PlusFormRow>\r\n\r\n";

		private string _filterDateTimePickerXamlTemplate = "<plus:PlusFormRow Label=\"{localization:Localize Key=$Product$$Dialog$_lbl$Name$, Source=$Product$Localizer}\">\r\n" +
		                                                   "    <plus:PlusDateTimePicker\r\n" +
		                                                   "         VerticalAlignment=\"Center\"\r\n" +
		                                                   "         ClockHeader=\"{localization:Localize Key=PlusDateTimePicker_lblClockHeader, Source=PlusUIWPFLocalizer}\"\r\n" +
		                                                   "         DateTimeWatermarkContent=\"{localization:Localize Key=PlusDateTimePicker_lblWatermarkContent, Source=PlusUIWPFLocalizer}\"\r\n" +
														   "         SelectedValue=\"{Binding $Name$, UpdateSourceTrigger=PropertyChanged, Delay=300}\"\r\n" +
		                                                   "         TodayButtonLocalization=\"{localization:Localize Key=PlusDateTimePicker_lblNowButton, Source=PlusUIWPFLocalizer}\" />\r\n" +
		                                                   "</plus:PlusFormRow>\r\n\r\n";

		#endregion Members

		#region Construction

		public ConfigureViewModel()
		{
			_dataLayout = new ObservableCollection<PlusDataItemProperty>();

			GenerateTempData();

			StartCommand = new DelegateCommand(StartExecuted);
			AddCommand = new DelegateCommand(AddExecuted);
			DeleteCommand = new DelegateCommand(DeleteExecuted, DeleteCanExecute);
			ImportSettingsCommand = new DelegateCommand(ImportSettingsExecuted);
			ExportSettingsCommand = new DelegateCommand(ExportSettingsExecuted);
		}

		#endregion Construction

		#region Methods

		#region Import/Export

		private void ExportSettingsExecuted()
		{
			Configuration configuration = new Configuration()
			{
				IsCreateDto = _isCreateDto,
				IsCreateDtoFactory = _isCreateDtoFactory,
				IsCreateGateway = _isCreateGateway,
				IsCreateBusinessService = _isCreateBusinessService,
				IsCreateDataItem = _isCreateDataItem,
				IsCreateDataItemFactory = _isCreateDataItemFactory,
				IsCreateRepositoryDtoFactory = _isCreateRepositoryDtoFactory,
				IsCreateRepository = _isCreateRepository,
				IsCreateUi = _isCreateUi,
				IsCreateUiFilter = _isCreateUiFilter,
				Product = _product,
				Item = _item,
				DialogName = _dialogName,
				DataLayout = _dataLayout.ToList()
			};

			SaveFileDialog dialog = new SaveFileDialog();
			dialog.Title = "Export settings";
			dialog.Filter = "Config files (*.cfg)|*.cfg";
			dialog.ShowDialog();

			if (dialog.FileName != string.Empty)
			{
				ExportSettingsToFile(dialog.FileName, configuration);
			}
		}
		
		private void ExportSettingsToFile(string fileName, Configuration configuration)
		{
			FileInfo fileInfo = new FileInfo(fileName);
			fileInfo.Directory.Create();
			File.WriteAllText(fileInfo.FullName, Helpers.Serialize(configuration));
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
					if (property.PropertyType == typeof(IList<PlusDataItemProperty>))
					{
						IList<PlusDataItemProperty> plusDataItemProperties =
							property.GetValue(configuration) as IList<PlusDataItemProperty>;
						DataLayout.Clear();
						foreach (PlusDataItemProperty plusDataItemProperty in plusDataItemProperties)
						{
							DataLayout.Add(plusDataItemProperty);
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

		private void DeleteExecuted()
		{
			_dataLayout.Remove(SelectedItem);
		}

		private bool DeleteCanExecute()
		{
			return SelectedItem != null;
		}

		private void AddExecuted()
		{
			_dataLayout.Add(new PlusDataItemProperty());
		}

		private void StartExecuted()
		{
			_templateDirectory = InputPath + SelectedTemplateMode + "\\";

			// Gateway
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


			// Business Service
			if (IsCreateBusinessService)
			{
				CreateFile(GetInputhPath(_templateDirectory + @"Service\Contracts\IServiceTemplate.cs"), OutputPath + @"Service\Contracts\I" + Product + DialogName + "Service.cs");
				CreateFile(GetInputhPath(_templateDirectory + @"Service\ServiceTemplate.cs"), OutputPath + @"Service\" + Product + DialogName + "Service.cs");
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
				CreateLocalization();
			}

			MessageBox.Show("Done", "Info", MessageBoxButton.OK);
		}

		#region Gateway

		private void CreateGateway()
		{
			Random rnd = new Random();
			string key = string.Empty;
			string identifier = string.Empty;
			string mock = string.Empty;

			foreach (PlusDataItemProperty plusDataObject in DataLayout.Where(t => t.IsKey))
			{
				key += Helpers.ToPascalCase(Item) + "." + plusDataObject.Name + ", ";
				identifier += "x.Key." + plusDataObject.Name + ".Equals(dto." + plusDataObject.Name + ") &&";
				if (plusDataObject.Type == "string")
				{
					mock += Helpers.ToPascalCase(Item) + "." + plusDataObject.Name + " = " + plusDataObject.Name + " + i,\r\n";
				}
				if (plusDataObject.Type == "int")
				{
					mock += Helpers.ToPascalCase(Item) + "." + plusDataObject.Name + " = " + rnd.Next(0, GetMaxValue(plusDataObject.Length))  + ",\r\n";
				}
				if (plusDataObject.Type == "bool")
				{
					mock += Helpers.ToPascalCase(Item) + "." + plusDataObject.Name + " = true,\r\n";
				}
				if (plusDataObject.Type == "DateTime")
				{
					mock += Helpers.ToPascalCase(Item) + "." + plusDataObject.Name + " = new DateTime(2016, 12, 25),\r\n";
				}
			}

			key = key.Substring(0, key.Length - 2);
			identifier = identifier.Substring(0, identifier.Length - 3);

			CreateFile(GetInputhPath(_templateDirectory + @"Gateway\Contracts\IGatewayTemplate.cs"), OutputPath + @"Gateway\Contracts\I" + Product + DialogName + "Gateway.cs");
			CreateFile(GetInputhPath(_templateDirectory + @"Gateway\GatewayTemplate.cs"), OutputPath + @"Gateway\" + Product + DialogName + "Gateway.cs", key, identifier);
			CreateFile(GetInputhPath(_templateDirectory + @"Gateway\GatewayMockTemplate.cs"), OutputPath + @"Gateway\" + Product + DialogName + "GatewayMock.cs", mock);				
		}

		private int GetMaxValue(string length)
		{
			if (length == string.Empty)
			{
				return 999999;
			}

			int value;
			string computedString = string.Empty;
			if (int.TryParse(length, out value))
			{
				for (int i = 1; i < value; i++)
				{
					computedString += "9";
				}
			}
			return int.Parse(computedString);
		}

		private void CreateDto()
		{
			string dtoContent = string.Empty;
			foreach (PlusDataItemProperty plusDataObject in DataLayout)
			{
				dtoContent += "public " + plusDataObject.Type + " " + plusDataObject.Name + " {get; set;}\r\n\r\n";
			}

			CreateFile(GetInputhPath(_templateDirectory + @"Gateway\Dtos\DtoTemplate.cs"), OutputPath + @"Gateway\Dtos\" + Product + Item + ".cs",
				dtoContent);
		}

		private void CreateDtoFactory()
		{
			string dtoFactoryContent = string.Empty;
			foreach (PlusDataItemProperty plusDataObject in DataLayout)
			{
				if (plusDataObject.IsKey)
				{
					dtoFactoryContent += plusDataObject.Name + " = bo.Key." + plusDataObject.Name + ",\r\n";
				}
				else
				{
					dtoFactoryContent += plusDataObject.Name + " = bo." + plusDataObject.Name + ",\r\n";
				}
			}

			CreateFile(GetInputhPath(_templateDirectory + @"Gateway\DtoFactoryTemplate.cs"), OutputPath + @"Gateway\" + Product + "DtoFactory.cs",
				dtoFactoryContent);
		}

		#endregion Gateway

		#region Localization

		private void CreateLocalization()
		{
			string keyContentEn = string.Empty;
			string keyContentOther = string.Empty;

			keyContentEn += Product + DialogName + "_lblCaption=" + Product + " - " + DialogName + "\r\n";
			keyContentOther += Product + DialogName + "_lblCaption=@@" + Product + " - " + DialogName + "\r\n";
			keyContentEn += Product + DialogName + "_lbl" + Item + "s=" + Item + "s\r\n";
			keyContentOther += Product + DialogName + "_lbl" + Item + "s=@@" + Product + " - " + DialogName + "\r\n";


			foreach (PlusDataItemProperty plusDataObject in DataLayout)
			{
				keyContentEn += Product + DialogName + "_lbl" + plusDataObject.Name + "=" + plusDataObject.Name + "\r\n";
				keyContentOther += Product + DialogName + "_lbl" + plusDataObject.Name + "=@@" + plusDataObject.Name + "\r\n";
			}

			string[] languages = {"en", "fr", "de", "hu", "pt", "zh-CHS"};

			foreach (string language in languages)
			{
				string fileContent = File.ReadAllText(InputPath +_commonPath + @"Localization\localization.txt");
				fileContent = DoReplaces(fileContent);
				fileContent = fileContent.Replace("$language$", language);
				fileContent = fileContent.Replace("$date$", DateTime.Now.ToShortDateString());
				fileContent = fileContent.Replace("$Product$", Product);
				if (language == "en")
				{
					fileContent = fileContent.Replace("$Keys$", keyContentEn);
				}
				else
				{
					fileContent = fileContent.Replace("$Keys$", keyContentOther);
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
			foreach (PlusDataItemProperty plusDataObject in DataLayout)
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
							filterPropertiesContent += _filterComboBoxPropertyTemplate.Replace(plusDataObject.Type, "string");
							filterPredicatesContent += ".IsContainedInList(x => x." + plusDataObject.Name + filterSuffix + ", x => x." +
							                           plusDataObject.Name + "MultiSelector.SelectedValues)\r\n";
							filterMultiSelectorsInitializeContent +=
								plusDataObject.Name + "MultiSelector = new MultiValueSelector<string>(SourceCollection.Select(x => x." +
								plusDataObject.Name + ").Distinct(), RefreshCollectionView, AllValue);";
							filterXamlContent += DoReplaces(_filterComboBoxXamlTemplate);
						}
					}

					if (plusDataObject.Type == "bool")
					{
						filterMembersContent += "private " + plusDataObject.Type + "? _" + Helpers.ToPascalCase(plusDataObject.Name) + ";\r\n";
						filterPropertiesContent += _filterPropertyTemplate;
						filterPredicateResetContent += plusDataObject.Name + " = null;";
						filterPredicatesContent += ".IsEqual(x => x." + plusDataObject.Name + ", x => x." + plusDataObject.Name + ")\r\n";
						filterXamlContent += DoReplaces(_filterCheckBoxXamlTemplate);
					}

					if (plusDataObject.Type == "DateTime")
					{
						filterMembersContent += "private " + plusDataObject.Type + "? _" + Helpers.ToPascalCase(plusDataObject.Name) + ";\r\n";
						filterPropertiesContent += _filterPropertyTemplate;
						filterPredicateResetContent += plusDataObject.Name + " = null;";
						filterPredicatesContent += ".IsEqual(x => x." + plusDataObject.Name + ", x => x." + plusDataObject.Name + ")\r\n";
						filterXamlContent += DoReplaces(_filterDateTimePickerXamlTemplate);
					}

					filterMembersContent = filterMembersContent.Replace("$Name$", plusDataObject.Name).Replace("$name$", Helpers.ToPascalCase(plusDataObject.Name)).Replace("$Type$", plusDataObject.Type);
					filterPropertiesContent = filterPropertiesContent.Replace("$Name$", plusDataObject.Name).Replace("$name$", Helpers.ToPascalCase(plusDataObject.Name)).Replace("$Type$", plusDataObject.Type);
					filterPredicateResetContent = filterPredicateResetContent.Replace("$Name$", plusDataObject.Name).Replace("$name$", Helpers.ToPascalCase(plusDataObject.Name)).Replace("$Type$", plusDataObject.Type);
					filterPredicatesContent = filterPredicatesContent.Replace("$Name$", plusDataObject.Name).Replace("$name$", Helpers.ToPascalCase(plusDataObject.Name)).Replace("$Type$", plusDataObject.Type);
					filterXamlContent = filterXamlContent.Replace("$Name$", plusDataObject.Name).Replace("$name$", Helpers.ToPascalCase(plusDataObject.Name)).Replace("$Type$", plusDataObject.Type);
				}
			}

			CreateFile(GetInputhPath(_templateDirectory) + @"UI\Filter\FilterViewTemplate.xaml", OutputPath + @"UI\Filter\FilterView.xaml", filterXamlContent);
			CreateFile(GetInputhPath(_templateDirectory) + @"UI\Filter\FilterViewTemplate.xaml.cs", OutputPath + @"UI\Filter\FilterView.xaml.cs", string.Empty);

			string fileContent = File.ReadAllText(InputPath +_commonPath + @"UI\Filter\FilterViewModelTemplate.cs");
			fileContent = DoReplaces(fileContent);
			fileContent = fileContent.Replace("$filterMembers$", filterMembersContent);
			fileContent = fileContent.Replace("$filterPredicates$", filterPredicatesContent);
			fileContent = fileContent.Replace("$filterPredicateReset$", filterPredicateResetContent);
			fileContent = fileContent.Replace("$filterMultiSelectorsInitialize$", filterMultiSelectorsInitializeContent);
			fileContent = fileContent.Replace("$filterProperties$", filterPropertiesContent);

			FileInfo fileInfo = new FileInfo(OutputPath + @"UI\Filter\FilterViewModel.cs");
			fileInfo.Directory.Create();
			File.WriteAllText(fileInfo.FullName, fileContent);
		}

		private void CreateUi()
		{
			//Detail
			string detailViewContent = string.Empty;
			string yesNoConverterString = ", Converter={StaticResource BoolToLocalizedYesNoConverterConverter}";

			detailViewContent += "<plus:PlusGroupBox Header=\"" + GetLocalizedString(Item) + "s\">";
			detailViewContent += "    <StackPanel>";

			foreach (PlusDataItemProperty plusDataObject in DataLayout)
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
						detailViewContent += "            <plus:PlusCheckbox " + addintionalInformation + " IsChecked=\"{Binding DataItem." + plusDataObject.Name + "}\" />\r\n";
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

						detailViewContent += "            <plus:PlusTextBox" + addintionalInformation + " Text=\"{Binding DataItem." + plusDataObject.Name +
							                    ", UpdateSourceTrigger=PropertyChanged}\" />\r\n";
					}
				}

				detailViewContent += "        </plus:PlusFormRow>\r\n";
			}
			detailViewContent += "    </StackPanel>";
			detailViewContent += "</plus:PlusGroupBox>";


			CreateFile(GetInputhPath(_templateDirectory + @"UI\Detail\DetailViewModelTemplate.cs"), OutputPath + @"UI\Detail\DetailViewModel.cs", string.Empty);
			CreateFile(GetInputhPath(_templateDirectory + @"UI\Detail\DetailViewTemplate.xaml"), OutputPath + @"UI\Detail\DetailView.xaml", detailViewContent);
			CreateFile(GetInputhPath(_templateDirectory + @"UI\Detail\DetailViewTemplate.xaml.cs"), OutputPath + @"UI\Detail\DetailView.xaml.cs", string.Empty);


			//Master
			string masterViewContent = string.Empty;
			foreach (PlusDataItemProperty plusDataObject in DataLayout)
			{
				if (plusDataObject.Type == "bool")
				{
					masterViewContent += "                <plus:PlusGridViewCheckColumn Width=\"*\" DataMemberBinding=\"{Binding " +
											plusDataObject.Name + "}\" Header=\"" + GetLocalizedString(plusDataObject.Name) + "\"/>\r\n";
				}
				else
				{
					masterViewContent += "                <plus:PlusGridViewTextColumn Width=\"*\" DataMemberBinding=\"{Binding " +
											plusDataObject.Name + "}\" Header=\"" + GetLocalizedString(plusDataObject.Name) + "\"/>\r\n";
				}
			}
			CreateFile(GetInputhPath(_templateDirectory + @"UI\Master\MasterViewModelTemplate.cs"), OutputPath + @"UI\" + DialogName + @"Master\MasterViewModel.cs", string.Empty);
			CreateFile(GetInputhPath(_templateDirectory + @"UI\Master\MasterViewTemplate.xaml"), OutputPath + @"UI\" + DialogName + @"Master\MasterView.xaml", masterViewContent);
			CreateFile(GetInputhPath(_templateDirectory + @"UI\Master\MasterViewTemplate.xaml.cs"), OutputPath + @"UI\" + DialogName + @"Master\MasterView.xaml.cs", string.Empty);
		}

		#endregion UI

		#region Repository

		private void CreateRepository()
		{
			string identifier = string.Empty;
			foreach (PlusDataItemProperty plusDataObject in DataLayout.Where(t => t.IsKey))
			{
				identifier += "x." + plusDataObject.Name + ".Equals(dataItem." + plusDataObject.Name + ") &&";
			}

			CreateFile(GetInputhPath(_templateDirectory + @"Repository\Contracts\IRepositoryTemplate.cs"), OutputPath + @"Repository\Contracts\I" + Product + DialogName + "Repository.cs", string.Empty);
			CreateFile(GetInputhPath(_templateDirectory + @"Repository\RepositoryTemplate.cs"), OutputPath + @"Repository\" + Product + DialogName + "Repository.cs", identifier);
		}

		private void CreateDataItem()
		{
			string dataItemContent = string.Empty;
			foreach (PlusDataItemProperty plusDataObject in DataLayout)
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
						dataItemContent += "NumericRange(0, " + GetMaxValue(plusDataObject.Length) + ")";
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

			CreateFile(GetInputhPath(_templateDirectory + @"Repository\DataItems\DataItemTemplate.cs"),
				OutputPath + @"Repository\DataItems\" + Product + Item + "DataItem.cs", dataItemContent);
		}

		private void CreateRepositoryDtoFactory()
		{
			string dtoFactoryContent = string.Empty;
			foreach (PlusDataItemProperty plusDataObject in DataLayout)
			{
				dtoFactoryContent += plusDataObject.Name + " = dataItem." + plusDataObject.Name + ",\r\n";
			}

			CreateFile(GetInputhPath(_templateDirectory + @"Repository\DtoFactoryTemplate.cs"), OutputPath + @"Repository\" + Product + "DtoFactory.cs",
				dtoFactoryContent);
		}

		private void CreateDataItemFactory()
		{
			string dataItemFactoryContent = string.Empty;
			foreach (PlusDataItemProperty plusDataObject in DataLayout)
			{
				dataItemFactoryContent += plusDataObject.Name + " = dto." + plusDataObject.Name + ",\r\n";
			}

			CreateFile(GetInputhPath(_templateDirectory + @"Repository\DataItemFactoryTemplate.cs"),
				OutputPath + @"Repository\" + Product + "DataItemFactory.cs", dataItemFactoryContent);
		}

		#endregion Repository

		#region Helpers

		private string GetLocalizedString(string input)
		{
			return "{localization:Localize Key=" + Product + DialogName + "_lbl" + input + ", Source=" + Product +
			       "Localizer}";
		}

		private void CreateFile(string input, string output)
		{
			CreateFile(input, output, string.Empty, string.Empty);
		}

		private void CreateFile(string input, string output, string specialContent)
		{
			CreateFile(input, output, specialContent, string.Empty);
		}

		private void CreateFile(string input, string output, string specialContent, string specialContent2)
		{
			string fileContent = File.ReadAllText(input);
			fileContent = DoReplaces(fileContent);
			fileContent = fileContent.Replace("$specialContent$", specialContent);
			fileContent = fileContent.Replace("$specialContent2$", specialContent2);

			FileInfo fileInfo = new FileInfo(output);
			fileInfo.Directory.Create();
			File.WriteAllText(fileInfo.FullName, fileContent);
		}

		private string DoReplaces(string input)
		{
			input = input.Replace("$Product$", Product);
			input = input.Replace("$product$", Helpers.ToPascalCase(Product));
			input = input.Replace("$Item$", Item);
			input = input.Replace("$item$", Helpers.ToPascalCase(Item));
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

		public DelegateCommand StartCommand { get; set; }
		public DelegateCommand ImportSettingsCommand { get; set; }
		public DelegateCommand ExportSettingsCommand
		{
			get;
			set;
		}
		public DelegateCommand AddCommand
		{
			get;
			set;
		}
		public DelegateCommand DeleteCommand
		{
			get;
			set;
		}

		public PlusDataItemProperty SelectedItem
		{
			get
			{
				return _selectedItem;
			}
			set
			{
				SetProperty(ref _selectedItem, value);
				DeleteCommand.RaiseCanExecuteChanged();
			}
		}

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

		public ObservableCollection<PlusDataItemProperty> DataLayout
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

		public string Item
		{
			get
			{
				return _item;
			}
			set
			{
				SetProperty(ref _item, value);
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
			Item = "Site";
			DialogName = "AdministrationOfSites";

			PlusDataItemProperty plusDataItemProperty = new PlusDataItemProperty()
			{
				IsRequired = true,
				Name = "Id",
				Type = "int",
				Length = "3",
				IsKey = true,
				IsFilterProperty = true,
				FilterPropertyType = "TextBox"
			};

			DataLayout.Add(plusDataItemProperty);

			plusDataItemProperty = new PlusDataItemProperty()
			{
				IsRequired = false,
				Name = "Description",
				Type = "string",
				Length = "20",
				IsKey = false,
				IsFilterProperty = true,
				FilterPropertyType = "ComboBox"
			};

			DataLayout.Add(plusDataItemProperty);
		}

		#endregion Temp
	}

	public enum TemplateMode
	{
		OneDataItemReadOnly,
		OneDataItemReadAndSave,
		OneDataItemReadAndSaveMulti,
		TwoDataItemsSplittedReadOnly,
		//TwoDataItemsSplittedReadAndSave,
		OneDataItemWithVersion
	}
}

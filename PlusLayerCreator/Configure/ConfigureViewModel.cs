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

		#region Template

		private string _keyReadOnlyTemplate =
			" IsReadOnly=\"{Binding IsNewItem, Converter={StaticResource InvertBoolConverter}}\"";

		private string _readOnlyTemplate = " IsReadOnly=\"True\"";

		private string _isNumericTemplate = " IsNumeric=\"True\"";

		private string _masterGridTemplate;
		
		private string _createGatewayDtoTemplateUpperPart;
		private string _createGatewayDtoTemplateLowerPart;

		private string _createRepositoryDtoTemplateUpperPart;
		private string _createRepositoryDtoTemplateLowerPart;
		private string _createDataItemTemplateUpperPart;
		private string _createDataItemTemplateLowerPart;

		#endregion Template

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
			Configuration configuration = new Configuration()
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
					ser.WriteObject(writer, configuration);
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

			// Gateway
			if (!IsUseBusinessServiceWithoutBo)
			{
				if (IsCreateDto)
				{
					CreateDto(true);
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
				if (IsUseBusinessServiceWithoutBo)
				{
					CreateDto(false);
					CreateTandem();
					CreateBusinessService(false);	
				}
				else
				{
					CreateBusinessService(true);	
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
				CreateUiInfrastructure();
				CreateUi();
				CreateUiFilter();
			}

			CreateLocalization();

			MessageBox.Show("Done", "Info", MessageBoxButton.OK);
		}

		#endregion Commands

		#region Methods

		#region Gateway

		private void CreateGateway()
		{
			string interfaceReadContent = string.Empty;
			string interfaceSaveContent = string.Empty;

			string gatewayReadContent = string.Empty;
			string gatewaySaveContent = string.Empty;

			string key = string.Empty;
			string identifier = string.Empty;
			string readOnlyMappingDto = string.Empty;
			string readOnlyMappingBo = string.Empty;

			//Random rnd = new Random();
			//string mock = string.Empty;

			foreach (PlusDataItem dataItem in DataLayout)
			{
				if (dataItem.CanRead)
				{
					if (string.IsNullOrEmpty(dataItem.Parent))
					{
						interfaceReadContent += Helpers.DoReplaces(File.ReadAllText(InputPath  + @"Gateway\Contracts\GetPart.txt"), dataItem.Name) + "\r\n\r\n";
						gatewayReadContent += Helpers.DoReplaces(File.ReadAllText(InputPath + @"Gateway\GetPart.txt"), dataItem.Name) + "\r\n\r\n";
					}
					else
					{
						string content = Helpers.DoReplaces(File.ReadAllText(InputPath + @"Gateway\Contracts\GetChildPart.txt"), dataItem.Name) +
						                 "\r\n\r\n";
						content = Helpers.ReplaceSpecialContent(content, new[] { Product + dataItem.Parent });
						interfaceReadContent += content;


						content = Helpers.DoReplaces(File.ReadAllText(InputPath + @"Gateway\GetChildPart.txt"), dataItem.Name) +
						          "\r\n\r\n";
						content = Helpers.ReplaceSpecialContent(content, new[] { Product + dataItem.Parent });
						gatewayReadContent += content;
					}
				}

				if (dataItem.CanEdit && !dataItem.CanEditMultiple)
				{
					if (string.IsNullOrEmpty(dataItem.Parent))
					{
						interfaceSaveContent += Helpers.DoReplaces(File.ReadAllText(InputPath + @"Gateway\Contracts\SavePart.txt"), dataItem.Name) + "\r\n\r\n";
						gatewaySaveContent += Helpers.DoReplaces(File.ReadAllText(InputPath + @"Gateway\SavePart.txt"), dataItem.Name) + "\r\n\r\n";
					}
				}
				if (dataItem.CanEdit && dataItem.CanEditMultiple)
				{
					if (string.IsNullOrEmpty(dataItem.Parent))
					{
						interfaceSaveContent += Helpers.DoReplaces(File.ReadAllText(InputPath + @"Gateway\Contracts\SaveMultiPart.txt"), dataItem.Name) + "\r\n\r\n";
						gatewaySaveContent += Helpers.DoReplaces(File.ReadAllText(InputPath + @"Gateway\SaveMultiPart.txt"), dataItem.Name) + "\r\n\r\n";
					}
				}

				foreach (PlusDataItemProperty plusDataObject in dataItem.Properties.Where(t => t.IsKey))
				{
					key += Helpers.ToPascalCase(dataItem.Name) + "." + plusDataObject.Name + ", ";
					identifier += "x.Key." + plusDataObject.Name + ".Equals(dto." + plusDataObject.Name + ") &&";
					//if (plusDataObject.Type == "string")
					//{
					//	mock += Helpers.ToPascalCase(dataItem.Name) + "." + plusDataObject.Name + " = " + plusDataObject.Name + " + i,\r\n";
					//}
					//if (plusDataObject.Type == "int")
					//{
					//	mock += Helpers.ToPascalCase(dataItem.Name) + "." + plusDataObject.Name + " = " +
					//			rnd.Next(1, Helpers.GetMaxValue(plusDataObject.Length)) + ",\r\n";
					//}
					//if (plusDataObject.Type == "bool")
					//{
					//	mock += Helpers.ToPascalCase(dataItem.Name) + "." + plusDataObject.Name + " = true,\r\n";
					//}
					//if (plusDataObject.Type == "DateTime")
					//{
					//	mock += Helpers.ToPascalCase(dataItem.Name) + "." + plusDataObject.Name + " = new DateTime(2016, 12, 25),\r\n";
					//}
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

				//special content in gateway
				gatewaySaveContent = Helpers.ReplaceSpecialContent(gatewaySaveContent,
					new[] {key, identifier, readOnlyMappingDto, readOnlyMappingBo});

				Helpers.CreateFile(InputPath + @"Gateway\Contracts\IGatewayTemplate.cs",
					OutputPath + @"Gateway\Contracts\I" + Product + dataItem.Name + "Gateway.cs", new[] { interfaceReadContent, interfaceSaveContent }, dataItem.Name);
				Helpers.CreateFile(InputPath + @"Gateway\GatewayTemplate.cs",
					OutputPath + @"Gateway\" + Product + dataItem.Name + "Gateway.cs", new[] { gatewayReadContent, gatewaySaveContent }, dataItem.Name);

			}
		}

		private void CreateDto(bool withBO)
		{
			string layer = withBO ? "Gateway" : "Service";

			foreach (PlusDataItem dataItem in DataLayout)
			{
				string dtoContent = string.Empty;

				foreach (PlusDataItemProperty plusDataObject in dataItem.Properties)
				{
					dtoContent += "public " + plusDataObject.Type + " " + plusDataObject.Name + " {get; set;}\r\n\r\n";
				}

				Helpers.CreateFile(InputPath + layer + @"\Dtos\DtoTemplate.cs",
					OutputPath + layer + @"\Dtos\" + Product + dataItem.Name + ".cs", new[] { dtoContent }, dataItem.Name);
			}
		}

		private void CreateDtoFactory()
		{
			string factoryContent = string.Empty;
			foreach (PlusDataItem dataItem in DataLayout)
			{
				factoryContent += Helpers.DoReplaces(_createGatewayDtoTemplateUpperPart + "\r\n", dataItem.Name);
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
				factoryContent += Helpers.DoReplaces(_createGatewayDtoTemplateLowerPart, dataItem.Name);
			}

			string[] contentsDtoFactory = {
				factoryContent
			};

			Helpers.CreateFile(InputPath + @"Gateway\DtoFactoryTemplate.cs", OutputPath + @"Gateway\" + Product + "DtoFactory.cs", contentsDtoFactory);
		}

		#endregion Gateway

		#region BusinessService
		
		private void CreateBusinessService(bool withBo)
		{
			string fileNameExtension = withBo ? string.Empty : "NoBO";

			foreach (PlusDataItem dataItem in DataLayout)
			{
				string interfaceReadContent = string.Empty;
				string interfaceSaveContent = string.Empty;

				string serviceReadContent = string.Empty;
				string serviceSaveContent = string.Empty;

				if (dataItem.CanRead)
				{
					if (string.IsNullOrEmpty(dataItem.Parent))
					{
						interfaceReadContent += Helpers.DoReplaces(File.ReadAllText(InputPath + @"Service\Contracts\GetPart" + fileNameExtension + ".txt"), dataItem.Name) + "\r\n\r\n";
						serviceReadContent += Helpers.DoReplaces(File.ReadAllText(InputPath + @"Service\GetPart" + fileNameExtension + ".txt"), dataItem.Name) + "\r\n\r\n";
					}
					else
					{
						string content = Helpers.DoReplaces(File.ReadAllText(InputPath + @"Service\Contracts\GetChildPart" + fileNameExtension + ".txt"), dataItem.Name) +
						                 "\r\n\r\n";
						content = Helpers.ReplaceSpecialContent(content, new[] { Product + dataItem.Parent });
						interfaceReadContent += content;


						content = Helpers.DoReplaces(File.ReadAllText(InputPath + @"Service\GetChildPart" + fileNameExtension + ".txt"), dataItem.Name) +
						          "\r\n\r\n";
						content = Helpers.ReplaceSpecialContent(content, new[] { Product + dataItem.Parent });
						serviceReadContent += content;
					}
				}

				if (dataItem.CanEdit && !dataItem.CanEditMultiple)
				{
					if (string.IsNullOrEmpty(dataItem.Parent))
					{
						interfaceSaveContent += Helpers.DoReplaces(File.ReadAllText(InputPath + @"Service\Contracts\SavePart" + fileNameExtension + ".txt"), dataItem.Name) + "\r\n\r\n";
						serviceSaveContent += Helpers.DoReplaces(File.ReadAllText(InputPath + @"Service\SavePart" + fileNameExtension + ".txt"), dataItem.Name) + "\r\n\r\n";
					}
				}
				if (dataItem.CanEdit && dataItem.CanEditMultiple)
				{
					if (string.IsNullOrEmpty(dataItem.Parent))
					{
						interfaceSaveContent += Helpers.DoReplaces(File.ReadAllText(InputPath + @"Service\Contracts\SaveMultiPart" + fileNameExtension + ".txt"), dataItem.Name) + "\r\n\r\n";
						serviceSaveContent += Helpers.DoReplaces(File.ReadAllText(InputPath + @"Service\SaveMultiPart" + fileNameExtension + ".txt"), dataItem.Name) + "\r\n\r\n";
					}
				}

				string servicePath = withBo ? InputPath + @"Service\Contracts\IServiceTemplate.cs" : InputPath + @"Service\Contracts\IServiceNoBOTemplate.cs";

				Helpers.CreateFile(servicePath,
					OutputPath + @"Service\Contracts\I" + Product + dataItem.Name + "Service.cs", new[] { interfaceReadContent, interfaceSaveContent }, dataItem.Name);


				servicePath = withBo ? InputPath + @"Service\ServiceTemplate.cs" : InputPath + @"Service\ServiceNoBOTemplate.cs";

				Helpers.CreateFile(servicePath,
					OutputPath + @"Service\" + Product + dataItem.Name + "Service.cs", new[] { serviceReadContent, serviceSaveContent }, dataItem.Name);
			}
		}
		
		private void CreateTandem()
		{
			string converterMessageToBoContent = string.Empty;
			string converterBoToMessageContent = string.Empty;
			foreach (PlusDataItem dataItem in DataLayout)
			{
				string serverMappingReadContent = string.Empty;
				string serverMappingWriteContent = string.Empty;
				if (dataItem.CanRead)
				{
					serverMappingReadContent = Helpers.DoReplaces(File.ReadAllText(InputPath + @"Service\Tandem\ServerMappingGetPart.txt"), dataItem.Name) + "\r\n\r\n";
				}

				if (dataItem.CanEdit && !dataItem.CanEditMultiple)
				{
					serverMappingWriteContent = Helpers.DoReplaces(File.ReadAllText(InputPath + @"Service\Tandem\ServerMappingSavePart.txt"), dataItem.Name) + "\r\n\r\n";
				}
				
				if (dataItem.CanEdit && dataItem.CanEditMultiple)
				{
					serverMappingWriteContent = Helpers.DoReplaces(File.ReadAllText(InputPath + @"Service\Tandem\ServerMappingSaveMultiPart.txt"), dataItem.Name) + "\r\n\r\n";
				}
				

				foreach (PlusDataItemProperty plusDataObject in dataItem.Properties)
				{
					converterMessageToBoContent += "serviceMessage." + plusDataObject.Name + "(" + Helpers.ToPascalCase(plusDataObject.Name) + "." +
					                               plusDataObject.Name + ", i);\r\n";
					converterBoToMessageContent += plusDataObject.Name + " = serviceMessage." + plusDataObject.Name + "(i),\r\n";
				}

				Helpers.CreateFile(InputPath + @"Service\Tandem\Converter.cs", OutputPath + @"Service\Tandem\" + Product + dataItem.Name + "Converter.cs", new[] { converterMessageToBoContent, converterBoToMessageContent }, dataItem.Name);
				Helpers.CreateFile(InputPath + @"Service\Tandem\ServerMapping.cs", OutputPath + @"Service\Tandem\" + Product + dataItem.Name + "ServerMapping.cs", new[] { serverMappingReadContent, serverMappingWriteContent }, dataItem.Name);
			}
		}

		#endregion

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

			lblEn += Product + DialogName + "_lblCaption=" + Product + " - " + DialogTranslationEnglish + "\r\n";
			lblEnOut += Product + DialogName + "_lblCaption=@@" + Product + " - " + DialogTranslationEnglish + "\r\n";
			lblDe += Product + DialogName + "_lblCaption=" + Product + " - " + DialogTranslationGerman + "\r\n";
			lblDeOut += Product + DialogName + "_lblCaption=@@" + Product + " - " + DialogTranslationGerman + "\r\n";

			foreach (PlusDataItem dataItem in DataLayout)
			{
				lblEn += Product + DialogName + "_lbl" + dataItem.Name + "s=" + dataItem.Name + "s\r\n";
				lblEnOut += Product + DialogName + "_lbl" + dataItem.Name + "s=@@" + dataItem.Name + "s\r\n";
				lblDe += Product + DialogName + "_lbl" + dataItem.Name + "s=" + dataItem.Translation + Helpers.GetLocaliatzionExtension(dataItem.Translation) + "\r\n";
				lblDeOut += Product + DialogName + "_lbl" + dataItem.Name + "s=@@" + dataItem.Translation + Helpers.GetLocaliatzionExtension(dataItem.Translation) +"\r\n";

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
				string fileContent = File.ReadAllText(InputPath +@"Localization\localization.txt");
				fileContent = Helpers.DoReplaces(fileContent, string.Empty);
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
				if (fileInfo.Directory != null)
				{
					fileInfo.Directory.Create();
				}
				File.WriteAllText(fileInfo.FullName, fileContent, Encoding.UTF8);				
			}

		}

		#endregion Localization

		#region UI

		private void CreateUiFilter()
		{
			string filterViewModelTemplate1 = "public FilterViewModel(IViewModelBaseServices baseServices,\r\n";
			string filterViewModelTemplate2 = ": base(baseServices)\r\n" +
												"{\r\n" +
												"DisplayName = GlobalLocalizer.Singleton.Global_lblFilter.Translation;\r\n";
			string filterViewModelTemplate3 = "}";

			string filterViewModelConstructionContent = string.Empty;
			string filterViewModelInitializationContent = string.Empty;

			string filterViewContent = string.Empty;
			string filterChildViewModelContent = string.Empty;

			//filterMembersContent
			foreach (PlusDataItem dataItem in DataLayout)
			{
				string filterMembersContent = string.Empty;
				string filterPredicatesContent = string.Empty;
				string filterPredicateResetContent = string.Empty;
				string filterMultiSelectorsInitializeContent = string.Empty;
				string filterPropertiesContent = string.Empty;

				foreach (PlusDataItemProperty plusDataObject in dataItem.Properties)
				{
					if (plusDataObject.IsFilterProperty)
					{
						string propertyFilterXamlContent;
						string propertyFilterMembersContent;
						string propertyFilterPredicatesContent;
						string propertyFilterPredicateResetContent;
						string propertyFilterMultiSelectorsInitializeContent;
						string propertyFilterPropertiesContent;

						Helpers.CreateFilterContents(plusDataObject, out propertyFilterMembersContent,
							out propertyFilterPropertiesContent, out propertyFilterPredicateResetContent,
							out propertyFilterPredicatesContent, out propertyFilterXamlContent,
							out propertyFilterMultiSelectorsInitializeContent);

						filterMembersContent += Helpers.DoReplaces2(propertyFilterMembersContent, plusDataObject.Name, dataItem.Name, plusDataObject.Type);
						filterPredicatesContent += Helpers.DoReplaces2(propertyFilterPredicatesContent, plusDataObject.Name, dataItem.Name, plusDataObject.Type);
						filterPredicateResetContent += Helpers.DoReplaces2(propertyFilterPredicateResetContent, plusDataObject.Name, dataItem.Name, plusDataObject.Type);
						filterMultiSelectorsInitializeContent += Helpers.DoReplaces2(propertyFilterMultiSelectorsInitializeContent, plusDataObject.Name, dataItem.Name, plusDataObject.Type);
						filterPropertiesContent += Helpers.DoReplaces2(propertyFilterPropertiesContent, plusDataObject.Name, dataItem.Name, plusDataObject.Type);
						filterViewContent += Helpers.DoReplaces2(propertyFilterXamlContent, plusDataObject.Name, dataItem.Name, plusDataObject.Type);
					}
				}

				string childViewModel = Helpers.FilterChildViewModelTemplate;
				childViewModel = Helpers.DoReplaces(childViewModel, dataItem.Name);
				childViewModel = childViewModel.Replace("$filterMembers$", filterMembersContent);
				childViewModel = childViewModel.Replace("$filterPredicates$", filterPredicatesContent);
				childViewModel = childViewModel.Replace("$filterPredicateReset$", filterPredicateResetContent);
				childViewModel = childViewModel.Replace("$filterMultiSelectorsInitialize$", filterMultiSelectorsInitializeContent);
				childViewModel = childViewModel.Replace("$filterProperties$", filterPropertiesContent);
				filterChildViewModelContent += childViewModel;

				filterViewModelConstructionContent += "IFilterSourceProvider<" + Product + dataItem.Name + "DataItem> " + Helpers.ToPascalCase(Product) + dataItem.Name +"DataItemFilterSourceProvider)\r\n";
				filterViewModelInitializationContent += Product + dataItem.Name + "DataItemFilterViewModel = new " + Product +
				                                        dataItem.Name + "DataItemFilterViewModel(" + Helpers.ToPascalCase(Product) +
				                                        dataItem.Name + "DataItemFilterSourceProvider);\r\n";
			}

			var filterViewModelContent = filterViewModelTemplate1 + filterViewModelConstructionContent + filterViewModelTemplate2 +
			                                filterViewModelInitializationContent + filterViewModelTemplate3;

			Helpers.CreateFile(InputPath + @"UI\Regions\Filter\FilterViewTemplate.xaml",
				OutputPath + @"UI\Regions\Filter\FilterView.xaml", new[] { filterViewContent });
			Helpers.CreateFile(InputPath + @"UI\Regions\Filter\FilterViewTemplate.xaml.cs",
				OutputPath + @"UI\Regions\Filter\FilterView.xaml.cs");

			Helpers.CreateFile(InputPath + @"UI\Regions\Filter\FilterViewModelTemplate.cs",
				OutputPath + @"UI\Regions\Filter\FilterViewModel.cs", new[] { filterViewModelContent, filterChildViewModelContent });
		}

		private void CreateUiInfrastructure()
		{
			string moduleContent = string.Empty;
			string moduleContent2 = string.Empty;
			string viewNamesContent = "public static readonly string " + DialogName + "MasterView = \"" + DialogName + "MasterView\";\r\n";

			foreach (PlusDataItem dataItem in DataLayout)
			{
				moduleContent += "_container.RegisterType<object, " + dataItem.Name + "DetailView>(ViewNames." + dataItem.Name + "DetailView);\r\n";
				moduleContent2 += "_container.RegisterType<IFilterSourceProvider<" + Product + dataItem.Name + "DataItem>, " +
								  DialogName + "MasterViewModel>(new ContainerControlledLifetimeManager());\r\n";
				viewNamesContent += "public static readonly string " + dataItem.Name + "DetailView = \"" + dataItem.Name + "DetailView\";\r\n";
			}

			Helpers.CreateFile(InputPath + @"UI\ModuleTemplate.cs", OutputPath + @"UI\" + DialogName + @"Module.cs", new[] { moduleContent, moduleContent2 });
			Helpers.CreateFile(InputPath + @"UI\ViewTemplate.xaml", OutputPath + @"UI\" + DialogName + @"View.xaml");
			Helpers.CreateFile(InputPath + @"UI\WindowTemplate.xaml", OutputPath + @"UI\" + DialogName + @"Window.xaml");

			Helpers.CreateFile(InputPath + @"UI\Infrastructure\BootstrapperTemplate.cs", OutputPath + @"UI\Infrastructure\Bootstrapper.cs");
			Helpers.CreateFile(InputPath + @"UI\Infrastructure\CommandNamesTemplate.cs", OutputPath + @"UI\Infrastructure\CommandNames.cs");
			Helpers.CreateFile(InputPath + @"UI\Infrastructure\EventNamesTemplate.cs", OutputPath + @"UI\Infrastructure\EventNames.cs");
			Helpers.CreateFile(InputPath + @"UI\Infrastructure\ParameterNamesTemplate.cs", OutputPath + @"UI\Infrastructure\ParameterNames.cs");
			Helpers.CreateFile(InputPath + @"UI\Infrastructure\ViewNamesTemplate.cs", OutputPath + @"UI\Infrastructure\ViewNames.cs", new[] { viewNamesContent });
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

					if (!dataItem.CanEdit)
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

				Helpers.CreateFile(InputPath + @"UI\Regions\Detail\DetailViewModelTemplate.cs",
					OutputPath + @"UI\Regions\Detail\" + dataItem.Name + "DetailViewModel.cs", null, dataItem.Name);
				Helpers.CreateFile(InputPath + @"UI\Regions\Detail\DetailViewTemplate.xaml",
					OutputPath + @"UI\Regions\Detail\" + dataItem.Name + "DetailView.xaml", new[] { detailViewContent }, dataItem.Name);
				Helpers.CreateFile(InputPath + @"UI\Regions\Detail\DetailViewTemplate.xaml.cs",
					OutputPath + @"UI\Regions\Detail\" + dataItem.Name + "DetailView.xaml.cs", null, dataItem.Name);


				//Master
				
				string gridContent = _masterGridTemplate;
				string columnsContent = string.Empty;
				foreach (PlusDataItemProperty plusDataObject in dataItem.Properties)
				{
					if (plusDataObject.Type == "bool")
					{
						columnsContent += "                <plus:PlusGridViewCheckColumn Width=\"*\" DataMemberBinding=\"{Binding " +
						                     plusDataObject.Name + "}\" Header=\"" + GetLocalizedString(plusDataObject.Name) + "\"/>\r\n";
					}
					else
					{
						columnsContent += "                <plus:PlusGridViewTextColumnMinimal Width=\"*\" DataMemberBinding=\"{Binding " +
							plusDataObject.Name + "}\" Header=\"" + GetLocalizedString(plusDataObject.Name) + "\"/>\r\n";
					}
				}

				masterViewContent += gridContent.Replace("$specialContent1$", columnsContent);
			}

			string masterViewModelPath = string.Empty;

			switch (Template)
			{
				case TemplateMode.One:
				{
					if (DataLayout.Count == 1 && DataLayout.Any(t => t.CanEditMultiple))
					{
						masterViewModelPath = InputPath + @"UI\Regions\Master\MasterViewModelMultiTemplate.cs";
					}
					else
					{
						masterViewModelPath = InputPath + @"UI\Regions\Master\MasterViewModelTemplate.cs";
					}
					break;
				}
				case TemplateMode.MasterDetail:
				{
					masterViewModelPath = InputPath + @"UI\Regions\Master\MasterViewModelMasterDetailTemplate.cs";
					break;
				}
			}

			Helpers.CreateFile(masterViewModelPath, OutputPath + @"UI\Regions\Master\" + DialogName + @"MasterViewModel.cs");
			Helpers.CreateFile(InputPath + @"UI\Regions\Master\MasterViewTemplate.xaml", OutputPath + @"UI\Regions\Master\" + DialogName + @"MasterView.xaml", new[] { masterViewContent });
			Helpers.CreateFile(InputPath + @"UI\Regions\Master\MasterViewTemplate.xaml.cs", OutputPath + @"UI\Regions\Master\" + DialogName + @"MasterView.xaml.cs");
		}

		#endregion UI

		#region Repository

		private void CreateRepository()
		{
			string repositoryServiceMemberContent = string.Empty;
			string repositoryServiceParameterContent = string.Empty;
			string repositoryServiceConstructorContent = string.Empty;

			string interfaceContent = string.Empty;
			string repositoryContent = string.Empty;
			
			foreach (PlusDataItem dataItem in DataLayout)
			{
				repositoryServiceMemberContent += "private readonly I" + Product + dataItem.Name + "Service _" + Helpers.ToPascalCase(Product + dataItem.Name + "Service;\r\n");
				if (repositoryServiceParameterContent != string.Empty)
				{
					repositoryServiceParameterContent = ", " + repositoryServiceParameterContent;
				}
				repositoryServiceParameterContent += "I" + Product + dataItem.Name + "Service " + Helpers.ToPascalCase(Product) + dataItem.Name + "Service";
				repositoryServiceConstructorContent += "_" + Helpers.ToPascalCase(Product) + dataItem.Name + "Service = " + Helpers.ToPascalCase(Product) + dataItem.Name + "Service;\r\n";

				interfaceContent += "#region " + dataItem.Name + "\r\n\r\n";
				repositoryContent += "#region " + dataItem.Name + "\r\n\r\n";
				
				string identifier = GetIdentifier(dataItem);
				string readOnly = GetReadOnly(dataItem);

				if (dataItem.CanRead)
				{
					if (string.IsNullOrEmpty(dataItem.Parent))
					{
						if (DataLayout.Any(t => t.Parent == dataItem.Name))
						{
							//parent
							interfaceContent += Helpers.DoReplaces(File.ReadAllText(InputPath + @"Repository\Contracts\GetParentPart.txt"), dataItem.Name) + "\r\n\r\n";

							string childDataItem = DataLayout.FirstOrDefault(t => t.Parent == dataItem.Name).Name;
							string getChild = childDataItem +
							                  "s = new PlusLazyLoadingAsyncObservableCollection<" + Product + childDataItem + "DataItem>(" +
											  "() => Get" + childDataItem + "sAsync(callContext, " + Helpers.ToPascalCase(dataItem.Name) + "DataItem))" +
							                  "{ AutoAcceptAfterSuccessfullyLoading = true, AutoOverwriteParent = true };";

							string content = Helpers.DoReplaces(File.ReadAllText(InputPath + @"Repository\GetParentPart.txt"), dataItem.Name) +
							                 "\r\n\r\n";
							content = Helpers.ReplaceSpecialContent(content, new[] { getChild });
							repositoryContent += content;
						}
						else
						{
							//single
							interfaceContent += Helpers.DoReplaces(File.ReadAllText(InputPath + @"Repository\Contracts\GetPart.txt"), dataItem.Name) + "\r\n\r\n";
							repositoryContent += Helpers.DoReplaces(File.ReadAllText(InputPath + @"Repository\GetPart.txt"), dataItem.Name) + "\r\n\r\n";
						}
					}
					else
					{
						//child
						string transformParent = "_" + Helpers.ToPascalCase(Product) + "DtoFactory.Create" + dataItem.Parent + "FromDataItem";
						string content = Helpers.DoReplaces(File.ReadAllText(InputPath + @"Repository\Contracts\GetChildPart.txt"), dataItem.Name) +
						                 "\r\n\r\n";
						content = Helpers.ReplaceSpecialContent(content, new[] { Product + dataItem.Parent });
						interfaceContent += content;

						content = Helpers.DoReplaces(File.ReadAllText(InputPath + @"Repository\GetChildPart.txt"), dataItem.Name) +
						          "\r\n\r\n";
						content = Helpers.ReplaceSpecialContent(content, new[] { Product + dataItem.Parent, transformParent });
						repositoryContent += content;
					}
				}

				if (dataItem.CanClone)
				{
					interfaceContent += Helpers.DoReplaces(File.ReadAllText(InputPath + @"Repository\Contracts\ClonePart.txt"), dataItem.Name) + "\r\n\r\n";
					repositoryContent += Helpers.DoReplaces(File.ReadAllText(InputPath + @"Repository\ClonePart.txt"), dataItem.Name) + "\r\n\r\n";
				}

				if (dataItem.CanDelete)
				{
					interfaceContent += Helpers.DoReplaces(File.ReadAllText(InputPath + @"Repository\Contracts\DeletePart.txt"), dataItem.Name) + "\r\n\r\n";
					repositoryContent += Helpers.DoReplaces(File.ReadAllText(InputPath + @"Repository\DeletePart.txt"), dataItem.Name) + "\r\n\r\n";
				}

				if (dataItem.CanSort)
				{
					interfaceContent += Helpers.DoReplaces(File.ReadAllText(InputPath + @"Repository\Contracts\SortPart.txt"), dataItem.Name) + "\r\n\r\n";
					repositoryContent += Helpers.DoReplaces(File.ReadAllText(InputPath + @"Repository\SortPart.txt"), dataItem.Name) + "\r\n\r\n";
				}
				if (dataItem.CanEdit)
				{
					interfaceContent += Helpers.DoReplaces(File.ReadAllText(InputPath + @"Repository\Contracts\AddPart.txt"), dataItem.Name) + "\r\n\r\n";
					repositoryContent += Helpers.DoReplaces(File.ReadAllText(InputPath + @"Repository\AddPart.txt"), dataItem.Name) + "\r\n\r\n";

					if (!dataItem.CanEditMultiple)
					{
						if (string.IsNullOrEmpty(dataItem.Parent))
						{
							interfaceContent += Helpers.DoReplaces(File.ReadAllText(InputPath + @"Repository\Contracts\SavePart.txt"), dataItem.Name) + "\r\n\r\n";
							string content = Helpers.DoReplaces(File.ReadAllText(InputPath + @"Repository\SavePart.txt"), dataItem.Name) + "\r\n\r\n";
							content = Helpers.ReplaceSpecialContent(content, new[] { identifier, readOnly });
							repositoryContent += content;
						}
					}
					if (dataItem.CanEditMultiple)
					{
						if (string.IsNullOrEmpty(dataItem.Parent))
						{
							interfaceContent += Helpers.DoReplaces(File.ReadAllText(InputPath + @"Repository\Contracts\SaveMultiPart.txt"), dataItem.Name) + "\r\n\r\n";
							string content = Helpers.DoReplaces(File.ReadAllText(InputPath + @"Repository\SaveMultiPart.txt"), dataItem.Name) + "\r\n\r\n";
							content = Helpers.ReplaceSpecialContent(content, new[] { identifier, readOnly });
							repositoryContent += content;
						}
					}
				}
				
				interfaceContent += "#endregion " + dataItem.Name + "\r\n\r\n";
				repositoryContent += "#endregion " + dataItem.Name + "\r\n\r\n";
			}

			string dtoLayer = IsUseBusinessServiceWithoutBo ? "BusinessServiceLocal" : "Gateway";

			Helpers.CreateFile(InputPath + @"Repository\Contracts\IRepositoryTemplate.cs", OutputPath + @"Repository\Contracts\I" + Product + DialogName + "Repository.cs", new[] { interfaceContent });
			Helpers.CreateFile(InputPath + @"Repository\RepositoryTemplate.cs", OutputPath + @"Repository\" + Product + DialogName + "Repository.cs", new[] { repositoryServiceMemberContent, repositoryServiceParameterContent, repositoryServiceConstructorContent, repositoryContent, dtoLayer });
		}

		private string GetIdentifier(PlusDataItem dataItem)
		{
			string identifier = string.Empty;
			foreach (PlusDataItemProperty plusDataObject in dataItem.Properties.Where(t => t.IsKey))
			{
				if (identifier != string.Empty)
				{
					identifier = " && " + identifier;
				}

				identifier += "x." + plusDataObject.Name + ".Equals(dto." + plusDataObject.Name + ")";
			}

			return identifier;
		}

		private string GetReadOnly(PlusDataItem dataItem)
		{
			string readOnly = string.Empty;
			foreach (PlusDataItemProperty plusDataObject in dataItem.Properties.Where(t => t.IsKey))
			{
				if (plusDataObject.IsReadOnly)
				{
					readOnly +=
						Helpers.ToPascalCase(dataItem.Name) + "Dto." + plusDataObject.Name + " = " + Helpers.ToPascalCase(dataItem.Name) + "." +
						plusDataObject.Name + ";\r\n";
				}
			}

			return readOnly;
		}

		private void CreateDataItem()
		{
			foreach (PlusDataItem dataItem in DataLayout)
			{
				string dataItemContent = string.Empty;

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
								dataItemContent += "MaxLength(" + plusDataObject.Length + ")";
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

					foreach (PlusDataItem childDataItem in DataLayout.Where(t => t.Parent == dataItem.Name))
					{
						dataItemContent += Helpers.DoReplaces(File.ReadAllText(InputPath + @"Repository\DataItems\DataItemCollectionPart.txt"), childDataItem.Name) + "\r\n";
					}
				}

				Helpers.CreateFile(InputPath + @"Repository\DataItems\DataItemTemplate.cs",
					OutputPath + @"Repository\DataItems\" + Product + dataItem.Name + "DataItem.cs", new[] { dataItemContent }, dataItem.Name);
			}
		}

		private void CreateRepositoryDtoFactory()
		{
			string factoryContent = string.Empty;
			foreach (PlusDataItem dataItem in DataLayout)
			{
				factoryContent += Helpers.DoReplaces(_createRepositoryDtoTemplateUpperPart + "\r\n", dataItem.Name);
				foreach (PlusDataItemProperty plusDataObject in dataItem.Properties)
				{
					factoryContent += plusDataObject.Name + " = dataItem." + plusDataObject.Name + ",\r\n";
				}
				factoryContent += Helpers.DoReplaces(_createRepositoryDtoTemplateLowerPart, dataItem.Name) + "\r\n";
			}

			string dtoLayer = IsUseBusinessServiceWithoutBo ? "BusinessServiceLocal" : "Gateway";

			Helpers.CreateFile(InputPath + @"Repository\DtoFactoryTemplate.cs", OutputPath + @"Repository\" + Product + "DtoFactory.cs",
				new[] { factoryContent , dtoLayer});
		}

		private void CreateDataItemFactory()
		{
			string factoryContent = string.Empty;
			foreach (PlusDataItem dataItem in DataLayout)
			{
				factoryContent += Helpers.DoReplaces(_createDataItemTemplateUpperPart + "\r\n", dataItem.Name);
				foreach (PlusDataItemProperty plusDataObject in dataItem.Properties)
				{
					factoryContent += plusDataObject.Name + " = dto." + plusDataObject.Name + ",\r\n";
				}
				factoryContent += Helpers.DoReplaces(_createDataItemTemplateLowerPart, dataItem.Name) + "\r\n";
			}

			string dtoLayer = IsUseBusinessServiceWithoutBo ? "BusinessServiceLocal" : "Gateway";

			Helpers.CreateFile(InputPath + @"Repository\DataItemFactoryTemplate.cs",
				OutputPath + @"Repository\" + Product + "DataItemFactory.cs", new[] { factoryContent, dtoLayer});
		}

		#endregion Repository

		#region Helpers

		private void GetTemplatesFromDisk()
		{
			Helpers.Product = Product;
			Helpers.DialogName = DialogName;

			_masterGridTemplate = File.ReadAllText(InputPath + @"UI\Regions\Master\MasterGrid.txt");
			Helpers.FilterChildViewModelTemplate = File.ReadAllText(InputPath + @"UI\Regions\Filter\FilterViewModelChildTemplate.txt");
			Helpers.FilterPropertyTemplate = File.ReadAllText(InputPath + @"UI\Regions\Filter\FilterProperty.txt");
			Helpers.FilterComboBoxPropertyTemplate = File.ReadAllText(InputPath + @"UI\Regions\Filter\FilterComboBox.txt");
			Helpers.FilterTextBoxXamlTemplate = File.ReadAllText(InputPath + @"UI\Regions\Filter\FilterTextBoxXaml.txt");
			Helpers.FilterComboBoxXamlTemplate = File.ReadAllText(InputPath + @"UI\Regions\Filter\FilterComboBoxXaml.txt");
			Helpers.FilterCheckBoxXamlTemplate = File.ReadAllText(InputPath + @"UI\Regions\Filter\FilterCheckBoxXaml.txt");
			Helpers.FilterDateTimePickerXamlTemplate = File.ReadAllText(InputPath + @"UI\Regions\Filter\FilterDateTimePickerXaml.txt");

			_createDataItemTemplateUpperPart = File.ReadAllText(InputPath + @"Repository\CreateDataItemUpperPart.txt");
			_createDataItemTemplateLowerPart = File.ReadAllText(InputPath + @"Repository\CreateDataItemLowerPart.txt");
			_createRepositoryDtoTemplateUpperPart = File.ReadAllText(InputPath + @"Repository\CreateDtoUpperPart.txt");
			_createRepositoryDtoTemplateLowerPart = File.ReadAllText(InputPath + @"Repository\CreateDtoLowerPart.txt");

			_createGatewayDtoTemplateUpperPart = File.ReadAllText(InputPath + @"Gateway\CreateDtoUpperPart.txt");
			_createGatewayDtoTemplateLowerPart = File.ReadAllText(InputPath + @"Gateway\CreateDtoLowerPart.txt");
		}

		private string GetLocalizedString(string input)
		{
			return "{localization:Localize Key=" + Product + DialogName + "_lbl" + input + ", Source=" + Product +
			       "Localizer}";
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

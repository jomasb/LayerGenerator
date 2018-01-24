using System.IO;
using System.Linq;
using PlusLayerCreator.Items;

namespace PlusLayerCreator.Configure
{
	public class UiPart
	{
		private Configuration _configuration;
		private string _keyReadOnlyTemplate =
			" IsReadOnly=\"{Binding IsNewItem, Converter={StaticResource InvertBoolConverter}}\"";
		private string _readOnlyTemplate = " IsReadOnly=\"True\"";
		private string _isNumericTemplate = " IsNumeric=\"True\"";
		private string _masterGridTemplate;

		public UiPart(Configuration configuration)
		{
			_configuration = configuration;
			_masterGridTemplate = File.ReadAllText(configuration.InputPath + @"UI\Regions\Master\MasterGrid.txt");
			
		}

		public void CreateUiFilter()
		{
			string filterViewModelTemplate1 = "public FilterViewModel(IViewModelBaseServices baseServices";
			string filterViewModelTemplate2 = ": base(baseServices)\r\n" +
												"{\r\n" +
												"DisplayName = GlobalLocalizer.Singleton.Global_lblFilter.Translation;\r\n";
			string filterViewModelTemplate3 = "}";

			string filterViewModelConstructionContent = string.Empty;
			string filterViewModelInitializationContent = string.Empty;

			string filterViewContent = string.Empty;
			string filterChildViewModelContent = string.Empty;

			//filterMembersContent
			foreach (PlusDataItem dataItem in _configuration.DataLayout)
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

						string type = plusDataObject.Type == "bool" ? "bool" : "string";

						filterMembersContent += Helpers.DoReplaces2(propertyFilterMembersContent, plusDataObject.Name, dataItem.Name, plusDataObject.Type);
						filterPredicatesContent += Helpers.DoReplaces2(propertyFilterPredicatesContent, plusDataObject.Name, dataItem.Name, plusDataObject.Type);
						filterPredicateResetContent += Helpers.DoReplaces2(propertyFilterPredicateResetContent, plusDataObject.Name, dataItem.Name, plusDataObject.Type);
						filterMultiSelectorsInitializeContent += Helpers.DoReplaces2(propertyFilterMultiSelectorsInitializeContent, plusDataObject.Name, dataItem.Name, plusDataObject.Type);
						filterPropertiesContent += Helpers.DoReplaces2(propertyFilterPropertiesContent, plusDataObject.Name, dataItem.Name, type);
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

				filterViewModelConstructionContent += ", IFilterSourceProvider<" + _configuration.Product + dataItem.Name + "DataItem> " + Helpers.ToPascalCase(_configuration.Product) + dataItem.Name + "DataItemFilterSourceProvider";
				filterViewModelInitializationContent += _configuration.Product + dataItem.Name + "DataItemFilterViewModel = new " + _configuration.Product +
														dataItem.Name + "DataItemFilterViewModel(" + Helpers.ToPascalCase(_configuration.Product) +
														dataItem.Name + "DataItemFilterSourceProvider);\r\n";
			}

			var filterViewModelContent = filterViewModelTemplate1 + filterViewModelConstructionContent + filterViewModelTemplate2 +
											filterViewModelInitializationContent + filterViewModelTemplate3;

			Helpers.CreateFile(_configuration.InputPath + @"UI\Regions\Filter\FilterViewTemplate.xaml",
				_configuration.OutputPath + @"UI\Regions\Filter\FilterView.xaml", new[] { filterViewContent });
			Helpers.CreateFile(_configuration.InputPath + @"UI\Regions\Filter\FilterViewTemplate.xaml.cs",
				_configuration.OutputPath + @"UI\Regions\Filter\FilterView.xaml.cs");

			Helpers.CreateFile(_configuration.InputPath + @"UI\Regions\Filter\FilterViewModelTemplate.cs",
				_configuration.OutputPath + @"UI\Regions\Filter\FilterViewModel.cs", new[] { filterViewModelContent, filterChildViewModelContent });
		}

		public void CreateUiInfrastructure()
		{
			string moduleContent = string.Empty;
			string moduleContent2 = string.Empty;
			string viewNamesContent = "public static readonly string " + _configuration.DialogName + "MasterView = \"" + _configuration.DialogName + "MasterView\";\r\n";

			foreach (PlusDataItem dataItem in _configuration.DataLayout)
			{
				moduleContent += "_container.RegisterType<object, " + dataItem.Name + "DetailView>(ViewNames." + dataItem.Name + "DetailView);\r\n";
				moduleContent2 += "_container.RegisterType<IFilterSourceProvider<" + _configuration.Product + dataItem.Name + "DataItem>, " +
								  _configuration.DialogName + "MasterViewModel>(new ContainerControlledLifetimeManager());\r\n";
				viewNamesContent += "public static readonly string " + dataItem.Name + "DetailView = \"" + dataItem.Name + "DetailView\";\r\n";
			}

			Helpers.CreateFile(_configuration.InputPath + @"UI\ModuleTemplate.cs", _configuration.OutputPath + @"UI\" + _configuration.DialogName + @"Module.cs", new[] { moduleContent, moduleContent2 });
			Helpers.CreateFile(_configuration.InputPath + @"UI\ViewTemplate.xaml", _configuration.OutputPath + @"UI\" + _configuration.DialogName + @"View.xaml");
			Helpers.CreateFile(_configuration.InputPath + @"UI\WindowTemplate.xaml", _configuration.OutputPath + @"UI\" + _configuration.DialogName + @"Window.xaml");

			Helpers.CreateFile(_configuration.InputPath + @"UI\Infrastructure\BootstrapperTemplate.cs", _configuration.OutputPath + @"UI\Infrastructure\Bootstrapper.cs");
			Helpers.CreateFile(_configuration.InputPath + @"UI\Infrastructure\CommandNamesTemplate.cs", _configuration.OutputPath + @"UI\Infrastructure\CommandNames.cs");
			Helpers.CreateFile(_configuration.InputPath + @"UI\Infrastructure\EventNamesTemplate.cs", _configuration.OutputPath + @"UI\Infrastructure\EventNames.cs");
			Helpers.CreateFile(_configuration.InputPath + @"UI\Infrastructure\ParameterNamesTemplate.cs", _configuration.OutputPath + @"UI\Infrastructure\ParameterNames.cs");
			Helpers.CreateFile(_configuration.InputPath + @"UI\Infrastructure\ViewNamesTemplate.cs", _configuration.OutputPath + @"UI\Infrastructure\ViewNames.cs", new[] { viewNamesContent });
		}

		/// <summary>
		/// Creates the UI.
		/// </summary>
		public void CreateUi()
		{
			string masterViewContent = string.Empty;

			foreach (PlusDataItem dataItem in _configuration.DataLayout)
			{
				//Detail
				string detailViewContent = string.Empty;
				string yesNoConverterString = ", Converter={StaticResource BoolToLocalizedYesNoConverterConverter}";

				detailViewContent += "<plus:PlusGroupBox Header=\"" + GetLocalizedString(dataItem.Name, true) + "\">";
				detailViewContent += "    <StackPanel>";

				foreach (PlusDataItemProperty plusDataObject in dataItem.Properties)
				{
					detailViewContent += "        <plus:PlusFormRow Label=\"" + GetLocalizedString(dataItem.Name + plusDataObject.Name) + "\">\r\n";

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

				Helpers.CreateFile(_configuration.InputPath + @"UI\Regions\Detail\DetailViewModelTemplate.cs",
					_configuration.OutputPath + @"UI\Regions\Detail\" + dataItem.Name + "DetailViewModel.cs", null, dataItem.Name);
				Helpers.CreateFile(_configuration.InputPath + @"UI\Regions\Detail\DetailViewTemplate.xaml",
					_configuration.OutputPath + @"UI\Regions\Detail\" + dataItem.Name + "DetailView.xaml", new[] { detailViewContent }, dataItem.Name);
				Helpers.CreateFile(_configuration.InputPath + @"UI\Regions\Detail\DetailViewTemplate.xaml.cs",
					_configuration.OutputPath + @"UI\Regions\Detail\" + dataItem.Name + "DetailView.xaml.cs", null, dataItem.Name);


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

			switch (_configuration.Template)
			{
				case TemplateMode.One:
					{
						if (_configuration.DataLayout.Count == 1 && _configuration.DataLayout.Any(t => t.CanEditMultiple))
						{
							masterViewModelPath = _configuration.InputPath + @"UI\Regions\Master\MasterViewModelMultiTemplate.cs";
						}
						else
						{
							masterViewModelPath = _configuration.InputPath + @"UI\Regions\Master\MasterViewModelTemplate.cs";
						}
						break;
					}
				case TemplateMode.MasterDetail:
					{
						masterViewModelPath = _configuration.InputPath + @"UI\Regions\Master\MasterViewModelMasterDetailTemplate.cs";
						break;
					}
			}

			Helpers.CreateFile(masterViewModelPath, _configuration.OutputPath + @"UI\Regions\Master\" + _configuration.DialogName + @"MasterViewModel.cs");
			Helpers.CreateFile(_configuration.InputPath + @"UI\Regions\Master\MasterViewTemplate.xaml", _configuration.OutputPath + @"UI\Regions\Master\" + _configuration.DialogName + @"MasterView.xaml", new[] { masterViewContent });
			Helpers.CreateFile(_configuration.InputPath + @"UI\Regions\Master\MasterViewTemplate.xaml.cs", _configuration.OutputPath + @"UI\Regions\Master\" + _configuration.DialogName + @"MasterView.xaml.cs");
		}

		private string GetLocalizedString(string input, bool multi = false)
		{
			string extension = multi ? "s" : string.Empty;
			return "{localization:Localize Key=" + _configuration.Product + _configuration.DialogName + "_lbl" + input + extension + ", Source=" + _configuration.Product +
			       "Localizer}";
		}

	}
}

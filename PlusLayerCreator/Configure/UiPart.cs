using System;
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
		private readonly string _masterGridTemplate;
	    private readonly string _masterGridMultiTemplate;

        public UiPart(Configuration configuration)
		{
			_configuration = configuration;
			_masterGridTemplate = File.ReadAllText(configuration.InputPath + @"UI\Regions\Master\MasterGridPart.txt");
		    _masterGridMultiTemplate = File.ReadAllText(configuration.InputPath + @"UI\Regions\Master\MasterGridMultiPart.txt");
        }

		public void CreateUiFilter()
		{
			string filterViewModelTemplate1 = "public FilterViewModel(IViewModelBaseServices baseServices";
			string filterViewModelTemplate2 = "): base(baseServices)\r\n" +
												"{\r\n" +
												"DisplayName = GlobalLocalizer.Singleton.Global_lblFilter.Translation;\r\n";
			string filterViewModelTemplate3 = "}";
		    string filterChildViewModelsContent = string.Empty;

            string filterViewModelConstructionContent = string.Empty;
			string filterViewModelInitializationContent = string.Empty;

			string filterViewContent = string.Empty;
			string filterChildViewModelContent = string.Empty;

			//filterMembersContent
			foreach (ConfigurationItem dataItem in _configuration.DataLayout)
			{
				string filterMembersContent = string.Empty;
				string filterPredicatesContent = string.Empty;
				string filterPredicateResetContent = string.Empty;
				string filterMultiSelectorsInitializeContent = string.Empty;
				string filterPropertiesContent = string.Empty;

			    filterChildViewModelsContent += "\r\npublic " + _configuration.Product + dataItem.Name + "DataItemFilterViewModel " + _configuration.Product + dataItem.Name + "DataItemFilterViewModel { get; set; }";
			    filterViewContent += "<StackPanel DataContext=\"{Binding " + _configuration.Product + dataItem.Name + "DataItemFilterViewModel}\">\r\n";
                foreach (ConfigurationProperty plusDataObject in dataItem.Properties)
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
						filterPropertiesContent += Helpers.DoReplaces2(propertyFilterPropertiesContent, plusDataObject.Name, dataItem.Name);
						filterViewContent += Helpers.DoReplaces2(propertyFilterXamlContent, plusDataObject.Name, dataItem.Name, plusDataObject.Type);
					}
				}

			    filterViewContent += "</StackPanel>\r\n";

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
											filterViewModelInitializationContent + filterViewModelTemplate3 + filterChildViewModelsContent;

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

			foreach (ConfigurationItem dataItem in _configuration.DataLayout)
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

	    public void CreateUiToolbar()
	    {
	        string toolbarViewContent = string.Empty;
	        foreach (ConfigurationItem dataItem in _configuration.DataLayout.Where(t => string.IsNullOrEmpty(t.Parent) && t.IsPreFilterItem == false))
	        {
	            if (dataItem.CanRead)
	            {
	                toolbarViewContent +=
	                    File.ReadAllText(_configuration.InputPath + @"UI\Regions\Toolbar\ToolbarRefreshButtonPart.txt") + "\r\n";
	            }
	            if (dataItem.CanEdit)
	            {
	                toolbarViewContent +=
	                    Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"UI\Regions\Toolbar\ToolbarAddButtonPart.txt") + "\r\n", dataItem.Name);
	                toolbarViewContent +=
	                    File.ReadAllText(_configuration.InputPath + @"UI\Regions\Toolbar\ToolbarSaveButtonPart.txt") + "\r\n";
                }
	            if (dataItem.CanClone)
	            {
	                toolbarViewContent +=
	                    File.ReadAllText(_configuration.InputPath + @"UI\Regions\Toolbar\ToolbarCopyButtonPart.txt") + "\r\n";
	            }
	            if (dataItem.CanDelete)
	            {
	                toolbarViewContent +=
	                    File.ReadAllText(_configuration.InputPath + @"UI\Regions\Toolbar\ToolbarDeleteButtonPart.txt") + "\r\n";
	            }
	            if (dataItem.CanEdit)
	            {
	                toolbarViewContent +=
	                    File.ReadAllText(_configuration.InputPath + @"UI\Regions\Toolbar\ToolbarCancelButtonPart.txt") + "\r\n";
	            }
            }

	        Helpers.CreateFile(_configuration.InputPath + @"UI\Regions\Toolbar\ToolbarViewTemplate.xaml.cs", _configuration.OutputPath + @"UI\Regions\Toolbar\ToolbarViewModel.cs");
	        Helpers.CreateFile(_configuration.InputPath + @"UI\Regions\Toolbar\ToolbarViewTemplate.xaml", _configuration.OutputPath + @"UI\Regions\Toolbar\ToolbarView.xaml", new[] { toolbarViewContent });
	        Helpers.CreateFile(_configuration.InputPath + @"UI\Regions\Toolbar\ToolbarViewTemplate.xaml.cs", _configuration.OutputPath + @"UI\Regions\Toolbar\ToolbarView.xaml.cs");
        }

        /// <summary>
        /// Creates the UI.
        /// </summary>
        public void CreateUi()
		{
			string masterViewContent = string.Empty;

			foreach (ConfigurationItem dataItem in _configuration.DataLayout)
			{
				//Detail
				string detailViewContent = string.Empty;
				string yesNoConverterString = ", Converter={StaticResource BoolToLocalizedYesNoConverterConverter}";

				detailViewContent += "<plus:PlusGroupBox Header=\"" + GetLocalizedString(dataItem.Name, true) + "\">";
				detailViewContent += "    <StackPanel>";

				foreach (ConfigurationProperty plusDataObject in dataItem.Properties)
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
							detailViewContent += "            <plus:PlusCheckBox " + addintionalInformation +
												 " IsChecked=\"{Binding DataItem." + plusDataObject.Name + "}\" />\r\n";
						}
						else if (plusDataObject.Type == "DateTime")
						{
						    detailViewContent += Helpers.DoReplaces2(File.ReadAllText(_configuration.InputPath + @"UI\Regions\Detail\DetailDateTimePickerXaml.txt"), "DataItem." + plusDataObject.Name);
						}
                        else
						{
							if (plusDataObject.Length != string.Empty)
							{
								addintionalInformation += " MaxLength =\"" + plusDataObject.Length + "\"";
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
			    
				string gridContent = Helpers.DoReplaces(dataItem.CanEditMultiple ? _masterGridMultiTemplate : _masterGridTemplate, dataItem.Name);
				string columnsContent = string.Empty;
				foreach (ConfigurationProperty plusDataObject in dataItem.Properties)
				{
					if (plusDataObject.Type == "bool")
					{
						columnsContent += "                <plus:PlusGridViewCheckColumn Width=\"*\" DataMemberBinding=\"{Binding " +
											 plusDataObject.Name + "}\" Header=\"" + GetLocalizedString(dataItem.Name + plusDataObject.Name) + "\"/>\r\n";
					}
					else
					{
						columnsContent += "                <plus:PlusGridViewTextColumnMinimal Width=\"*\" DataMemberBinding=\"{Binding " +
							plusDataObject.Name + "}\" Header=\"" + GetLocalizedString(dataItem.Name + plusDataObject.Name) + "\"/>\r\n";
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

		    string dataItemName = string.Empty;

		    if (_configuration.Template == TemplateMode.One)
		    {
		        ConfigurationItem dataItem = _configuration.DataLayout
		            .Where(t => string.IsNullOrEmpty(t.Parent) && t.IsPreFilterItem == false).FirstOrDefault();
		        if (dataItem != null)
		        {
		            dataItemName = dataItem.Name;
		        }
		    }

			Helpers.CreateFile(masterViewModelPath, _configuration.OutputPath + @"UI\Regions\Master\" + _configuration.DialogName + @"MasterViewModel.cs", new []{""}, dataItemName);
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

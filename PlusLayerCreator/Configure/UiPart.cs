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
		private readonly string _masterGridNoResourcesTemplate;
		private readonly string _masterGridVersionTemplate;
	    private readonly string _masterGridMultiTemplate;

        public UiPart(Configuration configuration)
		{
			_configuration = configuration;
			_masterGridTemplate = File.ReadAllText(configuration.InputPath + @"UI\Regions\Master\MasterGridPart.txt");
		    _masterGridNoResourcesTemplate = File.ReadAllText(configuration.InputPath + @"UI\Regions\Master\MasterGridNoResourcesPart.txt");
			_masterGridVersionTemplate = File.ReadAllText(configuration.InputPath + @"UI\Regions\Master\MasterVersionGridPart.txt");
		    _masterGridMultiTemplate = File.ReadAllText(configuration.InputPath + @"UI\Regions\Master\MasterGridMultiPart.txt");
        }

	    public void CreatePlusDialogInfrastructure()
	    {
	        Helpers.CreateFileFromPath(_configuration.InputPath + @"PlusDialog\DialogControllerHandleTemplate.txt", _configuration.OutputPath + @"PlusDialog\DialogControllerHandle.cs");
	        Helpers.CreateFileFromPath(_configuration.InputPath + @"PlusDialog\PlusMainControllerTemplate.txt", _configuration.OutputPath + @"PlusDialog\PlusMainController.cs");
	        Helpers.CreateFileFromPath(_configuration.InputPath + @"PlusDialog\PlusMainFormTemplate.txt", _configuration.OutputPath + @"PlusDialog\PlusMainForm.cs");
        }


        public void CreateLauncherConfigEntry()
	    {
	        string entry = "<Product Name=\"" + _configuration.Product + "\">\r\n";
	        entry += "<Module Handle=\"" + _configuration.ControllerHandle + "\" Name=\"" + _configuration.DialogName + "\" />\r\n";

	        if (_configuration.IsUseBusinessServiceWithoutBo)
	        {
	            foreach (ConfigurationItem configurationItem in _configuration.DataLayout)
	            {
	                entry += "<Dependency iface=\"I" + _configuration.Product + configurationItem.Name +
	                         "Service\" impl=\"" + _configuration.Product + configurationItem.Name + "Mock\" />\r\n";

	            }
	        }

	        entry += "</Product>";
            
	        FileInfo fileInfo = new FileInfo(_configuration.OutputPath + "PlusLauncher.xml");
	        if (fileInfo.Directory != null)
	        {
	            fileInfo.Directory.Create();
	        }
	        File.WriteAllText(fileInfo.FullName, entry);
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
			foreach (ConfigurationItem dataItem in _configuration.DataLayout.Where(t => !t.Name.EndsWith("Version")))
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

						Helpers.CreateFilterContents(dataItem, plusDataObject, out propertyFilterMembersContent,
							out propertyFilterPropertiesContent, out propertyFilterPredicateResetContent,
							out propertyFilterPredicatesContent, out propertyFilterXamlContent,
							out propertyFilterMultiSelectorsInitializeContent);

						filterMembersContent += Helpers.DoReplaces2(propertyFilterMembersContent, plusDataObject.Name, dataItem, plusDataObject.Type);
						filterPredicatesContent += Helpers.DoReplaces2(propertyFilterPredicatesContent, plusDataObject.Name, dataItem, plusDataObject.Type);
						filterPredicateResetContent += Helpers.DoReplaces2(propertyFilterPredicateResetContent, plusDataObject.Name, dataItem, plusDataObject.Type);
						filterMultiSelectorsInitializeContent += Helpers.DoReplaces2(propertyFilterMultiSelectorsInitializeContent, plusDataObject.Name, dataItem, plusDataObject.Type);
						filterPropertiesContent += Helpers.DoReplaces2(propertyFilterPropertiesContent, plusDataObject.Name, dataItem);
						filterViewContent += Helpers.DoReplaces2(propertyFilterXamlContent, plusDataObject.Name, dataItem, plusDataObject.Type);
					}
				}

			    filterViewContent += "</StackPanel>\r\n";

                string childViewModel = Helpers.FilterChildViewModelTemplate;
				childViewModel = Helpers.DoReplaces(childViewModel, dataItem);
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

			Helpers.CreateFileFromPath(_configuration.InputPath + @"UI\Regions\Filter\FilterViewTemplate.xaml",
				_configuration.OutputPath + @"UI\Regions\Filter\FilterView.xaml", new[] { filterViewContent });
			Helpers.CreateFileFromPath(_configuration.InputPath + @"UI\Regions\Filter\FilterViewTemplate.xaml.cs",
				_configuration.OutputPath + @"UI\Regions\Filter\FilterView.xaml.cs");

			Helpers.CreateFileFromPath(_configuration.InputPath + @"UI\Regions\Filter\FilterViewModelTemplate.cs",
				_configuration.OutputPath + @"UI\Regions\Filter\FilterViewModel.cs", new[] { filterViewModelContent, filterChildViewModelContent });
		}

		public void CreateUiInfrastructure()
		{
			string moduleContent = string.Empty;
			string moduleContent2 = string.Empty;
		    string commandNamesContent = string.Empty;
		    string eventNamesContent = string.Empty;

            string viewNamesContent = "public static readonly string " + _configuration.DialogName + "MasterView = \"" + _configuration.DialogName + "MasterView\";\r\n";

			foreach (ConfigurationItem dataItem in _configuration.DataLayout)
			{
				moduleContent += "_container.RegisterType<object, " + dataItem.Name + "DetailView>(ViewNames." + dataItem.Name + "DetailView);\r\n";

                if (!dataItem.Name.EndsWith("Version"))
                {
                    moduleContent2 += "_container.RegisterType<IFilterSourceProvider<" + _configuration.Product + dataItem.Name + "DataItem>, " +
								  _configuration.DialogName + "MasterViewModel>(new ContainerControlledLifetimeManager());\r\n";
                    
                }

				viewNamesContent += "public static readonly string " + dataItem.Name + "DetailView = \"" + dataItem.Name + "DetailView\";\r\n";

			    if (!string.IsNullOrEmpty(dataItem.Parent) && !dataItem.Name.EndsWith("Version"))
			    {
			        if (dataItem.CanDelete)
			        {
			            commandNamesContent += "public static readonly string Delete" + dataItem.Name + "Command = \"" + dataItem.Name + "DeleteCommand\";\r\n";
			        }
			        if (dataItem.CanEdit)
			        {
			            commandNamesContent += "public static readonly string Add" + dataItem.Name + "Command = \"" + dataItem.Name + "AddCommand\";\r\n";
			        }
			        if (dataItem.CanSort)
			        {
			            commandNamesContent += "public static readonly string Sort" + dataItem.Name + "UpCommand = \"" + dataItem.Name + "SortUpCommand\";\r\n";
			            commandNamesContent += "public static readonly string Sort" + dataItem.Name + "DownCommand = \"" + dataItem.Name + "SortDownCommand\";\r\n";
			        }
			    }

			    if (dataItem.Name.EndsWith("Version"))
			    {
			        eventNamesContent += "public static string VersionActivationChanged = \"VersionActivationChanged\";\r\n";

			    }
            }

			Helpers.CreateFileFromPath(_configuration.InputPath + @"UI\ModuleTemplate.cs", _configuration.OutputPath + @"UI\" + _configuration.DialogName + @"Module.cs", new[] { moduleContent, moduleContent2 });
			Helpers.CreateFileFromPath(_configuration.InputPath + @"UI\ControllerTemplate.cs", _configuration.OutputPath + @"UI\" + _configuration.DialogName + @"Controller.cs");
			Helpers.CreateFileFromPath(_configuration.InputPath + @"UI\ViewModelTemplate.cs", _configuration.OutputPath + @"UI\" + _configuration.DialogName + @"ViewModel.cs");
			Helpers.CreateFileFromPath(_configuration.InputPath + @"UI\ViewTemplate.xaml", _configuration.OutputPath + @"UI\" + _configuration.DialogName + @"View.xaml");
			Helpers.CreateFileFromPath(_configuration.InputPath + @"UI\ViewTemplate.xaml.cs", _configuration.OutputPath + @"UI\" + _configuration.DialogName + @"View.xaml.cs");
			Helpers.CreateFileFromPath(_configuration.InputPath + @"UI\WindowTemplate.xaml", _configuration.OutputPath + @"UI\" + _configuration.DialogName + @"Window.xaml");
			Helpers.CreateFileFromPath(_configuration.InputPath + @"UI\WindowTemplate.xaml.cs", _configuration.OutputPath + @"UI\" + _configuration.DialogName + @"Window.xaml.cs");
            Helpers.CreateFileFromPath(_configuration.InputPath + @"UI\Infrastructure\BootstrapperTemplate.cs", _configuration.OutputPath + @"UI\Infrastructure\Bootstrapper.cs");
            Helpers.CreateFileFromPath(_configuration.InputPath + @"UI\Infrastructure\CommandNamesTemplate.cs", _configuration.OutputPath + @"UI\Infrastructure\CommandNames.cs", new []{ commandNamesContent });
			Helpers.CreateFileFromPath(_configuration.InputPath + @"UI\Infrastructure\EventNamesTemplate.cs", _configuration.OutputPath + @"UI\Infrastructure\EventNames.cs", new[] { eventNamesContent });
            Helpers.CreateFileFromPath(_configuration.InputPath + @"UI\Infrastructure\ParameterNamesTemplate.cs", _configuration.OutputPath + @"UI\Infrastructure\ParameterNames.cs");
			Helpers.CreateFileFromPath(_configuration.InputPath + @"UI\Infrastructure\ViewNamesTemplate.cs", _configuration.OutputPath + @"UI\Infrastructure\ViewNames.cs", new[] { viewNamesContent });
		}

	    public void CreateUiStatusbar()
	    {
	        Helpers.CreateFileFromPath(_configuration.InputPath + @"UI\Regions\Statusbar\StatusbarViewModelTemplate.cs", _configuration.OutputPath + @"UI\Regions\Statusbar\StatusbarViewModel.cs");
	        Helpers.CreateFileFromPath(_configuration.InputPath + @"UI\Regions\Statusbar\StatusbarViewTemplate.xaml", _configuration.OutputPath + @"UI\Regions\Statusbar\StatusbarView.xaml");
	        Helpers.CreateFileFromPath(_configuration.InputPath + @"UI\Regions\Statusbar\StatusbarViewTemplate.xaml.cs", _configuration.OutputPath + @"UI\Regions\Statusbar\StatusbarView.xaml.cs");
        }


        public void CreateUiToolbar()
	    {
	        string toolbarViewContent = string.Empty;

	        toolbarViewContent +=
	            File.ReadAllText(_configuration.InputPath + @"UI\Regions\Toolbar\ToolbarRefreshButtonPart.txt") + "\r\n";

            foreach (ConfigurationItem dataItem in _configuration.DataLayout.Where(t => string.IsNullOrEmpty(t.Parent) && t.IsPreFilterItem == false || t.Name.EndsWith("Version")))
	        {
	            if (dataItem.CanEdit)
	            {
	                toolbarViewContent +=
	                    Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"UI\Regions\Toolbar\ToolbarAddButtonPart.txt") + "\r\n", dataItem);
                }
            }

	        if (_configuration.DataLayout.Any(t => t.CanEdit))
	        {
	            toolbarViewContent +=
	                File.ReadAllText(_configuration.InputPath + @"UI\Regions\Toolbar\ToolbarSaveButtonPart.txt") + "\r\n";
	        }

            if (_configuration.DataLayout.Any(t => t.CanClone))
	        {
	            toolbarViewContent +=
	                File.ReadAllText(_configuration.InputPath + @"UI\Regions\Toolbar\ToolbarCopyButtonPart.txt") + "\r\n";
	        }
	        if (_configuration.DataLayout.Any(t => t.CanDelete))
	        {
	            toolbarViewContent +=
	                File.ReadAllText(_configuration.InputPath + @"UI\Regions\Toolbar\ToolbarDeleteButtonPart.txt") + "\r\n";
	        }
	        if (_configuration.DataLayout.Any(t => t.CanEdit))
	        {
	            toolbarViewContent +=
	                File.ReadAllText(_configuration.InputPath + @"UI\Regions\Toolbar\ToolbarCancelButtonPart.txt") + "\r\n";
	        }

            Helpers.CreateFileFromPath(_configuration.InputPath + @"UI\Regions\Toolbar\ToolbarViewModelTemplate.cs", _configuration.OutputPath + @"UI\Regions\Toolbar\ToolbarViewModel.cs");
	        Helpers.CreateFileFromPath(_configuration.InputPath + @"UI\Regions\Toolbar\ToolbarViewTemplate.xaml", _configuration.OutputPath + @"UI\Regions\Toolbar\ToolbarView.xaml", new[] { toolbarViewContent });
	        Helpers.CreateFileFromPath(_configuration.InputPath + @"UI\Regions\Toolbar\ToolbarViewTemplate.xaml.cs", _configuration.OutputPath + @"UI\Regions\Toolbar\ToolbarView.xaml.cs");
        }

	    private void CreateUiDetail()
	    {
	        foreach (ConfigurationItem dataItem in _configuration.DataLayout)
	        {
	            string detailViewContent = string.Empty;
	            string yesNoConverterString = ", Converter={StaticResource BoolToLocalizedYesNoConverterConverter}";

	            if (!string.IsNullOrEmpty(dataItem.Parent))
	            {
	                var parent = _configuration.DataLayout.FirstOrDefault(t => t.Name == dataItem.Parent);
                    
                    // Parent key fields
                    detailViewContent += "<plus:PlusGroupBox Header=\"" + GetLocalizedString(dataItem.Parent) + "\">";
	                detailViewContent += "    <StackPanel>";

	                foreach (ConfigurationProperty property in parent.Properties)
	                {
	                    detailViewContent += "        <plus:PlusFormRow Label=\"" +
	                                         GetLocalizedString(parent.Name + property.Name) + "\">\r\n";
	                    if (property.Type == "bool")
	                    {
	                        detailViewContent += "            <plus:PlusLabel Content=\"{Binding DataItem.Parent.Parent." +
	                                             property.Name +
	                                             yesNoConverterString + "}\" />\r\n";
	                    }
	                    else
	                    {
	                        detailViewContent += "            <plus:PlusLabel Content=\"{Binding DataItem.Parent.Parent." +
	                                             property.Name +
	                                             "}\" />\r\n";
	                    }
                        detailViewContent += "        </plus:PlusFormRow>\r\n";
                    }
	                detailViewContent += "    </StackPanel>";
	                detailViewContent += "</plus:PlusGroupBox>";
                }

                detailViewContent += "<plus:PlusGroupBox Header=\"" + GetLocalizedString(dataItem.Name, true) + "\">";
	            detailViewContent += "    <StackPanel>";

	            if (dataItem.Name.EndsWith("Version"))
	            {
	                detailViewContent += File.ReadAllText(_configuration.InputPath + @"UI\Regions\Detail\VersionDetailButtonXaml.txt");
                }

                foreach (ConfigurationProperty property in dataItem.Properties)
	            {
	                if (dataItem.Name.EndsWith("Version") && property.Name == "IsActive")
	                {
	                    continue;
	                }

	                detailViewContent += "        <plus:PlusFormRow Label=\"" +
	                                     GetLocalizedString(dataItem.Name + property.Name) + "\">\r\n";

	                if (!dataItem.CanEdit)
	                {
	                    if (property.Type == "bool")
	                    {
	                        detailViewContent += "            <plus:PlusLabel Content=\"{Binding DataItem." +
	                                             property.Name +
	                                             yesNoConverterString + "}\" />\r\n";
	                    }
	                    else
	                    {
	                        detailViewContent += "            <plus:PlusLabel Content=\"{Binding DataItem." +
	                                             property.Name +
	                                             "}\" />\r\n";
	                    }
	                }
	                else
	                {
	                    string addintionalInformation = string.Empty;
	                    if (property.IsKey)
	                    {
	                        addintionalInformation += _keyReadOnlyTemplate;
	                    }
	                    else if (property.IsReadOnly)
	                    {
	                        addintionalInformation += _readOnlyTemplate;
	                    }

	                    if (property.Type == "bool")
	                    {
	                        detailViewContent += "            <plus:PlusCheckBox " + addintionalInformation +
	                                             " IsChecked=\"{Binding DataItem." + property.Name + "}\" />\r\n";
	                    }
	                    else if (property.Type == "DateTime")
	                    {
	                        detailViewContent +=
	                            Helpers.DoReplaces2(
	                                File.ReadAllText(_configuration.InputPath +
	                                                 @"UI\Regions\Detail\DetailDateTimePickerXaml.txt"),
	                                "DataItem." + property.Name);
	                    }
	                    else
	                    {
	                        if (property.Length != string.Empty)
	                        {
	                            addintionalInformation += " MaxLength =\"" + property.Length + "\"";
	                        }

	                        if (property.Type == "int")
	                        {
	                            addintionalInformation += _isNumericTemplate;
	                        }

	                        detailViewContent += "            <plus:PlusTextBox" + addintionalInformation +
	                                             " Text=\"{Binding DataItem." +
	                                             property.Name +
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

	            if (dataItem.Name.EndsWith("Version"))
	            {
	                Helpers.CreateFileFromPath(_configuration.InputPath + @"UI\Regions\Detail\VersionDetailViewModelTemplate.cs",
	                    _configuration.OutputPath + @"UI\Regions\Detail\" + dataItem.Name + "DetailViewModel.cs", null,
	                    dataItem);
                }
	            else
	            {
	                Helpers.CreateFileFromPath(_configuration.InputPath + @"UI\Regions\Detail\DetailViewModelTemplate.cs",
	                    _configuration.OutputPath + @"UI\Regions\Detail\" + dataItem.Name + "DetailViewModel.cs", null,
	                    dataItem);
                }
                Helpers.CreateFileFromPath(_configuration.InputPath + @"UI\Regions\Detail\DetailViewTemplate.xaml",
	                _configuration.OutputPath + @"UI\Regions\Detail\" + dataItem.Name + "DetailView.xaml",
	                new[] {detailViewContent}, dataItem);
	            Helpers.CreateFileFromPath(_configuration.InputPath + @"UI\Regions\Detail\DetailViewTemplate.xaml.cs",
	                _configuration.OutputPath + @"UI\Regions\Detail\" + dataItem.Name + "DetailView.xaml.cs", null,
	                dataItem);
	        }
	    }

	    private string GetGridXaml(ConfigurationItem dataItem, bool old = false)
	    {
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
	                if (old)
	                {
	                    columnsContent += "                <plus:PlusGridViewTextColumn Width=\"*\" DataMemberBinding=\"{Binding " +
	                                      plusDataObject.Name + "}\" Header=\"" + GetLocalizedString(dataItem.Name + plusDataObject.Name) + "\"/>\r\n";
                    }
	                else
	                {
	                    columnsContent += "                <plus:PlusGridViewTextColumnMinimal Width=\"*\" DataMemberBinding=\"{Binding " +
	                                      plusDataObject.Name + "}\" Header=\"" + GetLocalizedString(dataItem.Name + plusDataObject.Name) + "\"/>\r\n";
                    }
	            }
	        }

	        return columnsContent;
	    }

	    private void CreateUiViewMaster()
	    {
	        string masterViewContent = string.Empty;
            string masterViewCodeBehindContent = string.Empty;

            // One
	        if (_configuration.DataLayout.Count == 1)
	        {
	            ConfigurationItem dataItem = _configuration.DataLayout.FirstOrDefault();
                string gridContent = Helpers.DoReplaces(dataItem.CanEditMultiple ? _masterGridMultiTemplate : _masterGridTemplate, dataItem);
	            masterViewContent += gridContent.Replace("$specialContent1$", GetGridXaml(dataItem));
	            masterViewCodeBehindContent += "viewModel.ColumnProvider = Filtered" + dataItem.Name + "sGridView;\r\n\r\n";
	        }

            // Master/Detail
	        if (_configuration.DataLayout.Count == 2 && _configuration.DataLayout.Count(t => string.IsNullOrEmpty(t.Parent)) == 1 && _configuration.DataLayout.All(t => !t.Name.EndsWith("Version")))
	        {
	            ConfigurationItem master = _configuration.DataLayout.FirstOrDefault(t => string.IsNullOrEmpty(t.Parent));
	            ConfigurationItem detail = _configuration.DataLayout.FirstOrDefault(t => !string.IsNullOrEmpty(t.Parent));

	            masterViewCodeBehindContent += "viewModel.ColumnProvider = Filtered" + master.Name + "sGridView;\r\n\r\n";
	            masterViewCodeBehindContent += "viewModel.ColumnProvider = Filtered" + detail.Name + "sGridView;\r\n\r\n";

                masterViewContent += File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\MasterDetailLayout.txt") + "\r\n\r\n";
	            string gridContent = Helpers.DoReplaces(_masterGridNoResourcesTemplate, master);
	            masterViewContent += gridContent.Replace("$specialContent1$", GetGridXaml(master));
	            if (detail.CanEditMultiple)
	            {
	                string commandButtonsContent = string.Empty;
	                if (detail.CanSort)
	                {
	                    commandButtonsContent = Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\ChildGridCommandWithSortButtons.txt"), detail);
                    }
	                else
	                {
	                    commandButtonsContent = Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\ChildGridCommandButtons.txt"), detail);
                    }
	                gridContent = Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\ChildGridMultiPart.txt") + "\r\n\r\n", detail);
	                gridContent = gridContent.Replace("$specialContent2$", commandButtonsContent);
	            }
	            else
	            {
	                gridContent = Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\ChildGridPart.txt") + "\r\n\r\n", detail);
                }
	            masterViewContent += gridContent.Replace("$specialContent1$", GetGridXaml(detail));
            }

		    if (_configuration.DataLayout.Count == 2 && _configuration.DataLayout.Count(t => string.IsNullOrEmpty(t.Parent)) == 1 && _configuration.DataLayout.Any(t => t.Name.EndsWith("Version")))
		    {
			    ConfigurationItem master = _configuration.DataLayout.FirstOrDefault(t => string.IsNullOrEmpty(t.Parent));
			    ConfigurationItem detail = _configuration.DataLayout.FirstOrDefault(t => t.Name.EndsWith("Version"));
			    masterViewCodeBehindContent += "viewModel.ColumnProvider = Filtered" + master.Name + "sGridView;\r\n\r\n";

			    string gridContent = Helpers.DoReplaces(_masterGridVersionTemplate, master);
			    masterViewContent += gridContent.Replace("$specialContent1$", GetGridXaml(master, true)).Replace("$specialContent2$", GetGridXaml(detail, true));
		    }

		    Helpers.CreateFileFromPath(_configuration.InputPath + @"UI\Regions\Master\MasterViewTemplate.xaml", _configuration.OutputPath + @"UI\Regions\Master\" + _configuration.DialogName + @"MasterView.xaml", new[] { masterViewContent });
	        Helpers.CreateFileFromPath(_configuration.InputPath + @"UI\Regions\Master\MasterViewTemplate.xaml.cs", _configuration.OutputPath + @"UI\Regions\Master\" + _configuration.DialogName + @"MasterView.xaml.cs", new[] { masterViewCodeBehindContent});
        }

	    private void CreateUiMasterViewModel()
	    {
	        string masterViewModelPath = string.Empty;

	        if (_configuration.DataLayout.Count == 1)
	        {
	            if (_configuration.DataLayout.Any(t => t.CanEditMultiple))
	            {
	                masterViewModelPath = _configuration.InputPath + @"UI\Regions\Master\MasterViewModelMultiTemplate.cs";
	            }
	            else if (_configuration.DataLayout.Any(t => !t.CanEdit))
	            {
	                masterViewModelPath = _configuration.InputPath + @"UI\Regions\Master\MasterViewModelReadTemplate.cs";
	            }
	            else
	            {
	                masterViewModelPath = _configuration.InputPath + @"UI\Regions\Master\MasterViewModelTemplate.cs";
	            }

	            ConfigurationItem configurationItem = _configuration.DataLayout
	                .Where(t => string.IsNullOrEmpty(t.Parent) && t.IsPreFilterItem == false).FirstOrDefault();

	            Helpers.CreateFileFromPath(masterViewModelPath,
	                _configuration.OutputPath + @"UI\Regions\Master\" + _configuration.DialogName + @"MasterViewModel.cs",
	                new[] { "" }, configurationItem);
            }
	        else
	        {
	            if (_configuration.DataLayout.Any(t => t.Name.EndsWith("Version")))
	            {
	                masterViewModelPath = _configuration.InputPath + @"UI\Regions\Master\MasterViewModelVersionTemplate.cs";
	            }
                else if (_configuration.DataLayout.Any(t => !string.IsNullOrEmpty(t.Parent)))
	            {
	                masterViewModelPath = _configuration.InputPath + @"UI\Regions\Master\MasterViewModelMasterDetailTemplate.cs";
	            }

	            ConfigurationItem master = _configuration.DataLayout.FirstOrDefault(t => string.IsNullOrEmpty(t.Parent));
	            ConfigurationItem detail = _configuration.DataLayout.FirstOrDefault(t => !string.IsNullOrEmpty(t.Parent));
	            string fileContent = Helpers.DoReplaces(File.ReadAllText(masterViewModelPath).Replace("$MasterDataItem$", master.Name).Replace("$masterDataItem$", Helpers.ToPascalCase(master.Name)).Replace("$ChildDataItem$", detail.Name).Replace("$childDataItem$", Helpers.ToPascalCase(detail.Name)));

	            FileInfo fileInfo = new FileInfo(_configuration.OutputPath + @"UI\Regions\Master\" + _configuration.DialogName + @"MasterViewModel.cs");
	            if (fileInfo.Directory != null)
	            {
	                fileInfo.Directory.Create();
	            }
	            File.WriteAllText(fileInfo.FullName, fileContent);
            }
	    }

	    /// <summary>
        /// Creates the UI.
        /// </summary>
        public void CreateUi()
		{
			CreateUiDetail();
		    CreateUiViewMaster();
            CreateUiMasterViewModel();
        }

		private string GetLocalizedString(string input, bool multi = false)
		{
			string extension = multi ? "s" : string.Empty;
			return "{localization:Localize Key=" + _configuration.Product + _configuration.DialogName + "_lbl" + input + extension + ", Source=" + _configuration.Product +
			       "Localizer}";
		}
	}
}

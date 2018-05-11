using System;
using System.IO;
using System.Linq;
using PlusLayerCreator.Items;

namespace PlusLayerCreator.Configure
{
	public class UiPart
	{
		private Configuration _configuration;
		private string _keyReadOnlyTemplate = " IsReadOnly=\"{Binding IsNewItem, Converter={StaticResource InvertBoolConverter}}\"";
		private string _readOnlyTemplate = " IsReadOnly=\"True\"";
		private string _isNumericTemplate = " IsNumeric=\"True\"";
		private readonly string _masterGridTemplate;
		private readonly string _masterGridReadonlyTemplate;
		private readonly string _masterGridVersionTemplate;
	    private readonly string _masterGridVersionResourcesTemplate;
	    private readonly string _masterGridMultiTemplate;

        public UiPart(Configuration configuration)
		{
			_configuration = configuration;
			_masterGridTemplate = File.ReadAllText(configuration.InputPath + @"UI\Regions\Master\Grid\MasterGridPart.txt");
		    _masterGridReadonlyTemplate = File.ReadAllText(configuration.InputPath + @"UI\Regions\Master\Grid\MasterGridReadonlyPart.txt");
            _masterGridMultiTemplate = File.ReadAllText(configuration.InputPath + @"UI\Regions\Master\Grid\MasterGridMultiPart.txt");
		    _masterGridVersionTemplate = File.ReadAllText(configuration.InputPath + @"UI\Regions\Master\Grid\MasterVersionGridPart.txt");
		    _masterGridVersionResourcesTemplate = File.ReadAllText(configuration.InputPath + @"UI\Regions\Master\Grid\MasterVersionGridResourcesPart.txt");
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
					    Helpers.CreateFilterContents(dataItem, plusDataObject, out var propertyFilterMembersContent,
							out var propertyFilterPropertiesContent, out var propertyFilterPredicateResetContent,
							out var propertyFilterPredicatesContent, out var propertyFilterXamlContent,
							out var propertyFilterMultiSelectorsInitializeContent);

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

			Helpers.CreateFileFromPath(Files.FilterViewTemplate, _configuration.OutputPath + @"UI\Regions\Filter\FilterView.xaml", new[] { filterViewContent });
			Helpers.CreateFileFromPath(Files.FilterViewCodeBehindTemplate, _configuration.OutputPath + @"UI\Regions\Filter\FilterView.xaml.cs");
            Helpers.CreateFileFromPath(Files.FilterViewModelTemplate, _configuration.OutputPath + @"UI\Regions\Filter\FilterViewModel.cs", new[] { filterViewModelContent, filterChildViewModelContent });
		}

	    public void CreateUiStatusbar()
	    {
	        Helpers.CreateFileFromPath(Files.StatusbarViewModelTemplate, _configuration.OutputPath + @"UI\Regions\Statusbar\StatusbarViewModel.cs");
	        Helpers.CreateFileFromPath(Files.StatusbarViewTemplate, _configuration.OutputPath + @"UI\Regions\Statusbar\StatusbarView.xaml");
	        Helpers.CreateFileFromPath(Files.StatusbarViewCodeBehindTemplate, _configuration.OutputPath + @"UI\Regions\Statusbar\StatusbarView.xaml.cs");
        }
        
        public void CreateUiToolbar()
	    {
	        string toolbarViewContent = string.Empty;

	        toolbarViewContent +=
	            File.ReadAllText(_configuration.InputPath + @"UI\Regions\Toolbar\ToolbarRefreshButtonPart.txt") + "\r\n";

            foreach (ConfigurationItem dataItem in _configuration.DataLayout.Where(t => string.IsNullOrEmpty(t.Parent) && t.IsPreFilterItem == false && !t.Name.EndsWith("Version")))
	        {
	            if (dataItem.CanEdit)
	            {
	                toolbarViewContent +=
	                    Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"UI\Regions\Toolbar\ToolbarAddButtonPart.txt") + "\r\n", dataItem);
                }
            }

	        foreach (ConfigurationItem dataItem in _configuration.DataLayout.Where(t => string.IsNullOrEmpty(t.Parent) && t.IsPreFilterItem == false && t.Name.EndsWith("Version")))
	        {
	            if (dataItem.CanEdit)
	            {
	                toolbarViewContent +=
	                    Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"UI\Regions\Toolbar\ToolbarAddVersionButtonPart.txt") + "\r\n", dataItem);
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

            Helpers.CreateFileFromPath(Files.ToolbarViewCodeBehindTemplate, _configuration.OutputPath + @"UI\Regions\Toolbar\ToolbarViewModel.cs");
	        Helpers.CreateFileFromPath(Files.ToolbarViewTemplate, _configuration.OutputPath + @"UI\Regions\Toolbar\ToolbarView.xaml", new[] { toolbarViewContent });
	        Helpers.CreateFileFromPath(Files.ToolbarViewCodeBehindTemplate, _configuration.OutputPath + @"UI\Regions\Toolbar\ToolbarView.xaml.cs");
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
                    detailViewContent += "<plus:PlusGroupBox Header=\"" + Helpers.GetLocalizedString(dataItem.Parent) + "\">";
	                detailViewContent += "    <StackPanel>";

	                foreach (ConfigurationProperty property in parent.Properties)
	                {
	                    detailViewContent += "        <plus:PlusFormRow Label=\"" +
	                                         Helpers.GetLocalizedString(parent.Name + property.Name) + "\">\r\n";
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

                detailViewContent += "<plus:PlusGroupBox Header=\"" + Helpers.GetLocalizedString(dataItem.Name, true) + "\">";
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
	                                     Helpers.GetLocalizedString(dataItem.Name + property.Name) + "\">\r\n";

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
	                Helpers.CreateFileFromPath(Files.VersionDetailViewModelTemplate, _configuration.OutputPath + @"UI\Regions\Detail\" + dataItem.Name + "DetailViewModel.cs", null, dataItem);
                }
	            else
	            {
	                Helpers.CreateFileFromPath(Files.DetailViewModelTemplate, _configuration.OutputPath + @"UI\Regions\Detail\" + dataItem.Name + "DetailViewModel.cs", null, dataItem);
                }
                Helpers.CreateFileFromPath(Files.DetailViewTemplate, _configuration.OutputPath + @"UI\Regions\Detail\" + dataItem.Name + "DetailView.xaml", new[] {detailViewContent}, dataItem);
	            Helpers.CreateFileFromPath(Files.DetailViewCodeBehindTemplate, _configuration.OutputPath + @"UI\Regions\Detail\" + dataItem.Name + "DetailView.xaml.cs", null, dataItem);
	        }
	    }

	    
	    private void CreateUiViewMaster()
	    {
	        int startOrder = 0;
	        int rowNumber = 0;
            string masterViewContent = string.Empty;
            string masterViewResourcesContent = string.Empty;
            string masterViewCodeBehindContent = string.Empty;

	        masterViewContent += "<Grid.RowDefinitions>\r\n";

            int layoutDependedDataItems = _configuration.DataLayout.Count(t => !t.Name.EndsWith("Version") && !t.IsPreFilterItem);
	        if (_configuration.DataLayout.Any(t => t.IsPreFilterItem))
	        {
	            masterViewContent += "<RowDefinition Height=\"Auto\">\r\n";
            }
            
	        masterViewContent += "<RowDefinition Height=\"*\" MinHeight=\"170\">\r\n";

	        for (int i = 1; i < layoutDependedDataItems; i++)
	        {
	            masterViewContent += "<RowDefinition Height=\"2\">\r\n";
	            masterViewContent += "<RowDefinition Height=\"*\" MinHeight=\"170\">\r\n";
	        }
	        masterViewContent += "</Grid.RowDefinitions>\r\n";
	        
            //Prefilter
            if (_configuration.DataLayout.Any(t => t.IsPreFilterItem))
            {
                string preFilterItemContent = string.Empty;
                string preFilterContent = File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\PreFilterPart.txt");
                foreach (var preFilterItem in _configuration.DataLayout.Where(t => t.IsPreFilterItem))
                {
                    startOrder = preFilterItem.Order + 1;
                    preFilterItemContent += Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\PreFilterItemPart.txt"), preFilterItem);
                }

                masterViewContent += Helpers.ReplaceSpecialContent(preFilterContent, new []{preFilterItemContent});
                rowNumber++;
	        }

            // One
            if (_configuration.DataLayout.Count(t => !t.IsPreFilterItem) == 1)
	        {
	            ConfigurationItem dataItem = _configuration.DataLayout.FirstOrDefault();
	            string gridTemplate;
	            if (dataItem.CanEditMultiple)
	            {
	                gridTemplate = _masterGridMultiTemplate;
	            }
	            else if (dataItem.CanEdit)
	            {
	                gridTemplate = _masterGridTemplate;
	            }
	            else
	            {
	                gridTemplate = _masterGridReadonlyTemplate;
	            }
                string gridContent = Helpers.DoReplaces(gridTemplate);
	            masterViewContent += Helpers.ReplaceSpecialContent(gridContent, new[] { GetGridXaml(dataItem), rowNumber.ToString() });
                masterViewCodeBehindContent += "viewModel.ColumnProvider = Filtered" + dataItem.Name + "GridView;\r\n\r\n";
	        }
            else
	        {
	            int nextDataItem = 0;

                if (_configuration.DataLayout.Any(t => t.Name.EndsWith("Version")))
                {

                    nextDataItem = 2;
                    ConfigurationItem version = _configuration.DataLayout.First(t => t.Name.EndsWith("Version"));
                    ConfigurationItem master = _configuration.DataLayout.First(t => t.Order == version.Order - 1);
	                
                    masterViewCodeBehindContent += "viewModel.ColumnProvider = Filtered" + master.Name + "sGridView;\r\n\r\n";

	                string gridContent = Helpers.DoReplaces(_masterGridVersionTemplate, master);
	                masterViewContent += gridContent.Replace("$specialContent1$", GetGridXaml(master, true));
	                masterViewResourcesContent = Helpers.DoReplaces(_masterGridVersionResourcesTemplate.Replace("$specialContent1$", GetGridXaml(version, true)), master);
                }
                else
                {
                    ConfigurationItem dataItem = _configuration.DataLayout.First(t => t.Order == startOrder);
                    string gridTemplate;
                    if (dataItem.CanEdit)
                    {
                        gridTemplate = _masterGridTemplate;
                    }
                    else
                    {
                        gridTemplate = _masterGridReadonlyTemplate;
                    }
                    string gridContent = Helpers.DoReplaces(gridTemplate, dataItem);
                    masterViewContent += Helpers.ReplaceSpecialContent(gridContent, new[] { GetGridXaml(dataItem), rowNumber.ToString() });
                    masterViewCodeBehindContent += "viewModel.ColumnProvider = Filtered" + dataItem.Name + "GridView;\r\n\r\n";
                    nextDataItem = dataItem.Order + 1;
                }

	            for (int i = nextDataItem; i < _configuration.DataLayout.OrderBy(t => t.Order).Count(); i++)
	            {
	                masterViewContent += Helpers.ReplaceSpecialContent(File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\Grid\GridSplitterPart.txt"), new []{ rowNumber.ToString() });
	                rowNumber++;

	                string gridContent;
	                string buttons;
	                ConfigurationItem dataItem = _configuration.DataLayout.First(t => t.Order == i);
	                var columns = GetGridXaml(dataItem);
                    if (dataItem.CanEditMultiple)
                    {
                        gridContent = Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\Grid\ChildGridMultiPart.txt"), dataItem);

                        if (dataItem.CanSort)
	                    {
	                        buttons = Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\Grid\Toolbar\ChildGridCommandWithSortButtons.txt"), dataItem);
	                    }
	                    else
	                    {
	                        buttons = Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\Grid\Toolbar\ChildGridCommandButtons.txt"), dataItem);
	                    }
	                    masterViewContent += Helpers.ReplaceSpecialContent(gridContent, new[] { columns, buttons, rowNumber.ToString() });
                    }
	                else
	                {
	                    gridContent = Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\Grid\ChildGridPart.txt"), dataItem);
                        buttons = Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\Grid\Toolbar\ChildGridCommandButtons.txt"), dataItem);
	                    masterViewContent += Helpers.ReplaceSpecialContent(gridContent, new[] { columns, buttons, rowNumber.ToString() });
                    }
	                rowNumber++;
                }
            }
            
		    Helpers.CreateFileFromPath(Files.MasterViewTemplate, _configuration.OutputPath + @"UI\Regions\Master\" + _configuration.DialogName + @"MasterView.xaml", new[] { masterViewContent, masterViewResourcesContent });
	        Helpers.CreateFileFromPath(Files.MasterViewCodeBehindTemplate, _configuration.OutputPath + @"UI\Regions\Master\" + _configuration.DialogName + @"MasterView.xaml.cs", new[] { masterViewCodeBehindContent});
        }

	    private void CreateUiMasterViewModel()
	    {
	        string masterViewModelPath = string.Empty;

	        if (_configuration.DataLayout.Count == 1)
	        {
	            if (_configuration.DataLayout.Any(t => t.CanEditMultiple))
	            {
	                masterViewModelPath = Files.MasterViewModelMultiTemplate;

	            }
	            else if (_configuration.DataLayout.Any(t => !t.CanEdit))
	            {
	                masterViewModelPath = Files.MasterViewModelReadTemplate;
	            }
	            else
	            {
	                masterViewModelPath = Files.MasterViewModelTemplate;
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
	                masterViewModelPath = Files.MasterViewModelVersionTemplate;
	            }
                else if (_configuration.DataLayout.Any(t => !string.IsNullOrEmpty(t.Parent)))
	            {
	                masterViewModelPath = Files.MasterViewModelMasterDetailTemplate;
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

        #region Config/Initialize

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
            Helpers.CreateFileFromPath(_configuration.InputPath + @"UI\Infrastructure\CommandNamesTemplate.cs", _configuration.OutputPath + @"UI\Infrastructure\CommandNames.cs", new[] { commandNamesContent });
            Helpers.CreateFileFromPath(_configuration.InputPath + @"UI\Infrastructure\EventNamesTemplate.cs", _configuration.OutputPath + @"UI\Infrastructure\EventNames.cs", new[] { eventNamesContent });
            Helpers.CreateFileFromPath(_configuration.InputPath + @"UI\Infrastructure\ParameterNamesTemplate.cs", _configuration.OutputPath + @"UI\Infrastructure\ParameterNames.cs");
            Helpers.CreateFileFromPath(_configuration.InputPath + @"UI\Infrastructure\ViewNamesTemplate.cs", _configuration.OutputPath + @"UI\Infrastructure\ViewNames.cs", new[] { viewNamesContent });
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


        #endregion

        #region Helpers

        private string GetGridXaml(ConfigurationItem dataItem, bool old = false)
	    {
	        string columnsContent = string.Empty;
	        foreach (ConfigurationProperty plusDataObject in dataItem.Properties)
	        {
	            if (plusDataObject.Type == "bool")
	            {
	                columnsContent += "                <plus:PlusGridViewCheckColumn Width=\"*\" DataMemberBinding=\"{Binding " +
	                                  plusDataObject.Name + "}\" Header=\"" + Helpers.GetLocalizedString(dataItem.Name + plusDataObject.Name) + "\"/>\r\n";
	            }
	            else
	            {
	                if (old)
	                {
	                    columnsContent += "                <plus:PlusGridViewTextColumn Width=\"*\" DataMemberBinding=\"{Binding " +
	                                      plusDataObject.Name + "}\" Header=\"" + Helpers.GetLocalizedString(dataItem.Name + plusDataObject.Name) + "\"/>\r\n";
	                }
	                else
	                {
	                    columnsContent += "                <plus:PlusGridViewTextColumnMinimal Width=\"*\" DataMemberBinding=\"{Binding " +
	                                      plusDataObject.Name + "}\" Header=\"" + Helpers.GetLocalizedString(dataItem.Name + plusDataObject.Name) + "\"/>\r\n";
	                }
	            }
	        }

	        return columnsContent;
	    }

        #endregion
    }
}

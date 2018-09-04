using System.IO;
using System.Linq;
using PlusLayerCreator.Items;

namespace PlusLayerCreator.Configure
{
    public class UiPart
    {
        #region Members

        private readonly Configuration _configuration;

        private readonly string _masterGridMultiTemplate;
        private readonly string _masterGridReadonlyTemplate;
        private readonly string _masterGridTemplate;
        private readonly string _masterGridVersionResourcesTemplate;
        private readonly string _masterGridVersionTemplate;

        private readonly string _masterViewModelNavigation;
        private readonly string _masterViewModelResetNavigation;
        private readonly string _masterViewModelSort;
        private readonly string _masterViewModelSave;
        private readonly string _masterViewModelSaveCommand;
        private readonly string _masterViewModelSaveVersionCommand;
        private readonly string _masterViewModelSaveMulti;
        private readonly string _masterViewModelAdd;
        private readonly string _masterViewModelAddVersion;
        private readonly string _masterViewModelAddChild;
        private readonly string _masterViewModelCancel;
        private readonly string _masterViewModelDelete;
        private readonly string _masterViewModelDeleteVersion;
        private readonly string _masterViewModelDeleteChild;
        private readonly string _masterViewModelClone;
        private readonly string _masterViewModelCloneCommand;
        private readonly string _masterViewModelInitializeItemsList;
        private readonly string _masterViewModelItemsList;
        private readonly string _masterViewModelSelectedItem;
        private readonly string _masterViewModelSelectedItemChildList;
        private readonly string _masterViewModelActiveItem;
        private readonly string _masterViewModelFilterSourceProvider;
        private readonly string _masterViewModelLazyLoadingMain;
        private readonly string _masterViewModelMainCollectionHasAnyChanges;
        private readonly string _masterViewModelLazyLoadingCollection;
        private readonly string _masterViewModelLazyLoadingCollectionSelect;
        private readonly string _masterViewModelLazyLoadingCollectionSelectVersion;
        private readonly string _masterViewModelLazyLoadingChildCollection;
        private readonly string _masterViewModelLazyLoadingFilter;
        private readonly string _masterViewModelItemColumnProvider;
        private readonly string _masterViewModelChildItemSettingsCommand;
        private readonly string _masterViewModelOpenChildItemSettingsDialogCommand;
        private readonly string _masterViewModelItemFilterCollection;
        private readonly string _statusBarViewDirectHopButton;
        private readonly string _statusBarViewDirectHopButtonConstruction;
        private readonly string _statusBarViewDirectHopButtonProperties;
        private readonly string _statusBarViewDirectHopButtonCommand;

	    private readonly string _detailViewModelComboBoxConstructorListLoad;
	    private readonly string _detailViewModelComboBoxMember;
	    private readonly string _detailViewModelComboBoxOnNavigatedTo;
	    private readonly string _detailViewModelComboBoxOnRegisteredLazyCollectionLoaded;
	    private readonly string _detailViewModelComboBoxProperties;
	    private readonly string _detailViewModelComboBoxXaml;

		private readonly string _yesNoConverterString = ", Converter={StaticResource BoolToLocalizedYesNoConverterConverter}";

        private readonly string _readOnlyTemplate = " IsReadOnly=\"True\"";

        #endregion Members

        #region Construction

        public UiPart(Configuration configuration)
        {
            _configuration = configuration;
            _masterGridTemplate =
                File.ReadAllText(configuration.InputPath + @"UI\Regions\Master\Grid\MasterGridPart.txt");
            _masterGridReadonlyTemplate =
                File.ReadAllText(configuration.InputPath + @"UI\Regions\Master\Grid\MasterGridReadonlyPart.txt");
            _masterGridMultiTemplate =
                File.ReadAllText(configuration.InputPath + @"UI\Regions\Master\Grid\MasterGridMultiPart.txt");
            _masterGridVersionTemplate =
                File.ReadAllText(configuration.InputPath + @"UI\Regions\Master\Grid\MasterVersionGridPart.txt");
            _masterGridVersionResourcesTemplate =
                File.ReadAllText(configuration.InputPath +
                                 @"UI\Regions\Master\Grid\MasterVersionGridResourcesPart.txt");

            _masterViewModelNavigation =
                File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\ViewModel\NavigationPart.txt");
            _masterViewModelResetNavigation =
                File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\ViewModel\ResetNavigationPart.txt");
            _masterViewModelSort =
                File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\ViewModel\SortPart.txt");
            _masterViewModelSave =
                File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\ViewModel\SavePart.txt");
            _masterViewModelSaveCommand =
                File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\ViewModel\SaveCommandPart.txt");
            _masterViewModelSaveVersionCommand =
                File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\ViewModel\SaveVersionCommandPart.txt");
            _masterViewModelSaveMulti =
                File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\ViewModel\SaveMultiPart.txt");
            _masterViewModelAdd =
                File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\ViewModel\AddPart.txt");
            _masterViewModelAddVersion =
                File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\ViewModel\AddVersionPart.txt");
            _masterViewModelAddChild =
                File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\ViewModel\AddChildPart.txt");
            _masterViewModelCancel =
                File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\ViewModel\CancelPart.txt");
            _masterViewModelDelete =
                File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\ViewModel\DeletePart.txt");
            _masterViewModelDeleteVersion =
                File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\ViewModel\DeleteVersionPart.txt");
            _masterViewModelDeleteChild =
                File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\ViewModel\DeleteChildPart.txt");
            _masterViewModelClone =
                File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\ViewModel\ClonePart.txt");
            _masterViewModelCloneCommand =
                File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\ViewModel\CloneCommandPart.txt");
            _masterViewModelSelectedItem =
                File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\ViewModel\SeletedItemPart.txt");
            _masterViewModelSelectedItemChildList =
                File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\ViewModel\SeletedItemChildListPart.txt");
            _masterViewModelActiveItem =
                File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\ViewModel\ActiveItemPart.txt");
            _masterViewModelItemsList =
                File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\ViewModel\ItemsListPart.txt");
            _masterViewModelInitializeItemsList =
                File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\ViewModel\InitializeItemsListPart.txt");
            _masterViewModelFilterSourceProvider =
                File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\ViewModel\IFilterSourceProviderPart.txt");
            _masterViewModelLazyLoadingMain =
                File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\ViewModel\LazyLoadingMainPart.txt");
            _masterViewModelMainCollectionHasAnyChanges =
                File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\ViewModel\MainCollectionHasAnyChangesPart.txt");
            _masterViewModelLazyLoadingCollection =
                File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\ViewModel\LazyLoadingCollectionPart.txt");
            _masterViewModelLazyLoadingChildCollection =
                File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\ViewModel\LazyLoadingChildCollectionPart.txt");
            _masterViewModelLazyLoadingFilter =
                File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\ViewModel\LazyLoadingFilterPart.txt");
            _masterViewModelLazyLoadingCollectionSelect =
                File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\ViewModel\LazyLoadingCollectionSelectPart.txt");
            _masterViewModelLazyLoadingCollectionSelectVersion =
                File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\ViewModel\LazyLoadingCollectionSelectVersionPart.txt");
            _masterViewModelItemColumnProvider =
                File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\ViewModel\ItemColumnProviderPart.txt");
            _masterViewModelChildItemSettingsCommand =
                File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\ViewModel\ChildItemSettingsCommandPart.txt");
            _masterViewModelOpenChildItemSettingsDialogCommand =
                File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\ViewModel\OpenChildItemSettingsDialogCommandPart.txt");
            _masterViewModelItemFilterCollection =
                File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\ViewModel\ItemFilterCollectionPart.txt");
            _statusBarViewDirectHopButton =
                File.ReadAllText(_configuration.InputPath + @"UI\Regions\Statusbar\DirectHopButtonPart.txt");
            _statusBarViewDirectHopButtonConstruction =
                File.ReadAllText(_configuration.InputPath + @"UI\Regions\Statusbar\DirectHopButtonConstructionPart.txt");
            _statusBarViewDirectHopButtonProperties =
                File.ReadAllText(_configuration.InputPath + @"UI\Regions\Statusbar\DirectHopButtonPropertiesPart.txt");
            _statusBarViewDirectHopButtonCommand =
                File.ReadAllText(_configuration.InputPath + @"UI\Regions\Statusbar\DirectHopButtonCommandPart.txt");

	        _detailViewModelComboBoxConstructorListLoad =
		        File.ReadAllText(_configuration.InputPath + @"UI\Regions\Detail\DetailComboBoxConstructorListLoad.txt");
	        _detailViewModelComboBoxMember =
		        File.ReadAllText(_configuration.InputPath + @"UI\Regions\Detail\DetailComboBoxMember.txt");
	        _detailViewModelComboBoxOnNavigatedTo =
		        File.ReadAllText(_configuration.InputPath + @"UI\Regions\Detail\DetailComboBoxOnNavigatedTo.txt");
	        _detailViewModelComboBoxOnRegisteredLazyCollectionLoaded =
		        File.ReadAllText(_configuration.InputPath + @"UI\Regions\Detail\DetailComboBoxOnRegisteredLazyCollectionLoaded.txt");
	        _detailViewModelComboBoxProperties =
		        File.ReadAllText(_configuration.InputPath + @"UI\Regions\Detail\DetailComboBoxProperties.txt");
	        _detailViewModelComboBoxXaml =
		        File.ReadAllText(_configuration.InputPath + @"UI\Regions\Detail\DetailComboBoxXaml.txt");
		}

        /// <summary>
        ///     Creates the UI.
        /// </summary>
        public void CreateUi()
        {
            CreateUiDetail();
            CreateUiViewMaster();
            CreateUiMasterViewModel();
        }

        #endregion Construction

        #region Filter

        public void CreateUiFilter()
        {
            var filterViewModelTemplate1 = "public FilterViewModel(IViewModelBaseServices baseServices";
            var filterViewModelTemplate2 = "): base(baseServices)\r\n" +
                                           "{\r\n" +
                                           "DisplayName = GlobalLocalizer.Singleton.Global_lblFilter.Translation;\r\n";
            var filterViewModelTemplate3 = "}";
            var filterChildViewModelsContent = string.Empty;

            var filterViewModelConstructionContent = string.Empty;
            var filterViewModelInitializationContent = string.Empty;

            var filterViewContent = string.Empty;
            var filterChildViewModelContent = string.Empty;

            //filterMembersContent
            foreach (var dataItem in _configuration.DataLayout.Where(t => t.Properties.Any(c => c.IsFilterProperty)))
            {
                var filterMembersContent = string.Empty;
                var filterPredicatesContent = string.Empty;
                var filterPredicateResetContent = string.Empty;
                var filterMultiSelectorsInitializeContent = string.Empty;
                var filterPropertiesContent = string.Empty;

                filterChildViewModelsContent += "\r\npublic " + _configuration.Product + dataItem.Name +
                                                "DataItemFilterViewModel " + _configuration.Product + dataItem.Name +
                                                "DataItemFilterViewModel { get; set; }";
                filterViewContent += "<StackPanel DataContext=\"{Binding " + _configuration.Product + dataItem.Name +
                                     "DataItemFilterViewModel}\">\r\n";
                foreach (var plusDataObject in dataItem.Properties)
                    if (plusDataObject.IsFilterProperty)
                    {
                        Helpers.CreateFilterContents(dataItem, plusDataObject, out var propertyFilterMembersContent,
                            out var propertyFilterPropertiesContent, out var propertyFilterPredicateResetContent,
                            out var propertyFilterPredicatesContent, out var propertyFilterXamlContent,
                            out var propertyFilterMultiSelectorsInitializeContent);

                        filterMembersContent +=
                            propertyFilterMembersContent.DoReplaces(dataItem, null, plusDataObject.Name, 
                                plusDataObject.Type);
                        filterPredicatesContent +=
                            propertyFilterPredicatesContent.DoReplaces(dataItem, null, plusDataObject.Name, 
                                plusDataObject.Type);
                        filterPredicateResetContent +=
                            propertyFilterPredicateResetContent.DoReplaces(dataItem, null, plusDataObject.Name, 
                                plusDataObject.Type);
                        filterMultiSelectorsInitializeContent +=
                            propertyFilterMultiSelectorsInitializeContent.DoReplaces(dataItem, null, plusDataObject.Name, 
                                plusDataObject.Type);
                        filterPropertiesContent +=
                            propertyFilterPropertiesContent.DoReplaces(dataItem, null, plusDataObject.Name);
                        filterViewContent +=
                            propertyFilterXamlContent.DoReplaces(dataItem, null, plusDataObject.Name,  plusDataObject.Type);
                    }

                filterViewContent += "</StackPanel>\r\n";

                var childViewModel = Helpers.FilterChildViewModelTemplate;
                childViewModel = childViewModel.DoReplaces(dataItem);
                childViewModel = childViewModel.Replace("$filterMembers$", filterMembersContent);
                childViewModel = childViewModel.Replace("$filterPredicates$", filterPredicatesContent);
                childViewModel = childViewModel.Replace("$filterPredicateReset$", filterPredicateResetContent);
                childViewModel = childViewModel.Replace("$filterMultiSelectorsInitialize$",
                    filterMultiSelectorsInitializeContent);
                childViewModel = childViewModel.Replace("$filterProperties$", filterPropertiesContent);
                filterChildViewModelContent += childViewModel;

                filterViewModelConstructionContent += ", IFilterSourceProvider<" + _configuration.Product +
                                                      dataItem.Name + "DataItem> " +
                                                      _configuration.Product.ToPascalCase() + dataItem.Name +
                                                      "DataItemFilterSourceProvider";
                filterViewModelInitializationContent +=
                    _configuration.Product + dataItem.Name + "DataItemFilterViewModel = new " + _configuration.Product +
                    dataItem.Name + "DataItemFilterViewModel(" + _configuration.Product.ToPascalCase() +
                    dataItem.Name + "DataItemFilterSourceProvider);\r\n";
            }

            var filterViewModelContent = filterViewModelTemplate1 + filterViewModelConstructionContent +
                                         filterViewModelTemplate2 +
                                         filterViewModelInitializationContent + filterViewModelTemplate3 +
                                         filterChildViewModelsContent;

            Helpers.CreateFileFromPath(Files.FilterViewTemplate,
                _configuration.OutputPath + @"UI\Regions\Filter\FilterView.xaml", new[] { filterViewContent });
            Helpers.CreateFileFromPath(Files.FilterViewCodeBehindTemplate,
                _configuration.OutputPath + @"UI\Regions\Filter\FilterView.xaml.cs");
            Helpers.CreateFileFromPath(Files.FilterViewModelTemplate,
                _configuration.OutputPath + @"UI\Regions\Filter\FilterViewModel.cs",
                new[] { filterViewModelContent, filterChildViewModelContent });
        }

        #endregion

        #region Toolbar/Statusbar

        public void CreateUiStatusbar()
        {
            string directHopButtonsConstructionContent = string.Empty;
            string directHopButtonsPropertiesContent = string.Empty;
            string directHopButtonsCommandContent = string.Empty;
            foreach (var directHop in _configuration.DirectHops)
            {
                directHopButtonsConstructionContent += _statusBarViewDirectHopButtonConstruction.ReplaceSpecialContent(new[] { directHop.DialogName, directHop.ControllerHandle });
                directHopButtonsPropertiesContent += _statusBarViewDirectHopButtonProperties.ReplaceSpecialContent(new[] { directHop.DialogName });
                directHopButtonsCommandContent += _statusBarViewDirectHopButtonCommand.ReplaceSpecialContent(new[] { directHop.DialogName, directHop.ControllerHandle });
            }


            Helpers.CreateFileFromPath(Files.StatusbarViewModelTemplate,
                _configuration.OutputPath + @"UI\Regions\Statusbar\StatusbarViewModel.cs", new []{ directHopButtonsConstructionContent, directHopButtonsPropertiesContent, directHopButtonsCommandContent });


            string directHopButtonsContent = string.Empty;
            foreach (var directHop in _configuration.DirectHops)
            {
                directHopButtonsContent += _statusBarViewDirectHopButton.ReplaceSpecialContent(new []{ directHop.DialogName, directHop.LocalizationKey, directHop.Product });
            }

            Helpers.CreateFileFromPath(Files.StatusbarViewTemplate,
                _configuration.OutputPath + @"UI\Regions\Statusbar\StatusbarView.xaml", new []{ directHopButtonsContent });
            Helpers.CreateFileFromPath(Files.StatusbarViewCodeBehindTemplate,
                _configuration.OutputPath + @"UI\Regions\Statusbar\StatusbarView.xaml.cs");
        }

        public void CreateUiToolbar()
        {
            var toolbarViewContent = string.Empty;

            toolbarViewContent +=
                File.ReadAllText(_configuration.InputPath + @"UI\Regions\Toolbar\ToolbarRefreshButtonPart.txt") +
                "\r\n";

            foreach (var dataItem in _configuration.DataLayout.Where(t =>
                string.IsNullOrEmpty(t.Parent) && t.IsPreFilterItem == false && !t.Name.EndsWith("Version")))
            {
                if (dataItem.CanEdit)
                {
                    toolbarViewContent +=
                        (File.ReadAllText(_configuration.InputPath + @"UI\Regions\Toolbar\ToolbarAddButtonPart.txt") +
                         "\r\n").DoReplaces(dataItem);
                    
                }   
            }

            foreach (var dataItem in _configuration.DataLayout.Where(t => t.IsPreFilterItem == false && t.Name.EndsWith("Version")))
            {
                if (dataItem.CanEdit)
                {
                    toolbarViewContent +=
                        (File.ReadAllText(_configuration.InputPath +
                                          @"UI\Regions\Toolbar\ToolbarAddVersionButtonPart.txt") + "\r\n")
                        .DoReplaces(dataItem);
                }
            }

            if (_configuration.DataLayout.Any(t => t.CanEdit))
                toolbarViewContent +=
                    File.ReadAllText(_configuration.InputPath + @"UI\Regions\Toolbar\ToolbarSaveButtonPart.txt") +
                    "\r\n";

            if (_configuration.DataLayout.Any(t => t.CanClone))
                toolbarViewContent +=
                    File.ReadAllText(_configuration.InputPath + @"UI\Regions\Toolbar\ToolbarCopyButtonPart.txt") +
                    "\r\n";
            if (_configuration.DataLayout.Any(t => t.CanDelete))
                toolbarViewContent +=
                    File.ReadAllText(_configuration.InputPath + @"UI\Regions\Toolbar\ToolbarDeleteButtonPart.txt") +
                    "\r\n";
            if (_configuration.DataLayout.Any(t => t.CanEdit))
                toolbarViewContent +=
                    File.ReadAllText(_configuration.InputPath + @"UI\Regions\Toolbar\ToolbarCancelButtonPart.txt") +
                    "\r\n";

            Helpers.CreateFileFromPath(Files.ToolbarViewModelTemplate,
                _configuration.OutputPath + @"UI\Regions\Toolbar\ToolbarViewModel.cs");
            Helpers.CreateFileFromPath(Files.ToolbarViewTemplate,
                _configuration.OutputPath + @"UI\Regions\Toolbar\ToolbarView.xaml", new[] {toolbarViewContent});
            Helpers.CreateFileFromPath(Files.ToolbarViewCodeBehindTemplate,
                _configuration.OutputPath + @"UI\Regions\Toolbar\ToolbarView.xaml.cs");
        }

        #endregion Toolbar/Statusbar

        #region Detail

        private void CreateUiDetail()
        {
			foreach (var dataItem in _configuration.DataLayout.Where(t => !t.IsPreFilterItem && !t.IsDetailComboBoxItem))
            {
                var detailViewContent = string.Empty;
	            string memberContent = string.Empty;
	            string constructorListContent = string.Empty;
	            string propetiesContent = string.Empty;
	            string onNavigatedToContent = string.Empty;
	            string lazyLoadingContent = string.Empty;

				if (!string.IsNullOrEmpty(dataItem.Parent))
                {
                    int level = 0;
                    string parentContent = string.Empty;
                    ConfigurationItem parent = Helpers.GetParent(dataItem);
                    while (parent != null)
                    {
                        parentContent = GetParentInformation(parent, level) + parentContent;
                        parent = Helpers.GetParent(parent);
                        level++;
                    }

                    detailViewContent += parentContent;
                }

                detailViewContent += "<plus:PlusGroupBox Header=\"" + dataItem.Name.GetLocalizedString(true) + "\">";
                detailViewContent += "    <StackPanel>\r\n";

                if (dataItem.Name.EndsWith("Version"))
                    detailViewContent +=
                        File.ReadAllText(_configuration.InputPath + @"UI\Regions\Detail\VersionDetailButtonXaml.txt");

                foreach (var property in dataItem.Properties)
                {
	                if (dataItem.Name.EndsWith("Version") && property.Name == "IsActive")
		                continue;

	                detailViewContent += "        <plus:PlusFormRow Label=\"" +
	                                     (dataItem.Name + property.Name).GetLocalizedString() + "\">\r\n";

					if (property.Type == "DataItem")
					{

						ConfigurationItem propertyItem = _configuration.DataLayout.First(t => t.Name == property.Name);
						memberContent += _detailViewModelComboBoxMember.DoReplaces(propertyItem);
						constructorListContent += _detailViewModelComboBoxConstructorListLoad.DoReplaces(propertyItem);
						propetiesContent += _detailViewModelComboBoxProperties.DoReplaces(propertyItem);
						onNavigatedToContent += _detailViewModelComboBoxOnNavigatedTo.DoReplaces(propertyItem);
						lazyLoadingContent += _detailViewModelComboBoxOnRegisteredLazyCollectionLoaded.DoReplaces(propertyItem);
					}

		            detailViewContent += GetItemControl(dataItem, property);
	                detailViewContent += "        </plus:PlusFormRow>\r\n";

				}

				detailViewContent +=
                    "				<plus:PlusFormRow Label=\"{localization:Localize Key=Global_lblLupdTimestamp, Source=GlobalLocalizer}\">";
                detailViewContent += "				    <plus:PlusLabel Content=\"{Binding DataItem.LupdTimestamp}\" />\r\n";
                detailViewContent += "				</plus:PlusFormRow>\r\n";
                detailViewContent +=
                    "				<plus:PlusFormRow Label=\"{localization:Localize Key=Global_lblLupdUser, Source=GlobalLocalizer}\">";
                detailViewContent += "				    <plus:PlusLabel Content=\"{Binding DataItem.LupdUser}\" />\r\n";
                detailViewContent += "				</plus:PlusFormRow>\r\n";

                detailViewContent += "    </StackPanel>\r\n";
                detailViewContent += "</plus:PlusGroupBox>\r\n";

	            if (dataItem.Name.EndsWith("Version"))
	            {
		            Helpers.CreateFileFromPath(Files.VersionDetailViewModelTemplate,
			            _configuration.OutputPath + @"UI\Regions\Detail\" + dataItem.Name + "DetailViewModel.cs", new []{ memberContent, constructorListContent, propetiesContent, onNavigatedToContent, lazyLoadingContent},
			            dataItem);
	            }
	            else
	            {
		            Helpers.CreateFileFromPath(Files.DetailViewModelTemplate,
						_configuration.OutputPath + @"UI\Regions\Detail\" + dataItem.Name + "DetailViewModel.cs", new[] { memberContent, constructorListContent, propetiesContent, onNavigatedToContent, lazyLoadingContent },
						dataItem);
	            }

	            Helpers.CreateFileFromPath(Files.DetailViewTemplate,
                    _configuration.OutputPath + @"UI\Regions\Detail\" + dataItem.Name + "DetailView.xaml",
                    new[] {detailViewContent}, dataItem);
                Helpers.CreateFileFromPath(Files.DetailViewCodeBehindTemplate,
                    _configuration.OutputPath + @"UI\Regions\Detail\" + dataItem.Name + "DetailView.xaml.cs", null,
                    dataItem);
            }
        }

        #endregion Detail

        #region Master

        private void CreateUiViewMaster()
        {
            var startOrder = 0;
            var rowNumber = 0;
            var masterViewContent = string.Empty;
            var masterViewResourcesContent = string.Empty;
            var masterViewCodeBehindContent = string.Empty;

            masterViewContent += "<Grid.RowDefinitions>\r\n";

            var layoutDependedDataItems =
                _configuration.DataLayout.Count(t => !t.Name.EndsWith("Version") && !t.IsPreFilterItem);
            if (_configuration.DataLayout.Any(t => t.IsPreFilterItem))
                masterViewContent += "<RowDefinition Height=\"Auto\"/>\r\n";

            masterViewContent += "<RowDefinition Height=\"*\" MinHeight=\"170\"/>\r\n";

            for (var i = 1; i < layoutDependedDataItems; i++)
            {
                masterViewContent += "<RowDefinition Height=\"2\"/>\r\n";
                masterViewContent += "<RowDefinition Height=\"*\" MinHeight=\"170\"/>\r\n";
            }

            masterViewContent += "</Grid.RowDefinitions>\r\n";

            //Prefilter
            if (_configuration.DataLayout.Any(t => t.IsPreFilterItem))
            {
                var preFilterItemContent = string.Empty;
                var preFilterContent =
                    File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\PreFilterPart.txt");
                foreach (var preFilterItem in _configuration.DataLayout.Where(t => t.IsPreFilterItem))
                {
                    startOrder = preFilterItem.Order + 1;
                    preFilterItemContent +=
                        File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\PreFilterItemPart.txt")
                            .DoReplaces(preFilterItem);
                }

                masterViewContent += preFilterContent.ReplaceSpecialContent(new[] {preFilterItemContent});
                rowNumber++;
            }

            // One
            if (_configuration.DataLayout.Count(t => !t.IsPreFilterItem) == 1)
            {
                var dataItem = _configuration.DataLayout.FirstOrDefault();
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

                var gridContent = gridTemplate.DoReplaces(dataItem);
                masterViewContent +=
                    gridContent.ReplaceSpecialContent(new[] {GetGridXaml(dataItem), rowNumber.ToString()});
                masterViewCodeBehindContent +=
                    "viewModel.ColumnProvider = Filtered" + dataItem.Name + "GridView;\r\n\r\n";
            }
            else
            {
                var nextDataItem = 0;

                if (_configuration.DataLayout.Any(t => t.Name.EndsWith("Version")))
                {
                    nextDataItem = 2;
                    var version = _configuration.DataLayout.First(t => t.Name.EndsWith("Version"));
                    var master = _configuration.DataLayout.First(t => t.Order == version.Order - 1);

                    masterViewCodeBehindContent +=
                        "viewModel.ColumnProvider = Filtered" + master.Name + "rGridView;\r\n\r\n";

                    var gridContent = _masterGridVersionTemplate.DoReplaces(master);
                    masterViewContent +=
                        gridContent.ReplaceSpecialContent(new[] {GetGridXaml(master, true), rowNumber.ToString()});
                    masterViewResourcesContent = _masterGridVersionResourcesTemplate
                        .Replace("$specialContent1$", GetGridXaml(version, true)).DoReplaces(master);
                }
                else
                {
                    var dataItem = _configuration.DataLayout.First(t => t.Order == startOrder);
                    string gridTemplate;
                    if (dataItem.CanEdit)
                        gridTemplate = _masterGridTemplate;
                    else
                        gridTemplate = _masterGridReadonlyTemplate;
                    var gridContent = gridTemplate.DoReplaces(dataItem);
                    masterViewContent +=
                        gridContent.ReplaceSpecialContent(new[] {GetGridXaml(dataItem), rowNumber.ToString()});
                    masterViewCodeBehindContent +=
                        "viewModel.ColumnProvider = Filtered" + dataItem.Name + "GridView;\r\n\r\n";
                    nextDataItem = dataItem.Order + 1;
                }

                rowNumber++;

                for (var i = nextDataItem; i < _configuration.DataLayout.OrderBy(t => t.Order).Count(); i++)
                {
                    masterViewContent +=
                        File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\Grid\GridSplitterPart.txt")
                            .ReplaceSpecialContent(new[] {rowNumber.ToString()});
                    rowNumber++;

                    string gridContent;
                    string buttons;
                    var dataItem = _configuration.DataLayout.First(t => t.Order == i);
                    var columns = GetGridXaml(dataItem);
                    if (dataItem.CanEditMultiple)
                    {
                        gridContent = File
                            .ReadAllText(_configuration.InputPath + @"UI\Regions\Master\Grid\ChildGridMultiPart.txt")
                            .DoReplaces(dataItem);

                        if (dataItem.CanSort)
                            buttons = File
                                .ReadAllText(_configuration.InputPath +
                                             @"UI\Regions\Master\Grid\Toolbar\ChildGridCommandWithSortButtons.txt")
                                .DoReplaces(dataItem);
                        else
                            buttons = File
                                .ReadAllText(_configuration.InputPath +
                                             @"UI\Regions\Master\Grid\Toolbar\ChildGridCommandButtons.txt")
                                .DoReplaces(dataItem);
                        masterViewContent +=
                            gridContent.ReplaceSpecialContent(new[] {columns, buttons, rowNumber.ToString()});
                    }
                    else
                    {
                        gridContent = File
                            .ReadAllText(_configuration.InputPath + @"UI\Regions\Master\Grid\ChildGridPart.txt")
                            .DoReplaces(dataItem);
                        buttons = File
                            .ReadAllText(_configuration.InputPath +
                                         @"UI\Regions\Master\Grid\Toolbar\ChildGridCommandButtons.txt")
                            .DoReplaces(dataItem);
                        masterViewContent +=
                            gridContent.ReplaceSpecialContent(new[] {columns, buttons, rowNumber.ToString()});
                    }

                    masterViewCodeBehindContent +=
                        "viewModel." + dataItem.Name + "ColumnProvider = Filtered" + dataItem.Name + "GridView;\r\n\r\n";
                    rowNumber++;
                }
            }

            Helpers.CreateFileFromPath(Files.MasterViewTemplate,
                _configuration.OutputPath + @"UI\Regions\Master\" + _configuration.DialogName + @"MasterView.xaml",
                new[] {masterViewContent, masterViewResourcesContent});
            Helpers.CreateFileFromPath(Files.MasterViewCodeBehindTemplate,
                _configuration.OutputPath + @"UI\Regions\Master\" + _configuration.DialogName + @"MasterView.xaml.cs",
                new[] {masterViewCodeBehindContent});
        }

        private void CreateUiMasterViewModel()
        {
            ConfigurationItem masterItem = _configuration.GetMasterItem();
            string masterViewModelTemplate;
            string membersContent = string.Empty; //1
            string initializeContent = string.Empty; //2
            string propertiesContent = string.Empty; //3
            string methodsContent = string.Empty; //3
            string filterParamContent = string.Empty; //5
            string commandContent = string.Empty; //6
            string navigationContent = string.Empty; //7
            string filterSourceProviderContent = string.Empty; //8
            string resetNavigationContent = string.Empty;
            string lazyLoadingContent = string.Empty;
            string lazyLoadingCollectionContent = string.Empty;
            string cloneContent = string.Empty;
            

            initializeContent += "CommandService.SubscribeCommand(GlobalCommandNames.OpenSettingsDialogCommand, OpenSettingsDialogCommandExecuted);\r\n";
            initializeContent += "CommandService.SubscribeCommand(GlobalCommandNames.RefreshCommand, RefreshCommandExecuted, RefreshCommandCanExecute);\r\n";
            membersContent += "private readonly I$Product$$Dialog$Repository _$product$$Dialog$Repository;".DoReplaces();

            foreach (var configurationItem in _configuration.DataLayout)
            {
                if (!configurationItem.IsPreFilterItem)
                {
                    resetNavigationContent = GetResetNavigationContent(configurationItem) + resetNavigationContent;
                    lazyLoadingContent += GetLazyLoadingPart(configurationItem);
                    navigationContent += _masterViewModelNavigation.DoReplaces(configurationItem);
                    membersContent += "private $Product$$Item$DataItem _selected$Item$DataItem;\r\n".DoReplaces(configurationItem);

                    string childListContent = string.Empty;
                    foreach (var child in _configuration.DataLayout.Where(t => t.Parent == configurationItem.Name))
                    {
                        childListContent += _masterViewModelSelectedItemChildList.DoReplaces(configurationItem, child);
                    }
                    propertiesContent += _masterViewModelSelectedItem.ReplaceSpecialContent(new[] { childListContent }).DoReplaces(configurationItem);

                    if (configurationItem.Properties.Any(t => t.IsFilterProperty))
                    {
                        filterParamContent += ", IFilterSourceProvider<" + _configuration.Product + configurationItem.Name + "DataItem>";
                    }
                    
                    if (!configurationItem.Name.EndsWith("Version"))
                    {
                        if (string.IsNullOrEmpty(configurationItem.Parent))
                        {
                            string selectPart = _configuration.DataLayout.Any(t => t.Name.EndsWith("Version"))
                                ? _masterViewModelLazyLoadingCollectionSelectVersion.DoReplaces(configurationItem)
                                : _masterViewModelLazyLoadingCollectionSelect.DoReplaces(configurationItem);

                            lazyLoadingCollectionContent += _masterViewModelLazyLoadingCollection.ReplaceSpecialContent(new []{selectPart}).DoReplaces(configurationItem);
                            
                            
                            if (configurationItem.CanEdit)
                            {
                                initializeContent += "CommandService.SubscribeAsyncCommand(GlobalCommandNames.AddCommand, AddCommandExecuted, AddCommandCanExecute);\r\n";
                                commandContent += GetAddCommand(configurationItem);
                            }
                            if (configurationItem.CanDelete)
                            {
                                initializeContent += "CommandService.SubscribeAsyncCommand(GlobalCommandNames.DeleteCommand, DeleteCommandExecuted, DeleteCommandCanExecute);\r\n";
                                commandContent += GetDeleteCommand(configurationItem);
                            }
                        }
                        else
                        {
                            initializeContent += _masterViewModelChildItemSettingsCommand.DoReplaces(configurationItem);
                            propertiesContent +=
                                _masterViewModelOpenChildItemSettingsDialogCommand.DoReplaces(configurationItem);
                            propertiesContent += _masterViewModelItemFilterCollection.DoReplaces(configurationItem);
                            propertiesContent += _masterViewModelItemColumnProvider.DoReplaces(configurationItem);

                            if (configurationItem.CanEdit)
                            {
                                initializeContent += "Add$Item$Command = CommandService.GetCommand(CommandNames.Add$Item$Command);\r\n".DoReplaces(configurationItem);
                                initializeContent += "CommandService.SubscribeAsyncCommand(CommandNames.Add$Item$Command, Add$Item$CommandExecuted, Add$Item$CommandCanExecute);\r\n".DoReplaces(configurationItem);
                                commandContent += GetAddCommand(configurationItem);
                                propertiesContent += "public IPlusCommand Add$Item$Command { get; set; }\r\n".DoReplaces(configurationItem);
                            }
                            if (configurationItem.CanDelete)
                            {
                                initializeContent += "Delete$Item$Command = CommandService.GetCommand(CommandNames.Delete$Item$Command);\r\n".DoReplaces(configurationItem);
                                initializeContent += "CommandService.SubscribeAsyncCommand(CommandNames.Delete$Item$Command, Delete$Item$CommandExecuted, Delete$Item$CommandCanExecute);\r\n".DoReplaces(configurationItem);
                                commandContent += GetDeleteCommand(configurationItem);
                                propertiesContent += "public IPlusCommand Delete$Item$Command { get; set; }\r\n".DoReplaces(configurationItem);
                            }
                        }
                    }
                    else
                    {
                        membersContent += "private PlusStateDataItem _activeItem;\r\n".DoReplaces(configurationItem);
                        initializeContent += "CommandService.SubscribeAsyncCommand(GlobalCommandNames.AddVersionCommand, AddVersionCommandExecuted, AddVersionCommandCanExecute);\r\n";
                        commandContent += GetAddCommand(configurationItem);
                        propertiesContent += _masterViewModelActiveItem.DoReplaces(_configuration.DataLayout.First(t => t.Name == configurationItem.Parent));
                    }

                    if (configurationItem.Properties.Any(t => t.IsFilterProperty))
                    {
                        filterSourceProviderContent += _masterViewModelFilterSourceProvider.DoReplaces(configurationItem);
                        string filter = _masterViewModelLazyLoadingFilter.DoReplaces(configurationItem);

                    }
                    
                    if (configurationItem.CanSort)
                    {
                        initializeContent += "Sort$Item$UpCommand = CommandService.GetCommand(CommandNames.Sort$Item$UpCommand);\r\n".DoReplaces(configurationItem);
                        initializeContent += "Sort$Item$DownCommand = CommandService.GetCommand(CommandNames.Sort$Item$DownCommand);\r\n".DoReplaces(configurationItem);
                        initializeContent += "CommandService.SubscribeAsyncCommand(CommandNames.Sort$Item$UpCommand, Sort$Item$UpCommandExecuted, Sort$Item$UpCommandCanExecute);\r\n".DoReplaces(configurationItem);
                        initializeContent += "CommandService.SubscribeAsyncCommand(CommandNames.Sort$Item$DownCommand, Sort$Item$DownCommandExecuted, Sort$Item$DownCommandCanExecute);\r\n".DoReplaces(configurationItem);
                        propertiesContent += "public IPlusCommand Sort$Item$UpCommand { get; set; }\r\n".DoReplaces(configurationItem);
                        propertiesContent += "public IPlusCommand Sort$Item$DownCommand { get; set; }\r\n".DoReplaces(configurationItem);
                        commandContent += GetSortCommand(configurationItem);
                    }
                }

                if (string.IsNullOrEmpty(configurationItem.Parent))
                {
                    membersContent +=
                        "private PlusLazyLoadingAsyncObservableCollection<$Product$$Item$DataItem> _$product$$Item$DataItemsList;\r\n"
                            .DoReplaces(configurationItem);
                    propertiesContent += _masterViewModelItemsList.DoReplaces(configurationItem);
                }

	            if (configurationItem.Name != masterItem.Name)
	            {
		            lazyLoadingCollectionContent += _masterViewModelLazyLoadingChildCollection.ReplaceSpecialContent(new[] { string.Empty }).DoReplaces(configurationItem);
	            }
			}

            if (_configuration.DataLayout.Any(t => t.CanEdit))
            {
                methodsContent += _masterViewModelMainCollectionHasAnyChanges.DoReplaces(masterItem);
                membersContent += "private PropertyObserver<PlusLazyLoadingAsyncObservableCollection<$Product$$Item$DataItem>> _isDirtyObserver;\r\n".DoReplaces(masterItem);
                initializeContent += "CommandService.SubscribeAsyncCommand(GlobalCommandNames.CancelCommand, CancelCommandExecuted, CancelCommandCanExecute);\r\n";
                initializeContent += "CommandService.SubscribeAsyncCommand(GlobalCommandNames.SaveCommand, SaveCommandExecuted, SaveCommandCanExecute);\r\n";
                commandContent += _masterViewModelCancel.DoReplaces(masterItem);
                if (masterItem.CanEditMultiple)
                {
                    string preFilter = string.Empty;
                    commandContent += _masterViewModelSaveMulti.ReplaceSpecialContent(new []{ preFilter }).DoReplaces(masterItem);
                }
                else
                {
                    commandContent += GetSaveCommand();
                }
            }

            if (_configuration.DataLayout.Any(t => t.CanClone))
            {
                initializeContent += "CommandService.SubscribeAsyncCommand(GlobalCommandNames.CopyCommand, CopyCommandExecuted, CopyCommandCanExecute);\r\n";

                foreach (var configurationItem in _configuration.DataLayout.Where(t => t.CanClone))
                {
                    cloneContent += GetCloneCommand(configurationItem);
                }

                commandContent += _masterViewModelClone.ReplaceSpecialContent(new[] {cloneContent}).DoReplaces(masterItem);//TODO falsch;
            }

            if (masterItem.CanEdit && !masterItem.CanEditMultiple)
            {
                initializeContent += "SelectingActiveItemCommand = new PlusCommand<CancelableSelectionArgs<object>>(SelectingActiveItemCommandExecuted);\r\n";
            }

            if (!_configuration.DataLayout.Any(t => t.IsPreFilterItem))
            {
                initializeContent += _masterViewModelInitializeItemsList.DoReplaces(masterItem);
            }
            else
            {
                //todo read prefilterlists here
            }

            lazyLoadingContent = _masterViewModelLazyLoadingMain.ReplaceSpecialContent(new[] {lazyLoadingContent});

            methodsContent += lazyLoadingContent;
            methodsContent += lazyLoadingCollectionContent;
            
            if (!masterItem.CanEditMultiple)
            {
                masterViewModelTemplate = Files.MasterViewModelTemplate;
            }
            else
            {
                masterViewModelTemplate = Files.MasterViewModelMultiTemplate;
            }

            Helpers.CreateFileFromPath(masterViewModelTemplate,
                _configuration.OutputPath + @"UI\Regions\Master\" + _configuration.DialogName + @"MasterViewModel.cs",
                new[] { membersContent, initializeContent, propertiesContent, methodsContent, filterParamContent, commandContent, navigationContent, filterSourceProviderContent, resetNavigationContent }, masterItem);

        }

        #endregion Master

        #region Helpers

        #region View

        private string GetItemControl(ConfigurationItem item, ConfigurationProperty property)
        {
            string retValue = string.Empty;
	        string specialContent = string.Empty;

			if (!item.CanEdit || item.Name == "Sequence")
            {
                if (property.Type == "bool")
                    retValue += "            <plus:PlusLabel Content=\"{Binding DataItem." +
                                         property.Name +
                                         _yesNoConverterString + "}\" />\r\n";
                else
                    retValue += "            <plus:PlusLabel Content=\"{Binding DataItem." +
                                         property.Name +
                                         "}\" />\r\n";
            }
            else
            {
	            if (property.Order == item.Properties.Min(t => t.Order))
	            {
		            specialContent += " plus:FocusHelper.IsFocused=\"{Binding IsNewItem}\"";
	            }
	            if (property.IsKey)
	            {
		            specialContent += " IsReadOnly=\"{Binding IsNewItem, Converter={StaticResource InvertBoolConverter}}\"";
	            }
	            if (property.IsReadOnly)
	            {
		            specialContent += _readOnlyTemplate;
	            }
	            switch (property.Type)
	            {
					case "bool":
						retValue += File.ReadAllText(_configuration.InputPath + @"UI\Regions\Detail\DetailCheckBoxXaml.txt");
						break;
		            case "DateTime":
			            retValue += File.ReadAllText(_configuration.InputPath + @"UI\Regions\Detail\DetailDateTimePickerXaml.txt");
						break;
		            case "DataItem":
			            retValue += File.ReadAllText(_configuration.InputPath + @"UI\Regions\Detail\DetailComboBoxXaml.txt");
						break;
					default:
						if (property.Length != string.Empty)
						{
							specialContent += " MaxLength =\"" + property.Length + "\"";
						}
						if (property.Type == "int")
						{
							specialContent += " IsNumeric=\"True\"";
						}

						retValue += File.ReadAllText(_configuration.InputPath + @"UI\Regions\Detail\DetailTextBoxXaml.txt");
						break;
				}
			}

            return retValue.DoReplaces(null, null, "DataItem." + property.Name).ReplaceSpecialContent(new[] { specialContent });
        }

        private string GetParentInformation(ConfigurationItem parent, int level)
        {
            string detailViewContent = string.Empty;
            string parentLevelString = string.Empty;

            for (int i = 0; i <= level; i++)
            {
                parentLevelString += "Parent.Parent.";
            }

            // Parent key fields
            detailViewContent += "<plus:PlusGroupBox Header=\"" + parent.Name.GetLocalizedString() +
                                 "\">\r\n";
            detailViewContent += "    <StackPanel\r\n>";

            foreach (var property in parent.Properties.Where(t => t.IsKey || t.IsRequired))
            {
                detailViewContent += "        <plus:PlusFormRow Label=\"" +
                                     (parent.Name + property.Name).GetLocalizedString() + "\">\r\n";
                if (property.Type == "bool")
                    detailViewContent +=
                        "            <plus:PlusLabel Content=\"{Binding DataItem." + parentLevelString +
                        property.Name +
                        _yesNoConverterString + "}\" />\r\n";
                else
                    detailViewContent +=
                        "            <plus:PlusLabel Content=\"{Binding DataItem." + parentLevelString +
                        property.Name +
                        "}\" />\r\n";
                detailViewContent += "        </plus:PlusFormRow>\r\n";
            }

            detailViewContent += "    </StackPanel>\r\n";
            detailViewContent += "</plus:PlusGroupBox>\r\n";

            return detailViewContent;
        }

        private string GetGridXaml(ConfigurationItem dataItem, bool old = false)
        {
            var columnsContent = string.Empty;
	        foreach (var plusDataObject in dataItem.Properties.Where(t => t.IsVisibleInGrid))
	        {
		        if (plusDataObject.Type == "bool")
		        {
			        if (old)
				        columnsContent +=
					        "                <plus:PlusGridViewCheckColumn Width=\"*\" DataMemberBinding=\"{Binding " +
					        plusDataObject.Name + "}\" Header=\"" +
					        (dataItem.Name + plusDataObject.Name).GetLocalizedString() + "\"/>\r\n";
			        else
						columnsContent +=
					        "                <plus:PlusGridViewCheckColumnMinimal Width=\"*\" DataMemberBinding=\"{Binding " +
					        plusDataObject.Name + "}\" Header=\"" +
					        (dataItem.Name + plusDataObject.Name).GetLocalizedString() + "\"/>\r\n";
		        }
				else if (plusDataObject.Type == "DataItem")
		        {
			        columnsContent +=
						"                <plus:PlusGridViewTextColumnMinimal Width=\"*\" DataMemberBinding=\"{Binding " +
				        plusDataObject.Name + "}\".ComposedIdAndDescription Header=\"" +
				        (dataItem.Name + plusDataObject.Name).GetLocalizedString() + "\"/>\r\n";
		        }
		        else
		        {
			        if (old)
				        columnsContent +=
					        "                <plus:PlusGridViewTextColumn Width=\"*\" DataMemberBinding=\"{Binding " +
					        plusDataObject.Name + "}\" Header=\"" +
					        (dataItem.Name + plusDataObject.Name).GetLocalizedString() + "\"/>\r\n";
			        else
				        columnsContent +=
					        "                <plus:PlusGridViewTextColumnMinimal Width=\"*\" DataMemberBinding=\"{Binding " +
					        plusDataObject.Name + "}\" Header=\"" +
					        (dataItem.Name + plusDataObject.Name).GetLocalizedString() + "\"/>\r\n";
		        }
	        }

	        return columnsContent;
        }

        #endregion View

        #region ViewModel

        private string GetResetNavigationContent(ConfigurationItem item)
        {
            return _masterViewModelResetNavigation.DoReplaces(item);
        }

        private string GetLazyLoadingPart(ConfigurationItem item)
        {
			return File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\ViewModel\GetLazyLoadingPart.txt").DoReplaces(item);
        }

        private string GetSortCommand(ConfigurationItem item)
        {
            string content = _masterViewModelSort;
            string acceptPoint;
            string isNullCheck = string.Empty;

            if (string.IsNullOrEmpty(item.Parent))
            {
                acceptPoint = item.Name + "sList" + item.Name + "sList";
            }
            else
            {
                acceptPoint = "Selected" + item.Parent + "DataItem." + item.Name + "s";
                isNullCheck = " && Selected" + item.Parent + "DataItem != null && Selected" + item.Parent + "DataItem." + item.Name + "s != null";
            }


            return content.ReplaceSpecialContent(new[] { acceptPoint, isNullCheck }).DoReplaces(item);
        }

        private string GetSaveCommand()
        {
            string content = _masterViewModelSave;
            string commandContent = string.Empty;
            foreach (var item in _configuration.DataLayout.Where(t => t.CanEdit && !t.CanEditMultiple).OrderByDescending(t => t.Order))
            {
                string acceptPoint;
                string saveChildPart = string.Empty;

                foreach (var childMultiSaveItem in _configuration.DataLayout.Where(t => t.CanEditMultiple && t.Parent == item.Name))
                {
                    saveChildPart =
                        "await _" + _configuration.Product.ToPascalCase() + _configuration.DialogName + "Repository.Save" + childMultiSaveItem.Name + "sAsync(CreateNewCallContext(true), Selected" + item.Name + "DataItem." + childMultiSaveItem.Name + "s, Selected" + item.Name + "DataItem);";
                }

                if (string.IsNullOrEmpty(item.Parent))
                {
                    acceptPoint = _configuration.Product + item.Name + "DataItemsList";
                    commandContent += _masterViewModelSaveCommand.ReplaceSpecialContent(new[] { saveChildPart, acceptPoint }).DoReplaces(item);
                }
                else if (!item.Name.EndsWith("Version"))
                {
                    acceptPoint = "Selected" + item.Parent + "DataItem." + item.Name + "s";
                    commandContent += _masterViewModelSaveCommand.ReplaceSpecialContent(new[] { saveChildPart, acceptPoint }).DoReplaces(item);
                    commandContent += " else ";
                }
                else
                {
                    commandContent += _masterViewModelSaveVersionCommand.DoReplaces(item);
                    break;
                }

            }

            return content.DoReplaces().ReplaceSpecialContent(new[] { commandContent });
        }

        private string GetAddCommand(ConfigurationItem item)
        {
            string content = string.Empty;
            if (item.Name == _configuration.GetMasterItem().Name)
            {
                content = _masterViewModelAdd.DoReplaces(item);
            }
            else if (item.Name.EndsWith("Version"))
            {
                content = _masterViewModelAddVersion.DoReplaces(Helpers.GetParent(item));
            }
            else
            {
                content = _masterViewModelAddChild.DoReplaces(item);
            }

            return content;
        }

        private string GetDeleteCommand(ConfigurationItem item)
        {
            string content = string.Empty;
            if (item.Name == _configuration.GetMasterItem().Name)
            {
                if (_configuration.DataLayout.Any(t => t.Name.EndsWith("Version")))
                {
                    content = _masterViewModelDeleteVersion;
                }
                else
                {
                    content = _masterViewModelDelete;
                }
            }
            else
            {
                content = _masterViewModelDeleteChild;
            }

            return content.DoReplaces(item);
        }

        private string GetCloneCommand(ConfigurationItem item)
        {
            return _masterViewModelCloneCommand.ReplaceSpecialContent(new []{ GetItemsList(item)}).DoReplaces(item);
        }

        private string GetItemsList(ConfigurationItem item)
        {
            if (string.IsNullOrEmpty(item.Parent))
            {
                return _configuration.Product + item.Name + "DataItemsList";
            }

            return "Selected" + item.Parent + "DataItem." + item.Name + "s";
        }

        #endregion ViewModel

        #endregion

        #region Config/Initialize

        public void CreateUiInfrastructure()
        {
            var parameterNamesContent = string.Empty;
            var moduleContent = string.Empty;
            var moduleContent2 = string.Empty;
            var commandNamesContent = string.Empty;
            var eventNamesContent = string.Empty;

            var viewNamesContent = "public static readonly string " + _configuration.DialogName + "MasterView = \"" +
                                   _configuration.DialogName + "MasterView\";\r\n";

            foreach (var dataItem in _configuration.DataLayout)
            {
                moduleContent += "_container.RegisterType<object, " + dataItem.Name + "DetailView>(ViewNames." +
                                 dataItem.Name + "DetailView);\r\n";

                if (dataItem.Properties.Any(t => t.IsFilterProperty))
                {
                    moduleContent2 += "_container.RegisterType<IFilterSourceProvider<" + _configuration.Product +
                                      dataItem.Name + "DataItem>, " +
                                      _configuration.DialogName +
                                      "MasterViewModel>(new ContainerControlledLifetimeManager());\r\n";
                }

                viewNamesContent += "public static readonly string " + dataItem.Name + "DetailView = \"" +
                                    dataItem.Name + "DetailView\";\r\n";

                parameterNamesContent += "public static readonly string Selected" + dataItem.Name + "DataItem = \"Selected" + dataItem.Name +
                                         "DataItem\";\r\n";

                if (!string.IsNullOrEmpty(dataItem.Parent) && !dataItem.Name.EndsWith("Version"))
                {
                    commandNamesContent += "public static readonly string Open" + dataItem.Name + "SettingsDialogCommand = \"Open" +
                                           dataItem.Name + "SettingsDialogCommand\";\r\n";

                    if (dataItem.CanDelete)
                        commandNamesContent += "public static readonly string Delete" + dataItem.Name + "Command = \"" +
                                               dataItem.Name + "DeleteCommand\";\r\n";
                    if (dataItem.CanEdit)
                        commandNamesContent += "public static readonly string Add" + dataItem.Name + "Command = \"" +
                                               dataItem.Name + "AddCommand\";\r\n";
                    if (dataItem.CanSort)
                    {
                        commandNamesContent += "public static readonly string Sort" + dataItem.Name + "UpCommand = \"" +
                                               dataItem.Name + "SortUpCommand\";\r\n";
                        commandNamesContent += "public static readonly string Sort" + dataItem.Name +
                                               "DownCommand = \"" + dataItem.Name + "SortDownCommand\";\r\n";
                    }
                }

                if (dataItem.Name.EndsWith("Version"))
                    eventNamesContent +=
                        "public static string VersionActivationChanged = \"VersionActivationChanged\";\r\n";
            }

            Helpers.CreateFileFromPath(_configuration.InputPath + @"UI\ModuleTemplate.cs",
                _configuration.OutputPath + @"UI\" + _configuration.DialogName + @"Module.cs",
                new[] {moduleContent, moduleContent2});
            Helpers.CreateFileFromPath(_configuration.InputPath + @"UI\ControllerTemplate.cs",
                _configuration.OutputPath + @"UI\" + _configuration.DialogName + @"Controller.cs");
            Helpers.CreateFileFromPath(_configuration.InputPath + @"UI\ViewModelTemplate.cs",
                _configuration.OutputPath + @"UI\" + _configuration.DialogName + @"ViewModel.cs");
            Helpers.CreateFileFromPath(_configuration.InputPath + @"UI\ViewTemplate.xaml",
                _configuration.OutputPath + @"UI\" + _configuration.DialogName + @"View.xaml");
            Helpers.CreateFileFromPath(_configuration.InputPath + @"UI\ViewTemplate.xaml.cs",
                _configuration.OutputPath + @"UI\" + _configuration.DialogName + @"View.xaml.cs");
            Helpers.CreateFileFromPath(_configuration.InputPath + @"UI\WindowTemplate.xaml",
                _configuration.OutputPath + @"UI\" + _configuration.DialogName + @"Window.xaml");
            Helpers.CreateFileFromPath(_configuration.InputPath + @"UI\WindowTemplate.xaml.cs",
                _configuration.OutputPath + @"UI\" + _configuration.DialogName + @"Window.xaml.cs");
            Helpers.CreateFileFromPath(_configuration.InputPath + @"UI\Infrastructure\BootstrapperTemplate.cs",
                _configuration.OutputPath + @"UI\Infrastructure\Bootstrapper.cs");
            Helpers.CreateFileFromPath(_configuration.InputPath + @"UI\Infrastructure\CommandNamesTemplate.cs",
                _configuration.OutputPath + @"UI\Infrastructure\CommandNames.cs", new[] {commandNamesContent});
            Helpers.CreateFileFromPath(_configuration.InputPath + @"UI\Infrastructure\EventNamesTemplate.cs",
                _configuration.OutputPath + @"UI\Infrastructure\EventNames.cs", new[] {eventNamesContent});
            Helpers.CreateFileFromPath(_configuration.InputPath + @"UI\Infrastructure\ParameterNamesTemplate.cs",
                _configuration.OutputPath + @"UI\Infrastructure\ParameterNames.cs", new[] { parameterNamesContent });
            Helpers.CreateFileFromPath(_configuration.InputPath + @"UI\Infrastructure\ViewNamesTemplate.cs",
                _configuration.OutputPath + @"UI\Infrastructure\ViewNames.cs", new[] {viewNamesContent});
        }


        public void CreatePlusDialogInfrastructure()
        {
            Helpers.CreateFileFromPath(_configuration.InputPath + @"PlusDialog\DialogControllerHandleTemplate.txt",
                _configuration.OutputPath + @"PlusDialog\DialogControllerHandle.cs");
            Helpers.CreateFileFromPath(_configuration.InputPath + @"PlusDialog\PlusMainControllerTemplate.txt",
                _configuration.OutputPath + @"PlusDialog\PlusMainController.cs");
            Helpers.CreateFileFromPath(_configuration.InputPath + @"PlusDialog\PlusMainFormTemplate.txt",
                _configuration.OutputPath + @"PlusDialog\PlusMainForm.cs");
        }

        public void CreateLauncherConfigEntry()
        {
            var entry = "<Product Name=\"" + _configuration.Product + "\">\r\n";
            entry += "<Module Handle=\"" + _configuration.ControllerHandle + "\" Name=\"" + _configuration.DialogName +
                     "\" />\r\n";

            if (_configuration.IsUseBusinessServiceWithoutBo)
                foreach (var configurationItem in _configuration.DataLayout)
                    entry += "<Dependency iface=\"I" + _configuration.Product + configurationItem.Name +
                             "Service\" impl=\"" + _configuration.Product + configurationItem.Name + "Mock\" />\r\n";

            entry += "</Product>";

            var fileInfo = new FileInfo(_configuration.OutputPath + "PlusLauncher.xml");
            if (fileInfo.Directory != null) fileInfo.Directory.Create();
            File.WriteAllText(fileInfo.FullName, entry);
        }

        #endregion
    }
}
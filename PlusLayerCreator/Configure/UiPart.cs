﻿using System.IO;
using System.Linq;
using PlusLayerCreator.Items;

namespace PlusLayerCreator.Configure
{
    public class UiPart
    {
        #region Members

        private readonly Configuration _configuration;
        private readonly string _isNumericTemplate = " IsNumeric=\"True\"";

        private readonly string _keyReadOnlyTemplate =
            " IsReadOnly=\"{Binding IsNewItem, Converter={StaticResource InvertBoolConverter}}\"";

        private readonly string _masterGridMultiTemplate;
        private readonly string _masterGridReadonlyTemplate;
        private readonly string _masterGridTemplate;
        private readonly string _masterGridVersionResourcesTemplate;
        private readonly string _masterGridVersionTemplate;

        private readonly string _masterViewModelNavigation;
        private readonly string _masterViewModelSave;
        private readonly string _masterViewModelSaveMulti;

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
            _masterViewModelSave =
                File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\ViewModel\SavePart.txt");
            _masterViewModelSaveMulti =
                File.ReadAllText(_configuration.InputPath + @"UI\Regions\Master\ViewModel\SaveMultiPart.txt");

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
            foreach (var dataItem in _configuration.DataLayout.Where(t => !t.Name.EndsWith("Version")))
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
                            propertyFilterMembersContent.DoReplaces2(plusDataObject.Name, dataItem,
                                plusDataObject.Type);
                        filterPredicatesContent +=
                            propertyFilterPredicatesContent.DoReplaces2(plusDataObject.Name, dataItem,
                                plusDataObject.Type);
                        filterPredicateResetContent +=
                            propertyFilterPredicateResetContent.DoReplaces2(plusDataObject.Name, dataItem,
                                plusDataObject.Type);
                        filterMultiSelectorsInitializeContent +=
                            propertyFilterMultiSelectorsInitializeContent.DoReplaces2(plusDataObject.Name, dataItem,
                                plusDataObject.Type);
                        filterPropertiesContent +=
                            propertyFilterPropertiesContent.DoReplaces2(plusDataObject.Name, dataItem);
                        filterViewContent +=
                            propertyFilterXamlContent.DoReplaces2(plusDataObject.Name, dataItem, plusDataObject.Type);
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
            Helpers.CreateFileFromPath(Files.StatusbarViewModelTemplate,
                _configuration.OutputPath + @"UI\Regions\Statusbar\StatusbarViewModel.cs");
            Helpers.CreateFileFromPath(Files.StatusbarViewTemplate,
                _configuration.OutputPath + @"UI\Regions\Statusbar\StatusbarView.xaml");
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
                if (dataItem.CanEdit)
                    toolbarViewContent +=
                        (File.ReadAllText(_configuration.InputPath + @"UI\Regions\Toolbar\ToolbarAddButtonPart.txt") +
                         "\r\n").DoReplaces(dataItem);

            foreach (var dataItem in _configuration.DataLayout.Where(t =>
                string.IsNullOrEmpty(t.Parent) && t.IsPreFilterItem == false && t.Name.EndsWith("Version")))
                if (dataItem.CanEdit)
                    toolbarViewContent +=
                        (File.ReadAllText(_configuration.InputPath +
                                          @"UI\Regions\Toolbar\ToolbarAddVersionButtonPart.txt") + "\r\n")
                        .DoReplaces(dataItem);

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

            Helpers.CreateFileFromPath(Files.ToolbarViewCodeBehindTemplate,
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
            foreach (var dataItem in _configuration.DataLayout)
            {
                var detailViewContent = string.Empty;
                var yesNoConverterString = ", Converter={StaticResource BoolToLocalizedYesNoConverterConverter}";

                if (!string.IsNullOrEmpty(dataItem.Parent))
                {
                    var parent = _configuration.DataLayout.FirstOrDefault(t => t.Name == dataItem.Parent);

                    // Parent key fields
                    detailViewContent += "<plus:PlusGroupBox Header=\"" + dataItem.Parent.GetLocalizedString() +
                                         "\">";
                    detailViewContent += "    <StackPanel>";

                    foreach (var property in parent.Properties)
                    {
                        detailViewContent += "        <plus:PlusFormRow Label=\"" +
                                             (parent.Name + property.Name).GetLocalizedString() + "\">\r\n";
                        if (property.Type == "bool")
                            detailViewContent +=
                                "            <plus:PlusLabel Content=\"{Binding DataItem.Parent.Parent." +
                                property.Name +
                                yesNoConverterString + "}\" />\r\n";
                        else
                            detailViewContent +=
                                "            <plus:PlusLabel Content=\"{Binding DataItem.Parent.Parent." +
                                property.Name +
                                "}\" />\r\n";
                        detailViewContent += "        </plus:PlusFormRow>\r\n";
                    }

                    detailViewContent += "    </StackPanel>";
                    detailViewContent += "</plus:PlusGroupBox>";
                }

                detailViewContent += "<plus:PlusGroupBox Header=\"" + dataItem.Name.GetLocalizedString(true) +
                                     "\">";
                detailViewContent += "    <StackPanel>";

                if (dataItem.Name.EndsWith("Version"))
                    detailViewContent +=
                        File.ReadAllText(_configuration.InputPath + @"UI\Regions\Detail\VersionDetailButtonXaml.txt");

                foreach (var property in dataItem.Properties)
                {
                    if (dataItem.Name.EndsWith("Version") && property.Name == "IsActive") continue;

                    detailViewContent += "        <plus:PlusFormRow Label=\"" +
                                         (dataItem.Name + property.Name).GetLocalizedString() + "\">\r\n";

                    if (!dataItem.CanEdit)
                    {
                        if (property.Type == "bool")
                            detailViewContent += "            <plus:PlusLabel Content=\"{Binding DataItem." +
                                                 property.Name +
                                                 yesNoConverterString + "}\" />\r\n";
                        else
                            detailViewContent += "            <plus:PlusLabel Content=\"{Binding DataItem." +
                                                 property.Name +
                                                 "}\" />\r\n";
                    }
                    else
                    {
                        var addintionalInformation = string.Empty;
                        if (property.IsKey)
                            addintionalInformation += _keyReadOnlyTemplate;
                        else if (property.IsReadOnly) addintionalInformation += _readOnlyTemplate;

                        if (property.Type == "bool")
                        {
                            detailViewContent += "            <plus:PlusCheckBox " + addintionalInformation +
                                                 " IsChecked=\"{Binding DataItem." + property.Name + "}\" />\r\n";
                        }
                        else if (property.Type == "DateTime")
                        {
                            detailViewContent +=
                                File.ReadAllText(_configuration.InputPath +
                                                 @"UI\Regions\Detail\DetailDateTimePickerXaml.txt")
                                    .DoReplaces2("DataItem." + property.Name);
                        }
                        else
                        {
                            if (property.Length != string.Empty)
                                addintionalInformation += " MaxLength =\"" + property.Length + "\"";

                            if (property.Type == "int") addintionalInformation += _isNumericTemplate;

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
                    Helpers.CreateFileFromPath(Files.VersionDetailViewModelTemplate,
                        _configuration.OutputPath + @"UI\Regions\Detail\" + dataItem.Name + "DetailViewModel.cs", null,
                        dataItem);
                else
                    Helpers.CreateFileFromPath(Files.DetailViewModelTemplate,
                        _configuration.OutputPath + @"UI\Regions\Detail\" + dataItem.Name + "DetailViewModel.cs", null,
                        dataItem);
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
                masterViewContent += "<RowDefinition Height=\"Auto\">\r\n";

            masterViewContent += "<RowDefinition Height=\"*\" MinHeight=\"170\">\r\n";

            for (var i = 1; i < layoutDependedDataItems; i++)
            {
                masterViewContent += "<RowDefinition Height=\"2\">\r\n";
                masterViewContent += "<RowDefinition Height=\"*\" MinHeight=\"170\">\r\n";
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
                    gridTemplate = _masterGridMultiTemplate;
                else if (dataItem.CanEdit)
                    gridTemplate = _masterGridTemplate;
                else
                    gridTemplate = _masterGridReadonlyTemplate;
                var gridContent = gridTemplate.DoReplaces();
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
                        "viewModel.ColumnProvider = Filtered" + master.Name + "sGridView;\r\n\r\n";

                    var gridContent = _masterGridVersionTemplate.DoReplaces(master);
                    masterViewContent += gridContent.Replace("$specialContent1$", GetGridXaml(master, true));
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
            string membersContent = string.Empty; //1
            string initializeContent = string.Empty; //2
            string filterParamContent = string.Empty; //5
            string commandContent = string.Empty; //6
            string navigationContent = string.Empty; //7

            foreach (var configurationItem in _configuration.DataLayout)
            {
                if (!configurationItem.IsPreFilterItem)
                {
                    filterParamContent += ", IFilterSourceProvider<" + _configuration.Product + configurationItem.Name + "DataItem>";
                    navigationContent += _masterViewModelNavigation.DoReplaces(configurationItem);
                }

                if (string.IsNullOrEmpty(configurationItem.Parent))
                {
                    if (configurationItem.CanEditMultiple)
                    {
                        commandContent += _masterViewModelSaveMulti.DoReplaces(configurationItem);
                    }
                    else
                    {
                        commandContent += _masterViewModelSave.DoReplaces(configurationItem);
                    }
                }

                if (_configuration.DataLayout.Any(t => t.CanEdit))
                {

                }

                if (_configuration.DataLayout.Any(t => t.CanDelete))
                {

                }

                if (_configuration.DataLayout.Any(t => t.CanClone))
                {

                }
            }
        }

        #endregion Master

        #region Helpers

        private string GetGridXaml(ConfigurationItem dataItem, bool old = false)
        {
            var columnsContent = string.Empty;
            foreach (var plusDataObject in dataItem.Properties)
                if (plusDataObject.Type == "bool")
                {
                    columnsContent +=
                        "                <plus:PlusGridViewCheckColumn Width=\"*\" DataMemberBinding=\"{Binding " +
                        plusDataObject.Name + "}\" Header=\"" +
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

            return columnsContent;
        }

        #endregion

        #region Config/Initialize

        public void CreateUiInfrastructure()
        {
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

                if (!dataItem.Name.EndsWith("Version"))
                    moduleContent2 += "_container.RegisterType<IFilterSourceProvider<" + _configuration.Product +
                                      dataItem.Name + "DataItem>, " +
                                      _configuration.DialogName +
                                      "MasterViewModel>(new ContainerControlledLifetimeManager());\r\n";

                viewNamesContent += "public static readonly string " + dataItem.Name + "DetailView = \"" +
                                    dataItem.Name + "DetailView\";\r\n";

                if (!string.IsNullOrEmpty(dataItem.Parent) && !dataItem.Name.EndsWith("Version"))
                {
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
                _configuration.OutputPath + @"UI\Infrastructure\ParameterNames.cs");
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
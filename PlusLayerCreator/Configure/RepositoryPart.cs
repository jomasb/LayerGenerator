using System.IO;
using System.Linq;
using System.Text;
using PlusLayerCreator.Items;

namespace PlusLayerCreator.Configure
{
    public class RepositoryPart
    {
        private readonly Configuration _configuration;
        private readonly string _createDataItemTemplatePart;
        private readonly string _createRepositoryDtoTemplatePart;

        public RepositoryPart(Configuration configuration)
        {
            _configuration = configuration;
            _createRepositoryDtoTemplatePart = File.ReadAllText(configuration.InputPath + @"Repository\CreateDtoPart.txt");
            _createDataItemTemplatePart = File.ReadAllText(configuration.InputPath + @"Repository\CreateDataItemPart.txt");
        }

        #region Repository

        public void CreateRepository()
        {
            var repositoryServiceMemberContent = string.Empty;
            var repositoryServiceParameterContent = string.Empty;
            var repositoryServiceConstructorContent = string.Empty;

            var interfaceContent = string.Empty;
            var repositoryContent = string.Empty;

            foreach (var dataItem in _configuration.DataLayout)
            {
                repositoryServiceMemberContent += "private readonly I" + _configuration.Product + dataItem.Name +
                                                  "Service _" +
                                                  (_configuration.Product + dataItem.Name + "Service;\r\n")
                                                  .ToPascalCase();
                if (repositoryServiceParameterContent != string.Empty)
                    repositoryServiceParameterContent = repositoryServiceParameterContent + ", ";
                repositoryServiceParameterContent += "I" + _configuration.Product + dataItem.Name + "Service " +
                                                     _configuration.Product.ToPascalCase() + dataItem.Name + "Service";
                repositoryServiceConstructorContent += "_" + _configuration.Product.ToPascalCase() + dataItem.Name +
                                                       "Service = " + _configuration.Product.ToPascalCase() +
                                                       dataItem.Name + "Service;\r\n";

                interfaceContent += "#region " + dataItem.Name + "\r\n\r\n";
                repositoryContent += "#region " + dataItem.Name + "\r\n\r\n";

                var identifier = GetIdentifier(dataItem);
                var readOnly = GetReadOnly(dataItem);

                string template = string.Empty;
                if (string.IsNullOrEmpty(dataItem.Parent))
                {
                    template = "GetPart.txt";
                }
                else
                {
                    template = "GetChildPart.txt";
                }

                interfaceContent += GetInterfaceGetPart(dataItem, template);
                repositoryContent += GetRepositoryGetPart(dataItem, template);
                

                if (dataItem.CanClone)
                {
                    interfaceContent +=
                        File.ReadAllText(_configuration.InputPath + @"Repository\Contracts\ClonePart.txt")
                            .DoReplaces(dataItem) + "\r\n\r\n";
                    repositoryContent +=
                        File.ReadAllText(_configuration.InputPath + @"Repository\ClonePart.txt").DoReplaces(dataItem) +
                        "\r\n\r\n";
                }

                if (dataItem.CanDelete)
                {
                    interfaceContent +=
                        File.ReadAllText(_configuration.InputPath + @"Repository\Contracts\DeletePart.txt")
                            .DoReplaces(dataItem) + "\r\n\r\n";
                    if (dataItem.CanEditMultiple)
                    {
                        repositoryContent += File.ReadAllText(_configuration.InputPath + @"Repository\DeletePart.txt").DoReplaces(dataItem) + "\r\n\r\n";
                    }
                    else
                    {
                        repositoryContent += File.ReadAllText(_configuration.InputPath + @"Repository\DeleteAndSavePart.txt").DoReplaces(dataItem) + "\r\n\r\n";
                    }
                }

                if (dataItem.CanSort)
                {
                    interfaceContent +=
                        File.ReadAllText(_configuration.InputPath + @"Repository\Contracts\SortPart.txt")
                            .DoReplaces(dataItem) + "\r\n\r\n";
                    repositoryContent +=
                        File.ReadAllText(_configuration.InputPath + @"Repository\SortPart.txt").DoReplaces(dataItem) +
                        "\r\n\r\n";
                }

                if (dataItem.CanEdit)
                {
                    interfaceContent +=
                        File.ReadAllText(_configuration.InputPath + @"Repository\Contracts\AddPart.txt")
                            .DoReplaces(dataItem) + "\r\n\r\n";
                    repositoryContent +=
                        File.ReadAllText(_configuration.InputPath + @"Repository\AddPart.txt").DoReplaces(dataItem) +
                        "\r\n\r\n";

                    if (!dataItem.CanEditMultiple)
                    {
                        interfaceContent +=
                            File.ReadAllText(_configuration.InputPath + @"Repository\Contracts\SavePart.txt")
                                .DoReplaces(dataItem) + "\r\n\r\n";
                        var content = File.ReadAllText(_configuration.InputPath + @"Repository\SavePart.txt")
                                            .DoReplaces(dataItem) + "\r\n\r\n";
                        content = content.ReplaceSpecialContent(new[] { identifier, readOnly, GetParentParameter(dataItem, 1, true)});
                        repositoryContent += content;
                    }

                    if (dataItem.CanEditMultiple)
                    {
                        string parentParameter = string.Empty;
                        if (!string.IsNullOrEmpty(dataItem.Parent))
                        {
                            parentParameter = ", $Product$$Parent$DataItem $parent$DataItem".DoReplaces(dataItem);
                        }

                        interfaceContent +=
                            File.ReadAllText(_configuration.InputPath + @"Repository\Contracts\SaveMultiPart.txt")
                                .ReplaceSpecialContent(new []{ parentParameter })
                                .DoReplaces(dataItem) + "\r\n\r\n";
                        var content = File.ReadAllText(_configuration.InputPath + @"Repository\SaveMultiPart.txt")
                                    .ReplaceSpecialContent(new[] { identifier, readOnly, GetParentParameter(dataItem, 1), parentParameter })
                                    .DoReplaces(dataItem) + "\r\n\r\n";
                        repositoryContent += content;
                    }
                }

                if (dataItem.Name.EndsWith("Version"))
                {
                    if (!string.IsNullOrEmpty(dataItem.Parent))
                    {
                        interfaceContent +=
                            File.ReadAllText(_configuration.InputPath + @"Repository\Contracts\VersionPart.txt")
                                .DoReplaces(dataItem) + "\r\n\r\n";
                        var content = File.ReadAllText(_configuration.InputPath + @"Repository\VersionPart.txt")
                                          .DoReplaces(dataItem) + "\r\n\r\n";
                        content = content.ReplaceSpecialContent(new[] { identifier, readOnly });
                        repositoryContent += content;
                    }
                }

                interfaceContent += "#endregion " + dataItem.Name + "\r\n\r\n";
                repositoryContent += "#endregion " + dataItem.Name + "\r\n\r\n";





            }

            var dtoLayer = _configuration.IsUseBusinessServiceWithoutBo ? "BusinessServiceLocal" : "Gateway";

            Helpers.CreateFileFromPath(_configuration.InputPath + @"Repository\Contracts\IRepositoryTemplate.cs",
                _configuration.OutputPath + @"Repository\Contracts\I" + _configuration.Product +
                _configuration.DialogName + "Repository.cs", new[] {interfaceContent});
            Helpers.CreateFileFromPath(_configuration.InputPath + @"Repository\RepositoryTemplate.cs",
                _configuration.OutputPath + @"Repository\" + _configuration.Product + _configuration.DialogName +
                "Repository.cs",
                new[]
                {
                    repositoryServiceMemberContent, repositoryServiceParameterContent,
                    repositoryServiceConstructorContent, repositoryContent, dtoLayer
                });
        }
        
        public void CreateDataItem()
        {
            foreach (var dataItem in _configuration.DataLayout)
            {
                var dataItemContent = string.Empty;

                if (string.IsNullOrEmpty(dataItem.Parent))
                    foreach (var preFilterDataItem in _configuration.DataLayout.Where(t =>
                        t.IsPreFilterItem && t.Properties.Any(z => z.Name == "Id")))
                    {
                        var pp = preFilterDataItem.Properties.FirstOrDefault(t => t.Name == "Id");
                        if (pp != null)
                            dataItemContent += "public " + pp.Type + " " + pp.Name + "\r\n" +
                                               "    {get\r\n" +
                                               "    {\r\n" +
                                               "        return Get<" + pp.Type + ">();\r\n" +
                                               "    }\r\n" +
                                               "    set\r\n" +
                                               "    {\r\n" +
                                               "        Set<" + pp.Type + ">(value);\r\n" +
                                               "    }}\r\n\r\n";
                    }

                foreach (var plusDataObject in dataItem.Properties)
                {
                    if (plusDataObject.IsRequired)
                    {
                        dataItemContent += "[Required]\r\n";
                    }
                    if (!string.IsNullOrEmpty(plusDataObject.Length))
                    {
                        if (plusDataObject.Type == "int")
                        {
                            dataItemContent +="[NumericRange(0, " + Helpers.GetMaxValue(plusDataObject.Length) + ")]\r\n";
                        }
                        if (plusDataObject.Type == "string")
                        {
                            dataItemContent += "[MaxLength(" + plusDataObject.Length + ")]\r\n";
                        }
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

                    foreach (var childDataItem in _configuration.DataLayout.Where(t => t.Parent == dataItem.Name))
                        dataItemContent +=
                            File.ReadAllText(_configuration.InputPath +
                                             @"Repository\DataItems\DataItemCollectionPart.txt")
                                .DoReplaces(childDataItem) + "\r\n";
                }

                Helpers.CreateFileFromPath(_configuration.InputPath + @"Repository\DataItems\DataItemTemplate.cs",
                    _configuration.OutputPath + @"Repository\DataItems\" + _configuration.Product + dataItem.Name +
                    "DataItem.cs", new[] {dataItemContent}, dataItem);
            }
        }

        public void CreateRepositoryDtoFactory()
        {
            var factoryContent = string.Empty;
            foreach (var dataItem in _configuration.DataLayout)
            {
                var itemContent = string.Empty;
                foreach (var plusDataObject in dataItem.Properties)
                {
                    itemContent += plusDataObject.Name + " = dataItem." + plusDataObject.Name + ",\r\n";
                }

                factoryContent += _createRepositoryDtoTemplatePart.DoReplaces(dataItem)
                                      .ReplaceSpecialContent(new[] {itemContent}) + "\r\n";
            }

            var dtoLayer = _configuration.IsUseBusinessServiceWithoutBo ? "BusinessServiceLocal" : "Gateway";

            Helpers.CreateFileFromPath(_configuration.InputPath + @"Repository\DtoFactoryTemplate.cs",
                _configuration.OutputPath + @"Repository\" + _configuration.Product + "DtoFactory.cs",
                new[] {factoryContent, dtoLayer});
        }

        public void CreateDataItemFactory()
        {
            var factoryContent = string.Empty;
            foreach (var dataItem in _configuration.DataLayout)
            {
                var itemContent = string.Empty;
                foreach (var plusDataObject in dataItem.Properties)
                    itemContent += plusDataObject.Name + " = dto." + plusDataObject.Name + ",\r\n";
                factoryContent +=
                    _createDataItemTemplatePart.DoReplaces(dataItem).ReplaceSpecialContent(new[] {itemContent}) +
                    "\r\n";
            }

            var dtoLayer = _configuration.IsUseBusinessServiceWithoutBo ? "BusinessServiceLocal" : "Gateway";

            Helpers.CreateFileFromPath(_configuration.InputPath + @"Repository\DataItemFactoryTemplate.cs",
                _configuration.OutputPath + @"Repository\" + _configuration.Product + "DataItemFactory.cs",
                new[] {factoryContent, dtoLayer});
        }

        #endregion Repository

        #region Helpers

        private string GetInterfaceGetPart(ConfigurationItem item, string template)
        {
            return File.ReadAllText(_configuration.InputPath + @"Repository\Contracts\" + template)
                .ReplaceSpecialContent(new[] { GetPreFilterInformation(item, "parameter"), GetGetItemsInformation(item, 1) })
                .DoReplaces(item) + "\r\n\r\n";
        }

        private string GetRepositoryGetPart(ConfigurationItem item, string template)
        {
            string getChild = string.Empty;
            if (_configuration.DataLayout.Any(t => t.Parent == item.Name))
            {
                var childDataItem = _configuration.DataLayout.FirstOrDefault(t => t.Parent == item.Name).Name;
                getChild = item.Name.ToPascalCase() + "DataItem." + childDataItem +
                               "s = new PlusLazyLoadingAsyncObservableCollection<" + _configuration.Product +
                               childDataItem + "DataItem>(" +
                               "() => Get" + childDataItem + "sAsync(callContext, " +
                               item.Name.ToPascalCase() + "DataItem))" +
                               "{ AutoAcceptAfterSuccessfullyLoading = true, AutoOverwriteParent = true };";
            }

            return File.ReadAllText(_configuration.InputPath + @"Repository\" + template)
                    .ReplaceSpecialContent(new[]
                    {
                        GetPreFilterInformation(item, "parameter"),
                        GetGetItemsInformation(item, 1),
                        GetPreFilterInformation(item, "mainCall"),
                        GetGetItemsInformation(item, 2),
                        getChild,
                        GetPreFilterInformation(item, "listCall"),
                        GetGetItemsInformation(item, 3)
                    })
                    .DoReplaces(item) + "\r\n\r\n";
        }

        private string GetGetItemsInformation(ConfigurationItem item, int part)
        {
            if (string.IsNullOrEmpty(item.Parent))
            {
                return string.Empty;
            }

            string retValue = string.Empty;

            switch (part)
            {
                case 1:
                    {
                        retValue += ", " + _configuration.Product + item.Parent + "DataItem " + item.Parent.ToPascalCase() + "DataItem";
                        break;
                    }
                case 2:
                    {
                        //retValue += ", _" + _configuration.Product.ToLower() + "DtoFactory.Create" + item.Parent +
                        //                   "FromDataItem(" + item.Parent.ToPascalCase() + "DataItem)";
                        retValue += GetParentParameter(item);
                        break;
                    }
                case 3:
                    {
                        retValue += ", " + item.Parent.ToPascalCase() + "DataItem";
                        break;
                    }
            }

            return retValue;
        }

        private string GetParentParameter(ConfigurationItem item, int startLevel = 0, bool startParent = false)
        {
            string retValue = string.Empty;
            int counter = startLevel;

            ConfigurationItem parent = Helpers.GetParent(item);
            ConfigurationItem ii = item;
            while (parent != null)
            {
                string startP = startParent ? item.Name : item.Parent;

                retValue = ", _" + _configuration.Product.ToLower() + "DtoFactory.Create" + parent.Name +
                            "FromDataItem(" + GetParentString(ii, startP, counter) + ")" + retValue;
                ii = parent;
                parent = Helpers.GetParent(parent);
                counter++;
            }

            return retValue;
        }

        private string GetParentString(ConfigurationItem item, string originalParent, int level)
        {
            string retValue = string.Empty;
            if (level == 0)
            {
                retValue = item.Parent.ToPascalCase() + "DataItem";
            }
            else
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("(").Append(_configuration.Product).Append(item.Parent).Append("DataItem)")
                    .Append(originalParent.ToPascalCase()).Append("DataItem");
                for (int i = 0; i < level; i++)
                {
                    retValue = builder.Append(".Parent.Parent").ToString();
                }
            }

            return retValue;
        }

        private string GetPreFilterInformation(ConfigurationItem item, string information)
        {
            if (item.IsPreFilterItem)
            {
                return string.Empty;
            }

            var filterParameter = string.Empty;
            foreach (var configurationItem in _configuration.DataLayout.Where(t => t.IsPreFilterItem))
            {
                if (information == "parameter")
                {
                    filterParameter += ", " + _configuration.Product + configurationItem.Name + "DataItem " +
                                   configurationItem.Name.ToPascalCase() + "DataItem";

                }

                if (information == "mainCall")
                {
                    filterParameter += ", _" + _configuration.Product.ToLower() + "DtoFactory.Create" + configurationItem.Name +
                                       "FromDataItem(" + configurationItem.Name.ToPascalCase() + "DataItem)";
                }

                if (information == "listCall")
                {
                    filterParameter += ", " + configurationItem.Name.ToPascalCase() + "DataItem";
                }
            }

            return filterParameter;
        }

        private string GetIdentifier(ConfigurationItem dataItem)
        {
            var identifier = string.Empty;
            foreach (var plusDataObject in dataItem.Properties.Where(t => t.IsKey))
            {
                if (identifier != string.Empty) identifier = " && " + identifier;

                identifier += "x." + plusDataObject.Name + ".Equals(dto." + plusDataObject.Name + ")";
            }

            return identifier;
        }

        private string GetReadOnly(ConfigurationItem dataItem)
        {
            var readOnly = string.Empty;
            foreach (var plusDataObject in dataItem.Properties.Where(t => t.IsKey))
                if (plusDataObject.IsReadOnly)
                    readOnly +=
                        dataItem.Name.ToPascalCase() + "Dto." + plusDataObject.Name + " = " +
                        dataItem.Name.ToPascalCase() + "." +
                        plusDataObject.Name + ";\r\n";

            return readOnly;
        }

        #endregion
    }
}
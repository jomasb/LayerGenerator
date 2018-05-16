using System.IO;
using System.Linq;
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

                if (string.IsNullOrEmpty(dataItem.Parent))
                {
                    if (_configuration.DataLayout.Any(t => t.Parent == dataItem.Name))
                    {
                        //parent
                        interfaceContent +=
                            File.ReadAllText(_configuration.InputPath + @"Repository\Contracts\GetParentPart.txt")
                                .DoReplaces(dataItem).ReplaceSpecialContent(new []{ GetPreFilterInformation(dataItem, "parameter") }) + "\r\n\r\n";

                        var childDataItem = _configuration.DataLayout.FirstOrDefault(t => t.Parent == dataItem.Name)
                            .Name;
                        var getChild = childDataItem +
                                       "s = new PlusLazyLoadingAsyncObservableCollection<" + _configuration.Product +
                                       childDataItem + "DataItem>(" +
                                       "() => Get" + childDataItem + "sAsync(callContext, " +
                                       dataItem.Name.ToPascalCase() + "DataItem))" +
                                       "{ AutoAcceptAfterSuccessfullyLoading = true, AutoOverwriteParent = true };";

                        var content =
                            File.ReadAllText(_configuration.InputPath + @"Repository\GetParentPart.txt")
                                .DoReplaces(dataItem).ReplaceSpecialContent(new[] { GetPreFilterInformation(dataItem, "parameter"), GetPreFilterInformation(dataItem, "mainCall"), GetPreFilterInformation(dataItem, "listCall") }) +
                            "\r\n\r\n";
                        content = content.ReplaceSpecialContent(new[] {getChild});
                        repositoryContent += content;
                    }
                    else
                    {
                        //single
                        interfaceContent +=
                            File.ReadAllText(_configuration.InputPath + @"Repository\Contracts\GetPart.txt")
                                .DoReplaces(dataItem).ReplaceSpecialContent(new[] { GetPreFilterInformation(dataItem, "parameter") }) + "\r\n\r\n";
                        repositoryContent +=
                            File.ReadAllText(_configuration.InputPath + @"Repository\GetPart.txt")
                                .DoReplaces(dataItem).ReplaceSpecialContent(new[] { GetPreFilterInformation(dataItem, "parameter"), GetPreFilterInformation(dataItem, "mainCall"), GetPreFilterInformation(dataItem, "listCall") }) + "\r\n\r\n";
                    }
                }
                else
                {
                    //child
                    var transformParent = "_" + _configuration.Product.ToPascalCase() + "DtoFactory.Create" +
                                          dataItem.Parent + "FromDataItem";
                    var content =
                        File.ReadAllText(_configuration.InputPath + @"Repository\Contracts\GetChildPart.txt")
                            .DoReplaces(dataItem) +
                        "\r\n\r\n";
                    content = content.ReplaceSpecialContent(new[] {_configuration.Product + dataItem.Parent});
                    interfaceContent += content;

                    content = File.ReadAllText(_configuration.InputPath + @"Repository\GetChildPart.txt")
                                  .DoReplaces(dataItem) +
                              "\r\n\r\n";
                    content = content.ReplaceSpecialContent(new[]
                        {_configuration.Product + dataItem.Parent, transformParent});
                    repositoryContent += content;
                }

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
                    repositoryContent +=
                        File.ReadAllText(_configuration.InputPath + @"Repository\DeletePart.txt").DoReplaces(dataItem) +
                        "\r\n\r\n";
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
                        content = content.ReplaceSpecialContent(new[] {identifier, readOnly});
                        repositoryContent += content;
                    }

                    if (dataItem.CanEditMultiple)
                        if (string.IsNullOrEmpty(dataItem.Parent))
                        {
                            interfaceContent +=
                                File.ReadAllText(_configuration.InputPath + @"Repository\Contracts\SaveMultiPart.txt")
                                    .DoReplaces(dataItem) + "\r\n\r\n";
                            var content = File.ReadAllText(_configuration.InputPath + @"Repository\SaveMultiPart.txt")
                                              .DoReplaces(dataItem) + "\r\n\r\n";
                            content = content.ReplaceSpecialContent(new[] {identifier, readOnly});
                            repositoryContent += content;
                        }
                }

                if (dataItem.Name.EndsWith("Version"))
                    if (!string.IsNullOrEmpty(dataItem.Parent))
                    {
                        interfaceContent +=
                            File.ReadAllText(_configuration.InputPath + @"Repository\Contracts\VersionPart.txt")
                                .DoReplaces(dataItem) + "\r\n\r\n";
                        var content = File.ReadAllText(_configuration.InputPath + @"Repository\VersionPart.txt")
                                          .DoReplaces(dataItem) + "\r\n\r\n";
                        content = content.ReplaceSpecialContent(new[] {identifier, readOnly});
                        repositoryContent += content;
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
                    if (plusDataObject.IsRequired || plusDataObject.Length != string.Empty)
                    {
                        dataItemContent += "[";
                        if (plusDataObject.IsRequired)
                        {
                            dataItemContent += "Required, ";
                        }

                        if (!string.IsNullOrEmpty(plusDataObject.Length))
                        {
                            if (plusDataObject.Type == "int")
                            {
                                dataItemContent += "NumericRange(0, " + Helpers.GetMaxValue(plusDataObject.Length) + "), ";
                            }

                            if (plusDataObject.Type == "string")
                            {
                                dataItemContent += "MaxLength(" + plusDataObject.Length + "), ";
                            }
                        }

                        dataItemContent = dataItemContent.Substring(0, dataItemContent.Length - 2);

                        if (dataItemContent.Length > 0)
                        {
                            dataItemContent += "]\r\n";
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

                }

                foreach (var childDataItem in _configuration.DataLayout.Where(t => t.Parent == dataItem.Name))
                    dataItemContent +=
                        File.ReadAllText(_configuration.InputPath +
                                         @"Repository\DataItems\DataItemCollectionPart.txt")
                            .DoReplaces(childDataItem) + "\r\n";

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
                    itemContent += plusDataObject.Name + " = dataItem." + plusDataObject.Name + ",\r\n";
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
    }
}
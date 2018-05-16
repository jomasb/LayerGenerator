using System.IO;
using System.Linq;
using PlusLayerCreator.Items;

namespace PlusLayerCreator.Configure
{
    public class BusinessServiceLocalPart
    {
        private readonly Configuration _configuration;

        public BusinessServiceLocalPart(Configuration configuration)
        {
            _configuration = configuration;
        }

        public void CreateBusinessService(bool withBo)
        {
            var fileNameExtension = withBo ? string.Empty : "NoBO";

            foreach (var dataItem in _configuration.DataLayout)
            {
                var interfaceReadContent = string.Empty;
                var interfaceSaveContent = string.Empty;

                var serviceReadContent = string.Empty;
                var serviceSaveContent = string.Empty;

                var serviceMockReadContent = string.Empty;
                var serviceMockSaveContent = string.Empty;

                if (string.IsNullOrEmpty(dataItem.Parent))
                {
                    //Parent
                    interfaceReadContent +=
                        File.ReadAllText(_configuration.InputPath + @"Service\Contracts\GetPart" +
                                            fileNameExtension + ".txt").DoReplaces(dataItem).ReplaceSpecialContent(new[] { GetPreFilterInformation(dataItem, "parameter") }) + "\r\n\r\n";
                    serviceReadContent +=
                        File.ReadAllText(_configuration.InputPath + @"Service\GetPart" + fileNameExtension + ".txt")
                            .DoReplaces(dataItem).ReplaceSpecialContent(new[] { GetPreFilterInformation(dataItem, "parameter"), GetPreFilterInformation(dataItem, "listCall"), GetPreFilterInformation(dataItem, "arguments") }) + "\r\n\r\n";

                    if (!withBo)
                    {
                        var propertyAssignments = Helpers.GetBusinessServiceLocalGetMock(dataItem);
                        serviceMockReadContent +=
                            File.ReadAllText(_configuration.InputPath + @"Service\GetPartNoBOMock.txt")
                                .ReplaceSpecialContent(new[] { propertyAssignments, GetPreFilterInformation(dataItem, "parameter")})
                                .DoReplaces(dataItem) + "\r\n\r\n";
                    }
                }
                else
                {
                    //Child
                    var content =
                        File.ReadAllText(_configuration.InputPath + @"Service\Contracts\GetChildPart" +
                                            fileNameExtension + ".txt").DoReplaces(dataItem) +
                        "\r\n\r\n";
                    content = content.ReplaceSpecialContent(new[] { GetParentParameter(dataItem, "param") });
                    interfaceReadContent += content;


                    content = File.ReadAllText(_configuration.InputPath + @"Service\GetChildPart" +
                                                fileNameExtension + ".txt").DoReplaces(dataItem) +
                                "\r\n\r\n";
                    content = content.ReplaceSpecialContent(new[] { GetParentParameter(dataItem, "param"), GetParentParameter(dataItem, "call"), GetParentParameter(dataItem, "") });
                    serviceReadContent += content;

                    if (!withBo)
                    {
                        var propertyAssignments = Helpers.GetBusinessServiceLocalGetMock(dataItem);
                        serviceMockReadContent +=
                            File.ReadAllText(_configuration.InputPath + @"Service\GetChildPartNoBOMock.txt")
                                .ReplaceSpecialContent(new []{ GetParentParameter(dataItem, "param"), propertyAssignments})
                                .DoReplaces(dataItem) +
                            "\r\n\r\n";
                    }
                }

                if (dataItem.CanEdit && !dataItem.CanEditMultiple)
                { 
                    interfaceSaveContent +=
                        File.ReadAllText(_configuration.InputPath + @"Service\Contracts\SavePart" +
                                            fileNameExtension + ".txt")
                            .ReplaceSpecialContent(new []{ GetParentParameter(dataItem, "param") })
                            .DoReplaces(dataItem) + "\r\n\r\n";
                    serviceSaveContent +=
                        File.ReadAllText(
                                _configuration.InputPath + @"Service\SavePart" + fileNameExtension + ".txt")
                            .ReplaceSpecialContent(new[] { GetParentParameter(dataItem, "param"), GetParentParameter(dataItem, "call"), GetParentParameter(dataItem, "") })
                            .DoReplaces(dataItem) + "\r\n\r\n";

                    if (!withBo)
                        serviceMockSaveContent +=
                            File.ReadAllText(_configuration.InputPath + @"Service\SavePartNoBOMock.txt")
                                .ReplaceSpecialContent(new[] { GetParentParameter(dataItem, "param") })
                                .DoReplaces(dataItem) + "\r\n\r\n";
                }

                if (dataItem.CanEdit && dataItem.CanEditMultiple)
                {
                    interfaceSaveContent +=
                        File.ReadAllText(_configuration.InputPath + @"Service\Contracts\SaveMultiPart" +
                                            fileNameExtension + ".txt")
                            .ReplaceSpecialContent(new[] { GetParentParameter(dataItem, "param") })
                            .DoReplaces(dataItem) + "\r\n\r\n";
                    serviceSaveContent +=
                        File.ReadAllText(_configuration.InputPath + @"Service\SaveMultiPart" + fileNameExtension +
                                            ".txt")
                            .ReplaceSpecialContent(new[] { GetParentParameter(dataItem, "param"), GetParentParameter(dataItem, "call"), GetParentParameter(dataItem, "") })
                            .DoReplaces(dataItem) + "\r\n\r\n";

                    if (!withBo)
                        serviceMockSaveContent +=
                            File.ReadAllText(_configuration.InputPath + @"Service\SaveMultiPartNoBOMock.txt")
                                .ReplaceSpecialContent(new[] { GetParentParameter(dataItem, "param") })
                                .DoReplaces(dataItem) + "\r\n\r\n";
                }

                if (dataItem.Name.EndsWith("Version"))
                    if (!string.IsNullOrEmpty(dataItem.Parent))
                    {
                        interfaceSaveContent +=
                            File.ReadAllText(_configuration.InputPath + @"Service\Contracts\VersionPart" +
                                             fileNameExtension + ".txt").DoReplaces(dataItem) + "\r\n\r\n";
                        serviceSaveContent +=
                            File.ReadAllText(_configuration.InputPath + @"Service\VersionPart" + fileNameExtension +
                                             ".txt").DoReplaces(dataItem) + "\r\n\r\n";

                        if (!withBo)
                            serviceMockSaveContent +=
                                File.ReadAllText(_configuration.InputPath + @"Service\VersionPartNoBOMock.txt")
                                    .DoReplaces(dataItem) + "\r\n\r\n";
                    }

                var servicePath = withBo
                    ? _configuration.InputPath + @"Service\Contracts\IServiceTemplate.cs"
                    : _configuration.InputPath + @"Service\Contracts\IServiceNoBOTemplate.cs";

                Helpers.CreateFileFromPath(servicePath,
                    _configuration.OutputPath + @"Service\Contracts\I" + _configuration.Product + dataItem.Name +
                    "Service.cs", new[] {interfaceReadContent, interfaceSaveContent}, dataItem);


                servicePath = withBo
                    ? _configuration.InputPath + @"Service\ServiceTemplate.cs"
                    : _configuration.InputPath + @"Service\ServiceNoBOTemplate.cs";

                Helpers.CreateFileFromPath(servicePath,
                    _configuration.OutputPath + @"Service\" + _configuration.Product + dataItem.Name + "Service.cs",
                    new[] {serviceReadContent, serviceSaveContent, string.Empty}, dataItem);

                if (!withBo)
                    Helpers.CreateFileFromPath(servicePath,
                        _configuration.OutputPath + @"Service\" + _configuration.Product + dataItem.Name +
                        "ServiceMock.cs", new[] {serviceMockReadContent, serviceMockSaveContent, "Mock"}, dataItem);
            }
        }

        private string GetParentParameter(ConfigurationItem item, string type)
        {
            string retValue = string.Empty;

            if (!string.IsNullOrEmpty(item.Parent))
            {
                if (type == "param")
            {
                retValue = ", " + _configuration.Product + item.Parent + " parent" + item.Parent;
            }
            else if (type == "call")
            {
                retValue = ", parent" + item.Parent;
            }
            else
            {
                retValue = "arguments.Add(\""+ item.Parent +"\", parent" + item.Parent + ");\r\n";
            }
                return GetParentParameter(_configuration.DataLayout.First(t => t.Name == item.Parent), type) + retValue;
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
                    filterParameter += ", " + _configuration.Product + configurationItem.Name + " " +
                                       configurationItem.Name.ToPascalCase();
                }

                if (information == "arguments")
                {
                    filterParameter += "arguments.Add(\"" + _configuration.Product + configurationItem.Name + "\", " +
                                       configurationItem.Name.ToPascalCase() + ");";
                }

                if (information == "listCall")
                {
                    filterParameter += ", " + configurationItem.Name.ToPascalCase();
                }
            }

            return filterParameter;
        }

        public void CreateTandem()
        {
            var converterMessageToBoContent = string.Empty;
            var converterBoToMessageContent = string.Empty;
            foreach (var dataItem in _configuration.DataLayout)
            {
                var serverMappingWriteContent = string.Empty;

                string serverMappingReadContent =
                    (File.ReadAllText(_configuration.InputPath + @"Service\Tandem\ServerMappingGetPart.txt")
                            .DoReplaces(dataItem) + "\r\n\r\n").ReplaceSpecialContent(new[]
                        {dataItem.Server + "Server.Singleton.AddTransaction" + dataItem.TransactionCodeRead});

                if (dataItem.CanEdit && !dataItem.CanEditMultiple)
                    serverMappingWriteContent =
                        (File.ReadAllText(_configuration.InputPath + @"Service\Tandem\ServerMappingSavePart.txt")
                             .DoReplaces(dataItem) + "\r\n\r\n").ReplaceSpecialContent(new[]
                        {
                            dataItem.Server + "Server.Singleton.AddTransaction" + dataItem.TransactionCodeWrite,
                            dataItem.Server + dataItem.RepRplWrite + "." + dataItem.TableCountProperty
                        });

                if (dataItem.CanEdit && dataItem.CanEditMultiple)
                    serverMappingWriteContent =
                        (File.ReadAllText(_configuration.InputPath + @"Service\Tandem\ServerMappingSaveMultiPart.txt")
                             .DoReplaces(dataItem) + "\r\n\r\n").ReplaceSpecialContent(new[]
                        {
                            dataItem.Server + "Server.Singleton.AddTransaction" + dataItem.TransactionCodeWrite,
                            dataItem.Server + dataItem.RepRplWrite + "." + dataItem.TableCountProperty
                        });


                foreach (var plusDataObject in dataItem.Properties)
                {
                    converterMessageToBoContent +=
                        "serviceMessage." + plusDataObject.Name + "(" + plusDataObject.Name.ToPascalCase() + "." +
                        plusDataObject.Name + ", i);\r\n";
                    converterBoToMessageContent +=
                        plusDataObject.Name + " = serviceMessage." + plusDataObject.Name + "(i),\r\n";
                }

                Helpers.CreateFileFromPath(_configuration.InputPath + @"Service\Tandem\Converter.cs",
                    _configuration.OutputPath + @"Service\Tandem\" + _configuration.Product + dataItem.Name +
                    "Converter.cs", new[] {converterMessageToBoContent, converterBoToMessageContent}, dataItem);
                Helpers.CreateFileFromPath(_configuration.InputPath + @"Service\Tandem\ServerMapping.cs",
                    _configuration.OutputPath + @"Service\Tandem\" + _configuration.Product + dataItem.Name +
                    "ServerMapping.cs", new[] {serverMappingReadContent, serverMappingWriteContent}, dataItem);
            }
        }
    }
}
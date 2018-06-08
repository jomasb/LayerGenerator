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
            var fileNameExtension = string.Empty;

            foreach (var dataItem in _configuration.DataLayout)
            {
                var interfaceReadContent = string.Empty;
                var interfaceSaveContent = string.Empty;

                var gatewayReadContent = string.Empty;
                var gatewaySaveContent = string.Empty;

                var key = string.Empty;
                var identifier = string.Empty;
                var readOnlyMappingDto = string.Empty;
                var readOnlyMappingBo = string.Empty;

                if (string.IsNullOrEmpty(dataItem.Parent))
                {
                    //Parent
                    interfaceReadContent +=
                        File.ReadAllText(_configuration.InputPath + @"Gateway\Contracts\GetPart" +
                                            fileNameExtension + ".txt").DoReplaces(dataItem).ReplaceSpecialContent(new[] { GetPreFilterInformation(dataItem, "parameter") }) + "\r\n\r\n";
                    gatewayReadContent +=
                        File.ReadAllText(_configuration.InputPath + @"Gateway\GetPart" + fileNameExtension + ".txt")
                            .DoReplaces(dataItem).ReplaceSpecialContent(new[] { GetPreFilterInformation(dataItem, "parameter"), GetPreFilterInformation(dataItem, "listCall"), GetPreFilterInformation(dataItem, "arguments") }) + "\r\n\r\n";
                }
                else
                {
                    //Child
                    var content =
                        File.ReadAllText(_configuration.InputPath + @"Gateway\Contracts\GetChildPart" +
                                            fileNameExtension + ".txt").DoReplaces(dataItem) +
                        "\r\n\r\n";
                    content = content.ReplaceSpecialContent(new[] { GetParentParameter(dataItem, "param") });
                    interfaceReadContent += content;


                    content = File.ReadAllText(_configuration.InputPath + @"Gateway\GetChildPart" +
                                                fileNameExtension + ".txt").DoReplaces(dataItem) +
                                "\r\n\r\n";
                    content = content.ReplaceSpecialContent(new[] { GetParentParameter(dataItem, "param"), GetParentParameter(dataItem, "call"), GetParentParameter(dataItem, "") });
                    gatewayReadContent += content;
                }

                if (dataItem.CanEdit && !dataItem.CanEditMultiple)
                {
                    //Save
                    interfaceSaveContent +=
                        File.ReadAllText(_configuration.InputPath + @"Gateway\Contracts\SavePart" +
                                            fileNameExtension + ".txt")
                            .ReplaceSpecialContent(new[] { GetParentParameter(dataItem, "param") })
                            .DoReplaces(dataItem) + "\r\n\r\n";
                    gatewaySaveContent +=
                        File.ReadAllText(
                                _configuration.InputPath + @"Gateway\SavePart" + fileNameExtension + ".txt")
                            .ReplaceSpecialContent(new[] { GetParentParameter(dataItem, "param"), GetParentParameter(dataItem, "call"), GetParentParameter(dataItem, "") })
                            .DoReplaces(dataItem) + "\r\n\r\n";

                    //Delete
                    interfaceSaveContent +=
                        File.ReadAllText(_configuration.InputPath + @"Gateway\Contracts\DeletePart" +
                                         fileNameExtension + ".txt")
                            .ReplaceSpecialContent(new[] { GetParentParameter(dataItem, "param") })
                            .DoReplaces(dataItem) + "\r\n\r\n";
                    gatewaySaveContent +=
                        File.ReadAllText(
                                _configuration.InputPath + @"Gateway\DeletePart" + fileNameExtension + ".txt")
                            .ReplaceSpecialContent(new[] { GetParentParameter(dataItem, "param"), GetParentParameter(dataItem, "call"), GetParentParameter(dataItem, "") })
                            .DoReplaces(dataItem) + "\r\n\r\n";
                }

                if (dataItem.CanEdit && dataItem.CanEditMultiple)
                {
                    interfaceSaveContent +=
                        File.ReadAllText(_configuration.InputPath + @"Gateway\Contracts\SaveMultiPart" +
                                            fileNameExtension + ".txt")
                            .ReplaceSpecialContent(new[] { GetParentParameter(dataItem, "param") })
                            .DoReplaces(dataItem) + "\r\n\r\n";
                    gatewaySaveContent +=
                        File.ReadAllText(_configuration.InputPath + @"Gateway\SaveMultiPart" + fileNameExtension +
                                            ".txt")
                            .ReplaceSpecialContent(new[] { GetParentParameter(dataItem, "param"), GetParentParameter(dataItem, "call"), GetParentParameter(dataItem, "") })
                            .DoReplaces(dataItem) + "\r\n\r\n";
                }

                if (dataItem.Name.EndsWith("Version"))
                    if (!string.IsNullOrEmpty(dataItem.Parent))
                    {
                        interfaceSaveContent +=
                            File.ReadAllText(_configuration.InputPath + @"Gateway\Contracts\VersionPart" +
                                             fileNameExtension + ".txt").DoReplaces(dataItem) + "\r\n\r\n";
                        gatewaySaveContent +=
                            File.ReadAllText(_configuration.InputPath + @"Gateway\VersionPart" + fileNameExtension +
                                             ".txt").DoReplaces(dataItem) + "\r\n\r\n";
                    }

                foreach (var plusDataObject in dataItem.Properties.Where(t => t.IsKey))
                {
                    key += dataItem.Name.ToPascalCase() + "." + plusDataObject.Name + ", ";
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
                            dataItem.Name.ToPascalCase() + "Dto." + plusDataObject.Name + " = " +
                            dataItem.Name.ToPascalCase() + "." +
                            plusDataObject.Name + ";\r\n";
                        readOnlyMappingBo +=
                            dataItem.Name.ToPascalCase() + "." + plusDataObject.Name + " = " +
                            dataItem.Name.ToPascalCase() + "Dto." +
                            plusDataObject.Name + ";\r\n";
                    }
                }

                if (key.Length > 0) key = key.Substring(0, key.Length - 2);

                if (identifier.Length > 0) identifier = identifier.Substring(0, identifier.Length - 3);

                //special content in gateway
                gatewaySaveContent = gatewaySaveContent.ReplaceSpecialContent(new[]
                    {key, identifier, readOnlyMappingDto, readOnlyMappingBo});

                var gatewayPath = _configuration.InputPath + @"Gateway\Contracts\IGatewayTemplate.cs";

                Helpers.CreateFileFromPath(gatewayPath,
                    _configuration.OutputPath + @"Gateway\Contracts\I" + _configuration.Product + dataItem.Name +
                    "Gateway.cs", new[] { interfaceReadContent, interfaceSaveContent }, dataItem);


                gatewayPath = _configuration.InputPath + @"Gateway\GatewayTemplate.cs";

                Helpers.CreateFileFromPath(gatewayPath,
                    _configuration.OutputPath + @"Gateway\" + _configuration.Product + dataItem.Name + "Gateway.cs",
                    new[] { gatewayReadContent, gatewaySaveContent, string.Empty }, dataItem);
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
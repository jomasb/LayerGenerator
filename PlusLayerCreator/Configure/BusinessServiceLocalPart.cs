using System.IO;
using PlusLayerCreator.Items;

namespace PlusLayerCreator.Configure
{
    public class BusinessServiceLocalPart
    {
        private readonly Configuration _configuration;
	    private readonly string _converterFillFromDto;
	    private readonly string _converterFillFromMessage;
	    private readonly string _converterReadReqRplPart;
	    private readonly string _converterWriteReqRplPart;

		public BusinessServiceLocalPart(Configuration configuration)
        {
            _configuration = configuration;
	        _converterFillFromDto =
		        File.ReadAllText(configuration.InputPath + @"Service\Tandem\ConverterFillFromDtoPart.txt");
	        _converterFillFromMessage =
		        File.ReadAllText(configuration.InputPath + @"Service\Tandem\ConverterFillFromMessagePart.txt");
	        _converterReadReqRplPart =
		        File.ReadAllText(configuration.InputPath + @"Service\Tandem\ConverterReadReqRplPart.txt");
	        _converterWriteReqRplPart =
		        File.ReadAllText(configuration.InputPath + @"Service\Tandem\ConverterWriteReqRplPart.txt");
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
                                            fileNameExtension + ".txt").DoReplacesClient(dataItem).ReplaceSpecialContent(new[] { Helpers.GetPreFilterInformation(dataItem, "parameter") }) + "\r\n\r\n";
                    serviceReadContent +=
                        File.ReadAllText(_configuration.InputPath + @"Service\GetPart" + fileNameExtension + ".txt")
                            .DoReplacesClient(dataItem).ReplaceSpecialContent(new[] { Helpers.GetPreFilterInformation(dataItem, "parameter"), Helpers.GetPreFilterInformation(dataItem, "listCall"), Helpers.GetPreFilterInformation(dataItem, "arguments") }) + "\r\n\r\n";

                    if (!withBo)
                    {
                        var propertyAssignments = Helpers.GetBusinessServiceLocalGetMock(dataItem);
                        serviceMockReadContent +=
                            File.ReadAllText(_configuration.InputPath + @"Service\GetPartNoBOMock.txt")
                                .ReplaceSpecialContent(new[] { propertyAssignments, Helpers.GetPreFilterInformation(dataItem, "parameter") })
                                .DoReplacesClient(dataItem) + "\r\n\r\n";
                    }
                }
                else
                {
                    //Child
                    var content =
                        File.ReadAllText(_configuration.InputPath + @"Service\Contracts\GetChildPart" +
                                            fileNameExtension + ".txt").DoReplacesClient(dataItem) +
                        "\r\n\r\n";
                    content = content.ReplaceSpecialContent(new[] { Helpers.GetParentParameter(dataItem, "param") });
                    interfaceReadContent += content;


                    content = File.ReadAllText(_configuration.InputPath + @"Service\GetChildPart" +
                                                fileNameExtension + ".txt").DoReplacesClient(dataItem) +
                                "\r\n\r\n";
                    content = content.ReplaceSpecialContent(new[] { Helpers.GetParentParameter(dataItem, "param"), Helpers.GetParentParameter(dataItem, "call"), Helpers.GetParentParameter(dataItem, "") });
                    serviceReadContent += content;

                    if (!withBo)
                    {
                        var propertyAssignments = Helpers.GetBusinessServiceLocalGetMock(dataItem);
                        serviceMockReadContent +=
                            File.ReadAllText(_configuration.InputPath + @"Service\GetChildPartNoBOMock.txt")
                                .ReplaceSpecialContent(new[] { Helpers.GetParentParameter(dataItem, "param"), propertyAssignments })
                                .DoReplacesClient(dataItem) +
                            "\r\n\r\n";
                    }
                }

                if (dataItem.CanEdit && !dataItem.CanEditMultiple)
                {
                    //Save
                    interfaceSaveContent +=
                        File.ReadAllText(_configuration.InputPath + @"Service\Contracts\SavePart" +
                                            fileNameExtension + ".txt")
                            .ReplaceSpecialContent(new[] { Helpers.GetParentParameter(dataItem, "param") })
                            .DoReplacesClient(dataItem) + "\r\n\r\n";
                    serviceSaveContent +=
                        File.ReadAllText(
                                _configuration.InputPath + @"Service\SavePart" + fileNameExtension + ".txt")
                            .ReplaceSpecialContent(new[] { Helpers.GetParentParameter(dataItem, "param"), Helpers.GetParentParameter(dataItem, "call"), Helpers.GetParentParameter(dataItem, "") })
                            .DoReplacesClient(dataItem) + "\r\n\r\n";

                    if (!withBo)
                        serviceMockSaveContent +=
                            File.ReadAllText(_configuration.InputPath + @"Service\SavePartNoBOMock.txt")
                                .ReplaceSpecialContent(new[] { Helpers.GetParentParameter(dataItem, "param") })
                                .DoReplacesClient(dataItem) + "\r\n\r\n";

                    //Delete
                    interfaceSaveContent +=
                        File.ReadAllText(_configuration.InputPath + @"Service\Contracts\DeletePart" +
                                         fileNameExtension + ".txt")
                            .ReplaceSpecialContent(new[] { Helpers.GetParentParameter(dataItem, "param") })
                            .DoReplacesClient(dataItem) + "\r\n\r\n";
                    serviceSaveContent +=
                        File.ReadAllText(
                                _configuration.InputPath + @"Service\DeletePart" + fileNameExtension + ".txt")
                            .ReplaceSpecialContent(new[] { Helpers.GetParentParameter(dataItem, "param"), Helpers.GetParentParameter(dataItem, "call"), Helpers.GetParentParameter(dataItem, "") })
                            .DoReplacesClient(dataItem) + "\r\n\r\n";

                    if (!withBo)
                        serviceMockSaveContent +=
                            File.ReadAllText(_configuration.InputPath + @"Service\DeletePartNoBOMock.txt")
                                .ReplaceSpecialContent(new[] { Helpers.GetParentParameter(dataItem, "param") })
                                .DoReplacesClient(dataItem) + "\r\n\r\n";
                }

                if (dataItem.CanEdit && dataItem.CanEditMultiple)
                {
                    interfaceSaveContent +=
                        File.ReadAllText(_configuration.InputPath + @"Service\Contracts\SaveMultiPart" +
                                            fileNameExtension + ".txt")
                            .ReplaceSpecialContent(new[] { Helpers.GetParentParameter(dataItem, "param") })
                            .DoReplacesClient(dataItem) + "\r\n\r\n";
                    serviceSaveContent +=
                        File.ReadAllText(_configuration.InputPath + @"Service\SaveMultiPart" + fileNameExtension +
                                            ".txt")
                            .ReplaceSpecialContent(new[] { Helpers.GetParentParameter(dataItem, "param"), Helpers.GetParentParameter(dataItem, "call"), Helpers.GetParentParameter(dataItem, "") })
                            .DoReplacesClient(dataItem) + "\r\n\r\n";

                    if (!withBo)
                        serviceMockSaveContent +=
                            File.ReadAllText(_configuration.InputPath + @"Service\SaveMultiPartNoBOMock.txt")
                                .ReplaceSpecialContent(new[] { Helpers.GetParentParameter(dataItem, "param") })
                                .DoReplacesClient(dataItem) + "\r\n\r\n";
                }

                if (dataItem.Name.EndsWith("Version"))
                    if (!string.IsNullOrEmpty(dataItem.Parent))
                    {
                        interfaceSaveContent +=
                            File.ReadAllText(_configuration.InputPath + @"Service\Contracts\VersionPart" +
                                             fileNameExtension + ".txt").DoReplacesClient(dataItem) + "\r\n\r\n";
                        serviceSaveContent +=
                            File.ReadAllText(_configuration.InputPath + @"Service\VersionPart" + fileNameExtension +
                                             ".txt").DoReplacesClient(dataItem) + "\r\n\r\n";

                        if (!withBo)
                            serviceMockSaveContent +=
                                File.ReadAllText(_configuration.InputPath + @"Service\VersionPartNoBOMock.txt")
                                    .DoReplacesClient(dataItem) + "\r\n\r\n";
                    }

                var servicePath = withBo
                    ? _configuration.InputPath + @"Service\Contracts\IServiceTemplate.cs"
                    : _configuration.InputPath + @"Service\Contracts\IServiceNoBOTemplate.cs";

                Helpers.CreateFileFromPath(servicePath,
                    _configuration.OutputPath + @"Service\Contracts\I" + _configuration.Product + dataItem.Name + fileNameExtension +
                    "Service.cs", new[] { interfaceReadContent, interfaceSaveContent }, dataItem);


                servicePath = withBo
                    ? _configuration.InputPath + @"Service\ServiceTemplate.cs"
                    : _configuration.InputPath + @"Service\ServiceNoBOTemplate.cs";

                Helpers.CreateFileFromPath(servicePath,
                    _configuration.OutputPath + @"Service\" + _configuration.Product + dataItem.Name + fileNameExtension + "Service.cs",
                    new[] { serviceReadContent, serviceSaveContent, string.Empty }, dataItem);

                if (!withBo)
                    Helpers.CreateFileFromPath(servicePath,
                        _configuration.OutputPath + @"Service\" + _configuration.Product + dataItem.Name + fileNameExtension +
                        "ServiceMock.cs", new[] { serviceMockReadContent, serviceMockSaveContent, "Mock" }, dataItem);
            }
        }

        public void CreateTandem()
        {
	        foreach (var dataItem in _configuration.DataLayout)
	        {
		        CreateServerMapping(dataItem);
		        CreateConverter(dataItem);
	        }
        }

	    private void CreateServerMapping(ConfigurationItem dataItem)
	    {
			var serverMappingWriteContent = string.Empty;
		    string tabAnz = dataItem.Server + "TabAnz";

			string serverMappingReadContent =
				(File.ReadAllText(_configuration.InputPath + @"Service\Tandem\ServerMappingGetPart.txt")
						.DoReplacesClient(dataItem) + "\r\n\r\n").ReplaceSpecialContent(new[]
					{dataItem.Server + "Server.Singleton.AddTransaction" + dataItem.TransactionCodeRead});

			if (dataItem.CanEdit && !dataItem.CanEditMultiple)
			{
				serverMappingWriteContent =
				(File.ReadAllText(_configuration.InputPath + @"Service\Tandem\ServerMappingSavePart.txt")
						.DoReplacesClient(dataItem) + "\r\n\r\n").ReplaceSpecialContent(new[]
				{
					dataItem.Server + "Server.Singleton.AddTransaction" + dataItem.TransactionCodeWrite,
					dataItem.Server + dataItem.RepRplWrite + "." + tabAnz
				});
			}

			if (dataItem.CanEdit && dataItem.CanEditMultiple)
			{
				serverMappingWriteContent =
					(File.ReadAllText(_configuration.InputPath + @"Service\Tandem\ServerMappingSaveMultiPart.txt")
							.DoReplacesClient(dataItem) + "\r\n\r\n").ReplaceSpecialContent(new[]
					{
						dataItem.Server + "Server.Singleton.AddTransaction" + dataItem.TransactionCodeWrite,
						dataItem.Server + dataItem.RepRplWrite + "." + tabAnz
					});
			}
				
			Helpers.CreateFileFromPath(_configuration.InputPath + @"Service\Tandem\ServerMapping.cs",
				_configuration.OutputPath + @"Service\Tandem\" + _configuration.Product + dataItem.Name +
				"ServerMapping.cs", new[] { serverMappingReadContent, serverMappingWriteContent }, dataItem);
		}

	    private void CreateConverter(ConfigurationItem dataItem)
	    {
		    string interfacePart = string.Empty;
		    string readPart = GetReadPart(dataItem);
		    string writePart = GetWritePart(dataItem);
		    string fillPart = GetFillPart(dataItem);
			
		    interfacePart += ", I" + dataItem.Server + dataItem.RepRplRead + "BusinessObject";
		    if (dataItem.CanEdit)
		    {
				interfacePart += ", I" + dataItem.Server + dataItem.RepRplWrite + "BusinessObject";
			}
			
			Helpers.CreateFileFromPath(_configuration.InputPath + @"Service\Tandem\Converter.cs",
				_configuration.OutputPath + @"Service\Tandem\" + _configuration.Product + dataItem.Name +
				"Converter.cs", new[] { interfacePart, readPart, writePart, fillPart }, dataItem);
		}

	    private string GetReadPart(ConfigurationItem dataItem)
		{
			string getPart = _converterReadReqRplPart
				.Replace("$Server$", dataItem.Server)
			    .Replace("$ReqRpl$", dataItem.RepRplRead)
				.Replace("$Transcode$", dataItem.TransactionCodeRead)
			    .Replace("$TabAnz$", dataItem.Server + "TabAnz")
				.DoReplacesClient(dataItem);

		    return getPart;
	    }
	    private string GetWritePart(ConfigurationItem dataItem)
	    {
		    string writePart = _converterWriteReqRplPart
			    .Replace("$Server$", dataItem.Server)
			    .Replace("$ReqRpl$", dataItem.RepRplWrite)
			    .Replace("$Transcode$", dataItem.TransactionCodeWrite)
			    .Replace("$TabAnz$", dataItem.Server + "TabAnz")
			    .DoReplacesClient(dataItem);

		    return writePart;
		}

		private string GetFillPart(ConfigurationItem dataItem)
	    {
		    string messageToDtoContent = string.Empty;
		    string dtoToMessageContent = string.Empty;

		    foreach (var plusDataObject in dataItem.Properties)
		    {
			    if (plusDataObject.Name == "LupdTimestamp")
			    {
				    messageToDtoContent += "serviceMessage." + plusDataObject.MessageField.ToCamelCase() + "(PlusFormat.FormatTandemTimestamp26(" + plusDataObject.Name.ToPascalCase() + "." + plusDataObject.Name + ", i));\r\n";
				    dtoToMessageContent += plusDataObject.Name + " = PlusFormat.ParseTandemTimestamp26(serviceMessage." + plusDataObject.MessageField.ToCamelCase() + "(i));\r\n";
				}
				else
			    {
				    messageToDtoContent += "serviceMessage." + plusDataObject.MessageField.ToCamelCase() + "(" + plusDataObject.Name.ToPascalCase() + "." + plusDataObject.Name + ", i);\r\n";
				    dtoToMessageContent += plusDataObject.Name + " = serviceMessage." + plusDataObject.MessageField.ToCamelCase() + "(i);\r\n";
				}
			}

		    string fillFromMessage = _converterFillFromMessage.DoReplacesClient(dataItem)
			    .ReplaceSpecialContent(new[] {messageToDtoContent});
			string fillFromDto = _converterFillFromDto.DoReplacesClient(dataItem)
			    .ReplaceSpecialContent(new[] {dtoToMessageContent});

			return fillFromMessage + "\r\n\r\n" + fillFromDto;
	    }
	}
}
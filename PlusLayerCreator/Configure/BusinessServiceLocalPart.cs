using System.IO;
using PlusLayerCreator.Items;

namespace PlusLayerCreator.Configure
{
	public class BusinessServiceLocalPart
	{
		private Configuration _configuration;

		public BusinessServiceLocalPart(Configuration configuration)
		{
			_configuration = configuration;
		}

		public void CreateBusinessService(bool withBo)
		{
			string fileNameExtension = withBo ? string.Empty : "NoBO";

			foreach (ConfigurationItem dataItem in _configuration.DataLayout)
			{
				string interfaceReadContent = string.Empty;
				string interfaceSaveContent = string.Empty;

				string serviceReadContent = string.Empty;
				string serviceSaveContent = string.Empty;

				string serviceMockReadContent = string.Empty;
				string serviceMockSaveContent = string.Empty;

				if (dataItem.CanRead)
				{
					if (string.IsNullOrEmpty(dataItem.Parent))
					{
                        //Parent
						interfaceReadContent += Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Service\Contracts\GetPart" + fileNameExtension + ".txt"), dataItem) + "\r\n\r\n";
						serviceReadContent += Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Service\GetPart" + fileNameExtension + ".txt"), dataItem) + "\r\n\r\n";

					    if (!withBo)
					    {
					        string propertyAssignments = Helpers.GetBusinessServiceLocalGetMock(dataItem);
					        serviceMockReadContent += Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Service\GetPartNoBOMock.txt").Replace("$specialContent1$", propertyAssignments), dataItem) + "\r\n\r\n";
                        }
                    }
					else
					{
					    //Child
                        string content = Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Service\Contracts\GetChildPart" + fileNameExtension + ".txt"), dataItem) +
										 "\r\n\r\n";
						content = Helpers.ReplaceSpecialContent(content, new[] { _configuration.Product + dataItem.Parent });
						interfaceReadContent += content;


						content = Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Service\GetChildPart" + fileNameExtension + ".txt"), dataItem) +
								  "\r\n\r\n";
						content = Helpers.ReplaceSpecialContent(content, new[] { _configuration.Product + dataItem.Parent });
						serviceReadContent += content;

					    if (!withBo)
					    {
					        string propertyAssignments = Helpers.GetBusinessServiceLocalGetMock(dataItem);
					        serviceMockReadContent += Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Service\GetChildPartNoBOMock.txt").Replace("$specialContent1$", dataItem.Parent).Replace("$specialContent2$", propertyAssignments), dataItem) + "\r\n\r\n";
					    }
                    }
				}

				if (dataItem.CanEdit && !dataItem.CanEditMultiple)
				{
					if (string.IsNullOrEmpty(dataItem.Parent))
					{
						interfaceSaveContent += Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Service\Contracts\SavePart" + fileNameExtension + ".txt"), dataItem) + "\r\n\r\n";
						serviceSaveContent += Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Service\SavePart" + fileNameExtension + ".txt"), dataItem) + "\r\n\r\n";

					    if (!withBo)
					    {
					        serviceMockSaveContent += Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Service\SavePartNoBOMock.txt"), dataItem) + "\r\n\r\n";
					    }

                    }
				}
				if (dataItem.CanEdit && dataItem.CanEditMultiple)
				{
					if (string.IsNullOrEmpty(dataItem.Parent))
					{
						interfaceSaveContent += Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Service\Contracts\SaveMultiPart" + fileNameExtension + ".txt"), dataItem) + "\r\n\r\n";
						serviceSaveContent += Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Service\SaveMultiPart" + fileNameExtension + ".txt"), dataItem) + "\r\n\r\n";

					    if (!withBo)
					    {
					        serviceMockSaveContent += Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Service\SaveMultiPartNoBOMock.txt"), dataItem) + "\r\n\r\n";
					    }
                    }
				}

			    if (dataItem.Name == "Version")
			    {
			        if (!string.IsNullOrEmpty(dataItem.Parent))
			        {
			            interfaceSaveContent += Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Service\Contracts\VersionPart" + fileNameExtension + ".txt"), dataItem) + "\r\n\r\n";
			            serviceSaveContent += Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Service\VersionPart" + fileNameExtension + ".txt"), dataItem) + "\r\n\r\n";

			            if (!withBo)
			            {
			                serviceMockSaveContent += Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Service\VersionPartNoBOMock.txt"), dataItem) + "\r\n\r\n";
			            }
			        }
                }

				string servicePath = withBo ? _configuration.InputPath + @"Service\Contracts\IServiceTemplate.cs" : _configuration.InputPath + @"Service\Contracts\IServiceNoBOTemplate.cs";

				Helpers.CreateFileFromPath(servicePath,
					_configuration.OutputPath + @"Service\Contracts\I" + _configuration.Product + dataItem.Name + "Service.cs", new[] { interfaceReadContent, interfaceSaveContent }, dataItem);


				servicePath = withBo ? _configuration.InputPath + @"Service\ServiceTemplate.cs" : _configuration.InputPath + @"Service\ServiceNoBOTemplate.cs";

				Helpers.CreateFileFromPath(servicePath,
					_configuration.OutputPath + @"Service\" + _configuration.Product + dataItem.Name + "Service.cs", new[] { serviceReadContent, serviceSaveContent, string.Empty }, dataItem);

			    if (!withBo)
			    {
			        Helpers.CreateFileFromPath(servicePath,
			            _configuration.OutputPath + @"Service\" + _configuration.Product + dataItem.Name + "ServiceMock.cs", new[] { serviceMockReadContent, serviceMockSaveContent, "Mock" }, dataItem);
                }
			}
		}

		public void CreateTandem()
		{
			string converterMessageToBoContent = string.Empty;
			string converterBoToMessageContent = string.Empty;
			foreach (ConfigurationItem dataItem in _configuration.DataLayout)
			{
				string serverMappingReadContent = string.Empty;
				string serverMappingWriteContent = string.Empty;
				if (dataItem.CanRead)
				{
					serverMappingReadContent = Helpers.ReplaceSpecialContent(Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Service\Tandem\ServerMappingGetPart.txt"), dataItem) + "\r\n\r\n", new[] { dataItem.Server + "Server.Singleton.AddTransaction" + dataItem.TransactionCodeRead });
				}

				if (dataItem.CanEdit && !dataItem.CanEditMultiple)
				{
					serverMappingWriteContent = Helpers.ReplaceSpecialContent(Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Service\Tandem\ServerMappingSavePart.txt"), dataItem) + "\r\n\r\n", new[] { dataItem.Server + "Server.Singleton.AddTransaction" + dataItem.TransactionCodeWrite, dataItem.Server + dataItem.RepRplWrite + "." + dataItem.TableCountProperty });
                }

				if (dataItem.CanEdit && dataItem.CanEditMultiple)
				{
				    serverMappingWriteContent = Helpers.ReplaceSpecialContent(Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Service\Tandem\ServerMappingSaveMultiPart.txt"), dataItem) + "\r\n\r\n", new[] { dataItem.Server + "Server.Singleton.AddTransaction" + dataItem.TransactionCodeWrite, dataItem.Server + dataItem.RepRplWrite + "." + dataItem.TableCountProperty });
                }


				foreach (ConfigurationProperty plusDataObject in dataItem.Properties)
				{
					converterMessageToBoContent += "serviceMessage." + plusDataObject.Name + "(" + Helpers.ToPascalCase(plusDataObject.Name) + "." +
												   plusDataObject.Name + ", i);\r\n";
					converterBoToMessageContent += plusDataObject.Name + " = serviceMessage." + plusDataObject.Name + "(i),\r\n";
				}

				Helpers.CreateFileFromPath(_configuration.InputPath + @"Service\Tandem\Converter.cs", _configuration.OutputPath + @"Service\Tandem\" + _configuration.Product + dataItem.Name + "Converter.cs", new[] { converterMessageToBoContent, converterBoToMessageContent }, dataItem);
				Helpers.CreateFileFromPath(_configuration.InputPath + @"Service\Tandem\ServerMapping.cs", _configuration.OutputPath + @"Service\Tandem\" + _configuration.Product + dataItem.Name + "ServerMapping.cs", new[] { serverMappingReadContent, serverMappingWriteContent }, dataItem);
			}
		}
	}
}

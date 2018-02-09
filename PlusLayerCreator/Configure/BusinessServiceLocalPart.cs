using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

				if (dataItem.CanRead)
				{
					if (string.IsNullOrEmpty(dataItem.Parent))
					{
						interfaceReadContent += Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Service\Contracts\GetPart" + fileNameExtension + ".txt"), dataItem.Name) + "\r\n\r\n";
						serviceReadContent += Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Service\GetPart" + fileNameExtension + ".txt"), dataItem.Name) + "\r\n\r\n";
					}
					else
					{
						string content = Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Service\Contracts\GetChildPart" + fileNameExtension + ".txt"), dataItem.Name) +
										 "\r\n\r\n";
						content = Helpers.ReplaceSpecialContent(content, new[] { _configuration.Product + dataItem.Parent });
						interfaceReadContent += content;


						content = Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Service\GetChildPart" + fileNameExtension + ".txt"), dataItem.Name) +
								  "\r\n\r\n";
						content = Helpers.ReplaceSpecialContent(content, new[] { _configuration.Product + dataItem.Parent });
						serviceReadContent += content;
					}
				}

				if (dataItem.CanEdit && !dataItem.CanEditMultiple)
				{
					if (string.IsNullOrEmpty(dataItem.Parent))
					{
						interfaceSaveContent += Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Service\Contracts\SavePart" + fileNameExtension + ".txt"), dataItem.Name) + "\r\n\r\n";
						serviceSaveContent += Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Service\SavePart" + fileNameExtension + ".txt"), dataItem.Name) + "\r\n\r\n";
					}
				}
				if (dataItem.CanEdit && dataItem.CanEditMultiple)
				{
					if (string.IsNullOrEmpty(dataItem.Parent))
					{
						interfaceSaveContent += Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Service\Contracts\SaveMultiPart" + fileNameExtension + ".txt"), dataItem.Name) + "\r\n\r\n";
						serviceSaveContent += Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Service\SaveMultiPart" + fileNameExtension + ".txt"), dataItem.Name) + "\r\n\r\n";
					}
				}

				string servicePath = withBo ? _configuration.InputPath + @"Service\Contracts\IServiceTemplate.cs" : _configuration.InputPath + @"Service\Contracts\IServiceNoBOTemplate.cs";

				Helpers.CreateFile(servicePath,
					_configuration.OutputPath + @"Service\Contracts\I" + _configuration.Product + dataItem.Name + "Service.cs", new[] { interfaceReadContent, interfaceSaveContent }, dataItem.Name);


				servicePath = withBo ? _configuration.InputPath + @"Service\ServiceTemplate.cs" : _configuration.InputPath + @"Service\ServiceNoBOTemplate.cs";

				Helpers.CreateFile(servicePath,
					_configuration.OutputPath + @"Service\" + _configuration.Product + dataItem.Name + "Service.cs", new[] { serviceReadContent, serviceSaveContent }, dataItem.Name);
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
					serverMappingReadContent = Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Service\Tandem\ServerMappingGetPart.txt"), dataItem.Name) + "\r\n\r\n";
				}

				if (dataItem.CanEdit && !dataItem.CanEditMultiple)
				{
					serverMappingWriteContent = Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Service\Tandem\ServerMappingSavePart.txt"), dataItem.Name) + "\r\n\r\n";
				}

				if (dataItem.CanEdit && dataItem.CanEditMultiple)
				{
					serverMappingWriteContent = Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Service\Tandem\ServerMappingSaveMultiPart.txt"), dataItem.Name) + "\r\n\r\n";
				}


				foreach (ConfigurationProperty plusDataObject in dataItem.Properties)
				{
					converterMessageToBoContent += "serviceMessage." + plusDataObject.Name + "(" + Helpers.ToPascalCase(plusDataObject.Name) + "." +
												   plusDataObject.Name + ", i);\r\n";
					converterBoToMessageContent += plusDataObject.Name + " = serviceMessage." + plusDataObject.Name + "(i),\r\n";
				}

				Helpers.CreateFile(_configuration.InputPath + @"Service\Tandem\Converter.cs", _configuration.OutputPath + @"Service\Tandem\" + _configuration.Product + dataItem.Name + "Converter.cs", new[] { converterMessageToBoContent, converterBoToMessageContent }, dataItem.Name);
				Helpers.CreateFile(_configuration.InputPath + @"Service\Tandem\ServerMapping.cs", _configuration.OutputPath + @"Service\Tandem\" + _configuration.Product + dataItem.Name + "ServerMapping.cs", new[] { serverMappingReadContent, serverMappingWriteContent }, dataItem.Name);
			}
		}
	}
}

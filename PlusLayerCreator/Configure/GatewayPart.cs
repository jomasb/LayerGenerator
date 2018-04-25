using System.IO;
using System.Linq;
using PlusLayerCreator.Items;

namespace PlusLayerCreator.Configure
{
	public class GatewayPart
	{
		private Configuration _configuration;
		private string _createGatewayDtoTemplateUpperPart;
		private string _createGatewayDtoTemplateLowerPart;

		public GatewayPart(Configuration configuration)
		{
			_configuration = configuration;
			_createGatewayDtoTemplateUpperPart = File.ReadAllText(configuration.InputPath + @"Gateway\CreateDtoUpperPart.txt");
			_createGatewayDtoTemplateLowerPart = File.ReadAllText(configuration.InputPath + @"Gateway\CreateDtoLowerPart.txt");
		}

		public void CreateGateway()
		{
			string interfaceReadContent = string.Empty;
			string interfaceSaveContent = string.Empty;

			string gatewayReadContent = string.Empty;
			string gatewaySaveContent = string.Empty;

			string key = string.Empty;
			string identifier = string.Empty;
			string readOnlyMappingDto = string.Empty;
			string readOnlyMappingBo = string.Empty;

			//Random rnd = new Random();
			//string mock = string.Empty;

			foreach (ConfigurationItem dataItem in _configuration.DataLayout)
			{
				if (dataItem.CanRead)
				{
					if (string.IsNullOrEmpty(dataItem.Parent))
					{
						interfaceReadContent += Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Gateway\Contracts\GetPart.txt"), dataItem.Name) + "\r\n\r\n";
						gatewayReadContent += Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Gateway\GetPart.txt"), dataItem.Name) + "\r\n\r\n";
					}
					else
					{
						string content = Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Gateway\Contracts\GetChildPart.txt"), dataItem.Name) +
										 "\r\n\r\n";
						content = Helpers.ReplaceSpecialContent(content, new[] { _configuration.Product + dataItem.Parent });
						interfaceReadContent += content;


						content = Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Gateway\GetChildPart.txt"), dataItem.Name) +
								  "\r\n\r\n";
						content = Helpers.ReplaceSpecialContent(content, new[] { _configuration.Product + dataItem.Parent });
						gatewayReadContent += content;
					}
				}

				if (dataItem.CanEdit && !dataItem.CanEditMultiple)
				{
					if (string.IsNullOrEmpty(dataItem.Parent))
					{
						interfaceSaveContent += Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Gateway\Contracts\SavePart.txt"), dataItem.Name) + "\r\n\r\n";
						gatewaySaveContent += Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Gateway\SavePart.txt"), dataItem.Name) + "\r\n\r\n";
					}
				}
				if (dataItem.CanEdit && dataItem.CanEditMultiple)
				{
					if (string.IsNullOrEmpty(dataItem.Parent))
					{
						interfaceSaveContent += Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Gateway\Contracts\SaveMultiPart.txt"), dataItem.Name) + "\r\n\r\n";
						gatewaySaveContent += Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Gateway\SaveMultiPart.txt"), dataItem.Name) + "\r\n\r\n";
					}
				}

				foreach (ConfigurationProperty plusDataObject in dataItem.Properties.Where(t => t.IsKey))
				{
					key += Helpers.ToPascalCase(dataItem.Name) + "." + plusDataObject.Name + ", ";
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
							Helpers.ToPascalCase(dataItem.Name) + "Dto." + plusDataObject.Name + " = " + Helpers.ToPascalCase(dataItem.Name) + "." +
							plusDataObject.Name + ";\r\n";
						readOnlyMappingBo +=
							Helpers.ToPascalCase(dataItem.Name) + "." + plusDataObject.Name + " = " + Helpers.ToPascalCase(dataItem.Name) + "Dto." +
							plusDataObject.Name + ";\r\n";
					}
				}

				if (key.Length > 0)
				{
					key = key.Substring(0, key.Length - 2);
				}

				if (identifier.Length > 0)
				{
					identifier = identifier.Substring(0, identifier.Length - 3);
				}

				//special content in gateway
				gatewaySaveContent = Helpers.ReplaceSpecialContent(gatewaySaveContent,
					new[] { key, identifier, readOnlyMappingDto, readOnlyMappingBo });

				Helpers.CreateFileFromPath(_configuration.InputPath + @"Gateway\Contracts\IGatewayTemplate.cs",
					_configuration.OutputPath + @"Gateway\Contracts\I" + _configuration.Product + dataItem.Name + "Gateway.cs", new[] { interfaceReadContent, interfaceSaveContent }, dataItem.Name);
				Helpers.CreateFileFromPath(_configuration.InputPath + @"Gateway\GatewayTemplate.cs",
					_configuration.OutputPath + @"Gateway\" + _configuration.Product + dataItem.Name + "Gateway.cs", new[] { gatewayReadContent, gatewaySaveContent }, dataItem.Name);

			}
		}

		public void CreateDto(bool withBO)
		{
			string layer = withBO ? "Gateway" : "Service";

			foreach (ConfigurationItem dataItem in _configuration.DataLayout)
			{
				string dtoContent = string.Empty;

				foreach (ConfigurationProperty plusDataObject in dataItem.Properties)
				{
					dtoContent += "public " + plusDataObject.Type + " " + plusDataObject.Name + " {get; set;}\r\n\r\n";
				}

				Helpers.CreateFileFromPath(_configuration.InputPath + layer + @"\Dtos\DtoTemplate.cs",
					_configuration.OutputPath + layer + @"\Dtos\" + _configuration.Product + dataItem.Name + ".cs", new[] { dtoContent }, dataItem.Name);
			}
		}

		public void CreateDtoFactory()
		{
			string factoryContent = string.Empty;
			foreach (ConfigurationItem dataItem in _configuration.DataLayout)
			{
				factoryContent += Helpers.DoReplaces(_createGatewayDtoTemplateUpperPart + "\r\n", dataItem.Name);
				foreach (ConfigurationProperty plusDataObject in dataItem.Properties)
				{
					if (plusDataObject.IsKey)
					{
						factoryContent += plusDataObject.Name + " = bo.Key." + plusDataObject.Name + ",\r\n";
					}
					else
					{
						factoryContent += plusDataObject.Name + " = bo." + plusDataObject.Name + ",\r\n";
					}
				}
				factoryContent += Helpers.DoReplaces(_createGatewayDtoTemplateLowerPart, dataItem.Name);
			}

			string[] contentsDtoFactory = {
				factoryContent
			};

			Helpers.CreateFileFromPath(_configuration.InputPath + @"Gateway\DtoFactoryTemplate.cs", _configuration.OutputPath + @"Gateway\" + _configuration.Product + "DtoFactory.cs", contentsDtoFactory);
		}
	}
}

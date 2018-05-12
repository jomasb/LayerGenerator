using System.IO;
using System.Linq;
using PlusLayerCreator.Items;

namespace PlusLayerCreator.Configure
{
    public class GatewayPart
    {
        private readonly Configuration _configuration;
        private readonly string _createGatewayDtoTemplatePart;

        public GatewayPart(Configuration configuration)
        {
            _configuration = configuration;
            _createGatewayDtoTemplatePart = File.ReadAllText(configuration.InputPath + @"Gateway\CreateDtoPart.txt");
        }

        public void CreateGateway()
        {
            var interfaceReadContent = string.Empty;
            var interfaceSaveContent = string.Empty;

            var gatewayReadContent = string.Empty;
            var gatewaySaveContent = string.Empty;

            var key = string.Empty;
            var identifier = string.Empty;
            var readOnlyMappingDto = string.Empty;
            var readOnlyMappingBo = string.Empty;

            //Random rnd = new Random();
            //string mock = string.Empty;

            foreach (var dataItem in _configuration.DataLayout)
            {
                if (string.IsNullOrEmpty(dataItem.Parent))
                {
                    interfaceReadContent +=
                        File.ReadAllText(_configuration.InputPath + @"Gateway\Contracts\GetPart.txt")
                            .DoReplaces(dataItem) + "\r\n\r\n";
                    gatewayReadContent +=
                        File.ReadAllText(_configuration.InputPath + @"Gateway\GetPart.txt").DoReplaces(dataItem) +
                        "\r\n\r\n";
                }
                else
                {
                    var content = File.ReadAllText(_configuration.InputPath + @"Gateway\Contracts\GetChildPart.txt")
                                        .DoReplaces(dataItem) +
                                    "\r\n\r\n";
                    content = content.ReplaceSpecialContent(new[] {_configuration.Product + dataItem.Parent});
                    interfaceReadContent += content;


                    content = File.ReadAllText(_configuration.InputPath + @"Gateway\GetChildPart.txt")
                                    .DoReplaces(dataItem) +
                                "\r\n\r\n";
                    content = content.ReplaceSpecialContent(new[] {_configuration.Product + dataItem.Parent});
                    gatewayReadContent += content;
                }

                if (dataItem.CanEdit && !dataItem.CanEditMultiple)
                    if (string.IsNullOrEmpty(dataItem.Parent))
                    {
                        interfaceSaveContent +=
                            File.ReadAllText(_configuration.InputPath + @"Gateway\Contracts\SavePart.txt")
                                .DoReplaces(dataItem) + "\r\n\r\n";
                        gatewaySaveContent +=
                            File.ReadAllText(_configuration.InputPath + @"Gateway\SavePart.txt").DoReplaces(dataItem) +
                            "\r\n\r\n";
                    }

                if (dataItem.CanEdit && dataItem.CanEditMultiple)
                    if (string.IsNullOrEmpty(dataItem.Parent))
                    {
                        interfaceSaveContent +=
                            File.ReadAllText(_configuration.InputPath + @"Gateway\Contracts\SaveMultiPart.txt")
                                .DoReplaces(dataItem) + "\r\n\r\n";
                        gatewaySaveContent +=
                            File.ReadAllText(_configuration.InputPath + @"Gateway\SaveMultiPart.txt")
                                .DoReplaces(dataItem) + "\r\n\r\n";
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

                Helpers.CreateFileFromPath(_configuration.InputPath + @"Gateway\Contracts\IGatewayTemplate.cs",
                    _configuration.OutputPath + @"Gateway\Contracts\I" + _configuration.Product + dataItem.Name +
                    "Gateway.cs", new[] {interfaceReadContent, interfaceSaveContent}, dataItem);
                Helpers.CreateFileFromPath(_configuration.InputPath + @"Gateway\GatewayTemplate.cs",
                    _configuration.OutputPath + @"Gateway\" + _configuration.Product + dataItem.Name + "Gateway.cs",
                    new[] {gatewayReadContent, gatewaySaveContent}, dataItem);
            }
        }

        public void CreateDto(bool withBO)
        {
            var layer = withBO ? "Gateway" : "Service";

            foreach (var dataItem in _configuration.DataLayout)
            {
                var dtoContent = string.Empty;

                foreach (var plusDataObject in dataItem.Properties)
                    dtoContent += "public " + plusDataObject.Type + " " + plusDataObject.Name + " {get; set;}\r\n\r\n";

                Helpers.CreateFileFromPath(_configuration.InputPath + layer + @"\Dtos\DtoTemplate.cs",
                    _configuration.OutputPath + layer + @"\Dtos\" + _configuration.Product + dataItem.Name + ".cs",
                    new[] {dtoContent}, dataItem);
            }
        }

        public void CreateDtoFactory()
        {
            var factoryContent = string.Empty;
            foreach (var dataItem in _configuration.DataLayout)
            {
                var itemContent = string.Empty;
                foreach (var plusDataObject in dataItem.Properties)
                    if (plusDataObject.IsKey)
                        itemContent += plusDataObject.Name + " = bo.Key." + plusDataObject.Name + ",\r\n";
                    else
                        itemContent += plusDataObject.Name + " = bo." + plusDataObject.Name + ",\r\n";
                factoryContent +=
                    _createGatewayDtoTemplatePart.DoReplaces(dataItem).ReplaceSpecialContent(new[] {itemContent}) +
                    "\r\n";
            }

            string[] contentsDtoFactory =
            {
                factoryContent
            };

            Helpers.CreateFileFromPath(_configuration.InputPath + @"Gateway\DtoFactoryTemplate.cs",
                _configuration.OutputPath + @"Gateway\" + _configuration.Product + "DtoFactory.cs", contentsDtoFactory);
        }
    }
}
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
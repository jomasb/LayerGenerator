using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using PlusLayerCreator.Items;
using PlusLayerCreator.Model;

namespace PlusLayerCreator
{
	public class Settings
	{
		public DataContractJsonSerializerSettings SerializerSettings =
			new DataContractJsonSerializerSettings
			{
				UseSimpleDictionaryFormat = true
			};



		public void Export(string fileName, Configuration configuration)
		{
			var stream = new FileStream(fileName, FileMode.Create);
			using (var writer = JsonReaderWriterFactory.CreateJsonWriter(
				stream, Encoding.UTF8, true, true, "  "))
			{
				var ser = new DataContractJsonSerializer(typeof(ConfigurationDto), SerializerSettings);
				ser.WriteObject(writer, ItemFactory.GetConfigurationFromItem(configuration));
				writer.Flush();
			}
		}

		public Configuration Import(string fileName)
		{
			try
			{
				var ser = new DataContractJsonSerializer(typeof(ConfigurationDto));
				var stream = new FileStream(fileName, FileMode.Open);
				var configuration = ser.ReadObject(stream) as ConfigurationDto;
				stream.Close();
				var properties = typeof(ConfigurationDto).GetProperties();
				foreach (var property in properties)
				{
					if (property.PropertyType == typeof(IList<ConfigurationItemDto>))
					{
						var dataItems =
							property.GetValue(configuration) as IList<ConfigurationItemDto>;
						configuration.DataLayout = new List<ConfigurationItemDto>();
						if (dataItems != null)
							foreach (var plusDataItem in dataItems)
								configuration.DataLayout.Add(plusDataItem);
					}
					else if (property.PropertyType == typeof(IList<DirectHopItemDto>))
					{
						var directHops =
							property.GetValue(configuration) as IList<DirectHopItemDto>;
						configuration.DirectHops = new List<DirectHopItemDto>();
						if (directHops != null)
							foreach (var directHopItem in directHops)
								configuration.DirectHops.Add(directHopItem);
					}
					else
					{
						var propertyInfo = configuration.GetType().GetProperty(property.Name);
						if (propertyInfo != null)
							propertyInfo.SetValue(configuration, property.GetValue(configuration));
						else
							throw new Exception("Property not found.");
					}
				}

				return ItemFactory.GetConfigurationFromDto(configuration);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}
	}
}

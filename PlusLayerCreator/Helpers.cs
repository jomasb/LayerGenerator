using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PlusLayerCreator
{
	public static class Helpers
	{
		public static string ToPascalCase(string input)
		{
			return input[0].ToString().ToLower() + input.Substring(1, input.Length - 1);
		}

		public static string Serialize(Configuration configuration)
		{
			string serializedConfiguration = string.Empty;
			serializedConfiguration += "[Settings]\r\n";
			PropertyInfo[] properties = typeof(Configuration).GetProperties();
			foreach (var property in properties)
			{
				if (property.PropertyType == typeof(IList<PlusDataItemProperty>))
				{
					IList<PlusDataItemProperty> list = property.GetValue(configuration) as IList<PlusDataItemProperty>;
					serializedConfiguration += "\r\n[DataItems]\r\n";
					foreach (var plusDataItemProperty in list)
					{
						PropertyInfo[] innerProperties = typeof(PlusDataItemProperty).GetProperties();
						foreach (var innerProperty in innerProperties)
						{
							serializedConfiguration += innerProperty.Name + "=" + innerProperty.GetValue(plusDataItemProperty) + ";";
						}
						serializedConfiguration = serializedConfiguration.Substring(0, serializedConfiguration.Length - 1);
						serializedConfiguration += "\r\n";
					}
				}
				else
				{
					serializedConfiguration += property.Name + "=" + property.GetValue(configuration) + "\r\n";
				}
			}
			return serializedConfiguration;
		}

		public static int GetMaxValue(string length)
		{
			if (length == string.Empty)
			{
				return 999999;
			}

			int value;
			string computedString = string.Empty;
			if (int.TryParse(length, out value))
			{
				for (int i = 1; i < value; i++)
				{
					computedString += "9";
				}
			}
			return int.Parse(computedString);
		}

		public static Configuration ImportSettingsFromFile(string dialogFileName)
		{
			Configuration configuration = new Configuration();
			configuration.DataLayout = new List<PlusDataItemProperty>();
			var lines = File.ReadAllLines(dialogFileName);
			foreach (var line in lines)
			{
				if (line.Length == 0 || line.StartsWith("["))
				{
					continue;
				}

				if (line.Contains(";"))
				{
					//muliple elements (PlusDataItemProperties)
					PlusDataItemProperty plusDataItemProperty = new PlusDataItemProperty();
					Char delimiter = ';';
					String[] substrings = line.Split(delimiter);

					foreach (string substring in substrings)
					{
						Char innerDelimiter = '=';
						String[] innerSubstrings = substring.Split(innerDelimiter);

						PropertyInfo pinfo = typeof(PlusDataItemProperty).GetProperty(innerSubstrings[0]);
						if (pinfo.PropertyType == typeof(bool))
						{
							pinfo.SetValue(plusDataItemProperty, bool.Parse(innerSubstrings[1]));
						}
						if (pinfo.PropertyType == typeof(string))
						{
							pinfo.SetValue(plusDataItemProperty, innerSubstrings[1]);
						}
					}

					configuration.DataLayout.Add(plusDataItemProperty);
				}
				else
				{
					//single elements
					Char delimiter = '=';
					String[] substrings = line.Split(delimiter);

					PropertyInfo pinfo = typeof(Configuration).GetProperty(substrings[0]);
					if (pinfo.PropertyType == typeof(bool))
					{
						pinfo.SetValue(configuration, bool.Parse(substrings[1]));
					}
					if (pinfo.PropertyType == typeof(string))
					{
						pinfo.SetValue(configuration, substrings[1]);
					}
				}
			}

			return configuration;
		}
	}
}

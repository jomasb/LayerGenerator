using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace PlusLayerCreator
{
	public static class Helpers
	{
		public static string ToPascalCase(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return input;
			}

			return input[0].ToString().ToLower() + input.Substring(1, input.Length - 1);
		}

		public static string Serialize(Configuration configuration)
		{
			string serializedConfiguration = string.Empty;
			serializedConfiguration += "[Settings]\r\n";
			PropertyInfo[] properties = typeof(Configuration).GetProperties();
			foreach (PropertyInfo property in properties)
			{
				if (property.PropertyType == typeof(IList<PlusDataItem>))
				{
					IList<PlusDataItem> dataItems = property.GetValue(configuration) as IList<PlusDataItem>;
					serializedConfiguration += "\r\n[DataItems]\r\n";
					foreach (var plusDataItem in dataItems)
					{
						PropertyInfo[] innerItems = typeof(PlusDataItem).GetProperties();
						foreach (PropertyInfo innerItem in innerItems)
						{
							//serializedConfiguration += "{";
							if (innerItem.PropertyType == typeof(ObservableCollection<PlusDataItemProperty>))
							{
								ObservableCollection<PlusDataItemProperty> dataItemProperties = innerItem.GetValue(plusDataItem) as ObservableCollection<PlusDataItemProperty>;
								serializedConfiguration += "\r\n";
								foreach (var plusDataItemProperty in dataItemProperties)
								{
									serializedConfiguration += "{DataItem=" + plusDataItem.Name + ";";
									PropertyInfo[] innerProperties = typeof(PlusDataItemProperty).GetProperties();
									foreach (var innerProperty in innerProperties)
									{
										serializedConfiguration += innerProperty.Name + "=" + innerProperty.GetValue(plusDataItemProperty) + ";";
									}
									serializedConfiguration = serializedConfiguration.Substring(0, serializedConfiguration.Length - 1);
									serializedConfiguration += "}\r\n";
								}
							}
							else
							{
								serializedConfiguration += innerItem.Name + "=" + innerItem.GetValue(plusDataItem) + ";";
							}
							//serializedConfiguration += "}";
						}
						serializedConfiguration = serializedConfiguration.Substring(0, serializedConfiguration.Length - 1) + "\r\n";
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
				for (int i = 0; i < value; i++)
				{
					computedString += "9";
				}
			}
			return int.Parse(computedString);
		}

		public static Configuration ImportSettingsFromFile(string dialogFileName)
		{
			Char delimiter = ';';
			Char innerDelimiter = '=';

			Configuration configuration = new Configuration();
			configuration.DataLayout = new List<PlusDataItem>();
			var lines = File.ReadAllLines(dialogFileName);
			foreach (var line in lines)
			{
				if (line.Length == 0 || line.StartsWith("["))
				{
					continue;
				}

				if (line.StartsWith("{"))
				{
					int counter = 0;
					String[] substrings = line.Replace("{", "").Replace("}", "").Split(delimiter);
					PlusDataItem dataItem = null;
					PlusDataItemProperty propertyDataItem = new PlusDataItemProperty();
					foreach (string substring in substrings)
					{
						if (counter == 0)
						{
							String[] innerSubstrings = substring.Split(innerDelimiter);
							dataItem = configuration.DataLayout.FirstOrDefault(t => t.Name == innerSubstrings[1]);
							if (dataItem.Properties == null)
							{
								dataItem.Properties = new ObservableCollection<PlusDataItemProperty>();
							}
						}
						else
						{
							String[] innerSubstrings = substring.Split(innerDelimiter);

							PropertyInfo pinfo = typeof(PlusDataItemProperty).GetProperty(innerSubstrings[0]);
							if (pinfo != null)
							{
								if (pinfo.PropertyType == typeof(bool))
								{
									pinfo.SetValue(propertyDataItem, bool.Parse(innerSubstrings[1]));
								}
								if (pinfo.PropertyType == typeof(string))
								{
									pinfo.SetValue(propertyDataItem, innerSubstrings[1]);
								}
							}
						}
						counter++;
					}
					dataItem.Properties.Add(propertyDataItem);
					continue;
				}


				if (line.Contains(";"))
				{
					//muliple elements (PlusDataItemProperties)
					PlusDataItem dataItem = new PlusDataItem();
					
					String[] substrings = line.Split(delimiter);

					foreach (string substring in substrings)
					{
						String[] innerSubstrings = substring.Split(innerDelimiter);

						if (innerSubstrings[0] != string.Empty)
						{
							PropertyInfo pinfo = typeof(PlusDataItem).GetProperty(innerSubstrings[0]);
							if (pinfo != null)
							{
								if (pinfo.PropertyType == typeof(bool))
								{
									pinfo.SetValue(dataItem, bool.Parse(innerSubstrings[1]));
								}
								if (pinfo.PropertyType == typeof(string))
								{
									pinfo.SetValue(dataItem, innerSubstrings[1]);
								}
							}
						}
					}

					configuration.DataLayout.Add(dataItem);
				}
				else
				{
					//single elements
					Char singleDelimiter = '=';
					String[] substrings = line.Split(singleDelimiter);

					PropertyInfo pinfo = typeof(Configuration).GetProperty(substrings[0]);
					if (pinfo != null)
					{
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
			}

			return configuration;
		}

		public static void ExportSettingsToFile(string fileName, Configuration configuration)
		{
			FileInfo fileInfo = new FileInfo(fileName);
			fileInfo.Directory.Create();
			File.WriteAllText(fileInfo.FullName, Helpers.Serialize(configuration));
		}
	}
}

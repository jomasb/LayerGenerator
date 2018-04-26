using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using PlusLayerCreator.Items;

namespace PlusLayerCreator
{
	public static class Helpers
	{
		public static Configuration Configuration;

		public static string FilterChildViewModelTemplate;
		public static string FilterPropertyTemplate;
		public static string FilterComboBoxPropertyTemplate;
		public static string FilterTextBoxXamlTemplate;
		public static string FilterComboBoxXamlTemplate;
		public static string FilterCheckBoxXamlTemplate;
		public static string FilterDateTimePickerXamlTemplate;

		public static T DeserializeJSon<T>(string jsonString)
		{
			DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
			MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
			T obj = (T)ser.ReadObject(stream);
			return obj;
		}

		public static string GetLocaliatzionExtension(string input)
		{
			if (input.EndsWith("e"))
			{
				return "n";
			}
			else if (input.EndsWith("er"))
			{
				return "";
			}
			else
			{
				return "en";
			}
		}

		public static string ToPascalCase(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return input;
			}

			return input[0].ToString().ToLower() + input.Substring(1, input.Length - 1);
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

		public static string DoReplaces(string input, ConfigurationItem item = null)
		{
			input = input.Replace("$PPRODUCT$", Configuration.Product.ToUpper());
			input = input.Replace("$Product$", Configuration.Product);
			input = input.Replace("$product$", ToPascalCase(Configuration.Product));
		    input = input.Replace("$Item$", item == null ? string.Empty : item.Name);
		    input = input.Replace("$item$", item == null ? string.Empty : ToPascalCase(item.Name));
			input = input.Replace("$Parent$", item == null ? string.Empty : item.Parent);
            input = input.Replace("$parent$", item == null ? string.Empty : ToPascalCase(item.Parent));
            input = input.Replace("$Dialog$", Configuration.DialogName);
			input = input.Replace("$dialog$", ToPascalCase(Configuration.DialogName));
		    input = input.Replace("$Handle$", Configuration.ControllerHandle);

            return input;
		}

		public static string DoReplaces2(string input, string name = "", ConfigurationItem item = null, string type = "")
		{
			input = input.Replace("$Product$", Configuration.Product);
			input = input.Replace("$product$", ToPascalCase(Configuration.Product));
			input = input.Replace("$Item$", item == null ? string.Empty : item.Name);
			input = input.Replace("$item$", item == null ? string.Empty : ToPascalCase(item.Name));
		    input = input.Replace("$Dialog$", Configuration.DialogName);
		    input = input.Replace("$dialog$", ToPascalCase(Configuration.DialogName));
            input = input.Replace("$Name$", name);
			input = input.Replace("$name$", ToPascalCase(name));
			input = input.Replace("$Type$", type);
			input = input.Replace("$type$", ToPascalCase(type));

			return input;
		}

		public static string ReplaceSpecialContent(string fileContent, string[] contents = null)
		{
			if (contents != null)
			{
				for (int i = 0; i < contents.Length; i++)
				{
					fileContent = fileContent.Replace("$specialContent" + (i + 1) + "$", contents[i]);
				}
			}

			return fileContent;
		}

	    public static void CreateFileFromString(string input, string output, string[] contents = null, ConfigurationItem item = null)
	    {
	        string fileContent = input;

	        fileContent = DoReplaces(fileContent, item);
	        fileContent = ReplaceSpecialContent(fileContent, contents);

	        FileInfo fileInfo = new FileInfo(output);
	        if (fileInfo.Directory != null)
	        {
	            fileInfo.Directory.Create();
	        }
	        File.WriteAllText(fileInfo.FullName, fileContent);
	    }

        public static void CreateFileFromPath(string inputPath, string output, string[] contents = null, ConfigurationItem item = null)
		{
			string fileContent = File.ReadAllText(inputPath);

		    CreateFileFromString(fileContent, output, contents, item);
		}

		public static void CreateFilterContents(ConfigurationItem plusDataItem, ConfigurationProperty plusDataObject, out string filterMembersContent, out string filterPropertiesContent, out string filterPredicateResetContent,
			out string filterPredicatesContent, out string filterXamlContent, out string filterMultiSelectorsInitializeContent)
		{
			filterMembersContent = string.Empty;
			filterPropertiesContent = string.Empty;
			filterPredicateResetContent = string.Empty;
			filterPredicatesContent = string.Empty;
			filterXamlContent = string.Empty;
			filterMultiSelectorsInitializeContent = string.Empty;
		    string filterType = string.Empty;


            string filterSuffix = plusDataObject.Type == "string" ? string.Empty : ".ToString()";
			if (plusDataObject.Type == "string" || plusDataObject.Type == "int")
			{
			    filterType = "string";

                if (plusDataObject.FilterPropertyType == "TextBox")
				{
					filterMembersContent = "private string _" + ToPascalCase(plusDataObject.Name) + ";";
					filterPropertiesContent = FilterPropertyTemplate;
					filterPredicateResetContent = plusDataObject.Name + " = string.Empty;";
					filterPredicatesContent = ".StartsWith(x => x." + plusDataObject.Name + filterSuffix + ", x => x." +
											   plusDataObject.Name + ")";
					filterXamlContent = DoReplaces(FilterTextBoxXamlTemplate, plusDataItem);
				}

				if (plusDataObject.FilterPropertyType == "ComboBox")
				{
					filterMembersContent = "private MultiValueSelector<string> _" +
											ToPascalCase(plusDataObject.Name) + "MultiSelector;";
					filterPropertiesContent = FilterComboBoxPropertyTemplate;
					filterPredicatesContent = ".IsContainedInList(x => x." + plusDataObject.Name + filterSuffix + ", x => x." +
											   plusDataObject.Name + "MultiSelector.SelectedValues)";
					filterMultiSelectorsInitializeContent =
						plusDataObject.Name + "MultiSelector = new MultiValueSelector<string>(SourceCollection.Select(x => x." +
						plusDataObject.Name + ".ToString()).Distinct(), RefreshCollectionView, AllValue);";
					filterXamlContent = DoReplaces(FilterComboBoxXamlTemplate);
				}
			}

			if (plusDataObject.Type == "bool")
			{
			    filterType = "bool?";

				filterMembersContent = "private " + filterType + " _" + ToPascalCase(plusDataObject.Name) + ";";
				filterPropertiesContent = FilterPropertyTemplate;
				filterPredicateResetContent = plusDataObject.Name + " = null;";
				filterPredicatesContent =
					".IsEqual(x => x." + plusDataObject.Name + ", x => x." + plusDataObject.Name + ")";
				filterXamlContent = DoReplaces(FilterCheckBoxXamlTemplate);
			}

			if (plusDataObject.Type == "DateTime")
			{
			    filterType = "DateTime?";

                filterMembersContent = "private " + filterType + " _" + ToPascalCase(plusDataObject.Name) + "From;\r\n";
			    filterMembersContent += "private " + filterType + " _" + ToPascalCase(plusDataObject.Name) + "To;";
                filterPropertiesContent = FilterPropertyTemplate.Replace("$Name$", plusDataObject.Name + "From").Replace("$name$", ToPascalCase(plusDataObject.Name) + "From");
			    filterPropertiesContent += "\r\n" + FilterPropertyTemplate.Replace("$Name$", plusDataObject.Name + "To").Replace("$name$", ToPascalCase(plusDataObject.Name) + "To");
                filterPredicateResetContent = plusDataObject.Name + "From = null;\r\n";
			    filterPredicateResetContent += plusDataObject.Name + "To = null;";
                filterPredicatesContent = ".IsInRange(x => x." + plusDataObject.Name + ", x => x." + plusDataObject.Name + "From, x => x." + plusDataObject.Name + "To)";
				filterXamlContent = DoReplaces2(FilterDateTimePickerXamlTemplate + "\r\n", plusDataObject.Name + "From");
			    filterXamlContent += DoReplaces2(FilterDateTimePickerXamlTemplate, plusDataObject.Name + "To");
			}

			filterMembersContent = filterMembersContent.Replace("$Name$", plusDataObject.Name)
				                       .Replace("$name$", ToPascalCase(plusDataObject.Name))
				                       .Replace("$Type$", filterType) + "\r\n";
			filterPropertiesContent = filterPropertiesContent.Replace("$Name$", plusDataObject.Name)
				.Replace("$name$", ToPascalCase(plusDataObject.Name)).Replace("$Type$", filterType) + "\r\n";
		    if (!string.IsNullOrEmpty(filterPredicateResetContent))
		    {
		        filterPredicateResetContent = filterPredicateResetContent.Replace("$Name$", plusDataObject.Name)
		                                          .Replace("$name$", ToPascalCase(plusDataObject.Name))
		                                          .Replace("$Type$", filterType) + "\r\n";
		    }

		    filterPredicatesContent = filterPredicatesContent.Replace("$Name$", plusDataObject.Name)
				.Replace("$name$", ToPascalCase(plusDataObject.Name)).Replace("$Type$", filterType) + "\r\n";
			filterXamlContent = filterXamlContent.Replace("$Name$", plusDataObject.Name)
				.Replace("$name$", ToPascalCase(plusDataObject.Name)).Replace("$Type$", filterType);
            if (!string.IsNullOrEmpty(filterMultiSelectorsInitializeContent))
            {
                filterMultiSelectorsInitializeContent = filterMultiSelectorsInitializeContent.Replace("$Name$", plusDataObject.Name)
				.Replace("$name$", ToPascalCase(plusDataObject.Name)).Replace("$Type$", filterType) + "\r\n";
            }
		}

	    public static string GetBusinessServiceLocalGetMock(ConfigurationItem dataItem)
	    {
	        string propertyAssignments = string.Empty;
	        Random random = new Random();
	        foreach (ConfigurationProperty property in dataItem.Properties)
	        {
	            if (property.Type == "string")
	            {
	                propertyAssignments += property.Name + " = \"" + property.Name + "\",\r\n";
	            }
	            if (property.Type == "int")
	            {
	                propertyAssignments += property.Name + " = " + random.Next(1, 9) + ",\r\n";
	            }
	            if (property.Type == "bool")
	            {
	                int ran = random.Next(1, 9);
	                bool val = ran % 2 == 0;
	                propertyAssignments += property.Name + " = " + val + ",\r\n";
	            }
	            if (property.Type == "DateTime")
	            {
	                propertyAssignments += property.Name + " = new DateTime(2016, 1, 1),\r\n";
	            }
	        }

	        return propertyAssignments;
	    }
	}
}

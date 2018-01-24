using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using PlusLayerCreator.Items;

namespace PlusLayerCreator
{
	public static class Helpers
	{
		public static string Product;
		public static string DialogName;

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

		public static string DoReplaces(string input, string item = "")
		{
			input = input.Replace("$Product$", Product);
			input = input.Replace("$product$", ToPascalCase(Product));
			input = input.Replace("$Item$", item);
			input = input.Replace("$item$", ToPascalCase(item));
			input = input.Replace("$Dialog$", DialogName);
			input = input.Replace("$dialog$", ToPascalCase(DialogName));

			return input;
		}

		public static string DoReplaces2(string input, string name = "", string item = "", string type = "")
		{
			input = input.Replace("$Product$", Product);
			input = input.Replace("$product$", ToPascalCase(Product));
			input = input.Replace("$Item$", item);
			input = input.Replace("$item$", ToPascalCase(item));
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

		public static void CreateFile(string input, string output, string[] contents = null, string item = "")
		{
			string fileContent = File.ReadAllText(input);

			fileContent = DoReplaces(fileContent, item);
			fileContent = ReplaceSpecialContent(fileContent, contents);

			FileInfo fileInfo = new FileInfo(output);
			if (fileInfo.Directory != null)
			{
				fileInfo.Directory.Create();
			}
			File.WriteAllText(fileInfo.FullName, fileContent);
		}

		public static void CreateFilterContents(PlusDataItemProperty plusDataObject, out string filterMembersContent, out string filterPropertiesContent, out string filterPredicateResetContent,
			out string filterPredicatesContent, out string filterXamlContent, out string filterMultiSelectorsInitializeContent)
		{
			filterMembersContent = string.Empty;
			filterPropertiesContent = string.Empty;
			filterPredicateResetContent = string.Empty;
			filterPredicatesContent = string.Empty;
			filterXamlContent = string.Empty;
			filterMultiSelectorsInitializeContent = string.Empty;

			string filterSuffix = plusDataObject.Type == "string" ? string.Empty : ".ToString()";
			if (plusDataObject.Type == "string" || plusDataObject.Type == "int")
			{
				if (plusDataObject.FilterPropertyType == "TextBox")
				{
					filterMembersContent = "private string _" + ToPascalCase(plusDataObject.Name) + ";\r\n";
					filterPropertiesContent = FilterPropertyTemplate.Replace(plusDataObject.Type, "string");
					filterPredicateResetContent = plusDataObject.Name + " = string.Empty;\r\n";
					filterPredicatesContent = ".StartsWith(x => x." + plusDataObject.Name + filterSuffix + ", x => x." +
											   plusDataObject.Name + ")\r\n";
					filterXamlContent = DoReplaces(FilterTextBoxXamlTemplate);
				}

				if (plusDataObject.FilterPropertyType == "ComboBox")
				{
					filterMembersContent = "private MultiValueSelector<string> _" +
											ToPascalCase(plusDataObject.Name) + "MultiSelector;\r\n";
					filterPropertiesContent = FilterComboBoxPropertyTemplate;
					filterPredicatesContent = ".IsContainedInList(x => x." + plusDataObject.Name + filterSuffix + ", x => x." +
											   plusDataObject.Name + "MultiSelector.SelectedValues)\r\n";
					filterMultiSelectorsInitializeContent =
						plusDataObject.Name + "MultiSelector = new MultiValueSelector<string>(SourceCollection.Select(x => x." +
						plusDataObject.Name + ".ToString()).Distinct(), RefreshCollectionView, AllValue);";
					filterXamlContent = DoReplaces(FilterComboBoxXamlTemplate);
				}
			}

			if (plusDataObject.Type == "bool")
			{
				filterMembersContent = "private " + plusDataObject.Type + "? _" + ToPascalCase(plusDataObject.Name) +
										";\r\n";
				filterPropertiesContent = FilterPropertyTemplate;
				filterPredicateResetContent = plusDataObject.Name + " = null;\r\n";
				filterPredicatesContent =
					".IsEqual(x => x." + plusDataObject.Name + ", x => x." + plusDataObject.Name + ")\r\n";
				filterXamlContent = DoReplaces(FilterCheckBoxXamlTemplate);
			}

			if (plusDataObject.Type == "DateTime")
			{
				filterMembersContent = "private " + plusDataObject.Type + "? _" + ToPascalCase(plusDataObject.Name) +
										";\r\n";
				filterPropertiesContent = FilterPropertyTemplate;
				filterPredicateResetContent = plusDataObject.Name + " = null;\r\n";
				filterPredicatesContent =
					".IsEqual(x => x." + plusDataObject.Name + ", x => x." + plusDataObject.Name + ")\r\n";
				filterXamlContent = DoReplaces(FilterDateTimePickerXamlTemplate);
			}

			filterMembersContent = filterMembersContent.Replace("$Name$", plusDataObject.Name)
				                       .Replace("$name$", ToPascalCase(plusDataObject.Name))
				                       .Replace("$Type$", plusDataObject.Type) + "\r\n";
			filterPropertiesContent = filterPropertiesContent.Replace("$Name$", plusDataObject.Name)
				.Replace("$name$", ToPascalCase(plusDataObject.Name)).Replace("$Type$", plusDataObject.Type) + "\r\n";
			filterPredicateResetContent = filterPredicateResetContent.Replace("$Name$", plusDataObject.Name)
				.Replace("$name$", ToPascalCase(plusDataObject.Name)).Replace("$Type$", plusDataObject.Type) + "\r\n";
			filterPredicatesContent = filterPredicatesContent.Replace("$Name$", plusDataObject.Name)
				.Replace("$name$", ToPascalCase(plusDataObject.Name)).Replace("$Type$", plusDataObject.Type) + "\r\n";
			filterXamlContent = filterXamlContent.Replace("$Name$", plusDataObject.Name)
				.Replace("$name$", ToPascalCase(plusDataObject.Name)).Replace("$Type$", plusDataObject.Type);
			filterMultiSelectorsInitializeContent = filterMultiSelectorsInitializeContent.Replace("$Name$", plusDataObject.Name)
				.Replace("$name$", ToPascalCase(plusDataObject.Name)).Replace("$Type$", plusDataObject.Type) + "\r\n";
		}
	}
}

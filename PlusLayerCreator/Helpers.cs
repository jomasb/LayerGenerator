using System;
using System.IO;
using System.Linq;
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
            var ser = new DataContractJsonSerializer(typeof(T));
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            var obj = (T) ser.ReadObject(stream);
            return obj;
        }

	    public static string ToCamelCase(this string input)
	    {
		    if (input == null || input.Length < 2)
			    return input;

		    string[] words = input.Split(new string[] {"-", "_", string.Empty}, StringSplitOptions.RemoveEmptyEntries);

		    string result = string.Empty;
		    for (int i = 0; i < words.Length; i++)
		    {
				result += words[i].Substring(0, 1).ToUpper() + words[i].Substring(1).ToLower();
		    }

		    return result;
	    }

		public static string DoReplacesClient(this string input, ConfigurationItem item = null, ConfigurationItem child = null, string property = "", string type = "", string name = "")
        {
            input = input.Replace("$PRODUCT$", Configuration.Product.ToUpper());
            input = input.Replace("$Product$", Configuration.Product);
            input = input.Replace("$product$", ToPascalCase(Configuration.Product));
            input = input.Replace("$Item$", item == null ? string.Empty : item.Name);
            input = input.Replace("$item$", item == null ? string.Empty : ToPascalCase(item.Name));
            input = input.Replace("$Version$", item == null ? string.Empty : item.Name + "Version");
            input = input.Replace("$version$", item == null ? string.Empty : ToPascalCase(item.Name + "Version"));
            input = input.Replace("$Parent$", item == null ? string.Empty : item.Parent);
            input = input.Replace("$parent$", item == null ? string.Empty : ToPascalCase(item.Parent));
            if (child != null)
            {
                input = input.Replace("$Child$", item == null ? string.Empty : child.Name);
                input = input.Replace("$child$", item == null ? string.Empty : ToPascalCase(child.Name));
            }
            input = input.Replace("$Dialog$", Configuration.DialogName);
            input = input.Replace("$dialog$", ToPascalCase(Configuration.DialogName));
            input = input.Replace("$Handle$", Configuration.ControllerHandle);
	        input = input.Replace("$Property$", property);
	        input = input.Replace("$property$", ToPascalCase(property));
	        input = input.Replace("$Type$", type);
	        input = input.Replace("$type$", ToPascalCase(type));
	        input = input.Replace("$Name$", name);
	        input = input.Replace("$name$", ToPascalCase(name));

			return input;
        }

        public static string DoReplacesServer(this string input, ConfigurationItem item)
        {
            input = input.Replace("%%TRANSCODE-LESEN%%", item.TransactionCodeRead);
            input = input.Replace("%%TRANSCODE-SCHREIBEN%%", item.TransactionCodeWrite);
            input = input.Replace("%%ANF%%", item.Requirement);
            input = input.Replace("%%AUTOR%%", "LayerCreator");
            input = input.Replace("%%DATUM%%", DateTime.Now.ToShortDateString());
            input = input.Replace("%%PRODUKT%%", Configuration.Product.ToUpper());
            input = input.Replace("%%SERVER%%", item.Server);
            input = input.Replace("%%SYS%%", item.Sys);
	        input = input.Replace("%%TBL%%", item.Table);
			input = input.Replace("%%Tabellen-Bezeichnung%%", item.TableDescription);
            input = input.Replace("%%ANZ-KOMPLETT%%", CalculateServerRows(item, false).ToString());
            input = input.Replace("%%ANZ-PF%%", CalculateServerRows(item, true).ToString());
	        input = input.Replace("%%FELDER%%", ArrangeProperties(item));
	        input = input.Replace("%%KEY-FELDER%%", ArrangeKeyProperties(item));
	        input = input.Replace("%%DELETE-FELDER-WERTE%%", ArrangeDelete(item));
	        input = input.Replace("%%CURSOR-FELDER%%", ArrangeCursorFields(item));
	        input = input.Replace("%%DATEN-AUS-REQUEST%%", ArrangeDataFromRequest(item));
	        input = input.Replace("%%DATEN-IN-REPLY%%", ArrangeDataForReply(item));
	        input = input.Replace("%%INSERT-FELDER%%", ArrangeInsertFields(item));
	        input = input.Replace("%%INSERT-WERTE%%", ArrangeInsertValues(item));
	        input = input.Replace("%%SCHLUESSELFELDER-PARAMETER%%", ArrangeKeys(item));
	        input = input.Replace("%%FETCH-CURSOR%%", ArrangeFetchCursor(item));
	        input = input.Replace("%%UPDATE-FELDER-WERTE%%", ArrangeUpdate(item));
	        //input = input.Replace("%%CURSOR-FELDER-WHERE%%", ArrangeCursorFieldsAdvanced(item, 0));
	        //input = input.Replace("%%CURSOR-FELDER-ROW%%", ArrangeCursorFieldsAdvanced(item, 1));
	        //input = input.Replace("%%CURSOR-FELDER-ORDER%%", ArrangeCursorFieldsAdvanced(item, 2));

			return input;
        }

		#region Client methods

	    public static string GetLocaliatzionExtension(this string input)
	    {
		    if (input.EndsWith("e"))
			    return "n";
		    if (input.EndsWith("er"))
			    return "";
		    return "en";
	    }

	    public static string ToPascalCase(this string input)
	    {
		    if (string.IsNullOrEmpty(input))
			    return input;

		    return input[0].ToString().ToLower() + input.Substring(1, input.Length - 1);
	    }


	    public static int GetMaxValue(string length)
	    {
		    if (length == string.Empty)
			    return 999999;

		    int value;
		    var computedString = string.Empty;
		    if (int.TryParse(length, out value))
			    for (var i = 0; i < value; i++)
				    computedString += "9";
		    return int.Parse(computedString);
	    }

		public static string ReplaceSpecialContent(this string fileContent, string[] contents = null)
        {
            if (contents != null)
                for (var i = 0; i < contents.Length; i++)
                    fileContent = fileContent.Replace("$specialContent" + (i + 1) + "$", contents[i]);

            return fileContent;
        }

        public static void CreateFileFromString(string input, string output, string[] contents = null,
            ConfigurationItem item = null)
        {
            var fileContent = input;

            fileContent = DoReplacesClient(fileContent, item);
            fileContent = ReplaceSpecialContent(fileContent, contents);

            var fileInfo = new FileInfo(output);
            if (fileInfo.Directory != null) fileInfo.Directory.Create();
            File.WriteAllText(fileInfo.FullName, fileContent);
        }

        public static void CreateFileFromPath(string inputPath, string output, string[] contents = null,
            ConfigurationItem item = null)
        {
            var fileContent = File.ReadAllText(inputPath);

            CreateFileFromString(fileContent, output, contents, item);
        }

        public static void CreateFilterContents(ConfigurationItem plusDataItem, ConfigurationProperty plusDataObject,
            out string filterMembersContent, out string filterPropertiesContent, out string filterPredicateResetContent,
            out string filterPredicatesContent, out string filterXamlContent,
            out string filterMultiSelectorsInitializeContent)
        {
            filterMembersContent = string.Empty;
            filterPropertiesContent = string.Empty;
            filterPredicateResetContent = string.Empty;
            filterPredicatesContent = string.Empty;
            filterXamlContent = string.Empty;
            filterMultiSelectorsInitializeContent = string.Empty;
            var filterType = string.Empty;


            var filterSuffix = plusDataObject.Type == "string" ? string.Empty : ".ToString()";
            if (plusDataObject.Type == "string" || plusDataObject.Type == "int")
            {
                filterType = "string";

                if (plusDataObject.FilterPropertyType == "TextBox")
                {
                    filterMembersContent = "private string _" + ToPascalCase(plusDataObject.Name) + ";";
                    filterPropertiesContent = FilterPropertyTemplate;
                    filterPredicateResetContent = plusDataObject.Name + " = string.Empty;";
                    filterPredicatesContent = ".Match(x => x." + plusDataObject.Name + filterSuffix + ", x => x." +
                                              plusDataObject.Name + ")";
                    filterXamlContent = DoReplacesClient(FilterTextBoxXamlTemplate, plusDataItem);
                }

                if (plusDataObject.FilterPropertyType == "ComboBox")
                {
                    filterMembersContent = "private MultiValueSelector<string> _" +
                                           ToPascalCase(plusDataObject.Name) + "MultiSelector;";
                    filterPropertiesContent = FilterComboBoxPropertyTemplate;
                    filterPredicatesContent = ".IsContainedInList(x => x." + plusDataObject.Name + filterSuffix +
                                              ", x => x." +
                                              plusDataObject.Name + "MultiSelector.SelectedValues)";
                    filterMultiSelectorsInitializeContent =
                        plusDataObject.Name +
                        "MultiSelector = new MultiValueSelector<string>(SourceCollection.Select(x => x." +
                        plusDataObject.Name + ".ToString()).Distinct(), RefreshCollectionView, AllValue);";
                    filterXamlContent = DoReplacesClient(FilterComboBoxXamlTemplate, plusDataItem);
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
                filterXamlContent = DoReplacesClient(FilterCheckBoxXamlTemplate, plusDataItem);
            }

            if (plusDataObject.Type == "DateTime")
            {
                filterType = "DateTime?";

                filterMembersContent = "private " + filterType + " _" + ToPascalCase(plusDataObject.Name) + "From;\r\n";
                filterMembersContent += "private " + filterType + " _" + ToPascalCase(plusDataObject.Name) + "To;";
                filterPropertiesContent = FilterPropertyTemplate.Replace("$Name$", plusDataObject.Name + "From")
                    .Replace("$name$", ToPascalCase(plusDataObject.Name) + "From");
                filterPropertiesContent += "\r\n" + FilterPropertyTemplate.Replace("$Name$", plusDataObject.Name + "To")
                                               .Replace("$name$", ToPascalCase(plusDataObject.Name) + "To");
                filterPredicateResetContent = plusDataObject.Name + "From = null;\r\n";
                filterPredicateResetContent += plusDataObject.Name + "To = null;";
                filterPredicatesContent = ".IsInRange(x => x." + plusDataObject.Name + ", x => x." +
                                          plusDataObject.Name + "From, x => x." + plusDataObject.Name + "To)";
                filterXamlContent =
                    DoReplacesClient(FilterDateTimePickerXamlTemplate + "\r\n", null, null, plusDataObject.Name + "From");
                filterXamlContent += DoReplacesClient(FilterDateTimePickerXamlTemplate, null, null, plusDataObject.Name + "To");
            }

            filterMembersContent = filterMembersContent.Replace("$Name$", plusDataObject.Name)
                                       .Replace("$name$", ToPascalCase(plusDataObject.Name))
                                       .Replace("$Type$", filterType) + "\r\n";
            filterPropertiesContent = filterPropertiesContent.Replace("$Name$", plusDataObject.Name)
                                          .Replace("$name$", ToPascalCase(plusDataObject.Name))
                                          .Replace("$Type$", filterType) + "\r\n";
            if (!string.IsNullOrEmpty(filterPredicateResetContent))
                filterPredicateResetContent = filterPredicateResetContent.Replace("$Name$", plusDataObject.Name)
                                                  .Replace("$name$", ToPascalCase(plusDataObject.Name))
                                                  .Replace("$Type$", filterType) + "\r\n";

            filterPredicatesContent = filterPredicatesContent.Replace("$Name$", plusDataObject.Name)
                                          .Replace("$name$", ToPascalCase(plusDataObject.Name))
                                          .Replace("$Type$", filterType) + "\r\n";
            filterXamlContent = filterXamlContent.Replace("$Name$", plusDataObject.Name)
                .Replace("$name$", ToPascalCase(plusDataObject.Name)).Replace("$Type$", filterType);
            if (!string.IsNullOrEmpty(filterMultiSelectorsInitializeContent))
                filterMultiSelectorsInitializeContent =
                    filterMultiSelectorsInitializeContent.Replace("$Name$", plusDataObject.Name)
                        .Replace("$name$", ToPascalCase(plusDataObject.Name)).Replace("$Type$", filterType) + "\r\n";
        }

        public static string GetBusinessServiceLocalGetMock(ConfigurationItem dataItem)
        {
            var propertyAssignments = string.Empty;
            var random = new Random();
            foreach (var property in dataItem.Properties)
            {
                if (property.Type == "string")
                    propertyAssignments += property.Name + " = \"" + property.Name + "\",\r\n";
                if (property.Type == "int") propertyAssignments += property.Name + " = " + random.Next(1, 9) + ",\r\n";
                if (property.Type == "bool")
                {
                    var ran = random.Next(1, 9);
                    var val = ran % 2 == 0;
                    propertyAssignments += property.Name + " = " + val.ToString().ToLower() + ",\r\n";
                }

                if (property.Type == "DateTime")
                    propertyAssignments += property.Name + " = new DateTime(2016, 1, 1),\r\n";
            }

            return propertyAssignments;
        }

        public static string GetLocalizedString(this string input, bool multi = false)
        {
            var extension = multi ? "s" : string.Empty;
            return "{localization:Localize Key=" + Configuration.Product + Configuration.DialogName + "_lbl" + input +
                   extension + ", Source=" + Configuration.Product +
                   "Localizer}";
        }

        public static ConfigurationItem GetParent(ConfigurationItem item)
        {
            if (string.IsNullOrEmpty(item.Parent))
            {
                return null;
            }

            return Configuration.DataLayout.First(t => t.Name == item.Parent);
        }

	    public static string GetParentParameter(ConfigurationItem item, string type)
        {
            string retValue = string.Empty;

            if (!string.IsNullOrEmpty(item.Parent))
            {
                if (type == "param")
                {
                    retValue = ", " + Configuration.Product + item.Parent + " parent" + item.Parent;
                }
                else if (type == "call")
                {
                    retValue = ", parent" + item.Parent;
                }
                else
                {
                    retValue = "arguments.Add(\"" + item.Parent + "\", parent" + item.Parent + ");\r\n";
                }
                return GetParentParameter(Configuration.DataLayout.First(t => t.Name == item.Parent), type) + retValue;
            }

            return retValue;
        }

        public static string GetPreFilterInformation(ConfigurationItem item, string information)
        {
            if (item.IsPreFilterItem)
            {
                return string.Empty;
            }

            var filterParameter = string.Empty;
            foreach (var configurationItem in Configuration.DataLayout.Where(t => t.IsPreFilterItem))
            {
                if (information == "parameter")
                {
                    filterParameter += ", " + Configuration.Product + configurationItem.Name + " " +
                                       configurationItem.Name.ToPascalCase();
                }

                if (information == "arguments")
                {
                    filterParameter += "arguments.Add(\"" + Configuration.Product + configurationItem.Name + "\", " +
                                       configurationItem.Name.ToPascalCase() + ");";
                }

                if (information == "listCall")
                {
                    filterParameter += ", " + configurationItem.Name.ToPascalCase();
                }
            }

            return filterParameter;
        }

		#endregion Client methods

		#region Server methods

	    public static string ToSqlName(this string name)
	    {
		    return name.Replace("-", "_");
	    }

	    public static string ToVariableName(this string name)
	    {
		    return name.Replace("_", "-");
	    }

	    public static int GetYearToFractionDigits(ConfigurationProperty property)
	    {
		    int length;

		    if (property == null || !int.TryParse(property.Length, out length))
		    {
			    return - 1;
		    }

			return length % 20;
		}

		public static int CalculateServerRows(ConfigurationItem item, bool modify)
	    {
		    int retValue = 0;

		    foreach (ConfigurationProperty property in item.Properties)
		    {
			    if (string.IsNullOrEmpty(property.Length))
			    {
				    retValue += 1;
			    }
			    else
			    {
				    retValue += int.Parse(property.Length);
			    }
		    }

		    if (modify)
		    {
			    retValue += 1;
		    }

		    retValue = (30000 - 4) / retValue;

		    return retValue;
	    }

	    public static string ArrangeProperties(ConfigurationItem item)
	    {
		    string retValue = string.Empty;
		    int level = 2; //temporary

		    foreach (ConfigurationProperty property in item.Properties)
		    {
			    retValue += "  " + level.ToString("00") + " " + property.MessageField.ToVariableName().PadRight(30) + "  TYPE " + property.MessageDataType + ".\r\n";
		    }

			return retValue.Substring(0, retValue.Length - 2);
	    }

	    public static string ArrangeKeyProperties(ConfigurationItem item)
	    {
		    string retValue = string.Empty;

		    for (int i = 1; i <= item.Properties.Count(t => t.IsKey) && i <= 7; i++)
		    {
			    retValue += "##" + i + "/";
		    }

			return retValue.Substring(0, retValue.Length - 1);
	    }

	    public static string ArrangeDelete(ConfigurationItem item)
	    {
		    string retValue = string.Empty;
		    int i = 0;

		    foreach (ConfigurationProperty property in item.Properties.Where(t => t.IsKey))
		    {
			    retValue += i == 0 ? "                   " : "         AND       ";
				retValue += property.MessageField.ToSqlName() + "\r\n                = :" + property.MessageField.ToVariableName().PadRight(30) + "    OF " + item.Table + "-ROW\r\n";
			    i++;
		    }

		    return retValue.Substring(0, retValue.Length - 2);
	    }

	    public static string ArrangeCursorFields(ConfigurationItem item)
	    {
		    string retValue = string.Empty;

		    for (int i = 0; i < item.Properties.Count; i++)
		    {
			    retValue += i == 0 ? "                   " : "                 , ";
				retValue += item.Properties[i].MessageField.ToSqlName() + "\r\n";
		    }

			return retValue.Substring(0, retValue.Length - 2);
		}

	 //   private static string ArrangeCursorFieldsAdvanced(ConfigurationItem item, int mode)
	 //   {
		//    string retValue = string.Empty;

		//    for (int i = 0; i < item.Properties.Count; i++)
		//    {
		//		if (!item.Properties[i].IsKey)
		//			continue;

		//	    switch (mode)
		//	    {
		//			case 0:
		//				retValue += i == 0 ? "                   " : "                 , ";
		//				retValue += item.Properties[i].MessageField + "\r\n";
		//				break;
		//			case 1:
		//				retValue += i == 0 ? "                  :" : "                 ,:";
		//				retValue += item.Properties[i].MessageField.PadRight(30) + "   OF " + item.Table + "-ROW" + "\r\n";
		//				break;
		//			case 2:
		//				retValue += i == 0 ? "                   " : "                 , ";
		//				retValue += item.Properties[i].MessageField.PadRight(30) + "ASC" +"\r\n";
		//				break;
					    
		//	    }
			   
		//	}
			
		//    return retValue.Substring(0, retValue.Length - 2);
		//}

	    public static string ArrangeDataFromRequest(ConfigurationItem item)
	    {
		    string retValue = string.Empty;

		    foreach (ConfigurationProperty property in item.Properties)
		    {
			    retValue += "    MOVE " + property.MessageField.ToVariableName().PadRight(30) + " OF " + item.Server + "-REQ-2(I)" + "\r\n";
			    retValue += "      TO " + property.MessageField.ToVariableName().PadRight(30) + " OF " + item.Table + "-ROW" + "\r\n";
		    }

			return retValue.Substring(0, retValue.Length - 2);
	    }

	    public static string ArrangeDataForReply(ConfigurationItem item)
	    {
		    string retValue = string.Empty;

		    foreach (ConfigurationProperty property in item.Properties)
		    {
			    retValue += "    MOVE " + property.MessageField.ToVariableName().PadRight(30) + " OF " + item.Table + "-ROW" + "\r\n";
			    retValue += "      TO " + property.MessageField.ToVariableName().PadRight(30) + " OF " + item.Server + "-RPL-1(WS-MR-ANZ-DS-AKTUELL)" + "\r\n";
			}

			return retValue.Substring(0, retValue.Length - 2);
	    }

	    public static string ArrangeInsertFields(ConfigurationItem item)
	    {
		    string retValue = string.Empty;

		    for (int i = 0; i < item.Properties.Count; i++)
		    {
			    retValue += i == 0 ? "                   " : "                 , ";
			    retValue += item.Properties[i].MessageField.ToSqlName() + "\r\n";
			}

			return retValue.Substring(0, retValue.Length - 2);
	    }

	    public static string ArrangeInsertValues(ConfigurationItem item)
	    {
		    string retValue = string.Empty;

		    for (int i = 0; i < item.Properties.Count; i++)
		    {
			    retValue += i == 0 ? "                  :" : "                 ,:";
				if (item.Properties[i].MessageField.ToVariableName() == "LUPD-TIMESTAMP")
				{
					retValue += "HV-LUPD-TIMESTAMP                 OF HV-" + item.Server + "\r\n";
				    retValue += "                   TYPE AS DATETIME YEAR TO FRACTION(" + GetYearToFractionDigits(item.Properties[i]) + ")" + "\r\n";
			    }
				else
			    {
				    retValue += item.Properties[i].MessageField.ToVariableName().PadRight(30) + "    OF " + item.Table + "-ROW" + "\r\n";
			    }
		    }

			return retValue.Substring(0, retValue.Length - 2);
	    }

	    public static string ArrangeKeys(ConfigurationItem item)
	    {
			string retValue = string.Empty;
		    int i = 1;

		    foreach (ConfigurationProperty property in item.Properties.Where(t => t.IsKey).Take(7))
		    {
			    retValue += "       MOVE " + property.MessageField.ToVariableName().PadRight(30) + "    OF " + item.Server + "-REQ-2(I)" + "\r\n";
			    retValue += "         TO PARAMETERTEXT              OF EMS-MELD-LNK (" + i++ + ")" + "\r\n";
		    }


		    return retValue.Substring(0, retValue.Length - 2);
		}

		public static string ArrangeFetchCursor(ConfigurationItem item)
	    {
		    string retValue = string.Empty;

		    for (int i = 0; i < item.Properties.Count; i++)
		    {
			    retValue += i == 0 ? "                  :" : "                 ,:";
			    if (item.Properties[i].MessageField.ToVariableName() == "LUPD-TIMESTAMP")
			    {
				    retValue += "LUPD-TIMESTAMP".PadRight(30) + "   OF " + item.Table + "-ROW" + "\r\n";
				    retValue += "                      TYPE AS DATETIME YEAR TO FRACTION(" + GetYearToFractionDigits(item.Properties[i]) + ")" + "\r\n";
				}
				else
			    {
				    retValue += item.Properties[i].MessageField.ToVariableName().PadRight(30) + "   OF " + item.Table + "-ROW" + "\r\n";
				}
		    }
		
		    return retValue.Substring(0, retValue.Length - 2);
	    }

		public static string ArrangeUpdate(ConfigurationItem item)
	    {
		    string retValue = string.Empty;
		    int i = 0;

		    foreach (var property in item.Properties.Where(t => !t.IsKey))
		    {
			    retValue += i == 0 ? "                   " : "               ,  :";
				retValue += property.MessageField.ToSqlName() + "\r\n";
			    retValue += "                = :";
			    if (property.MessageField.ToVariableName() == "LUPD-TIMESTAMP")
			    {
				    retValue += "HV-LUPD-TIMESTAMP                OF HV-" + item.Server + "" + "\r\n";
				    retValue += "                   TYPE AS DATETIME YEAR TO FRACTION(" + GetYearToFractionDigits(property) + ")" + "\r\n";
				}
				else
			    {
				    retValue += property.MessageField.ToVariableName().PadRight(30) + "   OF " + item.Table + "-ROW" + "\r\n";
				}
			    
			    i++;
		    }

		    i = 0;
		    foreach (var property in item.Properties.Where(t => t.IsKey))
		    {
				retValue += i == 0 ? "         WHERE     " : "         AND       ";
				retValue += property.MessageField.ToSqlName() + "\r\n";
			    retValue += "                = :";
			    retValue += property.MessageField.ToVariableName().PadRight(30) + "   OF " + item.Table + "-ROW" + "\r\n";
			    i++;
			}

		    return retValue;
	    }

	    #endregion Server methods
	}
}
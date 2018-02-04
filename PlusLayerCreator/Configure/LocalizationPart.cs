using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlusLayerCreator.Items;

namespace PlusLayerCreator.Configure
{
	public class LocalizationPart
	{
		private Configuration _configuration;

		public LocalizationPart(Configuration configuration)
		{
			_configuration = configuration;
		}

		public void CreateLocalization()
		{
			string lblEn = string.Empty;
			string lblEnOut = string.Empty;
			string lblDe = string.Empty;
			string lblDeOut = string.Empty;
			string strEn = string.Empty;
			string strEnOut = string.Empty;
			string strDe = string.Empty;
			string strDeOut = string.Empty;
			string btnEn = string.Empty;
			string btnEnOut = string.Empty;
			string btnDe = string.Empty;
			string btnDeOut = string.Empty;

			lblEn += _configuration.Product + _configuration.DialogName + "_lblCaption=" + _configuration.Product + " - " + _configuration.DialogTranslationEnglish + "\r\n";
			lblEnOut += _configuration.Product + _configuration.DialogName + "_lblCaption=@@" + _configuration.Product + " - " + _configuration.DialogTranslationEnglish + "\r\n";
			lblDe += _configuration.Product + _configuration.DialogName + "_lblCaption=" + _configuration.Product + " - " + _configuration.DialogTranslationGerman + "\r\n";
			lblDeOut += _configuration.Product + _configuration.DialogName + "_lblCaption=@@" + _configuration.Product + " - " + _configuration.DialogTranslationGerman + "\r\n";

			foreach (PlusDataItem dataItem in _configuration.DataLayout)
			{
				lblEn += _configuration.Product + _configuration.DialogName + "_lbl" + dataItem.Name + "s=" + dataItem.Name + "s\r\n";
				lblEnOut += _configuration.Product + _configuration.DialogName + "_lbl" + dataItem.Name + "s=@@" + dataItem.Name + "s\r\n";
				lblDe += _configuration.Product + _configuration.DialogName + "_lbl" + dataItem.Name + "s=" + dataItem.Translation + Helpers.GetLocaliatzionExtension(dataItem.Translation) + "\r\n";
				lblDeOut += _configuration.Product + _configuration.DialogName + "_lbl" + dataItem.Name + "s=@@" + dataItem.Translation + Helpers.GetLocaliatzionExtension(dataItem.Translation) + "\r\n";

			    if (dataItem.CanEdit || dataItem.CanEditMultiple)
			    {
			        btnEn += _configuration.Product + _configuration.DialogName + "_btnAdd" + dataItem.Name + "=New " + dataItem.Name + "\r\n";
			        btnEnOut += _configuration.Product + _configuration.DialogName + "_btnAdd" + dataItem.Name + "=@@New " + dataItem.Name + "\r\n";
			        btnDe += _configuration.Product + _configuration.DialogName + "_btnAdd" + dataItem.Name + "=Neues " + dataItem.Translation + "\r\n";
			        btnDeOut += _configuration.Product + _configuration.DialogName + "_btnAdd" + dataItem.Name + "=@@Neues " + dataItem.Translation + "\r\n";
                }

                foreach (PlusDataItemProperty plusDataObject in dataItem.Properties)
				{
				    lblEn += _configuration.Product + _configuration.DialogName + "_lbl" + dataItem.Name + plusDataObject.Name +
				                "=" + plusDataObject.Name + "\r\n";
				    lblEnOut += _configuration.Product + _configuration.DialogName + "_lbl" + dataItem.Name +
				                plusDataObject.Name + "=@@" + plusDataObject.Name + "\r\n";
				    lblDe += _configuration.Product + _configuration.DialogName + "_lbl" + dataItem.Name + plusDataObject.Name +
				                "=" + plusDataObject.Translation + "\r\n";
				    lblDeOut += _configuration.Product + _configuration.DialogName + "_lbl" + dataItem.Name +
				                plusDataObject.Name + "=@@" + plusDataObject.Translation + "\r\n";
				    
				    if (plusDataObject.Type == "DateTime")
                    {
				        lblEn += _configuration.Product + _configuration.DialogName + "_lbl" + dataItem.Name + plusDataObject.Name +
				                 "From=" + plusDataObject.Name + " from\r\n";
				        lblEnOut += _configuration.Product + _configuration.DialogName + "_lbl" + dataItem.Name +
				                    plusDataObject.Name + "From=@@" + plusDataObject.Name + " from\r\n";
				        lblDe += _configuration.Product + _configuration.DialogName + "_lbl" + dataItem.Name + plusDataObject.Name +
                                 "From=" + plusDataObject.Translation + " von\r\n";
				        lblDeOut += _configuration.Product + _configuration.DialogName + "_lbl" + dataItem.Name +
				                    plusDataObject.Name + "From=@@" + plusDataObject.Translation + " von\r\n";
				        lblEn += _configuration.Product + _configuration.DialogName + "_lbl" + dataItem.Name + plusDataObject.Name +
				                 "To=" + plusDataObject.Name + " to\r\n";
				        lblEnOut += _configuration.Product + _configuration.DialogName + "_lbl" + dataItem.Name +
				                    plusDataObject.Name + "To=@@" + plusDataObject.Name + " to\r\n";
				        lblDe += _configuration.Product + _configuration.DialogName + "_lbl" + dataItem.Name + plusDataObject.Name +
                                 "To=" + plusDataObject.Translation + " bis\r\n";
				        lblDeOut += _configuration.Product + _configuration.DialogName + "_lbl" + dataItem.Name +
				                    plusDataObject.Name + "To=@@" + plusDataObject.Translation + " bis\r\n";
                    }
				}
			}

			string[] languages = { "en", "fr", "de", "hu", "pt", "zh-CHS" };

			foreach (string language in languages)
			{
				string fileContent = File.ReadAllText(_configuration.InputPath + @"Localization\localization.txt");
				fileContent = Helpers.DoReplaces(fileContent, string.Empty);
				fileContent = fileContent.Replace("$language$", language);
				fileContent = fileContent.Replace("$date$", DateTime.Now.ToShortDateString());
				fileContent = fileContent.Replace("$_configuration.Product$", _configuration.Product);
				if (language == "en")
				{
					fileContent = fileContent.Replace("$KeysLabel$", lblEn);
					fileContent = fileContent.Replace("$KeysString$", strEn);
				    fileContent = fileContent.Replace("$ButtonsString$", btnEn);
				}
				else if (language == "de")
				{
					fileContent = fileContent.Replace("$KeysLabel$", lblDe);
					fileContent = fileContent.Replace("$KeysString$", strDe);
				    fileContent = fileContent.Replace("$ButtonsString$", btnDe);
                }
				else if (language == "fr" || language == "hu")
				{
					fileContent = fileContent.Replace("$KeysLabel$", lblDeOut);
					fileContent = fileContent.Replace("$KeysString$", strDeOut);
				    fileContent = fileContent.Replace("$ButtonsString$", btnDeOut);
                }
				else
				{
					fileContent = fileContent.Replace("$KeysLabel$", lblEnOut);
					fileContent = fileContent.Replace("$KeysString$", strEnOut);
				    fileContent = fileContent.Replace("$ButtonsString$", btnEnOut);
                }
				string languageExtension = language == "de" ? string.Empty : "." + language;
				FileInfo fileInfo = new FileInfo(_configuration.OutputPath + @"Localization\Dcx.Plus.Localization.Modules." + _configuration.Product + ".Nls" + languageExtension + ".txt");
				if (fileInfo.Directory != null)
				{
					fileInfo.Directory.Create();
				}
				File.WriteAllText(fileInfo.FullName, fileContent, Encoding.UTF8);
			}
		}
	}
}

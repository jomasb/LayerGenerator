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

				foreach (PlusDataItemProperty plusDataObject in dataItem.Properties)
				{
					lblEn += _configuration.Product + _configuration.DialogName + "_lbl" + dataItem.Name + plusDataObject.Name + "=" + plusDataObject.Name + "\r\n";
					lblEnOut += _configuration.Product + _configuration.DialogName + "_lbl" + dataItem.Name + plusDataObject.Name + "=@@" + plusDataObject.Name + "\r\n";
					lblDe += _configuration.Product + _configuration.DialogName + "_lbl" + dataItem.Name + plusDataObject.Name + "=" + plusDataObject.Translation + "\r\n";
					lblDeOut += _configuration.Product + _configuration.DialogName + "_lbl" + dataItem.Name + plusDataObject.Name + "=@@" + plusDataObject.Translation + "\r\n";
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
				}
				else if (language == "de")
				{
					fileContent = fileContent.Replace("$KeysLabel$", lblDe);
					fileContent = fileContent.Replace("$KeysString$", strDe);
				}
				else if (language == "fr" || language == "hu")
				{
					fileContent = fileContent.Replace("$KeysLabel$", lblDeOut);
					fileContent = fileContent.Replace("$KeysString$", strDeOut);
				}
				else
				{
					fileContent = fileContent.Replace("$KeysLabel$", lblEnOut);
					fileContent = fileContent.Replace("$KeysString$", strEnOut);
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

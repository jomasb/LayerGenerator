using System;
using System.IO;
using System.Linq;
using System.Text;
using PlusLayerCreator.Items;

namespace PlusLayerCreator.Configure
{
    public class LocalizationPart
    {
        private readonly Configuration _configuration;

        public LocalizationPart(Configuration configuration)
        {
            _configuration = configuration;
        }

        public void CreateLocalization()
        {
            var lblEn = string.Empty;
            var lblEnOut = string.Empty;
            var lblDe = string.Empty;
            var lblDeOut = string.Empty;
            var strEn = string.Empty;
            var strEnOut = string.Empty;
            var strDe = string.Empty;
            var strDeOut = string.Empty;
            var btnEn = string.Empty;
            var btnEnOut = string.Empty;
            var btnDe = string.Empty;
            var btnDeOut = string.Empty;

            CreateTandemLocalization();
            CreatePlusDialogLocalization();

            lblEn += _configuration.Product + _configuration.DialogName + "_lblCaption=" + _configuration.Product +
                     " - " + _configuration.DialogTranslationEnglish + "\r\n";
            lblEnOut += _configuration.Product + _configuration.DialogName + "_lblCaption=@@" + _configuration.Product +
                        " - " + _configuration.DialogTranslationEnglish + "\r\n";
            lblDe += _configuration.Product + _configuration.DialogName + "_lblCaption=" + _configuration.Product +
                     " - " + _configuration.DialogTranslationGerman + "\r\n";
            lblDeOut += _configuration.Product + _configuration.DialogName + "_lblCaption=@@" + _configuration.Product +
                        " - " + _configuration.DialogTranslationGerman + "\r\n";

            foreach (var dataItem in _configuration.DataLayout)
            {
                lblEn += _configuration.Product + _configuration.DialogName + "_lbl" + dataItem.Name + "=" +
                         dataItem.Name + "\r\n";
                lblEnOut += _configuration.Product + _configuration.DialogName + "_lbl" + dataItem.Name + "=@@" +
                            dataItem.Name + "\r\n";
                lblDe += _configuration.Product + _configuration.DialogName + "_lbl" + dataItem.Name + "=" +
                         dataItem.Translation + dataItem.Translation.GetLocaliatzionExtension() + "\r\n";
                lblDeOut += _configuration.Product + _configuration.DialogName + "_lbl" + dataItem.Name + "=@@" +
                            dataItem.Translation + dataItem.Translation.GetLocaliatzionExtension() + "\r\n";
                lblEn += _configuration.Product + _configuration.DialogName + "_lbl" + dataItem.Name + "s=" +
                         dataItem.Name + "s\r\n";
                lblEnOut += _configuration.Product + _configuration.DialogName + "_lbl" + dataItem.Name + "s=@@" +
                            dataItem.Name + "s\r\n";
                lblDe += _configuration.Product + _configuration.DialogName + "_lbl" + dataItem.Name + "s=" +
                         dataItem.Translation + dataItem.Translation.GetLocaliatzionExtension() + "\r\n";
                lblDeOut += _configuration.Product + _configuration.DialogName + "_lbl" + dataItem.Name + "s=@@" +
                            dataItem.Translation + dataItem.Translation.GetLocaliatzionExtension() + "\r\n";

                if ((dataItem.CanEdit || dataItem.CanEditMultiple) && (string.IsNullOrEmpty(dataItem.Parent) || _configuration.DataLayout.Any(x => x.Name == dataItem.Parent && x.IsPreFilterItem)))
                {
                    btnEn += _configuration.Product + _configuration.DialogName + "_btnAdd" + dataItem.Name + "=New " +
                             dataItem.Name + "\r\n";
                    btnEnOut += _configuration.Product + _configuration.DialogName + "_btnAdd" + dataItem.Name +
                                "=@@New " + dataItem.Name + "\r\n";
                    btnDe += _configuration.Product + _configuration.DialogName + "_btnAdd" + dataItem.Name +
                             "=Neues " + dataItem.Translation + "\r\n";
                    btnDeOut += _configuration.Product + _configuration.DialogName + "_btnAdd" + dataItem.Name +
                                "=@@Neues " + dataItem.Translation + "\r\n";
                }

                if (dataItem.Name.EndsWith("Version"))
                {
                    btnEn += _configuration.Product + _configuration.DialogName + "_btnAdd" + dataItem.Name + "=New " +
                             dataItem.Name + "\r\n";
                    btnEnOut += _configuration.Product + _configuration.DialogName + "_btnAdd" + dataItem.Name +
                                "=@@New " + dataItem.Name + "\r\n";
                    btnDe += _configuration.Product + _configuration.DialogName + "_btnAdd" + dataItem.Name + "=Neue " +
                             dataItem.Translation + "\r\n";
                    btnDeOut += _configuration.Product + _configuration.DialogName + "_btnAdd" + dataItem.Name +
                                "=@@Neue " + dataItem.Translation + "\r\n";

                    btnEn += _configuration.Product + _configuration.DialogName +
                             "_btnDeactivateVersion=Deactivate version\r\n";
                    btnEnOut += _configuration.Product + _configuration.DialogName +
                                "_btnDeactivateVersion=@@Deactivate version\r\n";
                    btnDe += _configuration.Product + _configuration.DialogName +
                             "_btnDeactivateVersion=Version deaktivieren\r\n";
                    btnDeOut += _configuration.Product + _configuration.DialogName +
                                "_btnDeactivateVersion=@@Version deaktivieren\r\n";

                    btnEn += _configuration.Product + _configuration.DialogName +
                             "_btnActivateVersion=Activate version\r\n";
                    btnEnOut += _configuration.Product + _configuration.DialogName +
                                "_btnActivateVersion=@@Activate version\r\n";
                    btnDe += _configuration.Product + _configuration.DialogName +
                             "_btnActivateVersion=Version aktivieren\r\n";
                    btnDeOut += _configuration.Product + _configuration.DialogName +
                                "_btnActivateVersion=@@Version aktivieren\r\n";
                }

                foreach (var plusDataObject in dataItem.Properties)
                {
                    lblEn += _configuration.Product + _configuration.DialogName + "_lbl" + dataItem.Name +
                             plusDataObject.Name +
                             "=" + plusDataObject.TranslationEn + "\r\n";
                    lblEnOut += _configuration.Product + _configuration.DialogName + "_lbl" + dataItem.Name +
                                plusDataObject.Name + "=@@" + plusDataObject.TranslationEn + "\r\n";
                    lblDe += _configuration.Product + _configuration.DialogName + "_lbl" + dataItem.Name +
                             plusDataObject.Name +
                             "=" + plusDataObject.TranslationDe + "\r\n";
                    lblDeOut += _configuration.Product + _configuration.DialogName + "_lbl" + dataItem.Name +
                                plusDataObject.Name + "=@@" + plusDataObject.TranslationDe + "\r\n";

                    if (plusDataObject.Type == "DateTime")
                    {
                        lblEn += _configuration.Product + _configuration.DialogName + "_lbl" + dataItem.Name +
                                 plusDataObject.Name +
                                 "From=" + plusDataObject.TranslationEn + " from\r\n";
                        lblEnOut += _configuration.Product + _configuration.DialogName + "_lbl" + dataItem.Name +
                                    plusDataObject.Name + "From=@@" + plusDataObject.TranslationEn + " from\r\n";
                        lblDe += _configuration.Product + _configuration.DialogName + "_lbl" + dataItem.Name +
                                 plusDataObject.Name +
                                 "From=" + plusDataObject.TranslationDe + " von\r\n";
                        lblDeOut += _configuration.Product + _configuration.DialogName + "_lbl" + dataItem.Name +
                                    plusDataObject.Name + "From=@@" + plusDataObject.TranslationDe + " von\r\n";
                        lblEn += _configuration.Product + _configuration.DialogName + "_lbl" + dataItem.Name +
                                 plusDataObject.Name +
                                 "To=" + plusDataObject.TranslationEn + " to\r\n";
                        lblEnOut += _configuration.Product + _configuration.DialogName + "_lbl" + dataItem.Name +
                                    plusDataObject.Name + "To=@@" + plusDataObject.TranslationEn + " to\r\n";
                        lblDe += _configuration.Product + _configuration.DialogName + "_lbl" + dataItem.Name +
                                 plusDataObject.Name +
                                 "To=" + plusDataObject.TranslationDe + " bis\r\n";
                        lblDeOut += _configuration.Product + _configuration.DialogName + "_lbl" + dataItem.Name +
                                    plusDataObject.Name + "To=@@" + plusDataObject.TranslationDe + " bis\r\n";
                    }
                }
            }

            string[] languages = {"en", "fr", "de", "hu", "pt", "zh-CHS"};

            foreach (var language in languages)
            {
                var fileContent = File.ReadAllText(_configuration.InputPath + @"Localization\localization.txt");
                fileContent = fileContent.DoReplaces();
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

                var languageExtension = language == "de" ? string.Empty : "." + language;
                var fileInfo = new FileInfo(_configuration.OutputPath + @"Localization\Dcx.Plus.Localization.Modules." +
                                            _configuration.Product + ".Nls" + languageExtension + ".txt");
                if (fileInfo.Directory != null) fileInfo.Directory.Create();
                File.WriteAllText(fileInfo.FullName, fileContent, Encoding.UTF8);
            }
        }

        private void CreateTandemLocalization()
        {
            var lblDataTandem = string.Empty;

            lblDataTandem += "MainDialogController_txtLogicalKey" + _configuration.ControllerHandle + "=" +
                             _configuration.Product.ToUpper() + " " + _configuration.DialogTranslationEnglish + "\r\n";
            lblDataTandem += "MainDialogController_txtLogicalKey" + _configuration.ControllerHandle + "=@@" +
                             _configuration.Product.ToUpper() + " " + _configuration.DialogTranslationEnglish + "\r\n";
            lblDataTandem += "MainDialogController_txtLogicalKey" + _configuration.ControllerHandle + "=" +
                             _configuration.Product.ToUpper() + " " + _configuration.DialogTranslationGerman + "\r\n";
            lblDataTandem += "MainDialogController_txtLogicalKey" + _configuration.ControllerHandle + "=@@" +
                             _configuration.Product.ToUpper() + " " + _configuration.DialogTranslationGerman + "\r\n";

            var fileInfo = new FileInfo(_configuration.OutputPath + @"Localization\DataTandem.txt");
            if (fileInfo.Directory != null) fileInfo.Directory.Create();
            File.WriteAllText(fileInfo.FullName, lblDataTandem, Encoding.UTF8);
        }

        private void CreatePlusDialogLocalization()
        {
            var lblPlusDialog = string.Empty;

            lblPlusDialog += "PlusMainForm_mnu" + _configuration.Product.ToUpper() + _configuration.DialogName + "=" +
                             _configuration.DialogTranslationEnglish + "\r\n";
            lblPlusDialog += "PlusMainForm_mnu" + _configuration.Product.ToUpper() + _configuration.DialogName + "=@@" +
                             _configuration.DialogTranslationEnglish + "\r\n";
            lblPlusDialog += "PlusMainForm_mnu" + _configuration.Product.ToUpper() + _configuration.DialogName + "=" +
                             _configuration.DialogTranslationGerman + "\r\n";
            lblPlusDialog += "PlusMainForm_mnu" + _configuration.Product.ToUpper() + _configuration.DialogName + "=@@" +
                             _configuration.DialogTranslationGerman + "\r\n";

            var fileInfo = new FileInfo(_configuration.OutputPath + @"Localization\PlusDialog.txt");
            if (fileInfo.Directory != null) fileInfo.Directory.Create();
            File.WriteAllText(fileInfo.FullName, lblPlusDialog, Encoding.UTF8);
        }
    }
}
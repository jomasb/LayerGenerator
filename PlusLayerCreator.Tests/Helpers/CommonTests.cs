using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlusLayerCreator.Items;

namespace PlusLayerCreator.Tests.Helpers
{
	[TestClass]
	public class HelperTests
	{
		#region ToLocalizedPlural

		[TestMethod]
		public void WhenLanguageSetToOne_ItShouldBeReturnedTheGermanPlural()
		{
			Assert.AreEqual("Eliten", "Elite".ToLocalizedPlural(1));
			Assert.AreEqual("Lehrlinge", "Lehrling".ToLocalizedPlural(1));
			Assert.AreEqual("Frisöre", "Frisör".ToLocalizedPlural(1));
			Assert.AreEqual("Studenten", "Student".ToLocalizedPlural(1));
			Assert.AreEqual("Fabrikanten", "Fabrikant".ToLocalizedPlural(1));
			Assert.AreEqual("Mechaniken", "Mechanik".ToLocalizedPlural(1));
			Assert.AreEqual("Mediatoren", "Mediator".ToLocalizedPlural(1));
			Assert.AreEqual("Freundschaften", "Freundschaft".ToLocalizedPlural(1));
			Assert.AreEqual("Paritäten", "Parität".ToLocalizedPlural(1));
			Assert.AreEqual("Versicherungen", "Versicherung".ToLocalizedPlural(1));
			Assert.AreEqual("Prequel", "Prequel".ToLocalizedPlural(1));
			Assert.AreEqual("Versicherer", "Versicherer".ToLocalizedPlural(1));
			Assert.AreEqual("Rahmen", "Rahmen".ToLocalizedPlural(1));
			Assert.AreEqual("Opas", "Opa".ToLocalizedPlural(1));
			Assert.AreEqual("Muttis", "Muttis".ToLocalizedPlural(1));
			Assert.AreEqual("Autos", "Auto".ToLocalizedPlural(1));
			Assert.AreEqual("Hobbys", "Hobby".ToLocalizedPlural(1));
			Assert.AreEqual("Akkus", "Akku".ToLocalizedPlural(1));
		}

		[TestMethod]
		public void WhenLanguageSetToTwo_ItShouldBeReturnedTheEnglishPlural()
		{
			Assert.AreEqual("Stations", "Station".ToLocalizedPlural(2));
			Assert.AreEqual("Skies", "Sky".ToLocalizedPlural(2));
		}

		#endregion

		#region GetMaxValue

		[TestMethod]
		public void WhenLengthIsNullOrEmpty_ItShouldBeReturned999999()
		{
			Assert.AreEqual(999999, PlusLayerCreator.Helpers.GetMaxValue(null));
			Assert.AreEqual(999999, PlusLayerCreator.Helpers.GetMaxValue(""));
		}

		[TestMethod]
		public void WhenLengthIsNotParsableToInt_ItShouldBeReturnedZero()
		{
			Assert.AreEqual(999999, PlusLayerCreator.Helpers.GetMaxValue(null));
			Assert.AreEqual(999999, PlusLayerCreator.Helpers.GetMaxValue(""));
		}

		[TestMethod]
		public void WhenLengthIsParsableToInt_ItShouldBeReturnedAsMuchNinesAsLengthIs()
		{
			Assert.AreEqual(0, PlusLayerCreator.Helpers.GetMaxValue("0"));
			Assert.AreEqual(9, PlusLayerCreator.Helpers.GetMaxValue("1"));
			Assert.AreEqual(99, PlusLayerCreator.Helpers.GetMaxValue("2"));
			Assert.AreEqual(999, PlusLayerCreator.Helpers.GetMaxValue("3"));
			Assert.AreEqual(9999, PlusLayerCreator.Helpers.GetMaxValue("4"));
			Assert.AreEqual(99999, PlusLayerCreator.Helpers.GetMaxValue("5"));
			Assert.AreEqual(999999, PlusLayerCreator.Helpers.GetMaxValue("6"));
		}

		#endregion

		#region ReplaceSpecialContent

		[TestMethod]
		public void WhenFileContentContainsAtLeastOneSpecicalContent_ItShouldBeReplacedByTheCorrespondingContent()
		{
			string givenValue = "The $specialContent1$ is very $specialContent2$";
			string expectedValue = "The sun is very hot";
			string expectedValue1 = "The sun is very $specialContent2$";
			string[] content = {"sun", "hot"};
			string[] content1 = {"sun"};

			Assert.AreEqual(expectedValue, givenValue.ReplaceSpecialContent(content));
			Assert.AreEqual(expectedValue1, givenValue.ReplaceSpecialContent(content1));
		}

		#endregion

		#region GetLocalizedString

		[TestMethod]
		public void WhenInputIsSetAndMultiIsFalse_ItShouldBeReturnedTheLocalizedMarkupExtensionXaml()
		{
			PlusLayerCreator.Helpers.Configuration = new Configuration()
			{
				DialogName = "AdminOfAbc",
				Product = "Tools"
			};

			string expectedValue = "{localization:Localize Key=ToolsAdminOfAbc_lblSite, Source=ToolsLocalizer}";

			Assert.AreEqual(expectedValue, "Site".GetLocalizedString(false));
		}

		[TestMethod]
		public void WhenInputIsSetAndMultiIsTrue_ItShouldBeReturnedTheLocalizedMarkupExtensionXamlInPlural()
		{
			PlusLayerCreator.Helpers.Configuration = new Configuration()
			{
				DialogName = "AdminOfAbc",
				Product = "Tools"
			};

			string expectedValue = "{localization:Localize Key=ToolsAdminOfAbc_lblSites, Source=ToolsLocalizer}";

			Assert.AreEqual(expectedValue, "Site".GetLocalizedString(true));
		}

		#endregion

		#region GetParent

		[TestMethod]
		public void WhenDataItemHasParent_ItShouldBeReturnedTheParentOtherwiseNull()
		{
			ConfigurationItem parent = new ConfigurationItem()
			{
				Name = "Parent",
				Order = 0
			};

			ConfigurationItem child = new ConfigurationItem()
			{
				Name = "Child",
				Order = 1,
				Parent = "Parent"
			};
			
			PlusLayerCreator.Helpers.Configuration = new Configuration()
			{
				DialogName = "AdminOfAbc",
				Product = "Tools",
				DataLayout = new ObservableCollection<ConfigurationItem>()
			};

			PlusLayerCreator.Helpers.Configuration.DataLayout.Add(parent);
			PlusLayerCreator.Helpers.Configuration.DataLayout.Add(child);

			Assert.AreEqual(parent, child.GetParent());
			Assert.AreEqual(null, parent.GetParent());
		}

		#endregion

		#region GetParentParameter

		[TestMethod]
		public void WhenDataItemHasParent_ItShouldBeReturnedTheCorrespondingParameter()
		{
			ConfigurationItem parent = new ConfigurationItem()
			{
				Name = "Parent",
				Order = 0
			};

			ConfigurationItem child = new ConfigurationItem()
			{
				Name = "Child",
				Order = 1,
				Parent = "Parent"
			};

			PlusLayerCreator.Helpers.Configuration = new Configuration()
			{
				DialogName = "AdminOfAbc",
				Product = "Tools",
				DataLayout = new ObservableCollection<ConfigurationItem>()
			};

			PlusLayerCreator.Helpers.Configuration.DataLayout.Add(parent);
			PlusLayerCreator.Helpers.Configuration.DataLayout.Add(child);

			string expectedValueParam = ", ToolsParent parentParent";
			string expectedValueCall = ", parentParent";
			string expectedValueEmpty = "arguments.Add(\"Parent\", parentParent);\r\n";

			Assert.AreEqual(expectedValueParam, child.GetParentParameter("param"));
			Assert.AreEqual(expectedValueCall, child.GetParentParameter("call"));
			Assert.AreEqual(expectedValueEmpty, child.GetParentParameter(""));
		}

		#endregion

		#region GetPreFilterInformation


		[TestMethod]
		public void WhenDataItemPreFilterItem_ItShouldBeReturnedTheCorrespondingFilterInformation()
		{
			ConfigurationItem filterItem1 = new ConfigurationItem()
			{
				Name = "FilterItem1",
				IsPreFilterItem = true,
				Order = 0
			};

			ConfigurationItem filterItem2 = new ConfigurationItem()
			{
				Name = "FilterItem2",
				IsPreFilterItem = true,
				Order = 1
			};

			ConfigurationItem mainItem = new ConfigurationItem()
			{
				Name = "MainItem",
				IsPreFilterItem = false,
				Order = 2
			};

			PlusLayerCreator.Helpers.Configuration = new Configuration()
			{
				DialogName = "AdminOfAbc",
				Product = "Tools",
				DataLayout = new ObservableCollection<ConfigurationItem>()
			};

			PlusLayerCreator.Helpers.Configuration.DataLayout.Add(filterItem1);
			PlusLayerCreator.Helpers.Configuration.DataLayout.Add(filterItem2);
			PlusLayerCreator.Helpers.Configuration.DataLayout.Add(mainItem);

			string expectedValueParam = ", ToolsFilterItem1 filterItem1, ToolsFilterItem2 filterItem2";
			string expectedValueArguments = "arguments.Add(\"ToolsFilterItem1\", filterItem1);\r\narguments.Add(\"ToolsFilterItem2\", filterItem2);\r\n";
			string expectedValueListCall = ", filterItem1, filterItem2";

			Assert.AreEqual(expectedValueParam, mainItem.GetPreFilterInformation("parameter"));
			Assert.AreEqual(expectedValueArguments, mainItem.GetPreFilterInformation("arguments"));
			Assert.AreEqual(expectedValueListCall, mainItem.GetPreFilterInformation("listCall"));
		}

		#endregion
	}
}
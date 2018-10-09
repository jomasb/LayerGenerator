using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlusLayerCreator.Configure;
using PlusLayerCreator.Items;

namespace PlusLayerCreator.Tests
{
	[TestClass]
	public class UiTests
	{
		#region Initialize

		private UiPart _uiPart;
		private Configuration _configuration;

		[TestInitialize]
		public void Initialize()
		{
			_configuration = new Configuration()
			{
				DialogName = "AdminOfAbc",
				Product = "Tools",
				InputPath = Environment.CurrentDirectory + @"\..\..\..\Templates\",
				DataLayout = new List<ConfigurationItem>()
				{
					new ConfigurationItem()
					{
						Name = "FilterItem1",
						IsPreFilterItem = true,
						Order = 0
					},
					new ConfigurationItem()
					{
						Name = "FilterItem2",
						IsPreFilterItem = true,
						Order = 1
					},
					new ConfigurationItem()
					{
						Name = "MainItem",
						IsPreFilterItem = false,
						CanEdit = true,
						Order = 2,
						Properties = new ObservableCollection<ConfigurationProperty>()
						{
							new ConfigurationProperty()
							{
								Order = 0,
								Type = "string",
								Name = "String1",
								IsKey = true,
								IsRequired = true,
								IsReadOnly = false,
								Length = "10",
							},
							new ConfigurationProperty()
							{
								Order = 1,
								Type = "string",
								Name = "String2",
								IsKey = false,
								IsRequired = false,
								IsReadOnly = false,
								Length = "10",
							},
							new ConfigurationProperty()
							{
								Order = 2,
								Type = "string",
								Name = "String3",
								IsRequired = false,
								IsReadOnly = true,
								Length = "10",
							},
							new ConfigurationProperty()
							{
								Order = 3,
								Type = "int",
								Name = "Int1",
								IsKey = true,
								IsRequired = true,
								IsReadOnly = false,
								Length = "2",
							},
							new ConfigurationProperty()
							{
								Order = 4,
								Type = "int",
								Name = "Int2",
								IsRequired = false,
								IsReadOnly = true,
								Length = "2",
							},
							new ConfigurationProperty()
							{
								Order = 5,
								Type = "bool",
								Name = "Bool1",
								IsRequired = true,
								IsReadOnly = false
							},
							new ConfigurationProperty()
							{
								Order = 6,
								Type = "bool",
								Name = "Bool2",
								IsRequired = false,
								IsReadOnly = true
							},
							new ConfigurationProperty()
							{
								Order = 7,
								Type = "DateTime",
								Name = "DateTime1",
								IsRequired = true,
								IsReadOnly = false
							},
							new ConfigurationProperty()
							{
								Order = 8,
								Type = "DateTime",
								Name = "DateTime2",
								IsRequired = false,
								IsReadOnly = true
							}
						}
					},
					new ConfigurationItem()
					{
						Name = "ChildItem",
						IsPreFilterItem = false,
						CanEdit = true,
						Order = 3,
						Parent = "MainItem",
						Properties = new ObservableCollection<ConfigurationProperty>()
						{
							new ConfigurationProperty()
							{
								Order = 0,
								Type = "string",
								Name = "String1",
								IsRequired = true,
								IsReadOnly = false,
								IsVisibleInGrid = true,
								Length = "10",
							},
							new ConfigurationProperty()
							{
								Order = 1,
								Type = "string",
								Name = "String2",
								IsRequired = false,
								IsReadOnly = false,
								IsVisibleInGrid = false,
								Length = "10",
							},
							new ConfigurationProperty()
							{
								Order = 2,
								Type = "string",
								Name = "String3",
								IsRequired = false,
								IsReadOnly = true,
								IsVisibleInGrid = false,
								Length = "10",
							},
							new ConfigurationProperty()
							{
								Order = 3,
								Type = "int",
								Name = "Int1",
								IsRequired = true,
								IsReadOnly = false,
								IsVisibleInGrid = true,
								Length = "2",
							},
							new ConfigurationProperty()
							{
								Order = 4,
								Type = "int",
								Name = "Int2",
								IsRequired = false,
								IsReadOnly = true,
								IsVisibleInGrid = false,
								Length = "2",
							},
							new ConfigurationProperty()
							{
								Order = 5,
								Type = "bool",
								Name = "Bool1",
								IsRequired = true,
								IsVisibleInGrid = true,
								IsReadOnly = false
							},
							new ConfigurationProperty()
							{
								Order = 6,
								Type = "bool",
								Name = "Bool2",
								IsRequired = false,
								IsVisibleInGrid = false,
								IsReadOnly = true
							},
							new ConfigurationProperty()
							{
								Order = 7,
								Type = "DateTime",
								Name = "DateTime1",
								IsRequired = true,
								IsVisibleInGrid = true,
								IsReadOnly = false
							},
							new ConfigurationProperty()
							{
								Order = 8,
								Type = "DateTime",
								Name = "DateTime2",
								IsRequired = false,
								IsVisibleInGrid = false,
								IsReadOnly = true
							}
						}
					}
				}
			};

			_uiPart = new UiPart(_configuration);
			PlusLayerCreator.Helpers.Configuration = _configuration;
		}

		#endregion

		#region Helpers

		#region View

		#region GetItemControl

		[TestMethod]
		public void WhenItemCanEditAndPropertyIsStringAndEditableAndIsFirstItem_ItShouldBeReturnedTheItemControlAsXaml()
		{
			ConfigurationItem mainItem = _configuration.DataLayout.First(t => t.Name == "MainItem");
			ConfigurationProperty firstProperty = mainItem.Properties.First(t => t.Order == 0);

			string testValue = _uiPart.GetItemControl(mainItem, firstProperty);
			string expectedValue = "<plus:PlusTextBox plus:FocusHelper.IsFocused=\"{Binding IsNewItem}\" IsReadOnly=\"{Binding IsNewItem, Converter={StaticResource InvertBoolConverter}}\" MaxLength =\"10\" Text=\"{Binding DataItem.String1, UpdateSourceTrigger=PropertyChanged}\" />";

			Assert.AreEqual(expectedValue, testValue);
		}

		[TestMethod]
		public void WhenItemCanEditAndPropertyIsStringAndEditable_ItShouldBeReturnedTheItemControlAsXaml()
		{
			ConfigurationItem mainItem = _configuration.DataLayout.First(t => t.Name == "MainItem");
			ConfigurationProperty firstProperty = mainItem.Properties.First(t => t.Order == 1);

			string testValue = _uiPart.GetItemControl(mainItem, firstProperty);
			string expectedValue = "<plus:PlusTextBox MaxLength =\"10\" Text=\"{Binding DataItem.String2, UpdateSourceTrigger=PropertyChanged}\" />";

			Assert.AreEqual(expectedValue, testValue);
		}

		[TestMethod]
		public void WhenItemCanEditAndPropertyIsStringAndNotEditable_ItShouldBeReturnedTheItemControlAsXaml()
		{
			ConfigurationItem mainItem = _configuration.DataLayout.First(t => t.Name == "MainItem");
			ConfigurationProperty firstProperty = mainItem.Properties.First(t => t.Order == 2);

			string testValue = _uiPart.GetItemControl(mainItem, firstProperty);
			string expectedValue = "<plus:PlusTextBox IsReadOnly=\"True\" MaxLength =\"10\" Text=\"{Binding DataItem.String3, UpdateSourceTrigger=PropertyChanged}\" />";

			Assert.AreEqual(expectedValue, testValue);
		}

		[TestMethod]
		public void WhenItemCanEditAndPropertyIsIntAndEditable_ItShouldBeReturnedTheItemControlAsXaml()
		{
			ConfigurationItem mainItem = _configuration.DataLayout.First(t => t.Name == "MainItem");
			ConfigurationProperty firstProperty = mainItem.Properties.First(t => t.Order == 3);

			string testValue = _uiPart.GetItemControl(mainItem, firstProperty);
			string expectedValue = "<plus:PlusTextBox IsReadOnly=\"{Binding IsNewItem, Converter={StaticResource InvertBoolConverter}}\" MaxLength =\"2\" IsNumeric=\"True\" Text=\"{Binding DataItem.Int1, UpdateSourceTrigger=PropertyChanged}\" />";

			Assert.AreEqual(expectedValue, testValue);
		}

		[TestMethod]
		public void WhenItemCanEditAndPropertyIsIntAndNotEditable_ItShouldBeReturnedTheItemControlAsXaml()
		{
			ConfigurationItem mainItem = _configuration.DataLayout.First(t => t.Name == "MainItem");
			ConfigurationProperty firstProperty = mainItem.Properties.First(t => t.Order == 4);

			string testValue = _uiPart.GetItemControl(mainItem, firstProperty);
			string expectedValue = "<plus:PlusTextBox IsReadOnly=\"True\" MaxLength =\"2\" IsNumeric=\"True\" Text=\"{Binding DataItem.Int2, UpdateSourceTrigger=PropertyChanged}\" />";

			Assert.AreEqual(expectedValue, testValue);
		}

		[TestMethod]
		public void WhenItemCanEditAndPropertyIsBoolAndEditable_ItShouldBeReturnedTheItemControlAsXaml()
		{
			ConfigurationItem mainItem = _configuration.DataLayout.First(t => t.Name == "MainItem");
			ConfigurationProperty firstProperty = mainItem.Properties.First(t => t.Order == 5);

			string testValue = _uiPart.GetItemControl(mainItem, firstProperty);
			string expectedValue = "<plus:PlusCheckBox IsChecked=\"{Binding DataItem.Bool1}\" />";

			Assert.AreEqual(expectedValue, testValue);
		}

		[TestMethod]
		public void WhenItemCanEditAndPropertyIsBoolAndNotEditable_ItShouldBeReturnedTheItemControlAsXaml()
		{
			ConfigurationItem mainItem = _configuration.DataLayout.First(t => t.Name == "MainItem");
			ConfigurationProperty firstProperty = mainItem.Properties.First(t => t.Order == 6);

			string testValue = _uiPart.GetItemControl(mainItem, firstProperty);
			string expectedValue = "<plus:PlusCheckBox IsReadOnly=\"True\" IsChecked=\"{Binding DataItem.Bool2}\" />";

			Assert.AreEqual(expectedValue, testValue);
		}

		[TestMethod]
		public void WhenItemCanEditAndPropertyIsDateTimeAndEditable_ItShouldBeReturnedTheItemControlAsXaml()
		{
			ConfigurationItem mainItem = _configuration.DataLayout.First(t => t.Name == "MainItem");
			ConfigurationProperty firstProperty = mainItem.Properties.First(t => t.Order == 7);

			string testValue = _uiPart.GetItemControl(mainItem, firstProperty);
			string expectedValue = "<plus:PlusDateTimePicker  \r\n" +
			                       "	VerticalAlignment=\"Center\"\r\n" +
								   "	ClockHeader=\"{localization:Localize Key=PlusDateTimePicker_lblClockHeader, Source=PlusUIWPFLocalizer}\"\r\n" +
								   "	DateTimeWatermarkContent=\"{localization:Localize Key=PlusDateTimePicker_lblWatermarkContent, Source=PlusUIWPFLocalizer}\"\r\n" +
								   "	SelectedValue=\"{Binding DataItem.DateTime1, UpdateSourceTrigger=PropertyChanged, Delay=300}\"\r\n" +
								   "	TodayButtonLocalization=\"{localization:Localize Key=PlusDateTimePicker_lblNowButton, Source=PlusUIWPFLocalizer}\" />";

			Assert.AreEqual(expectedValue, testValue);
		}

		[TestMethod]
		public void WhenItemCanEditAndPropertyIsDateTimeAndNotEditable_ItShouldBeReturnedTheItemControlAsXaml()
		{
			ConfigurationItem mainItem = _configuration.DataLayout.First(t => t.Name == "MainItem");
			ConfigurationProperty firstProperty = mainItem.Properties.First(t => t.Order == 8);

			string testValue = _uiPart.GetItemControl(mainItem, firstProperty);
			string expectedValue = "<plus:PlusDateTimePicker  IsReadOnly=\"True\" \r\n" +
			                       "	VerticalAlignment=\"Center\"\r\n" +
			                       "	ClockHeader=\"{localization:Localize Key=PlusDateTimePicker_lblClockHeader, Source=PlusUIWPFLocalizer}\"\r\n" +
			                       "	DateTimeWatermarkContent=\"{localization:Localize Key=PlusDateTimePicker_lblWatermarkContent, Source=PlusUIWPFLocalizer}\"\r\n" +
			                       "	SelectedValue=\"{Binding DataItem.DateTime2, UpdateSourceTrigger=PropertyChanged, Delay=300}\"\r\n" +
			                       "	TodayButtonLocalization=\"{localization:Localize Key=PlusDateTimePicker_lblNowButton, Source=PlusUIWPFLocalizer}\" />";

			Assert.AreEqual(expectedValue, testValue);
		}

		[TestMethod]
		public void WhenItemIsReadOnlyAndPropertyIsBool_ItShouldBeReturnedTheItemControlAsXaml()
		{
			ConfigurationItem mainItem = _configuration.DataLayout.First(t => t.Name == "MainItem");
			mainItem.CanEdit = false;
			ConfigurationProperty firstProperty = mainItem.Properties.First(t => t.Order == 5);

			string testValue = _uiPart.GetItemControl(mainItem, firstProperty);
			string expectedValue = "            <plus:PlusLabel Content=\"{Binding DataItem.Bool1, Converter={StaticResource BoolToLocalizedYesNoConverterConverter}}\" />\r\n";

			Assert.AreEqual(expectedValue, testValue);
		}


		[TestMethod]
		public void WhenItemIsReadOnlyAndPropertyIsNotBool_ItShouldBeReturnedTheItemControlAsXaml()
		{
			ConfigurationItem mainItem = _configuration.DataLayout.First(t => t.Name == "MainItem");
			mainItem.CanEdit = false;
			ConfigurationProperty firstProperty = mainItem.Properties.First(t => t.Order == 0);

			string testValue = _uiPart.GetItemControl(mainItem, firstProperty);
			string expectedValue = "            <plus:PlusLabel Content=\"{Binding DataItem.String1}\" />\r\n";

			Assert.AreEqual(expectedValue, testValue);
		}

		#endregion

		#region GetParentInformation

		[TestMethod]
		public void WhenItemHasParentOnLivelOne_ItShouldBeReturnedTheParentInformationForEachKeyField()
		{
			ConfigurationItem childItem = _configuration.DataLayout.First(t => t.Name == "ChildItem");
			
			string testValue = _uiPart.GetParentInformation(childItem, 1);
			string expectedValue = "<plus:PlusGroupBox Header=\"{localization:Localize Key=ToolsAdminOfAbc_lblMainItem, Source=ToolsLocalizer}\">\r\n" +
				"    <StackPanel>\r\n" +
				"        <plus:PlusFormRow Label=\"{localization:Localize Key=ToolsAdminOfAbc_lblMainItemString1, Source=ToolsLocalizer}\">\r\n" +
				"            <plus:PlusLabel Content=\"{Binding DataItem.Parent.Parent.Parent.Parent.String1}\" />\r\n" +
				"        </plus:PlusFormRow>\r\n" +
				"        <plus:PlusFormRow Label=\"{localization:Localize Key=ToolsAdminOfAbc_lblMainItemInt1, Source=ToolsLocalizer}\">\r\n" +
				"            <plus:PlusLabel Content=\"{Binding DataItem.Parent.Parent.Parent.Parent.Int1}\" />\r\n" +
				"        </plus:PlusFormRow>\r\n" +
				"    </StackPanel>\r\n" +
				"</plus:PlusGroupBox>\r\n";


			Assert.AreEqual(expectedValue, testValue);
		}

		#endregion

		#region GetGridColumnsXaml

		[TestMethod]
		public void WhenItemHasPropertiesWhichAreMarkedAsVisibleInGrid_ItShouldBeReturnedThoseColumns()
		{
			ConfigurationItem childItem = _configuration.DataLayout.First(t => t.Name == "ChildItem");

			string testValue = _uiPart.GetGridColumnsXaml(childItem, false);
			string expectedValue = "                <plus:PlusGridViewTextColumnMinimal Width=\"*\" DataMemberBinding=\"{Binding String1}\" Header=\"{localization:Localize Key=ToolsAdminOfAbc_lblChildItemString1, Source=ToolsLocalizer}\"/>\r\n" +
			                       "                <plus:PlusGridViewTextColumnMinimal Width=\"*\" DataMemberBinding=\"{Binding Int1}\" Header=\"{localization:Localize Key=ToolsAdminOfAbc_lblChildItemInt1, Source=ToolsLocalizer}\"/>\r\n" +
			                       "                <plus:PlusGridViewCheckColumnMinimal Width=\"*\" DataMemberBinding=\"{Binding Bool1}\" Header=\"{localization:Localize Key=ToolsAdminOfAbc_lblChildItemBool1, Source=ToolsLocalizer}\"/>\r\n" +
			                       "                <plus:PlusGridViewTextColumnMinimal Width=\"*\" DataMemberBinding=\"{Binding DateTime1}\" Header=\"{localization:Localize Key=ToolsAdminOfAbc_lblChildItemDateTime1, Source=ToolsLocalizer}\"/>\r\n";


			Assert.AreEqual(expectedValue, testValue);
		}

		[TestMethod]
		public void WhenItemHasPropertiesWhichAreMarkedAsVisibleInGridAndOldIsTrue_ItShouldBeReturnedThoseColumnsInOldGridStyle()
		{
			ConfigurationItem childItem = _configuration.DataLayout.First(t => t.Name == "ChildItem");

			string testValue = _uiPart.GetGridColumnsXaml(childItem, true);
			string expectedValue = "                <plus:PlusGridViewTextColumn Width=\"*\" DataMemberBinding=\"{Binding String1}\" Header=\"{localization:Localize Key=ToolsAdminOfAbc_lblChildItemString1, Source=ToolsLocalizer}\"/>\r\n" +
								   "                <plus:PlusGridViewTextColumn Width=\"*\" DataMemberBinding=\"{Binding Int1}\" Header=\"{localization:Localize Key=ToolsAdminOfAbc_lblChildItemInt1, Source=ToolsLocalizer}\"/>\r\n" +
			                       "                <plus:PlusGridViewCheckColumn Width=\"*\" DataMemberBinding=\"{Binding Bool1}\" Header=\"{localization:Localize Key=ToolsAdminOfAbc_lblChildItemBool1, Source=ToolsLocalizer}\"/>\r\n" +
								   "                <plus:PlusGridViewTextColumn Width=\"*\" DataMemberBinding=\"{Binding DateTime1}\" Header=\"{localization:Localize Key=ToolsAdminOfAbc_lblChildItemDateTime1, Source=ToolsLocalizer}\"/>\r\n";


			Assert.AreEqual(expectedValue, testValue);
		}

		#endregion

		#endregion

		#endregion
	}
}

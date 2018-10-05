using System;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlusLayerCreator.Items;

namespace PlusLayerCreator.Tests
{
	[TestClass]
	public class HelperTests
	{
		#region Common

		#region CamelCase

		[TestMethod]
		public void WhenStringSetToBeCamelCase_ItShouldBeFormattedAsCamelCase()
		{
			string testValue = "Das-ist_ein-Test";
			string expectedValue = "DasIstEinTest";
			Assert.AreEqual(expectedValue, testValue.ToCamelCase());
		}

		[TestMethod]
		public void WhenStringSetToBeCamelCaseAndStringIsNullOrLessThanTwoChars_GivenValueWillBeReturned()
		{
			string test = null;
			Assert.AreEqual(null, test.ToCamelCase());
			Assert.AreEqual("", "".ToCamelCase());
			Assert.AreEqual("A", "A".ToCamelCase());
		}

		#endregion

		#region PascalCase

		[TestMethod]
		public void WhenStringSetToBePascalCase_ItShouldBeFormattedAsPascalCase()
		{
			string testValue = "DasIstEinTest";
			string testValue1 = "dasIstEinTest";
			string expectedValue = "dasIstEinTest";
			Assert.AreEqual(expectedValue, testValue.ToPascalCase());
			Assert.AreEqual(expectedValue, testValue1.ToPascalCase());
		}

		[TestMethod]
		public void WhenSplittableStringSetToBePascalCase_ItShouldBeFormattedAsPascalCase()
		{
			string testValue = "Das-ist_ein-Test";
			string testValue1 = "Das ist ein Test";
			string testValue2 = "Das_ist_ein-Test";
			string expectedValue = "dasIstEinTest";
			Assert.AreEqual(expectedValue, testValue.ToPascalCase());
			Assert.AreEqual(expectedValue, testValue1.ToPascalCase());
			Assert.AreEqual(expectedValue, testValue2.ToPascalCase());
		}

		[TestMethod]
		public void WhenStringSetToBePascalCaseAndStringIsNullOrLessThanTwoChars_GivenValueWillBeReturned()
		{
			string test = null;
			Assert.AreEqual(test.ToCamelCase(), null);
			Assert.AreEqual("".ToCamelCase(), "");
			Assert.AreEqual("A".ToCamelCase(), "A");
		}

		#endregion

		#endregion Common

		#region Server

		#region ToSqlName

		[TestMethod]
		public void WhenStringSetToBeSqlName_ItShouldBeFormattedAsSqlName()
		{
			string testValue = "FELD-WERT";
			string expectedValue = "FELD_WERT";
			Assert.AreEqual(expectedValue, testValue.ToSqlName());
		}

		#endregion

		#region ToVariableName

		[TestMethod]
		public void WhenStringSetToBeVariableName_ItShouldBeFormattedAsVariableName()
		{
			string testValue = "FELD_WERT";
			string expectedValue = "FELD-WERT";
			Assert.AreEqual(expectedValue, testValue.ToVariableName());
		}

		#endregion

		#region GetYearToFractionDigits

		[TestMethod]
		public void WhenConfigurationPropertyLenghtIsNumber_ReturnsTheConvertedNumber()
		{
			ConfigurationProperty property = new ConfigurationProperty()
			{
				Order = 0,
				Length = "23"
			};
			Assert.AreEqual(3, Helpers.GetYearToFractionDigits(property));
		}

		[TestMethod]
		public void WhenConfigurationPropertyIsNull_ReturnsMinusOne()
		{
			ConfigurationProperty property = null;
			Assert.AreEqual(-1, Helpers.GetYearToFractionDigits(property));
		}

		[TestMethod]
		public void WhenConfigurationPropertyLenghtIsNotANumber_ReturnsMinusOne()
		{
			ConfigurationProperty property = new ConfigurationProperty()
			{
				Order = 0,
				Length = "abxc"
			};

			Assert.AreEqual(-1, Helpers.GetYearToFractionDigits(property));
		}

		#endregion

		#region CalculateServerRows

		[TestMethod]
		public void WhenConfigurationItemPropertiesLenghtIsSet_ReturnsTheCalculatedRows()
		{
			ConfigurationItem item = new ConfigurationItem();
			item.Properties = new ObservableCollection<ConfigurationProperty>();
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 0,
				Length = "10"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 1,
				Length = "24"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 2,
				Length = "15"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 3,
				Length = "60"
			});

			//no modify property in request
			Assert.AreEqual(275, Helpers.CalculateServerRows(item, false));

			//modify property in request
			Assert.AreEqual(272, Helpers.CalculateServerRows(item, true));
		}

		#endregion

		#region ArrangeProperties

		[TestMethod]
		public void WhenConfigurationItemPropertiesIsSet_ReturnsTheArrangedProperties()
		{
			string expectedValue = "  02 FELD-NAME                       TYPE string.\r\n" +
			                       "  02 FELD-WERT                       TYPE string.\r\n" +
			                       "  02 LUPD-TIMESTAMP                  TYPE DateTime.";

			ConfigurationItem item = new ConfigurationItem();
			item.Properties = new ObservableCollection<ConfigurationProperty>();
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 0,
				MessageField = "FELD-NAME",
				MessageDataType = "string"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 1,
				MessageField = "FELD_WERT",
				MessageDataType = "string"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 2,
				MessageField = "LUPD-TIMESTAMP",
				MessageDataType = "DateTime"
			});

			Assert.AreEqual(expectedValue, Helpers.ArrangeProperties(item));
		}

		#endregion
		
		#region ArrangeKeyProperties

		[TestMethod]
		public void WhenConfigurationItemPropertiesIsSet_ReturnsTheArrangedKeyProperties()
		{
			string expectedValue = "##1/##2";

			ConfigurationItem item = new ConfigurationItem();
			item.Properties = new ObservableCollection<ConfigurationProperty>();
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 0,
				IsKey = true,
				MessageField = "FELD-NAME",
				MessageDataType = "string"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 1,
				IsKey = true,
				MessageField = "FELD_WERT",
				MessageDataType = "string"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 2,
				IsKey = false,
				MessageField = "LUPD-TIMESTAMP",
				MessageDataType = "DateTime"
			});

			Assert.AreEqual(expectedValue, Helpers.ArrangeKeyProperties(item));
		}

		[TestMethod]
		public void WhenConfigurationItemPropertiesIsSetButTheNumberIsGreaterThanSeven_ReturnsTheOnlySevenArrangedKeyProperties()
		{
			string expectedValue = "##1/##2/##3/##4/##5/##6/##7";

			ConfigurationItem item = new ConfigurationItem();
			item.Properties = new ObservableCollection<ConfigurationProperty>();
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 0,
				IsKey = true,
				MessageField = "FELD-NAME",
				MessageDataType = "string"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 1,
				IsKey = true,
				MessageField = "FELD_WERT",
				MessageDataType = "string"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 2,
				IsKey = true,
				MessageField = "FELD-NAME1",
				MessageDataType = "string"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 3,
				IsKey = true,
				MessageField = "FELD_WERT1",
				MessageDataType = "string"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 4,
				IsKey = true,
				MessageField = "FELD-NAME2",
				MessageDataType = "string"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 5,
				IsKey = true,
				MessageField = "FELD_WERT2",
				MessageDataType = "string"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 6,
				IsKey = true,
				MessageField = "FELD-NAME3",
				MessageDataType = "string"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 7,
				IsKey = true,
				MessageField = "FELD_WERT3",
				MessageDataType = "string"
			});

			Assert.AreEqual(Helpers.ArrangeKeyProperties(item), expectedValue);
		}

		#endregion
		
		#region ArrangeDelete

		[TestMethod]
		public void WhenConfigurationItemPropertiesIsSet_ReturnsTheArrangedDelete()
		{
			string expectedValue =  "                   FELD_NAME\r\n" +
									"                = :FELD-NAME                         OF S00D02-ROW\r\n" +
			                        "         AND       FELD_WERT\r\n" +
									"                = :FELD-WERT                         OF S00D02-ROW";

			ConfigurationItem item = new ConfigurationItem()
			{
				Table = "S00D02"
			};
			item.Properties = new ObservableCollection<ConfigurationProperty>();
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 0,
				IsKey = true,
				MessageField = "FELD-NAME"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 1,
				IsKey = true,
				MessageField = "FELD_WERT"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 2,
				IsKey = false,
				MessageField = "LUPD-TIMESTAMP"
			});

			Assert.AreEqual(expectedValue, Helpers.ArrangeDelete(item));
		}

		#endregion

		#region ArrangeCursorFields

		[TestMethod]
		public void WhenConfigurationItemPropertiesIsSet_ReturnsTheArrangedCursorFields()
		{
			string expectedValue = "                   FELD_NAME\r\n" +
									"                 , FELD_WERT\r\n" +
									"                 , LUPD_TIMESTAMP";

			ConfigurationItem item = new ConfigurationItem()
			{
				Table = "S00D02"
			};
			item.Properties = new ObservableCollection<ConfigurationProperty>();
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 0,
				IsKey = true,
				MessageField = "FELD-NAME"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 1,
				IsKey = true,
				MessageField = "FELD_WERT"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 2,
				IsKey = false,
				MessageField = "LUPD-TIMESTAMP"
			});

			Assert.AreEqual(expectedValue, Helpers.ArrangeCursorFields(item));
		}

		#endregion

		#region ArrangeDataFromRequest

		[TestMethod]
		public void WhenConfigurationItemPropertiesIsSet_ReturnsTheArrangedDataFromRequest()
		{
			string expectedValue = "    MOVE FELD-NAME                      OF S00S33-REQ-2(I)" + "\r\n" +
			                       "      TO FELD-NAME                      OF S00D02-ROW" + "\r\n" +
			                       "    MOVE FELD-WERT                      OF S00S33-REQ-2(I)" + "\r\n" +
			                       "      TO FELD-WERT                      OF S00D02-ROW" + "\r\n" +
								   "    MOVE LUPD-TIMESTAMP                 OF S00S33-REQ-2(I)" + "\r\n" +
								   "      TO LUPD-TIMESTAMP                 OF S00D02-ROW";

			ConfigurationItem item = new ConfigurationItem()
			{
				Server = "S00S33",
				Table = "S00D02"
			};
			item.Properties = new ObservableCollection<ConfigurationProperty>();
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 0,
				IsKey = true,
				MessageField = "FELD-NAME"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 1,
				IsKey = true,
				MessageField = "FELD_WERT"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 2,
				IsKey = false,
				MessageField = "LUPD-TIMESTAMP"
			});

			Assert.AreEqual(expectedValue, Helpers.ArrangeDataFromRequest(item));
		}

		#endregion

		#region ArrangeDataForReply

		[TestMethod]
		public void WhenConfigurationItemPropertiesIsSet_ReturnsTheArrangedDataForReply()
		{
			string expectedValue = "    MOVE FELD-NAME                      OF S00D02-ROW" + "\r\n" +
								   "      TO FELD-NAME                      OF S00S33-RPL-1(WS-MR-ANZ-DS-AKTUELL)" + "\r\n" +
								   "    MOVE FELD-WERT                      OF S00D02-ROW" + "\r\n" +
								   "      TO FELD-WERT                      OF S00S33-RPL-1(WS-MR-ANZ-DS-AKTUELL)" + "\r\n" +
								   "    MOVE LUPD-TIMESTAMP                 OF S00D02-ROW" + "\r\n" +
								   "      TO LUPD-TIMESTAMP                 OF S00S33-RPL-1(WS-MR-ANZ-DS-AKTUELL)";

			ConfigurationItem item = new ConfigurationItem()
			{
				Server = "S00S33",
				Table = "S00D02"
			};
			item.Properties = new ObservableCollection<ConfigurationProperty>();
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 0,
				IsKey = true,
				MessageField = "FELD-NAME"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 1,
				IsKey = true,
				MessageField = "FELD_WERT"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 2,
				IsKey = false,
				MessageField = "LUPD-TIMESTAMP"
			});

			Assert.AreEqual(expectedValue, Helpers.ArrangeDataForReply(item));
		}

		#endregion

		#region ArrangeInsertFields

		[TestMethod]
		public void WhenConfigurationItemPropertiesIsSet_ReturnsTheArrangedInsertFields()
		{
			string expectedValue = "                   FELD_NAME" + "\r\n" +
								   "                 , FELD_WERT" + "\r\n" +
								   "                 , LUPD_TIMESTAMP";

			ConfigurationItem item = new ConfigurationItem();
			item.Properties = new ObservableCollection<ConfigurationProperty>();
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 0,
				MessageField = "FELD-NAME"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 1,
				MessageField = "FELD_WERT"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 2,
				MessageField = "LUPD-TIMESTAMP"
			});

			Assert.AreEqual(expectedValue, Helpers.ArrangeInsertFields(item));
		}

		#endregion

		#region ArrangeInsertValues

		[TestMethod]
		public void WhenConfigurationItemPropertiesIsSet_ReturnsTheArrangedInsertValues()
		{
			string expectedValue = "                  :FELD-NAME                         OF S00D02-ROW" + "\r\n" +
			                       "                 ,:FELD-WERT                         OF S00D02-ROW" + "\r\n" +
								   "                 ,:HV-LUPD-TIMESTAMP                 OF HV-S00S33" + "\r\n" +
								   "                   TYPE AS DATETIME YEAR TO FRACTION(3)" + "\r\n" +
								   "                 ,:HV-LUPD-TIMESTAMP                 OF HV-S00S33" + "\r\n" +
								   "                   TYPE AS DATETIME YEAR TO FRACTION(6)";

			ConfigurationItem item = new ConfigurationItem()
			{
				Server = "S00S33",
				Table = "S00D02"
			};
			item.Properties = new ObservableCollection<ConfigurationProperty>();
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 0,
				MessageField = "FELD-NAME"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 1,
				MessageField = "FELD_WERT"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 2,
				MessageField = "LUPD-TIMESTAMP",
				Length = "23"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 3,
				MessageField = "LUPD-TIMESTAMP-F6",
				Length = "26"
			});

			Assert.AreEqual(expectedValue, Helpers.ArrangeInsertValues(item));
		}

		#endregion

		#region ArrangeKeys

		[TestMethod]
		public void WhenConfigurationItemPropertiesIsSet_ReturnsTheArrangedKeys()
		{
			string expectedValue = "       MOVE FELD-NAME                         OF S00S33-REQ-2(I)" + "\r\n" +
			                       "         TO PARAMETERTEXT              OF EMS-MELD-LNK (2)" + "\r\n" +
			                       "       MOVE FELD-WERT                         OF S00S33-REQ-2(I)" + "\r\n" +
			                       "         TO PARAMETERTEXT              OF EMS-MELD-LNK (3)";

			ConfigurationItem item = new ConfigurationItem()
			{
				Server = "S00S33",
				Table = "S00D02"
			};
			item.Properties = new ObservableCollection<ConfigurationProperty>();
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 0,
				IsKey = true,
				MessageField = "FELD-NAME"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 1,
				IsKey = true,
				MessageField = "FELD_WERT"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 2,
				MessageField = "LUPD-TIMESTAMP",
				Length = "26"
			});

			Assert.AreEqual(expectedValue, Helpers.ArrangeKeys(item));
		}

		[TestMethod]
		public void WhenConfigurationItemPropertiesIsSetButTheNumberIsGreaterThanSeven_ReturnsTheOnlySevenArrangedKeys()
		{
			string expectedValue = "       MOVE FELD-NAME                         OF S00S33-REQ-2(I)" + "\r\n" +
			                       "         TO PARAMETERTEXT              OF EMS-MELD-LNK (2)" + "\r\n" +
			                       "       MOVE FELD-WERT                         OF S00S33-REQ-2(I)" + "\r\n" +
			                       "         TO PARAMETERTEXT              OF EMS-MELD-LNK (3)" + "\r\n" +
			                       "       MOVE FELD-NAME1                        OF S00S33-REQ-2(I)" + "\r\n" +
			                       "         TO PARAMETERTEXT              OF EMS-MELD-LNK (4)" + "\r\n" +
			                       "       MOVE FELD-WERT1                        OF S00S33-REQ-2(I)" + "\r\n" +
			                       "         TO PARAMETERTEXT              OF EMS-MELD-LNK (5)" + "\r\n" +
			                       "       MOVE FELD-NAME2                        OF S00S33-REQ-2(I)" + "\r\n" +
			                       "         TO PARAMETERTEXT              OF EMS-MELD-LNK (6)" + "\r\n" +
			                       "       MOVE FELD-WERT2                        OF S00S33-REQ-2(I)" + "\r\n" +
			                       "         TO PARAMETERTEXT              OF EMS-MELD-LNK (7)" + "\r\n" +
			                       "       MOVE FELD-NAME3                        OF S00S33-REQ-2(I)" + "\r\n" +
			                       "         TO PARAMETERTEXT              OF EMS-MELD-LNK (8)";

			ConfigurationItem item = new ConfigurationItem()
			{
				Server = "S00S33",
				Table = "S00D02"
			};
			item.Properties = new ObservableCollection<ConfigurationProperty>();
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 0,
				IsKey = true,
				MessageField = "FELD-NAME"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 1,
				IsKey = true,
				MessageField = "FELD_WERT"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 2,
				IsKey = true,
				MessageField = "FELD-NAME1"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 3,
				IsKey = true,
				MessageField = "FELD_WERT1"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 4,
				IsKey = true,
				MessageField = "FELD-NAME2"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 5,
				IsKey = true,
				MessageField = "FELD_WERT2"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 6,
				IsKey = true,
				MessageField = "FELD-NAME3"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 7,
				IsKey = true,
				MessageField = "FELD_WERT3"
			});

			Assert.AreEqual(expectedValue, Helpers.ArrangeKeys(item));
		}

		#endregion

		#region ArrangeFetchCursor

		[TestMethod]
		public void WhenConfigurationItemPropertiesIsSet_ReturnsTheArrangedFetchCursor()
		{
			string expectedValue = "                  :FELD-NAME                        OF S00D02-ROW" + "\r\n" +
			                       "                 ,:FELD-WERT                        OF S00D02-ROW" + "\r\n" +
								   "                 ,:LUPD-TIMESTAMP                   OF S00D02-ROW" + "\r\n" +
								   "                      TYPE AS DATETIME YEAR TO FRACTION(3)" + "\r\n" +
								   "                 ,:LUPD-TIMESTAMP-F6                OF S00D02-ROW" + "\r\n" +
			                       "                      TYPE AS DATETIME YEAR TO FRACTION(6)";

			ConfigurationItem item = new ConfigurationItem()
			{
				Server = "S00S33",
				Table = "S00D02"
			};
			item.Properties = new ObservableCollection<ConfigurationProperty>();
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 0,
				MessageField = "FELD-NAME"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 1,
				MessageField = "FELD_WERT"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 2,
				MessageField = "LUPD-TIMESTAMP",
				Length = "23"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 3,
				MessageField = "LUPD-TIMESTAMP-F6",
				Length = "26"
			});

			Assert.AreEqual(expectedValue, Helpers.ArrangeFetchCursor(item));
		}

		#endregion

		#region ArrangeUpdate

		[TestMethod]
		public void WhenConfigurationItemPropertiesIsSet_ReturnsTheArrangedUpdate()
		{
			string expectedValue = "                   BEZEICHNUNG" + "\r\n" +
			                       "                = :BEZEICHNUNG                      OF S00D02-ROW" + "\r\n" +
			                       "               ,   LUPD_TIMESTAMP" + "\r\n" +
			                       "                = :HV-LUPD-TIMESTAMP                OF HV-S00S33" + "\r\n" +
			                       "                   TYPE AS DATETIME YEAR TO FRACTION(3)" + "\r\n" +
								   "               ,   LUPD_TIMESTAMP_F6" + "\r\n" +
			                       "                = :HV-LUPD-TIMESTAMP                OF HV-S00S33" + "\r\n" +
			                       "                   TYPE AS DATETIME YEAR TO FRACTION(6)" + "\r\n" +
			                       "         WHERE     FELD_NAME" + "\r\n" +
			                       "                = :FELD-NAME                        OF S00D02-ROW" + "\r\n" +
			                       "         AND       FELD_WERT" + "\r\n" +
			                       "                = :FELD-WERT                        OF S00D02-ROW" + "\r\n";

			ConfigurationItem item = new ConfigurationItem()
			{
				Server = "S00S33",
				Table = "S00D02"
			};
			item.Properties = new ObservableCollection<ConfigurationProperty>();
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 0,
				IsKey = true,
				MessageField = "FELD-NAME"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 1,
				IsKey = true,
				MessageField = "FELD_WERT"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 2,
				MessageField = "BEZEICHNUNG"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 3,
				MessageField = "LUPD-TIMESTAMP",
				Length = "23"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 4,
				MessageField = "LUPD-TIMESTAMP-F6",
				Length = "26"
			});

			Assert.AreEqual(expectedValue, Helpers.ArrangeUpdate(item));
		}

		#endregion

		#region ArrangeLupdTimestampRpl

		[TestMethod]
		public void WhenConfigurationItemPropertiesIsSet_ReturnsTheArrangedLupdTimestampRpl()
		{
			string expectedValue = "TO LUPD-TIMESTAMP          OF S00D02-TAB OF S00S33-RPL-2(I)";

			ConfigurationItem item = new ConfigurationItem()
			{
				Server = "S00S33",
				Table = "S00D02"
			};
			item.Properties = new ObservableCollection<ConfigurationProperty>();
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 0,
				IsKey = true,
				MessageField = "FELD-NAME"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 1,
				IsKey = true,
				MessageField = "FELD_WERT"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 2,
				MessageField = "BEZEICHNUNG"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 3,
				MessageField = "LUPD-TIMESTAMP",
				Length = "23"
			});

			Assert.AreEqual(expectedValue, Helpers.ArrangeLupdTimestampRpl(item));
		}

		[TestMethod]
		public void WhenConfigurationItemPropertiesDontContainsLupdTimestamp_ReturnsEmptyString()
		{
			string expectedValue = string.Empty;

			ConfigurationItem item = new ConfigurationItem()
			{
				Server = "S00S33",
				Table = "S00D02"
			};
			item.Properties = new ObservableCollection<ConfigurationProperty>();
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 0,
				IsKey = true,
				MessageField = "FELD-NAME"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 1,
				IsKey = true,
				MessageField = "FELD_WERT"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 2,
				MessageField = "BEZEICHNUNG"
			});

			Assert.AreEqual(expectedValue, Helpers.ArrangeLupdTimestampRpl(item));
		}

		[TestMethod]
		public void WhenConfigurationItemPropertiesContainsTwoLupdTimestamp_ReturnsTheArrangedLupdTimestampRplOfTheLastLupdTimestamp()
		{
			string expectedValue = string.Empty;

			ConfigurationItem item = new ConfigurationItem()
			{
				Server = "S00S33",
				Table = "S00D02"
			};
			item.Properties = new ObservableCollection<ConfigurationProperty>();
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 0,
				IsKey = true,
				MessageField = "FELD-NAME"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 1,
				IsKey = true,
				MessageField = "FELD_WERT"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Order = 2,
				MessageField = "BEZEICHNUNG"
			});

			Assert.AreEqual(expectedValue, Helpers.ArrangeLupdTimestampRpl(item));
		}

		#endregion

		#endregion Server
	}
}
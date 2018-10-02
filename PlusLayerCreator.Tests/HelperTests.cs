using System;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlusLayerCreator.Items;

namespace PlusLayerCreator.Tests
{
	[TestClass]
	public class HelperTests
	{
		#region Server

		#region CamelCase

		[TestMethod]
		public void WhenStringSetToBeCamelCase_ItShouldBeFormattedAsCamelCase()
		{
			string testValue = "Das-ist_ein-Test";
			string expectedValue = "DasIstEinTest";
			Assert.AreEqual(testValue.ToCamelCase(), expectedValue);
		}

		[TestMethod]
		public void WhenStringIsNullOrLessThanTwoChars_GivenValueWillBeReturned()
		{
			string test = null;
			Assert.AreEqual(test.ToCamelCase(), null);
			Assert.AreEqual("".ToCamelCase(), "");
			Assert.AreEqual("A".ToCamelCase(), "A");
		}

		#endregion

		#region ToSqlName

		[TestMethod]
		public void WhenStringSetToBeSqlName_ItShouldBeFormattedAsSqlName()
		{
			string testValue = "FELD-WERT";
			string expectedValue = "FELD_WERT";
			Assert.AreEqual(testValue.ToSqlName(), expectedValue);
		}

		#endregion

		#region ToVariableName

		[TestMethod]
		public void WhenStringSetToBeVariableName_ItShouldBeFormattedAsVariableName()
		{
			string testValue = "FELD_WERT";
			string expectedValue = "FELD-WERT";
			Assert.AreEqual(testValue.ToVariableName(), expectedValue);
		}

		#endregion

		#region GetYearToFractionDigits

		[TestMethod]
		public void WhenConfigurationPropertyLenghtIsNumber_ReturnsTheConvertedNumber()
		{
			ConfigurationProperty property = new ConfigurationProperty()
			{
				Length = "23"
			};
			Assert.AreEqual(Helpers.GetYearToFractionDigits(property), 3);
		}

		[TestMethod]
		public void WhenConfigurationPropertyIsNull_ReturnsMinusOne()
		{
			ConfigurationProperty property = null;
			Assert.AreEqual(Helpers.GetYearToFractionDigits(property), -1);
		}

		[TestMethod]
		public void WhenConfigurationPropertyLenghtIsNotANumber_ReturnsMinusOne()
		{
			ConfigurationProperty property = new ConfigurationProperty()
			{
				Length = "abxc"
			};

			Assert.AreEqual(Helpers.GetYearToFractionDigits(property), -1);
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
				Length = "10"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Length = "24"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Length = "15"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				Length = "60"
			});

			//no modify property in request
			Assert.AreEqual(Helpers.CalculateServerRows(item, false), 275);

			//modify property in request
			Assert.AreEqual(Helpers.CalculateServerRows(item, true), 272);
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
				MessageField = "FELD-NAME",
				MessageDataType = "string"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				MessageField = "FELD_WERT",
				MessageDataType = "string"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				MessageField = "LUPD-TIMESTAMP",
				MessageDataType = "DateTime"
			});

			Assert.AreEqual(Helpers.ArrangeProperties(item), expectedValue);
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
				IsKey = true,
				MessageField = "FELD-NAME",
				MessageDataType = "string"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				IsKey = true,
				MessageField = "FELD_WERT",
				MessageDataType = "string"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				IsKey = false,
				MessageField = "LUPD-TIMESTAMP",
				MessageDataType = "DateTime"
			});

			Assert.AreEqual(Helpers.ArrangeKeyProperties(item), expectedValue);
		}

		[TestMethod]
		public void WhenConfigurationItemPropertiesIsSetButTheNumberIsGreaterThanSeven_ReturnsTheOnlySevenArrangedKeyProperties()
		{
			string expectedValue = "##1/##2/##3/##4/##5/##6/##7";

			ConfigurationItem item = new ConfigurationItem();
			item.Properties = new ObservableCollection<ConfigurationProperty>();
			item.Properties.Add(new ConfigurationProperty()
			{
				IsKey = true,
				MessageField = "FELD-NAME",
				MessageDataType = "string"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				IsKey = true,
				MessageField = "FELD_WERT",
				MessageDataType = "string"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				IsKey = true,
				MessageField = "FELD-NAME1",
				MessageDataType = "string"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				IsKey = true,
				MessageField = "FELD_WERT1",
				MessageDataType = "string"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				IsKey = true,
				MessageField = "FELD-NAME2",
				MessageDataType = "string"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				IsKey = true,
				MessageField = "FELD_WERT2",
				MessageDataType = "string"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				IsKey = true,
				MessageField = "FELD-NAME3",
				MessageDataType = "string"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
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
				IsKey = true,
				MessageField = "FELD-NAME"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				IsKey = true,
				MessageField = "FELD_WERT"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				IsKey = false,
				MessageField = "LUPD-TIMESTAMP"
			});

			Assert.AreEqual(Helpers.ArrangeDelete(item), expectedValue);
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
				IsKey = true,
				MessageField = "FELD-NAME"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				IsKey = true,
				MessageField = "FELD_WERT"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				IsKey = false,
				MessageField = "LUPD-TIMESTAMP"
			});

			Assert.AreEqual(Helpers.ArrangeCursorFields(item), expectedValue);
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
				IsKey = true,
				MessageField = "FELD-NAME"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				IsKey = true,
				MessageField = "FELD_WERT"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				IsKey = false,
				MessageField = "LUPD-TIMESTAMP"
			});

			Assert.AreEqual(Helpers.ArrangeDataFromRequest(item), expectedValue);
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
				IsKey = true,
				MessageField = "FELD-NAME"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				IsKey = true,
				MessageField = "FELD_WERT"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				IsKey = false,
				MessageField = "LUPD-TIMESTAMP"
			});

			Assert.AreEqual(Helpers.ArrangeDataForReply(item), expectedValue);
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
				MessageField = "FELD-NAME"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				MessageField = "FELD_WERT"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				MessageField = "LUPD-TIMESTAMP"
			});

			Assert.AreEqual(Helpers.ArrangeInsertFields(item), expectedValue);
		}

		#endregion

		#region ArrangeInsertValues

		[TestMethod]
		public void WhenConfigurationItemPropertiesIsSet_ReturnsTheArrangedInsertValues()
		{
			string expectedValue = "                  :FELD-NAME                         OF S00D02-ROW" + "\r\n" +
			                       "                 ,:FELD-WERT                         OF S00D02-ROW" + "\r\n" +
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
				MessageField = "FELD-NAME"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				MessageField = "FELD_WERT"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				MessageField = "LUPD-TIMESTAMP",
				Length = "26"
			});

			Assert.AreEqual(Helpers.ArrangeInsertValues(item), expectedValue);
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
				IsKey = true,
				MessageField = "FELD-NAME"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				IsKey = true,
				MessageField = "FELD_WERT"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				MessageField = "LUPD-TIMESTAMP",
				Length = "26"
			});

			Assert.AreEqual(Helpers.ArrangeKeys(item), expectedValue);
		}

		[TestMethod]
		public void WhenConfigurationItemPropertiesIsSetButTheNumberIsGreaterThanSeven_ReturnsTheOnlySevenArrangedKeys()
		{
			string expectedValue = "       MOVE FELD-NAME                         OF S00S33-REQ-2(I)" + "\r\n" +
			                       "         TO PARAMETERTEXT              OF EMS-MELD-LNK (1)" + "\r\n" +
			                       "       MOVE FELD-WERT                         OF S00S33-REQ-2(I)" + "\r\n" +
			                       "         TO PARAMETERTEXT              OF EMS-MELD-LNK (2)" + "\r\n" +
			                       "       MOVE FELD-NAME1                        OF S00S33-REQ-2(I)" + "\r\n" +
			                       "         TO PARAMETERTEXT              OF EMS-MELD-LNK (3)" + "\r\n" +
			                       "       MOVE FELD-WERT1                        OF S00S33-REQ-2(I)" + "\r\n" +
			                       "         TO PARAMETERTEXT              OF EMS-MELD-LNK (4)" + "\r\n" +
			                       "       MOVE FELD-NAME2                        OF S00S33-REQ-2(I)" + "\r\n" +
			                       "         TO PARAMETERTEXT              OF EMS-MELD-LNK (5)" + "\r\n" +
			                       "       MOVE FELD-WERT2                        OF S00S33-REQ-2(I)" + "\r\n" +
			                       "         TO PARAMETERTEXT              OF EMS-MELD-LNK (6)" + "\r\n" +
			                       "       MOVE FELD-NAME3                        OF S00S33-REQ-2(I)" + "\r\n" +
			                       "         TO PARAMETERTEXT              OF EMS-MELD-LNK (7)";

			ConfigurationItem item = new ConfigurationItem()
			{
				Server = "S00S33",
				Table = "S00D02"
			};
			item.Properties = new ObservableCollection<ConfigurationProperty>();
			item.Properties.Add(new ConfigurationProperty()
			{
				IsKey = true,
				MessageField = "FELD-NAME"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				IsKey = true,
				MessageField = "FELD_WERT"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				IsKey = true,
				MessageField = "FELD-NAME1"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				IsKey = true,
				MessageField = "FELD_WERT1"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				IsKey = true,
				MessageField = "FELD-NAME2"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				IsKey = true,
				MessageField = "FELD_WERT2"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				IsKey = true,
				MessageField = "FELD-NAME3"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				IsKey = true,
				MessageField = "FELD_WERT3"
			});

			Assert.AreEqual(Helpers.ArrangeKeys(item), expectedValue);
		}

		#endregion

		#region ArrangeFetchCursor

		[TestMethod]
		public void WhenConfigurationItemPropertiesIsSet_ReturnsTheArrangedFetchCursor()
		{
			string expectedValue = "                  :FELD-NAME                        OF S00D02-ROW" + "\r\n" +
			                       "                 ,:FELD-WERT                        OF S00D02-ROW" + "\r\n" +
								   "                 ,:LUPD-TIMESTAMP                   OF S00D02-ROW" + "\r\n" +
			                       "                      TYPE AS DATETIME YEAR TO FRACTION(6)";

			ConfigurationItem item = new ConfigurationItem()
			{
				Server = "S00S33",
				Table = "S00D02"
			};
			item.Properties = new ObservableCollection<ConfigurationProperty>();
			item.Properties.Add(new ConfigurationProperty()
			{
				MessageField = "FELD-NAME"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				MessageField = "FELD_WERT"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				MessageField = "LUPD-TIMESTAMP",
				Length = "26"
			});

			Assert.AreEqual(Helpers.ArrangeFetchCursor(item), expectedValue);
		}

		#endregion

		#region ArrangeUpdate

		[TestMethod]
		public void WhenConfigurationItemPropertiesIsSet_ReturnsTheArrangedUpdate()
		{
			string expectedValue = "                   BEZEICHNUNG" + "\r\n" +
			                       "                = :BEZEICHNUNG                      OF S00D02-ROW" + "\r\n" +
			                       "               ,  :LUPD_TIMESTAMP" + "\r\n" +
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
				IsKey = true,
				MessageField = "FELD-NAME"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				IsKey = true,
				MessageField = "FELD_WERT"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				MessageField = "BEZEICHNUNG"
			});
			item.Properties.Add(new ConfigurationProperty()
			{
				MessageField = "LUPD-TIMESTAMP",
				Length = "26"
			});

			Assert.AreEqual(expectedValue, Helpers.ArrangeUpdate(item));
		}

		#endregion

		#endregion Server
	}
}
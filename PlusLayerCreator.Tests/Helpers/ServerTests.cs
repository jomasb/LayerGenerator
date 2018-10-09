using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlusLayerCreator.Items;

namespace PlusLayerCreator.Tests.Helpers
{
	[TestClass]
	public class ServerTests
	{
		private ConfigurationItem _item;

		[TestInitialize]
		public void Initialize()
		{
			_item = new ConfigurationItem()
			{
				Server = "S00S29",
				Table = "S00DX1",
				Properties = new ObservableCollection<ConfigurationProperty>()
				{
					new ConfigurationProperty()
					{
						Order = 0,
						IsKey = true,
						IsRequired = true,
						MessageField = "ID-TYP",
						MessageDataType = "*",
						Type = "string",
						Length = "12"
					},
					new ConfigurationProperty()
					{
						Order = 1,
						IsKey = true,
						IsRequired = true,
						MessageField = "ID-WERT",
						MessageDataType = "*",
						Type = "string",
						Length = "48"
					},
					new ConfigurationProperty()
					{
						Order = 2,
						IsKey = true,
						IsRequired = true,
						MessageField = "WERK",
						MessageDataType = "*",
						Type = "string",
						Length = "4"
					},
					new ConfigurationProperty()
					{
						Order = 3,
						IsKey = true,
						IsRequired = true,
						MessageField = "TERMIN",
						MessageDataType = "LUPD-TIMESTAMP-F6",
						Type = "DateTime",
						Length = "26"
					},
					new ConfigurationProperty()
					{
						Order = 4,
						IsKey = false,
						IsRequired = false,
						MessageField = "UTC_DATETIME",
						MessageDataType = "*",
						Type = "string",
						Length = "32"
					},
					new ConfigurationProperty()
					{
						Order = 5,
						IsKey = false,
						IsRequired = false,
						MessageField = "LUPD_TIMESTAMP_F6",
						MessageDataType = "*",
						Type = "DateTime",
						Length = "26"
					},
					new ConfigurationProperty()
					{
						Order = 6,
						IsKey = false,
						IsRequired = false,
						MessageField = "LUPD_PROZESSNAME",
						MessageDataType = "*",
						Type = "string",
						Length = "6"
					}
				}
			};
			
		}

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
			Assert.AreEqual(3, PlusLayerCreator.Helpers.GetYearToFractionDigits(property));
		}

		[TestMethod]
		public void WhenConfigurationPropertyIsNull_ReturnsMinusOne()
		{
			ConfigurationProperty property = null;
			Assert.AreEqual(-1, PlusLayerCreator.Helpers.GetYearToFractionDigits(property));
		}

		[TestMethod]
		public void WhenConfigurationPropertyLenghtIsNotANumber_ReturnsMinusOne()
		{
			ConfigurationProperty property = new ConfigurationProperty()
			{
				Order = 0,
				Length = "abxc"
			};

			Assert.AreEqual(-1, PlusLayerCreator.Helpers.GetYearToFractionDigits(property));
		}

		#endregion

		#region CalculateServerRows

		[TestMethod]
		public void WhenConfigurationItemPropertiesLenghtIsSet_ReturnsTheCalculatedRows()
		{
			//no modify property in request
			Assert.AreEqual(194, PlusLayerCreator.Helpers.CalculateServerRows(_item, false));

			//modify property in request
			Assert.AreEqual(193, PlusLayerCreator.Helpers.CalculateServerRows(_item, true));
		}

		#endregion

		#region ArrangeProperties

		[TestMethod]
		public void WhenConfigurationItemPropertiesIsSet_ReturnsTheArrangedProperties()
		{
			string expectedValue = "  02 ID-TYP                          TYPE *.\r\n" +
			                       "  02 ID-WERT                         TYPE *.\r\n" +
			                       "  02 WERK                            TYPE *.\r\n" +
								   "  02 TERMIN                          TYPE LUPD-TIMESTAMP-F6.\r\n" +
			                       "  02 UTC-DATETIME                    TYPE *.\r\n" +
			                       "  02 LUPD-TIMESTAMP-F6               TYPE *.\r\n" +
			                       "  02 LUPD-PROZESSNAME                TYPE *.";

			Assert.AreEqual(expectedValue, PlusLayerCreator.Helpers.ArrangeProperties(_item));
		}

		#endregion

		#region ArrangeKeyProperties

		[TestMethod]
		public void WhenConfigurationItemPropertiesIsSet_ReturnsTheArrangedKeyProperties()
		{
			string expectedValue = "##1/##2/##3/##4";

			Assert.AreEqual(expectedValue, PlusLayerCreator.Helpers.ArrangeKeyProperties(_item));
		}

		[TestMethod]
		public void
			WhenConfigurationItemPropertiesIsSetButTheNumberIsGreaterThanSeven_ReturnsTheOnlySevenArrangedKeyProperties()
		{
			string expectedValue = "##1/##2/##3/##4/##5/##6/##7";

			_item.Properties.Add(new ConfigurationProperty()
			{
				Order = 4,
				IsKey = true,
				MessageField = "FELD-NAME2",
				Type = "string"
			});
			_item.Properties.Add(new ConfigurationProperty()
			{
				Order = 5,
				IsKey = true,
				MessageField = "FELD_WERT2",
				Type = "string"
			});
			_item.Properties.Add(new ConfigurationProperty()
			{
				Order = 6,
				IsKey = true,
				MessageField = "FELD-NAME3",
				Type = "string"
			});
			_item.Properties.Add(new ConfigurationProperty()
			{
				Order = 7,
				IsKey = true,
				MessageField = "FELD_WERT3",
				Type = "string"
			});

			Assert.AreEqual(PlusLayerCreator.Helpers.ArrangeKeyProperties(_item), expectedValue);
		}

		#endregion

		#region ArrangeDelete

		[TestMethod]
		public void WhenConfigurationItemPropertiesIsSet_ReturnsTheArrangedDelete()
		{
			string expectedValue = "                   ID_TYP\r\n" +
			                       "                = :ID-TYP                            OF S00DX1-ROW\r\n" +
			                       "         AND       ID_WERT\r\n" +
			                       "                = :ID-WERT                           OF S00DX1-ROW\r\n" +
			                       "         AND       WERK\r\n" +
			                       "                = :WERK                              OF S00DX1-ROW\r\n" +
			                       "         AND       TERMIN\r\n" +
			                       "                = :TERMIN                            OF S00DX1-ROW";

			Assert.AreEqual(expectedValue, PlusLayerCreator.Helpers.ArrangeDelete(_item));
		}

		#endregion

		#region ArrangeCursorFields

		[TestMethod]
		public void WhenConfigurationItemPropertiesIsSet_ReturnsTheArrangedCursorFields()
		{
			string expectedValue = "                   ID_TYP\r\n" +
			                       "                 , ID_WERT\r\n" +
			                       "                 , WERK\r\n" +
			                       "                 , TERMIN\r\n" +
			                       "                 , UTC_DATETIME\r\n" +
			                       "                 , LUPD_TIMESTAMP_F6\r\n" +
			                       "                 , LUPD_PROZESSNAME";

			Assert.AreEqual(expectedValue, PlusLayerCreator.Helpers.ArrangeCursorFields(_item));
		}

		#endregion

		#region ArrangeDataFromRequest

		[TestMethod]
		public void WhenConfigurationItemPropertiesIsSet_ReturnsTheArrangedDataFromRequest()
		{
			string expectedValue = "    MOVE ID-TYP                         OF S00S29-REQ-2(I)\r\n" +
			                       "      TO ID-TYP                         OF S00DX1-ROW\r\n" +
			                       "    MOVE ID-WERT                        OF S00S29-REQ-2(I)\r\n" +
			                       "      TO ID-WERT                        OF S00DX1-ROW\r\n" +
			                       "    MOVE WERK                           OF S00S29-REQ-2(I)\r\n" +
			                       "      TO WERK                           OF S00DX1-ROW\r\n" +
			                       "    MOVE TERMIN                         OF S00S29-REQ-2(I)\r\n" +
			                       "      TO TERMIN                         OF S00DX1-ROW\r\n" +
			                       "    MOVE UTC-DATETIME                   OF S00S29-REQ-2(I)\r\n" +
			                       "      TO UTC-DATETIME                   OF S00DX1-ROW\r\n" +
			                       "    MOVE LUPD-TIMESTAMP-F6              OF S00S29-REQ-2(I)\r\n" +
			                       "      TO LUPD-TIMESTAMP-F6              OF S00DX1-ROW\r\n" +
			                       "    MOVE LUPD-PROZESSNAME               OF S00S29-REQ-2(I)\r\n" +
			                       "      TO LUPD-PROZESSNAME               OF S00DX1-ROW";

			Assert.AreEqual(expectedValue, PlusLayerCreator.Helpers.ArrangeDataFromRequest(_item));
		}

		#endregion

		#region ArrangeDataForReply

		[TestMethod]
		public void WhenConfigurationItemPropertiesIsSet_ReturnsTheArrangedDataForReply()
		{
			string expectedValue = "    MOVE ID-TYP                         OF S00DX1-ROW\r\n" +
			                       "      TO ID-TYP                         OF S00S29-RPL-1(WS-MR-ANZ-DS-AKTUELL)\r\n" +
			                       "    MOVE ID-WERT                        OF S00DX1-ROW\r\n" +
			                       "      TO ID-WERT                        OF S00S29-RPL-1(WS-MR-ANZ-DS-AKTUELL)\r\n" +
			                       "    MOVE WERK                           OF S00DX1-ROW\r\n" +
			                       "      TO WERK                           OF S00S29-RPL-1(WS-MR-ANZ-DS-AKTUELL)\r\n" +
			                       "    MOVE TERMIN                         OF S00DX1-ROW\r\n" +
			                       "      TO TERMIN                         OF S00S29-RPL-1(WS-MR-ANZ-DS-AKTUELL)\r\n" +
			                       "    MOVE UTC-DATETIME                   OF S00DX1-ROW\r\n" +
			                       "      TO UTC-DATETIME                   OF S00S29-RPL-1(WS-MR-ANZ-DS-AKTUELL)\r\n" +
			                       "    MOVE LUPD-TIMESTAMP-F6              OF S00DX1-ROW\r\n" +
			                       "      TO LUPD-TIMESTAMP-F6              OF S00S29-RPL-1(WS-MR-ANZ-DS-AKTUELL)\r\n" +
			                       "    MOVE LUPD-PROZESSNAME               OF S00DX1-ROW\r\n" +
			                       "      TO LUPD-PROZESSNAME               OF S00S29-RPL-1(WS-MR-ANZ-DS-AKTUELL)";

			Assert.AreEqual(expectedValue, PlusLayerCreator.Helpers.ArrangeDataForReply(_item));
		}

		#endregion

		#region ArrangeInsertFields

		[TestMethod]
		public void WhenConfigurationItemPropertiesIsSet_ReturnsTheArrangedInsertFields()
		{
			string expectedValue = "                   ID_TYP\r\n" +
			                       "                 , ID_WERT\r\n" +
			                       "                 , WERK\r\n" +
			                       "                 , TERMIN\r\n" +
			                       "                 , UTC_DATETIME\r\n" +
			                       "                 , LUPD_TIMESTAMP_F6\r\n" +
			                       "                 , LUPD_PROZESSNAME";

			Assert.AreEqual(expectedValue, PlusLayerCreator.Helpers.ArrangeInsertFields(_item));
		}

		#endregion

		#region ArrangeInsertValues

		[TestMethod]
		public void WhenConfigurationItemPropertiesIsSet_ReturnsTheArrangedInsertValues()
		{
			string expectedValue = "                  :ID-TYP                            OF S00DX1-ROW\r\n" +
			                       "                 ,:ID-WERT                           OF S00DX1-ROW\r\n" +
			                       "                 ,:WERK                              OF S00DX1-ROW\r\n" +
			                       "                 ,:TERMIN                            OF S00DX1-ROW\r\n" +
			                       "                   TYPE AS DATETIME YEAR TO FRACTION(6)\r\n" +
			                       "                 ,:UTC-DATETIME                      OF S00DX1-ROW\r\n" +
			                       "                 ,:HV-LUPD-TIMESTAMP                 OF HV-S00S29\r\n" +
			                       "                   TYPE AS DATETIME YEAR TO FRACTION(6)\r\n" +
			                       "                 ,:LUPD-PROZESSNAME                  OF S00DX1-ROW";

			Assert.AreEqual(expectedValue, PlusLayerCreator.Helpers.ArrangeInsertValues(_item));
		}

		#endregion

		#region ArrangeKeys

		[TestMethod]
		public void WhenConfigurationItemPropertiesIsSet_ReturnsTheArrangedKeys()
		{
			string expectedValue = "       MOVE ID-TYP                            OF S00S29-REQ-2(I)\r\n" +
			                       "         TO PARAMETERTEXT              OF EMS-MELD-LNK (2)\r\n" +
			                       "       MOVE ID-WERT                           OF S00S29-REQ-2(I)\r\n" +
			                       "         TO PARAMETERTEXT              OF EMS-MELD-LNK (3)\r\n" +
			                       "       MOVE WERK                              OF S00S29-REQ-2(I)\r\n" +
			                       "         TO PARAMETERTEXT              OF EMS-MELD-LNK (4)\r\n" +
			                       "       MOVE TERMIN                            OF S00S29-REQ-2(I)\r\n" +
			                       "         TO PARAMETERTEXT              OF EMS-MELD-LNK (5)";

			Assert.AreEqual(expectedValue, PlusLayerCreator.Helpers.ArrangeKeys(_item));
		}

		[TestMethod]
		public void WhenConfigurationItemPropertiesIsSetButTheNumberIsGreaterThanSeven_ReturnsTheOnlySevenArrangedKeys()
		{
			string expectedValue = "       MOVE ID-TYP                            OF S00S29-REQ-2(I)\r\n" +
			                       "         TO PARAMETERTEXT              OF EMS-MELD-LNK (2)\r\n" +
			                       "       MOVE ID-WERT                           OF S00S29-REQ-2(I)\r\n" +
			                       "         TO PARAMETERTEXT              OF EMS-MELD-LNK (3)\r\n" +
			                       "       MOVE WERK                              OF S00S29-REQ-2(I)\r\n" +
			                       "         TO PARAMETERTEXT              OF EMS-MELD-LNK (4)\r\n" +
			                       "       MOVE TERMIN                            OF S00S29-REQ-2(I)\r\n" +
			                       "         TO PARAMETERTEXT              OF EMS-MELD-LNK (5)\r\n" +
			                       "       MOVE FELD-NAME2                        OF S00S29-REQ-2(I)\r\n" +
			                       "         TO PARAMETERTEXT              OF EMS-MELD-LNK (6)\r\n" +
			                       "       MOVE FELD-WERT2                        OF S00S29-REQ-2(I)\r\n" +
			                       "         TO PARAMETERTEXT              OF EMS-MELD-LNK (7)\r\n" +
			                       "       MOVE FELD-NAME3                        OF S00S29-REQ-2(I)\r\n" +
			                       "         TO PARAMETERTEXT              OF EMS-MELD-LNK (8)";

			_item.Properties.Add(new ConfigurationProperty()
			{
				Order = 4,
				IsKey = true,
				MessageField = "FELD-NAME2"
			});
			_item.Properties.Add(new ConfigurationProperty()
			{
				Order = 5,
				IsKey = true,
				MessageField = "FELD_WERT2"
			});
			_item.Properties.Add(new ConfigurationProperty()
			{
				Order = 6,
				IsKey = true,
				MessageField = "FELD-NAME3"
			});
			_item.Properties.Add(new ConfigurationProperty()
			{
				Order = 7,
				IsKey = true,
				MessageField = "FELD_WERT3"
			});

			Assert.AreEqual(expectedValue, PlusLayerCreator.Helpers.ArrangeKeys(_item));
		}

		#endregion

		#region ArrangeFetchCursor

		[TestMethod]
		public void WhenConfigurationItemPropertiesIsSet_ReturnsTheArrangedFetchCursor()
		{
			string expectedValue = "                  :ID-TYP                           OF S00DX1-ROW\r\n" +
			                       "                 ,:ID-WERT                          OF S00DX1-ROW\r\n" +
			                       "                 ,:WERK                             OF S00DX1-ROW\r\n" +
			                       "                 ,:TERMIN                           OF S00DX1-ROW\r\n" +
			                       "                      TYPE AS DATETIME YEAR TO FRACTION(6)\r\n" +
								   "                 ,:UTC-DATETIME                     OF S00DX1-ROW\r\n" +
			                       "                 ,:LUPD-TIMESTAMP-F6                OF S00DX1-ROW\r\n" +
			                       "                      TYPE AS DATETIME YEAR TO FRACTION(6)\r\n" +
			                       "                 ,:LUPD-PROZESSNAME                 OF S00DX1-ROW";

			Assert.AreEqual(expectedValue, PlusLayerCreator.Helpers.ArrangeFetchCursor(_item));
		}

		#endregion

		#region ArrangeUpdate

		[TestMethod]
		public void WhenConfigurationItemPropertiesIsSet_ReturnsTheArrangedUpdate()
		{
			string expectedValue = "                   UTC_DATETIME\r\n" +
			                       "                = :UTC-DATETIME                     OF S00DX1-ROW\r\n" +
			                       "               ,   LUPD_TIMESTAMP_F6\r\n" +
			                       "                = :HV-LUPD-TIMESTAMP                OF HV-S00S29\r\n" +
			                       "                   TYPE AS DATETIME YEAR TO FRACTION(6)\r\n" +
			                       "               ,   LUPD_PROZESSNAME\r\n" +
			                       "                = :LUPD-PROZESSNAME                 OF S00DX1-ROW\r\n" +
			                       "         WHERE     ID_TYP\r\n" +
			                       "                = :ID-TYP                           OF S00DX1-ROW\r\n" +
			                       "         AND       ID_WERT\r\n" +
			                       "                = :ID-WERT                          OF S00DX1-ROW\r\n" +
			                       "         AND       WERK\r\n" +
			                       "                = :WERK                             OF S00DX1-ROW\r\n" +
			                       "         AND       TERMIN\r\n" +
			                       "                = :TERMIN                           OF S00DX1-ROW\r\n" +
			                       "                   TYPE AS DATETIME YEAR TO FRACTION(6)\r\n";


			Assert.AreEqual(expectedValue, PlusLayerCreator.Helpers.ArrangeUpdate(_item));
		}

		#endregion

		#region ArrangeLupdTimestampRpl

		[TestMethod]
		public void WhenConfigurationItemPropertiesIsSet_ReturnsTheArrangedLupdTimestampRpl()
		{
			string expectedValue = "TO LUPD_TIMESTAMP_F6       OF S00DX1-TAB OF S00S29-RPL-2(I)";

			Assert.AreEqual(expectedValue, PlusLayerCreator.Helpers.ArrangeLupdTimestampRpl(_item));
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

			Assert.AreEqual(expectedValue, PlusLayerCreator.Helpers.ArrangeLupdTimestampRpl(item));
		}

		[TestMethod]
		public void
			WhenConfigurationItemPropertiesContainsTwoLupdTimestamp_ReturnsTheArrangedLupdTimestampRplOfTheLastLupdTimestamp()
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

			Assert.AreEqual(expectedValue, PlusLayerCreator.Helpers.ArrangeLupdTimestampRpl(item));
		}

		#endregion
	}
}
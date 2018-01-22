using System;
using System.Collections.Generic;
using System.Linq;
using Dcx.Plus.BusinessServiceLocal.Modules.$Product$.Contracts;
using Dcx.Plus.Data.Tandem.Adapter;
using Dcx.Plus.Data.Tandem.Modules.$Product$;
using Dcx.Plus.Data.Tandem.Utilities.Format;
using Dcx.Plus.Gateway.FW.Contracts;
using Dcx.Plus.BusinessServiceLocal.Modules.$Product$.Contracts.DTOs;

namespace Dcx.Plus.BusinessServiceLocal.Modules.$Product$.Tandem
{
	/// <summary>
	/// Implements the converter of the $item$s.
	/// This converter should be used in a service that works without Business Objects!!!.
	/// </summary>
	/// <author></author>
	/// <company>abat+ GmbH</company>
	/// <date></date>
	public class $Product$$Item$Converter : Converter<I$Product$$Item$Service>, I<YourServer><YourRequest>BusinessObject, I<YourServer><YourReply>BusinessObject
	{
		#region I<YourServer><YourRequest>BusinessObject Members
		/// <summary>
		/// Convert the $Product$$Item$List business data to message <YourServer><YourRequest> data
		/// </summary>
		public void ConvertBusinessDataToMessageData<YourServer><YourRequest>(<YourServer><YourRequest> serviceMessage)
		{
			var transcode = serviceMessage.Transcode();
			if (!transcode.Equals(<YourServer><YourRequest>Transcode.RD) &&
				!transcode.Equals(<YourServer><YourRequest>Transcode.MR))
			{
				return;
			}

			if (transcode.Equals(<YourServer><YourRequest>Transcode.MR))
			{
				serviceMessage.MaxLupdTimestamp<YourTable>(PlusFormat.FormatTandemTimestamp26(DataProvider.MaxLupdTimestamp));

				IList<$Product$$Item$> $item$s = Arguments["$Item$sToSave"] as IList<$Product$$Item$>;

				int offset = (int)Arguments["$Item$sToSaveOffset"];

				int refCount = $item$s.Count;
				serviceMessage.RefTabAnz(refCount);

				int index = offset;

				for (; index < refCount && (index - offset) < <YourServer><YourRequest>.RefTab_MaxTimes;
					index++)
				{
					var $item$ = $item$s.ElementAt(index);
					Fill$Item$(serviceMessage, $item$, index);
					switch ($item$.SaveFlag)
					{
						case SaveFlag.Delete:
							serviceMessage.BearbeitungsKz("D", index);
							break;
						case SaveFlag.Modified:
							serviceMessage.BearbeitungsKz("U", index);
							break;
						case SaveFlag.New:
							serviceMessage.BearbeitungsKz("I", index);
							break;
					}
				}

				Arguments["$Item$sToSaveOffset"] = index;
			}
		}

		#endregion I<YourServer><YourRequest>BusinessObject Members

		#region I<YourServer><YourReply>BusinessObject Members

		/// <summary>
		/// Convert the message  <YourServer><YourReply> data to $Product$$Item$List business data
		///</summary>
		public void ConvertMessageDataToBusinessData<YourServer><YourReply>(<YourServer><YourReply> serviceMessage)
		{
			var transcode = serviceMessage.Transcode();
			if (!transcode.Equals(<YourServer><YourReply>Transcode.RD) &&
				!transcode.Equals(<YourServer><YourReply>Transcode.MR))
				return;
			if (serviceMessage == null)
			{
				throw new ArgumentNullException("serviceMessage");
			}
			if (!serviceMessage.IsTranssteuerungOk())
				return;

			DataProvider.MaxLupdTimestamp = PlusFormat.ParseTandemTimestamp26(serviceMessage.MaxLupdTimestampS83df5());

			IList<$Product$$Item$> checkpoints = DataProvider.$Product$$Item$s;
			for (int i = 0; i < serviceMessage.RefTabAnz(); i++)
			{
				$Product$$Item$ checkpoint = new $Product$$Item$()
				{
					$specialContent1$
					LupdTimestamp = PlusFormat.ParseTandemTimestamp26(serviceMessage.LupdTimestampF6(i)),
					LupdUser = serviceMessage.LupdUser(i)
				};
				checkpoints.Add(checkpoint);
			}
		}

		#endregion I<YourServer><YourReply>BusinessObject Members

		#region Helper

		/// <summary>
		/// Fills the $item$.
		/// </summary>
		/// <param name="serviceMessage">The service message.</param>
		/// <param name="$item$">The $item$.</param>
		/// <param name="i">The i.</param>
		private void Fill$Item$(<YourServer><YourRequest> serviceMessage, $Product$$Item$ $item$, int i)
		{
			$specialContent2$
			serviceMessage.LupdUser($item$.LupdUser, i);
			serviceMessage.LupdTimestampF6(PlusFormat.FormatTandemTimestamp26($item$.LupdTimestamp), i);
		}

		#endregion Helper
	}
}

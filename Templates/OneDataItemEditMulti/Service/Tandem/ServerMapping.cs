using System.Collections.Generic;
using Dcx.Plus.BusinessServiceLocal.Modules.$Product$.Contracts;
using Dcx.Plus.Data.Contracts;
using Dcx.Plus.Data.Tandem.Adapter;
using Dcx.Plus.Data.Tandem.Contracts;
using Dcx.Plus.Data.Tandem.Modules.$Product$;
using Dcx.Plus.BusinessServiceLocal.Modules.$Product$.Contracts.DTOs;

namespace Dcx.Plus.BusinessServiceLocal.Modules.$Product$.Tandem
{
	/// <summary>
	/// Implements the server mapping of the $item$s.
	/// This server mapping should be used in a service that works without Business Objects!!!.
	/// </summary>
	/// <author></author>
	/// <company>abat+ GmbH</company>
	/// <date></date>
	[ServerMapping(typeof(I$Product$$Item$Service))]
	public class $Product$$Item$ServerMapping : ServerMapping<I$Product$$Item$Service>
	{
		/// <summary>
		/// Reads the $item$ list.
		/// </summary>
		/// <param name="dataProvider">The data provider.</param>
		/// <param name="arguments">The arguments.</param>
		/// <param name="transaction">The transaction.</param>
		/// <returns></returns>
		[BackendCommand("Get$Item$List")]
		public bool Read$Item$List(I$Product$$Item$Service dataProvider, IDictionary<string, object> arguments, ITransaction transaction)
		{
			return CallBackend<$Product$$Item$Converter>(dataProvider, arguments, <YourServer>.Singleton.AddTransactionRD);
		}

		/// <summary>
		/// Saves the $item$ list.
		/// </summary>
		/// <param name="dataProvider">The data provider.</param>
		/// <param name="arguments">The arguments.</param>
		/// <param name="transaction">The transaction.</param>
		/// <returns></returns>
		[BackendCommand("Save$Item$List")]
		public bool Save$Item$List(I$Product$$Item$Service dataProvider, IDictionary<string, object> arguments, ITransaction transaction)
		{
			bool isSuccessful = true;

			IList<$Product$$Item$> $item$sToSave = arguments["$Item$sToSave"] as IList<$Product$$Item$>;

			int maxTimes = <YourServer>Req1.RefTab_MaxTimes;
			int numberOfRequests = $item$sToSave.Count / maxTimes + ($item$sToSave.Count % maxTimes > 0 ? 1 : 0);

			//set the offset 
			arguments["$Item$sToSaveOffset"] = 0;

			for (int i = 0; i < numberOfRequests; i++)
			{
				isSuccessful &= AddBackendCallsToTransaction<$Product$$Item$Converter>(dataProvider, transaction, arguments, AcceptionBackendCallback, <YourServer>.Singleton.AddTransactionMR);
			}

			return isSuccessful;
		}
	}
}

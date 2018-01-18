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
		[BackendCommand("Save$Item$")]
		public bool Save$Item$List(I$Product$$Item$Service dataProvider, IDictionary<string, object> arguments, ITransaction transaction)
		{
			bool isSuccessful = true;

			$Product$$Item$ $item$sToSave = arguments["$Item$ToSave"] as $Product$$Item$;

			return AddBackendCallsToTransaction<$Product$$Item$Converter>(dataProvider, transaction, arguments, AcceptionBackendCallback, <YourServer>.Singleton.AddTransactionMR);
		}
	}
}

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
		#region Get
		
		$specialContent1$
		
		#endregion Get
		
		#region Save
		
		$specialContent2$
		
		#endregion Save

	}
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dcx.Plus.BusinessServiceLocal.FW;
using Dcx.Plus.BusinessServiceLocal.FW.Contracts;
using Dcx.Plus.BusinessServiceLocal.Modules.$Product$.Contracts;
using Dcx.Plus.Gateway.Modules.$Product$.Contracts;
using Dcx.Plus.Gateway.Modules.$Product$.Contracts.DTOs;
using Dcx.Plus.Infrastructure.Base;
using Dcx.Plus.Infrastructure.Contracts.Application;

namespace Dcx.Plus.BusinessServiceLocal.Modules.$Product$
{
	/// <summary>
	/// Implements the business service local of the $Product$ module that handles information about $Item$.
	/// </summary>
	/// <author></author>
	/// <company>abat+ GmbH</company>
	/// <date></date>
	[ImplementationOf(typeof(I$Product$$Item$Service), isSingleton: true)]
	public class $Product$$Item$Service : BusinessServiceLocalBase<I$Product$$Item$Gateway>, I$Product$$Item$Service
	{
		#region Get
		
		$specialContent1$
		
		#endregion Get
		
		#region Save
		
		$specialContent2$
		
		#endregion Save
	}
}
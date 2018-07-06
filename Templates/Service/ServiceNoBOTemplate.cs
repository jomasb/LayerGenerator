#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dcx.Plus.BusinessServiceLocal.FW;
using Dcx.Plus.BusinessServiceLocal.FW.Contracts;
using Dcx.Plus.BusinessServiceLocal.Modules.$Product$.Contracts;
using Dcx.Plus.BusinessServiceLocal.Modules.$Product$.Contracts.DTOs;
using Dcx.Plus.Infrastructure.Base;
using Dcx.Plus.Infrastructure.Contracts.Application;
using Dcx.Plus.Infrastructure.Logging.Contracts.Trace;
using SaveFlag = Dcx.Plus.Gateway.FW.Contracts.SaveFlag;

#endregion Usings

namespace Dcx.Plus.BusinessServiceLocal.Modules.$Product$
{
	/// <summary>
	/// Implements the business service local of the $Product$ module that handles information about $Item$s.
	/// This service works without Business Objects!!!.
	/// </summary>
	/// <author></author>
	/// <company>abat+ GmbH</company>
	/// <date></date>
	[ImplementationOf(typeof(I$Product$$Item$NoBOService))]
	public class $Product$$Item$NoBOService$specialContent3$ : BusinessServiceLocalBaseNoBOs<I$Product$$Item$NoBOService>, I$Product$$Item$NoBOService
	{
		#region Get

		$specialContent1$

		#endregion Get

		#region Save

		$specialContent2$

		#endregion Save
	}
}
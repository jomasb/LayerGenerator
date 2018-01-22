#region Usings

using System;
using Dcx.Plus.Gateway.FW.Contracts;
using System.Collections.Generic;
using System.Linq;
using Dcx.Plus.Business.Modules.$Product$.Contracts;
using Dcx.Plus.Infrastructure.Base;
using Dcx.Plus.Infrastructure.Contracts;
using Dcx.Plus.Infrastructure.Contracts.Application;
using Dcx.Plus.Infrastructure.Logging.Contracts.Trace;
using Dcx.Plus.Gateway.Modules.$Product$.Contracts;
using Dcx.Plus.Gateway.Modules.$Product$.Contracts.DTOs;


#endregion Usings

namespace Dcx.Plus.Gateway.Modules.$Product$
{
	/// <summary>
	/// Implements the business gateway of the $Product$ module that handles information about $Item$s.
	/// </summary>
	/// <author></author>
	/// <company>abat+ GmbH</company>
	/// <date></date>
	[ImplementationOf(typeof(I$Product$$Item$Gateway))]
    public class $Product$$Item$Gateway : I$Product$$Item$Gateway
	{
		#region Properties
		
		public Guid SessionContextGuid { get; set; }
		public ISessionContext SessionContext { get; private set; }
		
		#endregion Properties
		
		#region Get
		
		$specialContent1$
		
		#endregion Get
		
		#region Save
		
		$specialContent2$
		
		#endregion Save
	}
}
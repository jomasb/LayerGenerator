using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dcx.Plus.BusinessServiceLocal.FW.Contracts;
using Dcx.Plus.Gateway.Modules.$Product$.Contracts;
using Dcx.Plus.Infrastructure.Contracts.Application;

namespace Dcx.Plus.BusinessServiceLocal.Modules.$Product$.Contracts
{
	public interface I$Product$$Dialog$Service
	{
		#region Get
		
		Task<CallResponse<IList<$Product$$Item$>>> Get$Item$sAsync(IServiceCallContext serviceCallContext);
		
		#endregion Get
	}
}

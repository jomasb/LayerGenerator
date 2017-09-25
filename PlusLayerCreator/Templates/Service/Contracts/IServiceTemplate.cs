using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dcx.Plus.BusinessServiceLocal.FW.Contracts;
using Dcx.Plus.Gateway.Modules.$product$.Contracts;
using Dcx.Plus.Infrastructure.Contracts.Application;

namespace Dcx.Plus.BusinessServiceLocal.Modules.$product$.Contracts
{
	public interface I$product$$dialog$Service
	{
		#region Get
		
		Task<CallResponse<IList<$product$$item$>>> Get$item$sAsync(IServiceCallContext serviceCallContext);
		
		#endregion Get
		
		#region Save
		
		Task<CallResponse<IList<$product$$item$>>> Save$item$sAsync(IServiceCallContext serviceCallContext, List<$product$$item$> $item$Dtos);
		
		#endregion Save
	}
}

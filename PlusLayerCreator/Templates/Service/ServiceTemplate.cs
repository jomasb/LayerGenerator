using System.Collections.Generic;
using System.Threading.Tasks;
using Dcx.Plus.BusinessServiceLocal.FW;
using Dcx.Plus.BusinessServiceLocal.FW.Contracts;
using Dcx.Plus.BusinessServiceLocal.Modules.$product$.Contracts;
using Dcx.Plus.Gateway.Modules.$product$.Contracts;
using Dcx.Plus.Infrastructure.Base;
using Dcx.Plus.Infrastructure.Contracts.Application;

namespace Dcx.Plus.BusinessServiceLocal.Modules.$product$
{
	[ImplementationOf(typeof(I$product$$dialog$Service), isSingleton: true)]
	public class $product$$dialog$Service : BusinessServiceLocalBase<I$product$$dialog$Gateway>, I$product$$dialog$Service
	{
		#region Get
		
		public Task<CallResponse<IList<$product$$item$>>> Get$item$s(IServiceCallContext serviceCallContext)
		{
			return GetGatewayResponse(r => r.Get$item$s(serviceCallContext), serviceCallContext);
		}
		
		#endregion Get
		
		#region Save
		
		public Task<CallResponse<IList<$product$$item$>>> Save$item$sAsync(IServiceCallContext serviceCallContext, List<$product$$item$> $item$Dtos)
		{
			return GetGatewayResponseAsync(r => r.Save$item$s(serviceCallContext, $item$Dtos), serviceCallContext);
		}
		
		#endregion Save
	}
}
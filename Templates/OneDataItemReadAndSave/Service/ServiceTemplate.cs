using System.Collections.Generic;
using Dcx.Plus.BusinessServiceLocal.FW;
using Dcx.Plus.BusinessServiceLocal.FW.Contracts;
using Dcx.Plus.BusinessServiceLocal.Modules.$Product$.Contracts;
using Dcx.Plus.Gateway.Modules.$Product$.Contracts;
using Dcx.Plus.Infrastructure.Base;
using Dcx.Plus.Infrastructure.Contracts.Application;

namespace Dcx.Plus.BusinessServiceLocal.Modules.$Product$
{
	[ImplementationOf(typeof(I$Product$$Dialog$Service), isSingleton: true)]
	public class $Product$$Dialog$Service : BusinessServiceLocalBase<I$Product$$Dialog$Gateway>, I$Product$$Dialog$Service
	{
		#region Get
		
		public Task<CallResponse<IList<$Product$$Item$>>> Get$Item$sAsync(IServiceCallContext serviceCallContext)
		{
			return GetGatewayResponseAsync(r => r.Get$Item$s(serviceCallContext), serviceCallContext);
		}
		
		#endregion Get
	}
		
		#region Save
		
		public Task<CallResponse<$Product$$Item$>> Save$Item$Async(IServiceCallContext serviceCallContext, $Product$$Item$ $item$Dto)
		{
			return GetGatewayResponseAsync(r => r.Save$Item$(serviceCallContext, $item$Dto), serviceCallContext);
		}
		
		#endregion Save
	}
}
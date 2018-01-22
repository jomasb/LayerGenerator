using System.Collections.Generic;
using Dcx.Plus.BusinessServiceLocal.FW;
using Dcx.Plus.BusinessServiceLocal.FW.Contracts;
using Dcx.Plus.BusinessServiceLocal.Modules.$Product$.Contracts;
using Dcx.Plus.Gateway.Modules.$Product$.Contracts;
using Dcx.Plus.Infrastructure.Base;
using Dcx.Plus.Infrastructure.Contracts.Application;

namespace Dcx.Plus.BusinessServiceLocal.Modules.$Product$
{
	[ImplementationOf(typeof(I$Item$Service), isSingleton: true)]
	public class $Item$Service : BusinessServiceLocalBase<I$Item$Gateway>, I$Item$Service
	{
		#region Get
		
		public Task<CallResponse<IList<$Product$$Item$>>> Get$Item$sAsync(IServiceCallContext serviceCallContext)
		{
			return GetGatewayResponseAsync(r => r.Get$Item$s(serviceCallContext), serviceCallContext);
		}
		
		#endregion Get
	}
}
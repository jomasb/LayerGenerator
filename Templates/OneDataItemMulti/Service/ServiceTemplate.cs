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
	[ImplementationOf(typeof(I$Item$Service), isSingleton: true)]
	public class $Item$Service : BusinessServiceLocalBase<I$Item$Gateway>, I$Item$Service
	{
		#region Get
		
		/// <summary>
		/// Gets the $Item$s asynchronous.
		/// </summary>
		/// <param name="serviceCallContext">The service call context.</param>
		/// <returns></returns>
		public Task<CallResponse<IList<$Product$$Item$>>> Get$Item$sAsync(IServiceCallContext serviceCallContext)
		{
			return GetGatewayResponseAsync(r => r.Get$Item$s(serviceCallContext), serviceCallContext);
		}
		
		#endregion Get
		
		#region Save
		
		/// <summary>
		/// Saves the $Item$s asynchronous.
		/// </summary>
		/// <param name="serviceCallContext">The service call context.</param>
		/// <param name="$Item$Dtos">The $Item$ dtos.</param>
		/// <returns></returns>
		public Task<CallResponse<IList<$Product$$Item$>>> Save$Item$sAsync(IServiceCallContext serviceCallContext, IList<$Product$$Item$> $item$Dtos)
		{
			return GetGatewayResponseAsync(r => r.Save$Item$s(serviceCallContext, $item$Dtos), serviceCallContext);
		}
		
		#endregion Save
	}
}
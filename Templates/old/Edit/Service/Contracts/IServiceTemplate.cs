using System.Collections.Generic;
using System.Threading.Tasks;
using Dcx.Plus.BusinessServiceLocal.FW.Contracts;
using Dcx.Plus.Gateway.Modules.$Product$.Contracts.DTOs;
using Dcx.Plus.Infrastructure.Contracts.Application;

namespace Dcx.Plus.BusinessServiceLocal.Modules.$Product$.Contracts
{
	/// <summary>
	/// Defines the interface of the business service local of the $Product$ module that handles information about $Item$s.
	/// </summary>
	/// <author></author>
	/// <company>abat+ GmbH</company>
	/// <date></date>
	public interface I$Product$$Item$Service
	{
		#region Get
		
		/// <summary>
		/// Gets the $Item$s asynchronous.
		/// </summary>
		/// <param name="serviceCallContext">The service call context.</param>
		/// <returns></returns>
		Task<CallResponse<IList<$Product$$Item$>>> Get$Item$sAsync(IServiceCallContext serviceCallContext);
		
		#endregion Get
		
		#region Save
		
		/// <summary>
		/// Saves the $Item$ asynchronous.
		/// </summary>
		/// <param name="serviceCallContext">The service call context.</param>
		/// <param name="$item$Dto">The $Item$ dto.</param>
		/// <returns></returns>
		Task<CallResponse<$Product$$Item$>> Save$Item$Async(IServiceCallContext serviceCallContext, $Product$$Item$ $item$Dto);
		
		#endregion Save
	}
}
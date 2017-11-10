#region Usings

using Dcx.Plus.Gateway.FW.Contracts;
using System.Collections.Generic;
using Dcx.Plus.Gateway.Modules.$Product$.Contracts.DTOs;
using Dcx.Plus.Infrastructure.Contracts;
using Dcx.Plus.Infrastructure.Contracts.Application;

#endregion Usings

namespace Dcx.Plus.Gateway.Modules.$Product$.Contracts
{
	/// <summary>
	/// Defines the interface of the business gateway of the $Product$ module that handles information about $Item$s.
	/// </summary>
	/// <author></author>
	/// <company>abat+ GmbH</company>
	/// <date></date>
	public interface I$Item$Gateway : IGateway
	{
		#region Get
		
		/// <summary>
		/// Gets the Product$$Item$s.
		/// </summary>
		/// <param name="callContext">The call context.</param>
		/// <returns></returns>
		CallResponse<IList<$Product$$Item$>> Get$Item$s(ICallContext callContext);
		
		#endregion Get
		
		#region Save
		
		/// <summary>
		/// Saves the $Product$$Item$s.
		/// </summary>
		/// <param name="callContext">The call context.</param>
		/// <param name="$item$Dtos">The $Product$$Item$ dtos.</param>
		/// <returns></returns>
		CallResponse<IList<$Product$$Item$>> Save$Item$s(ICallContext callContext, IList<$Product$$Item$> $item$Dtos);
		
		#endregion Save
	}
}
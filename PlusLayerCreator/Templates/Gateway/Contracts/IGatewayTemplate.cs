#region Usings

using Dcx.Plus.Gateway.FW.Contracts;
using System.Collections.Generic;
using Dcx.Plus.Business.Modules.$product$.Contracts;
using Dcx.Plus.Infrastructure.Contracts;
using Dcx.Plus.Infrastructure.Contracts.Application;

#endregion Usings

namespace Dcx.Plus.Gateway.Modules.$product$.Contracts
{
	public interface I$product$$dialog$Gateway : IGateway
	{
		#region Get
		
		CallResponse<IList<$product$$item$>> Get$item$s(ICallContext callContext);
		
		#endregion Get
		
		#region Save
		
		CallResponse<IList<$product$$item$>> Save$item$s(ICallContext callContext, List<$product$$item$> $item$Dtos);
		
		#endregion Save
	}
}
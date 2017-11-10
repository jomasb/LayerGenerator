#region Usings

using Dcx.Plus.Gateway.FW.Contracts;
using System.Collections.Generic;
using Dcx.Plus.Business.Modules.$Product$.Contracts;
using Dcx.Plus.Infrastructure.Contracts;
using Dcx.Plus.Infrastructure.Contracts.Application;

#endregion Usings

namespace Dcx.Plus.Gateway.Modules.$Product$.Contracts
{
	public interface I$Product$$Dialog$Gateway : IGateway
	{
		#region Get
		
		CallResponse<IList<$Product$$Item$>> Get$Item$s(ICallContext callContext);
		
		#endregion Get
		
		#region Save
		
		CallResponse<IList<$Product$$Item$>> Save$Item$s(ICallContext callContext, List<$Product$$Item$> $item$Dtos);
		
		#endregion Save
	}
}
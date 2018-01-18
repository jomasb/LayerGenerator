#region Usings

using Dcx.Plus.Gateway.FW.Contracts;
using System.Collections.Generic;
using Dcx.Plus.Business.Modules.$Product$.Contracts;
using Dcx.Plus.Infrastructure.Base;
using Dcx.Plus.Infrastructure.Contracts;
using Dcx.Plus.Infrastructure.Contracts.Application;
using Dcx.Plus.Gateway.Modules.$Product$.Contracts;
using Dcx.Plus.Gateway.Modules.$Product$.Contracts.DTOs;


#endregion Usings

namespace Dcx.Plus.Gateway.Modules.$Product$
{
	[ImplementationOf(typeof(I$Product$$Item$Gateway), condition: "Mock")]
    public class $Product$$Item$Gateway : I$Product$$Item$Gateway
	{
		#region Properties
		
		public Guid SessionContextGuid { get; set; }
		public ISessionContext SessionContext { get; private set; }
		
		#endregion Properties
		
		#region Get
		
		public CallResponse<IList<$Product$$Item$>> Get$Item$s(ICallContext callContext)
        {
			List<$Product$$Item$> $item$s = new List<$Product$$Item$>;
			for (int i = 0; i < 20; i++)
			{
				$item$s.Add(new $Product$$Item$()
				{
					$specialContent$
					LupdTimestamp = new DateTime(2016, 12, 25),
					LupdUser = "Mock",
					SaveFlag = SaveFlag.Persistent
				});
			}
			return CallResponse.FromSuccessfulResult((IList<$Product$$Item$>)$item$s);
        }
		
		#endregion Get
	}
}

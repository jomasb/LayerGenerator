#region Usings

using Dcx.Plus.Gateway.FW.Contracts;
using System.Collections.Generic;
using Dcx.Plus.Business.Modules.$Product$.Contracts;
using Dcx.Plus.Infrastructure.Base;
using Dcx.Plus.Infrastructure.Contracts;
using Dcx.Plus.Infrastructure.Contracts.Application;
using Dcx.Plus.Infrastructure.Logging.Contracts.Trace;
using Dcx.Plus.Gateway.Modules.$Product$.Contracts;

#endregion Usings

namespace Dcx.Plus.Gateway.Modules.$Product$
{
	[ImplementationOf(typeof(I$Item$Gateway))]
    public class $Item$Gateway : I$Item$Gateway
	{
		#region Properties
		
		public Guid SessionContextGuid { get; set; }
		public ISessionContext SessionContext { get; private set; }
		
		#endregion Properties
		
		#region Get
		
		public CallResponse<IList<$Product$$Item$>> Get$Item$s(ICallContext callContext)
        {
            using (TraceLog.Log(this, options: TraceLogOptions.All))
            {
				I$Product$$Item$ListFactory $item$ListFactory = BOFactoryProvider.Get<I$Product$$Item$ListFactory>();
				I$Product$$Item$List $item$List = _$item$ListFactory.Get(ApplicationProvider.SessionContextGuid, callContext);
				List<$Product$$Item$> $item$s = $item$List.$Item$s.Select($Product$DtoFactory.Create$Item$FromBO).ToList();

				return CallResponse.FromSuccessfulResult((IList<$Product$$Item$>)$item$s);
            }
        }
		
		#endregion Get
	}
}
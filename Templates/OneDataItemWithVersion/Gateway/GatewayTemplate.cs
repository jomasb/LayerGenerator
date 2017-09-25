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
	[ImplementationOf(typeof(I$Product$$Dialog$Gateway))]
    public class $Product$$Dialog$Gateway : I$Product$$Dialog$Gateway
	{
		#region Properties
		
		public Guid SessionContextGuid { get; set; }
		public ISessionContext SessionContext { get; private set; }
		
		#region Properties
		
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
		
		#region Save
		
		public CallResponse<IList<$Product$$Item$>> Save$Item$s(ICallContext callContext, List<$Product$$Item$> $item$Dtos)
		{
			I$Product$$Item$ListFactory $item$ListFactory = BOFactoryProvider.Get<I$Product$$Item$ListFactory>();
			I$Product$$Item$List $item$List = $item$ListFactory.Get(ApplicationProvider.SessionContextGuid, callContext);
			I$Product$$Item$Factory $item$Factory = BOFactoryProvider.Get<I$Product$$Item$Factory>();
			
			foreach ($Product$$Item$ $item$ in $item$Dtos)
			{
				switch ($item$.SaveFlag)
				{
					case SaveFlag.New:
						{
							$item$ = $item$Factory.Create(??, ApplicationProvider.SessionContextGuid);
							$item$ = ??;
							$item$.CallContext = callContext;
							break;
						}
					case SaveFlag.Delete:
						{
							$item$ = $item$List.$Item$s.FirstOrDefault(x => x.Key.?? == input.??);
							if ($item$ != null)
							{
								$item$ = ??;
								$item$.Delete();
							}
							break;
						}
				}
			}
			
			bool isSuccessfullySaved = $item$List.Save();
			if (isSuccessfullySaved)
			{
				foreach (var bo in $item$List)
				{
					bo.Accept();
				}
				
				$item$List.Accept();
				
				IList<$Product$$Item$> new$Item$Dtos = new List<$Product$$Item$>();
					
				foreach (var dto in $item$Dtos.Where(t => t.SaveFlag == SaveFlag.New))
				{
					dto.LupdTimestamp = ??;
					dto.SaveFlag = SaveFlag.Persistent;
					new$Item$Dtos.Add(dto);
				}

				return CallResponse.FromSuccessfulResult(new$Item$Dtos);
			}
			
			return CallResponse.FromFailedResult<IList<$Product$$Item$>>(null);
		}
		
		#endregion Save
	}
}
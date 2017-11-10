#region Usings

using Dcx.Plus.Gateway.FW.Contracts;
using System.Collections.Generic;
using Dcx.Plus.Business.Modules.$product$.Contracts;
using Dcx.Plus.Infrastructure.Contracts;
using Dcx.Plus.Infrastructure.Contracts.Application;

#endregion Usings

namespace Dcx.Plus.Gateway.Modules.$product$
{
	[ImplementationOf(typeof(I$product$$dialog$Gateway))]
    public class $product$$dialog$Gateway : I$product$$dialog$Gateway
	{
		#region Get
		
		public CallResponse<IList<$product$$item$>> Get$item$s(ICallContext callContext)
        {
            using (TraceLog.Log(this, options: TraceLogOptions.All))
            {
				I$product$$item$List $item$List = _$item$ListFactory.Get(ApplicationProvider.SessionContextGuid, callContext);
				List<$product$$item$> $item$s = $item$List.$item$s.Select($product$DtoFactory.Create$item$FromBO).ToList();

				return CallResponse.FromSuccessfulResult((IList<$product$$item$>)$item$s);
            }
        }
		
		#endregion Get
		
		#region Save
		
		public CallResponse<IList<$product$$item$>> Save$item$s(ICallContext callContext, List<$product$$item$> $item$Dtos)
		{
			I$product$$item$ListFactory $item$ListFactory = BOFactoryProvider.Get<I$product$$item$ListFactory>();
			I$product$$item$List $item$List = $item$ListFactory.Get(ApplicationProvider.SessionContextGuid, callContext);
			I$product$$item$Factory $item$Factory = BOFactoryProvider.Get<I$product$$item$Factory>();
			
			foreach ($product$$item$ dto in $item$Dtos)
			{
				switch (dto.SaveFlag)
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
							$item$ = $item$List.$item$s.FirstOrDefault(x => x.Key.?? == input.??);
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
				
				IList<$product$$item$> new$item$Dtos = new List<$product$$item$>();
					
				foreach (var dto in $item$Dtos.Where(t => t.SaveFlag == SaveFlag.New))
				{
					dto.LupdTimestamp = ??;
					dto.SaveFlag = SaveFlag.Persistent;
					new$item$Dtos.Add(dto);
				}

				return CallResponse.FromSuccessfulResult(new$item$Dtos);
			}
			
			return CallResponse.FromFailedResult<IList<$product$$item$>>(null);
		}
		
		#endregion Save
	}
}
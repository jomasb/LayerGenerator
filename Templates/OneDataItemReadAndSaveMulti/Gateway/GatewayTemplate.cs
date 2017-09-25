#region Usings

using Dcx.Plus.Gateway.FW.Contracts;
using System.Collections.Generic;
using Dcx.Plus.Business.Modules.$Product$.Contracts;
using Dcx.Plus.Infrastructure.Base;
using Dcx.Plus.Infrastructure.Contracts;
using Dcx.Plus.Infrastructure.Contracts.Application;
using Dcx.Plus.Infrastructure.Logging.Contracts.Trace;
using Dcx.Plus.Gateway.Modules.$Product$.Contracts;
using Dcx.Plus.Gateway.Modules.$Product$.Contracts.DTOs;


#endregion Usings

namespace Dcx.Plus.Gateway.Modules.$Product$
{
	/// <summary>
	/// Implements the business gateway of the $Product$ module that handles information about $Item$s.
	/// </summary>
	/// <author></author>
	/// <company>abat+ GmbH</company>
	/// <date></date>
	[ImplementationOf(typeof(I$Product$$Dialog$Gateway))]
    public class $Product$$Dialog$Gateway : I$Product$$Dialog$Gateway
	{
		#region Properties
		
		public Guid SessionContextGuid { get; set; }
		public ISessionContext SessionContext { get; private set; }
		
		#endregion Properties
		
		#region Get
		
		/// <summary>
		/// Gets the Product$$Item$s.
		/// </summary>
		/// <param name="callContext">The call context.</param>
		/// <returns></returns>
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
		
		/// <summary>
		/// Saves the $Product$$Item$s.
		/// </summary>
		/// <param name="callContext">The call context.</param>
		/// <param name="$item$Dtos">The $Product$$Item$ dtos.</param>
		/// <returns></returns>
		public CallResponse<IList<$Product$$Item$>> Save$Item$s(ICallContext callContext, IList<$Product$$Item$> $item$Dtos)
		{
			I$Product$$Item$ListFactory $item$ListFactory = BOFactoryProvider.Get<I$Product$$Item$ListFactory>();
			I$Product$$Item$List $item$List = $item$ListFactory.Get(ApplicationProvider.SessionContextGuid, callContext);
			I$Product$$Item$Factory $item$Factory = BOFactoryProvider.Get<I$Product$$Item$Factory>();

			foreach ($Product$$Item$ $item$Dto in $item$Dtos)
			{
				switch ($item$Dto.SaveFlag)
				{
					case SaveFlag.New:
					{
						I$Product$$Item$ $item$ = $item$Factory.Create($specialContent$, ApplicationProvider.SessionContextGuid);
						$item$.CallContext = callContext;
						break;
					}
					case SaveFlag.Delete:
					{
						I$Product$$Item$ $item$ = $item$List.$Item$s.FirstOrDefault(x => $specialContent2$);
						if ($item$ != null)
						{
							$item$.Delete();
						}
						break;
					}
				}
			}

			bool isSuccessfullySaved = $item$List.Save();
			if (isSuccessfullySaved)
			{
				foreach (var bo in $item$List.$Item$s)
				{
					bo.Accept();
				}

				$item$List.Accept();

				foreach (var dto in $item$Dtos.Where(t => t.SaveFlag == SaveFlag.New))
				{
					I$Product$$Item$ $item$ = $item$List.$Item$s.FirstOrDefault(x => $specialContent2$);
					if ($item$ != null)
					{
						dto.LupdTimestamp = $item$.LupdTimestamp;
						dto.LupdUser = $item$.LupdUser;
						$FillDataTiemFromDto$
						dto.SaveFlag = SaveFlag.Persistent;
					}
				}

				return CallResponse.FromSuccessfulResult($item$Dtos);
			}

			return CallResponse.FromFailedResult<IList<$Product$$Item$>>(null);
		}
		
		#endregion Save
	}
}
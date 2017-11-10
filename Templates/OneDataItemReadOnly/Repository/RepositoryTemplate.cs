using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dcx.Plus.BusinessServiceLocal.Modules.$Product$.Contracts;
using Dcx.Plus.Gateway.FW.Contracts;
using Dcx.Plus.Gateway.Modules.$Product$.Contracts;
using Dcx.Plus.Infrastructure.Base;
using Dcx.Plus.Infrastructure.Contracts;
using Dcx.Plus.Infrastructure.Contracts.Application;
using Dcx.Plus.Repository.FW;
using Dcx.Plus.Repository.FW.Collections;
using Dcx.Plus.Repository.FW.Contracts;
using Dcx.Plus.Repository.Modules.$Product$.Contracts;
using Dcx.Plus.Repository.Modules.$Product$.Contracts.DataItems;

namespace Dcx.Plus.Repository.Modules.$Product$
{
	[ImplementationOf(typeof(I$Product$$Dialog$Repository))]
	public class $Product$$Dialog$Repository : RepositoryBase, I$Product$$Dialog$Repository
	{
		#region Members
		
		private readonly I$Product$$Dialog$Service _$item$Service;
		private readonly $Product$DataItemFactory _$product$DataItemFactory;
		private readonly $Product$DtoFactory _$product$DtoFactory;
		
		#endregion Members

		#region Construction
		
		public $Product$$Dialog$Repository(I$Product$$Dialog$Service $item$Service)
		{
			_$item$Service = $item$Service;
			_$product$DataItemFactory = new $Product$DataItemFactory();
			_$product$DtoFactory = new $Product$DtoFactory();
		}
		
		#endregion Construction
		
		#region Get

		public async Task<CallResponse<IList<$Product$$Item$DataItem>>> Get$Item$sAsync(IRepositoryCallContext callContext)
		{
			try
			{
				IList<$Product$$Item$DataItem> $item$DataItems = new List<$Product$$Item$DataItem>();

				var callResponse = await _$item$Service.Get$Item$sAsync(callContext).ConfigureAwait(false);

				if (callResponse.IsSuccess)
				{
					IList<$Product$$Item$> $item$s = callResponse.Result;
					foreach ($product$$Item$ $Item$ in $item$s)
					{
						$Product$$Item$DataItem $item$DataItem = _$product$DataItemFactory.Create$Item$FromDto($item$);
						$item$DataItem.Accept();
						$item$DataItems.Add($item$DataItem);
					}
				}

				return CallResponse.FromResponse(callResponse, _ => $item$DataItems);
			}
			catch (Exception ex)
			{
				return CallResponse.FromException<IList<$Product$$Item$DataItem>>(ex);
			}
		}

		public PlusLazyLoadingAsyncObservableCollection<$Product$$Item$DataItem> Get$Item$s(IRepositoryCallContext callContext)
		{
			return new PlusLazyLoadingAsyncObservableCollection<$Product$$Item$DataItem>(() => Get$Item$sAsync(callContext))
			{
				AutoAcceptAfterSuccessfullyLoading = true
			};
		}
		
		#endregion Get
	}
}
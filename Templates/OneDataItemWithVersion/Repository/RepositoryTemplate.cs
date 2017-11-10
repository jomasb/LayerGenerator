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
		
		private readonly I$Product$$Dialog$Service _$dialog$Service;
		private readonly $Product$DataItemFactory _$product$DataItemFactory;
		
		#endregion Members

		#region Construction
		
		public $Product$$Dialog$Repository(I$Product$$Dialog$Service $dialog$Service)
		{
			_$dialog$Service = $dialog$Service;
			_$product$DataItemFactory = new $Product$DataItemFactory();
		}
		
		#endregion Construction
		
		#region Get

		public async Task<CallResponse<IList<$Product$$Item$DataItem>>> Get$Item$sAsync(IRepositoryCallContext callContext)
		{
			try
			{
				IList<$Product$$Item$DataItem> $item$DataItems = new List<$Product$$Item$DataItem>();

				var callResponse = await _$dialog$Service.Get$Item$sAsync(callContext).ConfigureAwait(false);

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
		
		#region Add
		
		public Task<CallResponse<$Product$$Item$DataItem>> AddNew$Item$Async(IRepositoryCallContext callContext, IObservableCollection<$Product$$Item$DataItem> $item$DataItems)
		{
			try
			{
			    $Product$$Item$DataItem $item$ = new $Product$$Item$DataItem();
			    $item$.Accept();
			    $item$DataItems.Add($item$);

			    return CallResponse.FromSuccessfulResult($item$);
			}
			catch (Exception ex)
			{
				return CallResponse.FromException<$Product$$Item$DataItem>(ex);
			}
		}
		
		#endregion Add
		
		#region Delete
		
		public Task<CallResponse<bool>> Delete$Item$Async(IRepositoryCallContext callContext, $Product$$Item$DataItem $item$, PlusObservableCollection<$Product$$Item$DataItem> $item$s)
		{
			try
			{
			    $item$s.Remove($item$)

			    return CallResponse.FromSuccessfulResult(true);
			}
			catch (Exception ex)
			{
				return CallResponse.FromException<$Product$$Item$DataItem>(ex);
			}
		}
		
		#endregion Delete
		
		#region Clone
		
		public Task<CallResponse<$Product$$Item$DataItem>> Clone$Item$Async(IRepositoryCallContext callContext, $Product$$Item$DataItem $item$, PlusObservableCollection<$Product$$Item$DataItem> $item$s)
		{
			$Product$$Item$DataItem new$Item$ = await $item$.DeepCloneData();

			new$Item$.ForEachTunneling<PlusStateDataItem>(y => y.State = DataItemState.New);

			$item$s.Add($item$);
			return CallResponse.FromSuccessfulResult($item$s);			
		}
		
		#endregion Clone
		
		#region Save
		
		public Task<CallResponse<PlusObservableCollection<$Product$$Item$DataItem>>> Save$Item$Async(IRepositoryCallContext callContext, PlusObservableCollection<$Product$$Item$DataItem> $item$s)
		{
			try
			{
				foreach ($Product$$Item$DataItem $item$DataItem in $item$s.DeletedItems)
				{
					$Product$$Item$ $item$ = $Product$DtoFactory.Create$Item$FromDataItem($item$DataItem);
					$item$.SaveFlag = SaveFlag.Delete;
					$item$s.Add($item$);
				}

				//Remove all existing assignments and recreate it afterwards
				foreach ($Product$$Item$DataItem $item$DataItem in $item$s.Where(x => x.State == DataItemState.Persistent))
				{
					$Product$$Item$ delete$Item$ = $Product$DtoFactory.Create$Item$FromDataItem($item$DataItem);
					delete$Item$.SaveFlag = SaveFlag.Delete;
					$item$s.Add(delete$Item$);

					$product$$Item$ create$Item$ = $Product$DtoFactory.Create$Item$FromDataItem($item$DataItem);
					create$Item$.SaveFlag = SaveFlag.New;
					$item$s.Add(create$Item$);
				}

				//Add all new assignments
				foreach ($Product$$Item$DataItem $item$DataItem in $item$s.Where(x => x.State == DataItemState.New))
				{
					$Product$$Item$ create$Item$ = $Product$DtoFactory.Create$Item$FromDataItem($item$DataItem);
					create$Item$.SaveFlag = SaveFlag.New;
					$Item$s.Add(create$Item$);
				}
				
				CallResponse<IList<$Product$$Item$>> response = await _$dialog$Service.Save$Item$sAsync(callContext, $item$s);
				if (response.IsSuccess)
				{
					foreach ($Product$$Item$DataItem dataItem in siteVersion.$Item$s)
					{
						dataItem.State = DataItemState.Persistent;
						dataItem.Accept();
					}

					return CallResponse.FromSuccessfulResult(true);
				}

				return CallResponse.FromFailedResult(false);
				}
			}
			catch (Exception ex)
			{
				return CallResponse.FromException<PlusObservableCollection<$Product$$Item$DataItem>>(ex);
			}
		}
		
		#endregion Save
	}
}
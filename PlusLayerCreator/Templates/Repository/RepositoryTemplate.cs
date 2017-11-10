using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dcx.Plus.BusinessServiceLocal.Modules.$product$.Contracts;
using Dcx.Plus.Gateway.FW.Contracts;
using Dcx.Plus.Gateway.Modules.$product$.Contracts;
using Dcx.Plus.Infrastructure.Base;
using Dcx.Plus.Infrastructure.Contracts;
using Dcx.Plus.Infrastructure.Contracts.Application;
using Dcx.Plus.Repository.FW;
using Dcx.Plus.Repository.FW.Collections;
using Dcx.Plus.Repository.FW.Contracts;
using Dcx.Plus.Repository.Modules.$product$.Contracts;
using Dcx.Plus.Repository.Modules.$product$.Contracts.DataItems;

namespace Dcx.Plus.Repository.Modules.$product$
{
	[ImplementationOf(typeof(I$product$$dialog$Repository))]
	public class $product$$dialog$Repository : RepositoryBase, I$product$$dialog$Repository
	{
		#region Members
		
		private readonly I$product$$dialog$Service _$dialog$Service;
		private readonly $product$DataItemFactory _$product$DataItemFactory;
		
		#endregion Members

		#region Construction
		
		public $product$$dialog$Repository(I$product$$dialog$Service $dialog$Service)
		{
			_$dialog$Service = $dialog$Service;
			_$product$DataItemFactory = new $product$DataItemFactory();
		}
		
		#endregion Construction
		
		#region Get

		public async Task<CallResponse<IList<$product$$item$DataItem>>> Get$item$sAsync(IRepositoryCallContext callContext)
		{
			try
			{
				IList<$product$$item$DataItem> $item$DataItems = new List<$product$$item$DataItem>();

				var callResponse = await _$dialog$Service.Get$item$sAsync(callContext).ConfigureAwait(false);

				if (callResponse.IsSuccess)
				{
					IList<$product$$item$> $item$s = callResponse.Result;
					foreach ($product$$item$ $item$ in $item$s)
					{
						$product$$item$DataItem $item$DataItem = _$product$DataItemFactory.Create$item$FromDto($item$);
						$item$DataItem.Accept();
						$item$DataItems.Add($item$DataItem);
					}
				}

				return CallResponse.FromResponse(callResponse, _ => $item$DataItems);
			}
			catch (Exception ex)
			{
				return CallResponse.FromException<IList<$product$$item$DataItem>>(ex);
			}
		}

		public PlusLazyLoadingAsyncObservableCollection<$product$$item$DataItem> Get$item$s(IRepositoryCallContext callContext)
		{
			return new PlusLazyLoadingAsyncObservableCollection<$product$$item$DataItem>(() => Get$item$sAsync(callContext))
			{
				AutoAcceptAfterSuccessfullyLoading = true
			};
		}
		
		#endregion Get
		
		#region Add
		
		public Task<CallResponse<$product$$item$DataItem>> AddNew$item$Async(IRepositoryCallContext callContext, IObservableCollection<$product$$item$DataItem> $item$DataItems)
		{
			try
			{
			    $product$$item$DataItem $item$ = new $product$$item$DataItem();
			    $item$.Accept();
			    $item$DataItems.Add($item$);

			    return CallResponse.FromSuccessfulResult($item$);
			}
			catch (Exception ex)
			{
				return CallResponse.FromException<$product$$item$DataItem>(ex);
			}
		}
		
		#endregion Add
		
		#region Delete
		
		public Task<CallResponse<bool>> Delete$item$Async(IRepositoryCallContext callContext, $product$$item$DataItem $item$, PlusObservableCollection<$product$$item$DataItem> $item$s)
		{
			try
			{
			    $item$s.Remove($item$)

			    return CallResponse.FromSuccessfulResult(true);
			}
			catch (Exception ex)
			{
				return CallResponse.FromException<$product$$item$DataItem>(ex);
			}
		}
		
		#endregion Delete
		
		#region Clone
		
		public Task<CallResponse<$product$$item$DataItem>> Clone$item$Async(IRepositoryCallContext callContext, $product$$item$DataItem $item$, PlusObservableCollection<$product$$item$DataItem> $item$s)
		{
			
		}
		
		#endregion Clone
		
		#region Save
		
		public Task<CallResponse<PlusObservableCollection<$product$$item$DataItem>>> Save$item$Async(IRepositoryCallContext callContext, PlusObservableCollection<$product$$item$DataItem> $item$s)
		{
			try
			{
				foreach ($product$$item$DataItem $item$DataItem in $item$s.DeletedItems)
				{
					$product$$item$ $item$ = $product$DtoFactory.Create$item$FromDataItem($item$DataItem);
					$item$.SaveFlag = SaveFlag.Delete;
					$item$s.Add($item$);
				}

				//Remove all existing assignments and recreate it afterwards
				foreach ($product$$item$DataItem $item$DataItem in $item$s.Where(x => x.State == DataItemState.Persistent))
				{
					$product$$item$ delete$item$ = $product$DtoFactory.Create$item$FromDataItem($item$DataItem);
					delete$item$.SaveFlag = SaveFlag.Delete;
					$item$s.Add(delete$item$);

					$product$$item$ create$item$ = $product$DtoFactory.Create$item$FromDataItem($item$DataItem);
					create$item$.SaveFlag = SaveFlag.New;
					$item$s.Add(create$item$);
				}

				//Add all new assignments
				foreach ($product$$item$DataItem $item$DataItem in $item$s.Where(x => x.State == DataItemState.New))
				{
					$product$$item$ create$item$ = $product$DtoFactory.Create$item$FromDataItem($item$DataItem);
					create$item$.SaveFlag = SaveFlag.New;
					$item$s.Add(create$item$);
				}
				
				CallResponse<IList<$product$$item$>> response = await _$dialog$Service.Save$item$sAsync(callContext, $item$s);
				if (response.IsSuccess)
				{
					foreach ($product$$item$DataItem dataItem in siteVersion.$item$s)
					{

						dataItem.Accept();
					}

					return CallResponse.FromSuccessfulResult(true);
				}

				return CallResponse.FromFailedResult(false);
				}
			catch (Exception ex)
			{
				return CallResponse.FromException<PlusObservableCollection<$product$$item$DataItem>>(ex);
			}
		}
		
		#endregion Save
	}
}
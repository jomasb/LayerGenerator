using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dcx.Plus.BusinessServiceLocal.Modules.$Product$.Contracts;
using Dcx.Plus.Gateway.FW.Contracts;
using Dcx.Plus.Gateway.Modules.$Product$.Contracts.DTOs;
using Dcx.Plus.Infrastructure.Base;
using Dcx.Plus.Infrastructure.Contracts.Application;
using Dcx.Plus.Infrastructure.HierarchicalItem;
using Dcx.Plus.Repository.FW;
using Dcx.Plus.Repository.FW.Collections;
using Dcx.Plus.Repository.FW.Contracts;
using Dcx.Plus.Repository.FW.DataItems;
using Dcx.Plus.Repository.FW.DataItems.Extensions;
using Dcx.Plus.Repository.Modules.$Product$.Contracts;
using Dcx.Plus.Repository.Modules.$Product$.Contracts.DataItems;

namespace Dcx.Plus.Repository.Modules.$Product$
{
	/// <summary>
	/// Implements the repository of the $Product$ module that handles information about $Item$.
	/// </summary>
	/// <author></author>
	/// <company>abat+ GmbH</company>
	/// <date></date>
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

		/// <summary>
		/// Gets the checkpoint references asynchronous.
		/// </summary>
		/// <param name="callContext">The call context.</param>
		/// <returns></returns>
		public async Task<CallResponse<IList<$Product$$Item$DataItem>>> Get$Item$sAsync(IRepositoryCallContext callContext)
		{
			try
			{
				IList<$Product$$Item$DataItem> $item$DataItems = new List<$Product$$Item$DataItem>();

				var callResponse = await _$item$Service.Get$Item$sAsync(callContext).ConfigureAwait(false);

				if (callResponse.IsSuccess)
				{
					IList<$Product$$Item$> $item$s = callResponse.Result;
					foreach ($Product$$Item$ $item$ in $item$s)
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

		/// <summary>
		/// Gets the checkpoint references.
		/// </summary>
		/// <param name="callContext">The call context.</param>
		/// <returns></returns>
		public PlusLazyLoadingAsyncObservableCollection<$Product$$Item$DataItem> Get$Item$s(IRepositoryCallContext callContext)
		{
			return new PlusLazyLoadingAsyncObservableCollection<$Product$$Item$DataItem>(() => Get$Item$sAsync(callContext))
			{
				AutoAcceptAfterSuccessfullyLoading = true
			};
		}
		
		#endregion Get
		
		#region Add
		
		/// <summary>
		/// Adds the new $Item$ asynchronous.
		/// </summary>
		/// <param name="callContext">The call context.</param>
		/// <param name="$Item$DataItems">The $Item$ data items.</param>
		/// <returns></returns>
		public async Task<CallResponse<$Product$$Item$DataItem>> AddNew$Item$Async(IRepositoryCallContext callContext, IObservableCollection<$Product$$Item$DataItem> $item$DataItems)
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
		
		/// <summary>
		/// Deletes the $Item$ asynchronous.
		/// </summary>
		/// <param name="callContext">The call context.</param>
		/// <param name="$Item$">The $Item$.</param>
		/// <param name="$Item$s">The $Item$s.</param>
		/// <returns></returns>
		public async Task<CallResponse<bool>> Delete$Item$Async(IRepositoryCallContext callContext, $Product$$Item$DataItem $item$, PlusObservableCollection<$Product$$Item$DataItem> $item$s)
		{
			try
			{
			    $item$s.Remove($item$);
			    return CallResponse.FromSuccessfulResult(true);
			}
			catch (Exception ex)
			{
				return CallResponse.FromFailedResult<bool>(false);
			}
		}
		
		#endregion Delete
		
		#region Clone
		
		/// <summary>
		/// Clones the $Item$ asynchronous.
		/// </summary>
		/// <param name="callContext">The call context.</param>
		/// <param name="$Item$">The $Item$.</param>
		/// <param name="$Item$s">The $Item$s.</param>
		/// <returns></returns>
		public async Task<CallResponse<$Product$$Item$DataItem>> Clone$Item$Async(IRepositoryCallContext callContext, $Product$$Item$DataItem $item$, PlusObservableCollection<$Product$$Item$DataItem> $item$s)
		{
			$Product$$Item$DataItem new$Item$ = await $item$.DeepCloneData();
			new$Item$.ForEachTunneling<PlusStateDataItem>(y => y.State = DataItemState.New);
			new$Item$.Accept();
			$item$s.Add($item$);

			return CallResponse.FromSuccessfulResult(new$Item$);			
		}
		
		#endregion Clone
		
		#region Save
		
		/// <summary>
		/// Saves the $Item$ asynchronous.
		/// </summary>
		/// <param name="callContext">The call context.</param>
		/// <param name="$Item$s">The $Item$s.</param>
		/// <returns></returns>
		public async Task<CallResponse<PlusObservableCollection<$Product$$Item$DataItem>>> Save$Item$Async(IRepositoryCallContext callContext, PlusObservableCollection<$Product$$Item$DataItem> $item$s)
		{
			try
			{
				List<$Product$$Item$> $item$Dtos = new List<$Product$$Item$>();
				
				//Add all deleted elements
				foreach ($Product$$Item$DataItem $item$DataItem in $item$s.DeletedItems)
				{
					$Product$$Item$ $item$ = _$product$DtoFactory.Create$Item$FromDataItem($item$DataItem);
					$item$.SaveFlag = SaveFlag.Delete;
					$item$Dtos.Add($item$);
				}

				//Add all new elements
				foreach ($Product$$Item$DataItem $item$DataItem in $item$s.Where(x => x.State == DataItemState.New))
				{
					$Product$$Item$ create$Item$ = _$product$DtoFactory.Create$Item$FromDataItem($item$DataItem);
					create$Item$.SaveFlag = SaveFlag.New;
					$item$Dtos.Add(create$Item$);
				}
				
				CallResponse<IList<$Product$$Item$>> response = await _$item$Service.Save$Item$sAsync(callContext, $item$Dtos);
				if (response.IsSuccess)
				{
					foreach ($Product$$Item$ dto in  dto in response.Result)
					{
						$Product$$Item$DataItem dataItem = $item$DataItems.FirstOrDefault(x => $specialContent$);
						if (dataItem != null)
						{
							$specialContent2$
							dataItem.LupdUser = dto.LupdUser;
							dataItem.LupdTimestamp = dto.LupdTimestamp;
							dataItem.State = DataItemState.Persistent;
						}
					}
					
					checkpointReferences.AcceptDeep();

					return CallResponse.FromSuccessfulResult($item$s);
				}

				return CallResponse.FromFailedResult<PlusObservableCollection<$Product$$Item$DataItem>>(null);
			}
			catch (Exception ex)
			{
				return CallResponse.FromException<PlusObservableCollection<$Product$$Item$DataItem>>(ex);
			}
		}
		
		#endregion Save
	}
}
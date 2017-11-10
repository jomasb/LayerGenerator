using System.Collections.Generic;
using System.Threading.Tasks;
using Dcx.Plus.Infrastructure.Contracts.Application;
using Dcx.Plus.Repository.FW.Collections;
using Dcx.Plus.Repository.FW.Contracts;
using Dcx.Plus.Repository.Modules.$product$.Contracts.DataItems;

namespace Dcx.Plus.Repository.Modules.$product$.Contracts
{
	public interface I$product$$dialog$Repository
	{
		#region Get
		
		Task<CallResponse<IList<$product$$item$DataItem>>> Get$item$sAsync(IRepositoryCallContext callContext);

		PlusLazyLoadingAsyncObservableCollection<$product$$item$DataItem> Get$item$s(IRepositoryCallContext callContext);
		
		#endregion Get
		
		#region Add
		
		Task<CallResponse<$product$$item$DataItem>> AddNew$item$Async(IRepositoryCallContext callContext, IObservableCollection<$product$$item$DataItem> $item$s);
		
		#endregion Add
		
		#region Delete
		
		Task<CallResponse<bool>> Delete$item$Async(IRepositoryCallContext callContext, $product$$item$DataItem $item$, PlusObservableCollection<$product$$item$DataItem> $item$s);
		
		#endregion Delete
		
		#region Clone
		
		Task<CallResponse<$product$$item$DataItem>> Clone$item$Async(IRepositoryCallContext callContext, $product$$item$DataItem $item$, PlusObservableCollection<$product$$item$DataItem> $item$s);
		
		#endregion Clone
		
		#region Save
		
		Task<CallResponse<PlusObservableCollection<$product$$item$DataItem>>> Save$item$Async(IRepositoryCallContext callContext, PlusObservableCollection<$product$$item$DataItem> $item$s);
		
		#endregion Save
	}
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Dcx.Plus.Infrastructure.Contracts.Application;
using Dcx.Plus.Repository.FW.Collections;
using Dcx.Plus.Repository.FW.Contracts;
using Dcx.Plus.Repository.Modules.$Product$.Contracts.DataItems;

namespace Dcx.Plus.Repository.Modules.$Product$.Contracts
{
	public interface I$Product$$Dialog$Repository
	{
		#region Get
		
		Task<CallResponse<IList<$Product$$Item$DataItem>>> Get$Item$sAsync(IRepositoryCallContext callContext);

		PlusLazyLoadingAsyncObservableCollection<$Product$$Item$DataItem> Get$Item$s(IRepositoryCallContext callContext);
		
		#endregion Get
		
		#region Add
		
		Task<CallResponse<$Product$$Item$DataItem>> AddNew$Item$Async(IRepositoryCallContext callContext, IObservableCollection<$Product$$Item$DataItem> $item$s);
		
		#endregion Add
		
		#region Delete
		
		Task<CallResponse<bool>> Delete$Item$Async(IRepositoryCallContext callContext, $Product$$Item$DataItem $item$, PlusObservableCollection<$Product$$Item$DataItem> $item$s);
		
		#endregion Delete
		
		#region Clone
		
		Task<CallResponse<$Product$$Item$DataItem>> Clone$Item$Async(IRepositoryCallContext callContext, $Product$$Item$DataItem $item$, PlusObservableCollection<$Product$$Item$DataItem> $item$s);
		
		#endregion Clone
		
		#region Save
		
		Task<CallResponse<PlusObservableCollection<$Product$$Item$DataItem>>> Save$Item$Async(IRepositoryCallContext callContext, PlusObservableCollection<$Product$$Item$DataItem> $item$s);
		
		#endregion Save
	}
}
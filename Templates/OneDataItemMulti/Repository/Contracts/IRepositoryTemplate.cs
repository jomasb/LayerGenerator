using System.Collections.Generic;
using System.Threading.Tasks;
using Dcx.Plus.Infrastructure.Contracts.Application;
using Dcx.Plus.Repository.FW.Collections;
using Dcx.Plus.Repository.FW.Contracts;
using Dcx.Plus.Repository.Modules.$Product$.Contracts.DataItems;

namespace Dcx.Plus.Repository.Modules.$Product$.Contracts
{
	/// <summary>
	/// Defines the interface of the repository of the $Product$ module that handles information about $Item$.
	/// </summary>
	/// <author></author>
	/// <company>abat+ GmbH</company>
	/// <date></date>
	public interface I$Product$$Dialog$Repository
	{
		#region Get
		
		/// <summary>
		/// Gets the checkpoint references asynchronous.
		/// </summary>
		/// <param name="callContext">The call context.</param>
		/// <returns></returns>
		Task<CallResponse<IList<$Product$$Item$DataItem>>> Get$Item$sAsync(IRepositoryCallContext callContext);

		/// <summary>
		/// Gets the checkpoint references.
		/// </summary>
		/// <param name="callContext">The call context.</param>
		/// <returns></returns>
		PlusLazyLoadingAsyncObservableCollection<$Product$$Item$DataItem> Get$Item$s(IRepositoryCallContext callContext);
		
		#endregion Get
		
		#region Add
		
		/// <summary>
		/// Adds the new $Item$ asynchronous.
		/// </summary>
		/// <param name="callContext">The call context.</param>
		/// <param name="$Item$DataItems">The $Item$ data items.</param>
		/// <returns></returns>
		Task<CallResponse<$Product$$Item$DataItem>> AddNew$Item$Async(IRepositoryCallContext callContext, IObservableCollection<$Product$$Item$DataItem> $item$s);
		
		#endregion Add
		
		#region Delete
		
		/// <summary>
		/// Deletes the $Item$ asynchronous.
		/// </summary>
		/// <param name="callContext">The call context.</param>
		/// <param name="$Item$">The $Item$.</param>
		/// <param name="$Item$s">The $Item$s.</param>
		/// <returns></returns>
		Task<CallResponse<bool>> Delete$Item$Async(IRepositoryCallContext callContext, $Product$$Item$DataItem $item$, PlusObservableCollection<$Product$$Item$DataItem> $item$s);
		
		#endregion Delete
		
		#region Clone
		
		/// <summary>
		/// Clones the $Item$ asynchronous.
		/// </summary>
		/// <param name="callContext">The call context.</param>
		/// <param name="$Item$">The $Item$.</param>
		/// <param name="$Item$s">The $Item$s.</param>
		/// <returns></returns>
		Task<CallResponse<$Product$$Item$DataItem>> Clone$Item$Async(IRepositoryCallContext callContext, $Product$$Item$DataItem $item$, PlusObservableCollection<$Product$$Item$DataItem> $item$s);
		
		#endregion Clone
		
		#region Save
		
		/// <summary>
		/// Saves the $Item$ asynchronous.
		/// </summary>
		/// <param name="callContext">The call context.</param>
		/// <param name="$Item$s">The $Item$s.</param>
		/// <returns></returns>
		Task<CallResponse<PlusObservableCollection<$Product$$Item$DataItem>>> Save$Item$Async(IRepositoryCallContext callContext, PlusObservableCollection<$Product$$Item$DataItem> $item$s);
		
		#endregion Save
	}
}
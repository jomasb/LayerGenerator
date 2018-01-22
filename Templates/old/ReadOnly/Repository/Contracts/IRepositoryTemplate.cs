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
	}
}
#region Usings

using Dcx.Plus.Gateway.Modules.$Product$.Contracts.DTOs;
using Dcx.Plus.Repository.Modules.$Product$.Contracts.DataItems;

#endregion Usings

namespace Dcx.Plus.Repository.Modules.$Product$
{
	/// <summary>
	/// The dto factory for all $Product$ dtos.
	/// </summary>
	/// <author></author>
	/// <company>abat+ GmbH</company>
	/// <date></date>
	public class $Product$DtoFactory
	{
		/// <summary>
		/// Creates the $Product$$Item$ from dataItem.
		/// </summary>
		/// <param name="bo">The bo.</param>
		/// <returns></returns>
		public $Product$$Item$ Create$Item$FromDataItem($Product$$Item$DataItem dataItem)
		{
			$Product$$Item$ dto = new $Product$$Item$()
			{
				$specialContent$				
				LupdUser = dataItem.LupdUser,
				LupdTimestamp = dataItem.LupdTimestamp,
			};
			
			return dto;
		}
	}
}
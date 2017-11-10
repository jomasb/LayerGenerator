#region Usings

using Dcx.Plus.Gateway.Modules.$Product$.Contracts.DTOs;
using Dcx.Plus.Repository.FW;
using Dcx.Plus.Repository.Modules.$Product$.Contracts.DataItems;

#endregion Usings

namespace Dcx.Plus.Repository.Modules.$Product$
{
	/// <summary>
	/// The dataitem factory for all $Product$ dataitems.
	/// </summary>
	/// <author></author>
	/// <company>abat+ GmbH</company>
	/// <date></date>
	public class $Product$DataItemFactory : DataItemFactoryBase
	{
		/// <summary>
		/// Creates the $Product$$Item$DataItem from dto.
		/// </summary>
		/// <param name="do">The dto.</param>
		public $Product$$Item$DataItem Create$Item$FromDto($Product$$Item$ dto)
		{
			return FinalCreate(new $Product$$Item$DataItem()
			{
				$specialContent$
				LupdUser = dto.LupdUser,
				LupdTimestamp = dto.LupdTimestamp,
				State = DataItemState.Persistent
			});
		}
	}
}
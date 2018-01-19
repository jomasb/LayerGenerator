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
		
		private readonly I$Product$$Item$Service _$product$$Item$Service;
		private readonly $Product$DataItemFactory _$product$DataItemFactory;
		private readonly $Product$DtoFactory _$product$DtoFactory;
		
		#endregion Members

		#region Construction
		
		public $Product$$Dialog$Repository(I$Product$$Item$Service $product$$Item$Service)
		{
			_$product$$Item$Service = $product$$Item$Service;
			_$product$DataItemFactory = new $Product$DataItemFactory();
			_$product$DtoFactory = new $Product$DtoFactory();
		}
		
		#endregion Construction
		
		$specialContent1$
	}
}
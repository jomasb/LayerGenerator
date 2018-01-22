#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dcx.Plus.BusinessServiceLocal.FW;
using Dcx.Plus.BusinessServiceLocal.FW.Contracts;
using Dcx.Plus.BusinessServiceLocal.Modules.$Product$.Contracts;
using Dcx.Plus.BusinessServiceLocal.Modules.$Product$.Contracts.DTOs;
using Dcx.Plus.Infrastructure.Base;
using Dcx.Plus.Infrastructure.Contracts.Application;
using Dcx.Plus.Infrastructure.Logging.Contracts.Trace;
using SaveFlag = Dcx.Plus.Gateway.FW.Contracts.SaveFlag;

#endregion Usings

namespace Dcx.Plus.BusinessServiceLocal.Modules.$Product$
{
	/// <summary>
	/// Implements the business service local of the $Product$ module that handles information about $Item$s.
	/// This service works without Business Objects!!!.
	/// </summary>
	/// <author></author>
	/// <company>abat+ GmbH</company>
	/// <date></date>
	[ImplementationOf(typeof(I$Product$$Item$Service))]
	public class $Product$$Item$Service : BusinessServiceLocalBaseNoBOs<I$Product$$Item$Service>, I$Product$$Item$Service
	{
		#region Members
		
		private IList<$Product$$Item$> _$item$s;

		#endregion Members

		#region Get

		$specialContent1$

		#endregion Get

		#region Save

		$specialContent2$

		#endregion Save

		#region DataProvider

		/// <summary>
		/// Gets or sets the $product$ $item$s.
		/// </summary>
		/// <value>
		/// The $product$ $item$s.
		/// </value>
		public IList<$Product$$Item$> $Product$$Item$s
		{
			get
			{
				return _$item$s ?? (_$item$s = new List<$Product$$Item$>());
			}
			set
			{
				_$item$s = value;
			}
		}

		/// <summary>
		/// Gets or sets the maximum lupd timestamp.
		/// </summary>
		/// <value>
		/// The maximum lupd timestamp.
		/// </value>
		public DateTime MaxLupdTimestamp
		{
			get;
			set;
		}

		#endregion DataProvider
	}
}
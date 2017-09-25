using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dcx.Plus.Business.Modules.$Product$.Contracts;
using Dcx.Plus.Gateway.Modules.$Product$.Contracts;

namespace Dcx.Plus.Gateway.Modules.$Product$
{
	/// <summary>
	/// The dto factory for all $Product$ dtos.
	/// </summary>
	/// <author></author>
	/// <company>abat+ GmbH</company>
	/// <date></date>
	internal class $Product$DtoFactory
	{
		/// <summary>
		/// Creates the $Product$$Item$ from bo.
		/// </summary>
		/// <param name="bo">The bo.</param>
		/// <returns></returns>
		public static $Product$$Item$ Create$Item$FromBO(I$Product$$Item$ bo)
		{
			$Product$$Item$ dto = new $Product$$Item$()
			{
				$specialContent$
			};

			return dto;
		}
	}
}

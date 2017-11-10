using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dcx.Plus.Business.Modules.$product$.Contracts;
using Dcx.Plus.Gateway.Modules.$product$.Contracts;

namespace Dcx.Plus.Gateway.Modules.$product$
{
	internal class $product$DtoFactory
	{
		public static $product$$item$ Create$item$FromBO(I$product$$item$ bo)
		{
			$product$$item$ dto = new $product$$item$()
			{
				
			};

			return dto;
		}
	}
}

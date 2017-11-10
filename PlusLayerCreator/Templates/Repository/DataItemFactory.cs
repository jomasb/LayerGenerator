#region Usings

using Dcx.Plus.Gateway.Modules.$product$.Contracts;
using Dcx.Plus.Gateway.Modules.$product$.Contracts.DTOs;
using Dcx.Plus.Repository.FW;
using Dcx.Plus.Repository.FW.Contracts;
using Dcx.Plus.Repository.FW.Validation.Attributes;
using Dcx.Plus.Repository.Modules.$product$.Contracts.DataItems;
using System;
using System.Collections.Generic;
using System.Linq;
using Dcx.Plus.Repository.Modules.$product$.Contracts;
using Dcx.Plus.Repository.Modules.$product$.Contracts.Enums;

#endregion Usings

namespace Dcx.Plus.Repository.Modules.$product$
{
	public class $product$DataItemFactory : DataItemFactoryBase
	{
		public $product$$item$DataItem Create$item$FromDto($product$$item$ dto)
		{
			return FinalCreate(new $product$$item$DataItem()
			{
				
				
				State = DataIte$product$ate.Persistent
			});
		}
	}
}
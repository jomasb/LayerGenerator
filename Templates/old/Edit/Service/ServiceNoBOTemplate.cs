#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dcx.Plus.BusinessServiceLocal.FW;
using Dcx.Plus.BusinessServiceLocal.FW.Contracts;
using Dcx.Plus.BusinessServiceLocal.Modules.$Product$.Contracts;
using Dcx.Plus.Gateway.Modules.$Product$.Contracts.DTOs;
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
	public class $Product$$Item$Service : BusinessServiceLocalBase<I$Product$$Item$Service>, I$Product$$Item$Service
	{
		#region Members
		
		private IList<$Product$$Item$> _$item$s;

		#endregion Members

		#region Get

		/// <summary>
		/// Gets the $item$s asynchronous.
		/// </summary>
		/// <param name="serviceCallContext">The service call context.</param>
		/// <returns></returns>
		public Task<CallResponse<IList<$Product$$Item$>>> Get$Item$sAsync(IServiceCallContext serviceCallContext)
		{
			ServiceCallContext = serviceCallContext;
			return GetGatewayResponseAsync(() => Get$Item$s(serviceCallContext), serviceCallContext);
		}

		#endregion Get

		#region Save

		/// <summary>
		/// Saves the $item$s asynchronous.
		/// </summary>
		/// <param name="serviceCallContext">The service call context.</param>
		/// <param name="$item$Dtos">The checkpoint reference dtos.</param>
		/// <returns></returns>
		public Task<CallResponse<IList<$Product$$Item$>>> Save$Item$sAsync(IServiceCallContext serviceCallContext, IList<$Product$$Item$> $item$Dtos)
		{
			ServiceCallContext = serviceCallContext;
			return GetGatewayResponseAsync(() => Save$Item$s($item$Dtos), serviceCallContext);
		}

		#endregion Save

		#region Getaway Code


		#region Get

		/// <summary>
		/// Gets the $item$s.
		/// </summary>
		/// <param name="callContext">The call context.</param>
		/// <returns></returns>
		private CallResponse<IList<$Product$$Item$>> Get$Item$s(IServiceCallContext callContext)
		{
			using (TraceLog.Log(this, options: TraceLogOptions.All))
			{		
				$Product$$Item$s.Clear();

				IDictionary<string, object> arguments = new Dictionary<string, object>();
				if (!DataAdapter.Execute<I$Product$$Item$Service>(this, "Get$Item$List", ref arguments))
				{
					return CallResponse.FromFailedResult($Product$$Item$s);
				}
				
				return CallResponse.FromSuccessfulResult($Product$$Item$s);
			}
		}

		#endregion Get

		#region Save

		/// <summary>
		/// Saves the $item$s.
		/// </summary>
		/// <param name="$item$Dtos">The checkpoint reference dtos.</param>
		/// <returns></returns>
		private CallResponse<IList<$Product$$Item$>> Save$Item$s(IList<$Product$$Item$> $item$Dtos)
		{
			using (TraceLog.Log(this, options: TraceLogOptions.All))
			{
				$Product$$Item$s.Clear();
				IDictionary<string, object> arguments = new Dictionary<string, object>();

				//filter the new and deleted $Item$s
				var $item$s = from $item$Dto in $item$Dtos
										   where $item$Dto.SaveFlag.Equals(SaveFlag.New) || $item$Dto.SaveFlag.Equals(SaveFlag.Delete) || $item$Dto.SaveFlag.Equals(SaveFlag.Modified)
										   select $item$Dto;

				var $item$sToSave = $item$s as IList<$Product$$Item$> ?? $item$s.ToList();

				arguments.Add("$Item$sToSave", $item$sToSave);

				if (!DataAdapter.Execute<I$Product$$Item$Service>(this, "Save$Item$List", ref arguments))
				{
					return CallResponse.FromFailedResult($Product$$Item$s);
				}

				return CallResponse.FromSuccessfulResult($Product$$Item$s);
			}
		}

		#endregion Save

		#endregion
		
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
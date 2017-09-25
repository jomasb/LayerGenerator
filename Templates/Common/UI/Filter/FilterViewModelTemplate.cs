using System;
using System.Linq;
using Dcx.Plus.Localization;
using Dcx.Plus.Repository.Modules.$Product$.Contracts.DataItems;
using Dcx.Plus.UI.WPF.FW.Shell.Infrastructure;
using Dcx.Plus.UI.WPF.FW.Shell.Infrastructure.Filter;
using Dcx.Plus.UI.WPF.FW.Shell.Infrastructure.ViewModels;
using Dcx.Plus.UI.WPF.FW.Shell.Interfaces;

namespace Dcx.Plus.UI.WPF.Modules.$Product$.Windows.$Dialog$.Regions.Filter
{
	/// <summary>
	/// Definition of the view model for the filter view.
	/// </summary>
	/// <seealso cref="Dcx.Plus.UI.WPF.FW.Shell.Infrastructure.RegionViewModelBase" />
	/// <author></author>
	/// <company>abat+ GmbH</company>
	/// <date></date>
	public class FilterViewModel : FilterRegionViewModelBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FilterViewModel"/> class.
		/// </summary>
		/// <param name="baseServices">The base services.</param>
		public FilterViewModel(IViewModelBaseServices baseServices,
			IFilterSourceProvider<$Product$$Item$DataItem> $product$$Item$DataItemFilterSourceProvider)
			: base(baseServices)
		{
			DisplayName = GlobalLocalizer.Singleton.Global_lblFilter.Translation;
			$Product$$Item$DataItemFilterViewModel = new $Product$$Item$DataItemFilterViewModel($product$$Item$DataItemFilterSourceProvider);
		}

		/// <summary>
		/// Gets or sets the $Product$$Item$ data item filter view model.
		/// </summary>
		/// <value>
		/// The $Product$$Item$ data item filter view model.
		/// </value>
		public $Product$$Item$DataItemFilterViewModel $Product$$Item$DataItemFilterViewModel
		{
			get;
			set;
		}
	}
	
	/// <summary>
	/// Definition of the view model for $Product$$Item$DataItemFilter.
	/// </summary>
	/// <seealso cref="Dcx.Plus.UI.WPF.FW.Shell.Infrastructure.FilterViewModelBase<T>" />
	/// <author></author>
	/// <company>abat+ GmbH</company>
	/// <date></date>
	public class $Product$$Item$DataItemFilterViewModel : FilterViewModelBase<$Product$$Item$DataItem>
	{
		$filterMembers$
		
		public $Product$$Item$DataItemFilterViewModel(IFilterSourceProvider<$Product$$Item$DataItem> filterSourceProvider)
			: base(filterSourceProvider)
		{
		}
		
		/// <summary>
		/// Builds the filter.
		/// </summary>
		/// <returns></returns>
		protected override Predicate<$Product$$Item$DataItem> BuildFilter()
		{
			var filterBuilder = new FilterBuilder<$Product$$Item$DataItem, $Product$$Item$DataItemFilterViewModel>(this);

			var filter = filterBuilder
				$filterPredicates$
				.Build();

			return filter;
		}
		
		/// <summary>
		/// Called when /[source collection changed].
		/// </summary>
		protected override void OnSourceCollectionChanged()
		{
			$filterPredicateReset$

			$filterMultiSelectorsInitialize$
		}
		
		$filterProperties$
	}
}
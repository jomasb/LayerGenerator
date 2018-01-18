using Dcx.Plus.Infrastructure.Base;
using Dcx.Plus.Repository.Modules.$Product$.Contracts;
using Dcx.Plus.Repository.Modules.$Product$.DataItems;
using Dcx.Plus.UI.WPF.FW.Shell.Infrastructure;
using Dcx.Plus.UI.WPF.FW.Shell.Infrastructure.Filter;
using Dcx.Plus.UI.WPF.FW.Shell.Views;
using Dcx.Plus.UI.WPF.Modules.$Product$.Windows.$Dialog$.Infrastructure;
using Dcx.Plus.UI.WPF.Modules.$Product$.Windows.$Dialog$.Regions.Detail;
using Dcx.Plus.UI.WPF.Modules.$Product$.Windows.$Dialog$.Regions.Filter;
using Dcx.Plus.UI.WPF.Modules.$Product$.Windows.$Dialog$.Regions.Master;
using Dcx.Plus.UI.WPF.Modules.$Product$.Windows.$Dialog$.Regions.StatusBar;
using Dcx.Plus.UI.WPF.Modules.$Product$.Windows.$Dialog$.Regions.ToolBar;
using Dcx.Plus.UI.WPF.Modules.$Product$.Windows.AdminOfGeometries.Regions.Master;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;

namespace Dcx.Plus.UI.WPF.Modules.$Product$.Windows.$Dialog$
{
	/// <summary>
	/// Represents the PRISM module for this dialog.
	/// </summary>
	/// <author></author>
	/// <company>abat+ GmbH</company>
	/// <date></date>
	/// <seealso cref="Prism.Modularity.IModule" />
	public class $Dialog$Module : IModule
	{
		private readonly IUnityContainer _container;
		private readonly IRegionManager _regionManager;

		/// <summary>
		/// Initializes a new instance of the <see cref="$Dialog$Module"/> class.
		/// </summary>
		/// <param name="container">The container.</param>
		/// <param name="regionManager">The region manager.</param>
		public $Dialog$Module(IUnityContainer container, IRegionManager regionManager)
		{
			_container = container;
			_regionManager = regionManager;
		}

		/// <summary>
		/// Notifies the module that it has be initialized.
		/// </summary>
		public void Initialize()
		{
			var $dialog$Repository =
				DependencyInjection.Resolve<I$Product$$Dialog$Repository>();

			//RegisterTypes
			_container.RegisterInstance($dialog$Repository, new ContainerControlledLifetimeManager());
			_container.RegisterType<IFilterSourceProvider<$Product$$Item$DataItem>, $Dialog$MasterViewModel>(new ContainerControlledLifetimeManager());

			// StaticViews
			_regionManager.RegisterViewWithRegion(RegionNames.MasterRegion,
				() => _container.Resolve<$Dialog$MasterView>());

			_regionManager.RegisterViewWithRegion(RegionNames.FilterRegion,
				() => _container.Resolve<FilterView>());

			_regionManager.RegisterViewWithRegion(RegionNames.ToolbarRegion,
				() => _container.Resolve<ToolbarView>());

			_regionManager.RegisterViewWithRegion(RegionNames.StatusBarRegion,
				() => _container.Resolve<StatusbarView>());

			_regionManager.RegisterViewWithRegion(RegionNames.ApplicationMenuRegion, () => _container.Resolve<
				ApplicationMenuView>());

			// DynamicViews
			$specialContent$

		}
	}
}
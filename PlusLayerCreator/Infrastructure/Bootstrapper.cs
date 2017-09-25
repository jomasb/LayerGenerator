using System;
using System.Windows;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Unity;

namespace PlusLayerCreator.Infrastructure
{
	public class Bootstrapper : UnityBootstrapper
	{
		protected override DependencyObject CreateShell()
		{
			return Container.Resolve<MainWindow>();
		}
		protected override void InitializeShell()
		{
			base.InitializeShell();
			App.Current.MainWindow = (Window)Shell;
			App.Current.MainWindow.Show();

			//App.Current.MainWindow = new Shell((RegionHost)Shell);
			//App.Current.MainWindow.Show();

		}
		protected override void ConfigureContainer()
		{
			base.ConfigureContainer();
			//Setup ViewModelLocator

			ViewModelLocationProvider.SetDefaultViewModelFactory(type => Container.Resolve(type));
			ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(viewType =>
			{
				string viewName = viewType.FullName;
				string viewModelName = viewName.Replace("Views", "ViewModels") + "Model";
				return Type.GetType(viewModelName);
			});
		}

		protected override IModuleCatalog CreateModuleCatalog()
		{
			var catalog = new ModuleCatalog();
			catalog.AddModule(typeof(Module));
			return catalog;
		}
	}
}

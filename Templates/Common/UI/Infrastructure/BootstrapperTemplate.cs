using System;
using Dcx.Plus.UI.FAT;
using Dcx.Plus.UI.WPF.FW.Shell.Infrastructure;
using Microsoft.Practices.Unity;
using Prism.Mvvm;

namespace Dcx.Plus.UI.WPF.Modules.$Product$.Windows.$Dialog$.Infrastructure
{
	/// <summary>
	/// The bootstrapper that creates the user interface parts for the administration dialog
	/// </summary>
	/// <seealso cref="Prism.Unity.UnityBootstrapper"/>
	/// <author></author>
	/// <company>abat+ GmbH</company>
	/// <date></date>
	public class Bootstrapper : Bootstrapper<AdministrationOfCheckpointSystemsWindow, AdministrationOfCheckpointSystemsModule>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Bootstrapper"/> class.
		/// </summary>
		/// <param name="parentChildController">
		/// The parent child controller that represents the dialog.
		/// </param>
		public Bootstrapper(IPlusParentChildController parentChildController)
			: base(parentChildController)
		{
		}

		/// <summary>
		/// Configures the <see cref="T:Microsoft.Practices.Unity.IUnityContainer"/>. May be
		/// overwritten in a derived class to add specific type mappings required by the application.
		/// </summary>
		protected override void ConfigureContainer()
		{
			base.ConfigureContainer();

			//Setup ViewModelLocator
			ViewModelLocationProvider.SetDefaultViewModelFactory(type => Container.Resolve(type));
			ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(viewType =>
			{
				string viewName = viewType.FullName;
				string viewModelName = viewName + "Model";
				return Type.GetType(viewModelName);
			});
		}
	}
}
using Dcx.Plus.UI.FAT;
using Dcx.Plus.UI.WPF.FW.Shell.Controllers;
using Dcx.Plus.UI.WPF.FW.Shell.Interfaces;
using Dcx.Plus.UI.WPF.Modules.$Product$.Windows.$Dialog$.Infrastructure;
using Microsoft.Practices.Unity;

namespace Dcx.Plus.UI.WPF.Modules.$Product$.Windows.$Dialog$
{
	/// <summary>
	/// The controller for the ...
	/// </summary>
	/// <author>jmaurer</author>
	/// <company>abat+ GmbH</company>
	/// <date>5/18/2017 10:01:21 AM</date>
	public class $Dialog$Controller : PlusRootController
	{
		#region Construction / Finalization

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="$Dialog$Controller"/> class.
		/// </summary>
		/// <param name="newParent">
		/// The <see cref="IPlusParentChildController"/> representing my parent controller.
		/// </param>
		/// <param name="logicalKey">
		/// The <see cref="PlusDialogControllerHandle"/> representing my logical dialog key.
		/// </param>
		/// <param name="dyingParent">
		/// The <see cref="IPlusParentChildController"/> representing my dyingParent.
		/// </param>
		/// <param name="parameter">
		/// The <see cref="PlusRootControllerParameter"/> representing my parameters.
		/// </param>
		protected $Dialog$Controller(IPlusParentChildController newParent,
			PlusDialogControllerHandle logicalKey,
			IPlusParentChildController dyingParent,
			IPlusControllerParameter parameter)
			: base(newParent, logicalKey, dyingParent, null)
		{
		}

		#endregion Construction / Finalization

		#region Intialization

		/// <summary>
		/// Creates the windows.
		/// </summary>
		protected override void CreateWindows()
		{
			CreateApp();

			var bootstrapper = new Bootstrapper(this);
			bootstrapper.Run();

			//ToDo: Previous Bootstrpping mechanics need to be integrated properly with Prism Bootstrapping

			var windowViewModel =
				CreateWindow(typeof($Dialog$Window),
					typeof($Dialog$ViewModel), bootstrapper.RootWindow, bootstrapper.Container);

			Window = windowViewModel.Key as $Dialog$Window;
			ViewModel = windowViewModel.Value as $Dialog$ViewModel;
			if (ViewModel != null)
			{
				ViewModel.BaseServices = bootstrapper.Container.Resolve<IViewModelBaseServices>();
				if (Window != null)
					Window.InitializeFinished(ViewModel);
			}
			if (Window != null)
			{
				Window.Show();
				Window.Activated += OnActivated;
			}
		}

		#endregion Intialization
	}
}
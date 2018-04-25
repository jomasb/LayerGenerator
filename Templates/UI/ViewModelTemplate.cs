using Dcx.Plus.Repository.Modules.Common.Contracts;
using Dcx.Plus.UI.FAT.Security;
using Dcx.Plus.UI.WPF.FW.Shell.Interfaces;
using Dcx.Plus.UI.WPF.FW.Shell.ViewModels;
using Microsoft.Practices.Unity;

namespace Dcx.Plus.UI.WPF.Modules.$Product$.Windows.$Dialog$
{
	/// <summary>
	/// Represents theview model for this dialog.
	/// </summary>
	/// <author></author>
	/// <company>abat+ GmbH</company>
	/// <date></date>
	/// <seealso cref="Dcx.Plus.UI.WPF.FW.Shell.ViewModels.PlusRootDialogViewModel" />
	/// <seealso cref="Dcx.Plus.UI.FAT.Security.IPlusFunctionalPermissionProvider" />
	public class $Dialog$ViewModel : PlusRootDialogViewModel, IPlusFunctionalPermissionProvider
	{
		#region Construction / Finalization

		/// <summary>
		/// Initializes a new instance of the <see cref="$Dialog$ViewModel"/> class.
		/// </summary>
		/// <param name="container">The container.</param>
		/// <param name="baseServices">The base services.</param>
		/// <param name="dialogBaseRepository">The dialog base repository.</param>
		public $Dialog$ViewModel(IUnityContainer container, IViewModelBaseServices baseServices,
			IDialogBaseRepository dialogBaseRepository)
			: base(container, baseServices, dialogBaseRepository)
		{
		}

		#endregion Construction / Finalization

		#region InitializeFinished

		/// <summary>
		/// Called when initialization has finished.
		/// </summary>
		public override void InitializeFinished()
		{
			base.InitializeFinished();
		}

		#endregion InitializeFinished

		#region IPlusFunctionalPermissionProvider Members

		/// <summary>
		/// Implement this method to define custom permissions of this provider. This method is
		/// invoked by the import permission process.
		/// </summary>
		/// <returns>The list of permissions provided by this instance.</returns>
		public PlusPermissionList GetPermissions()
		{
			PlusPermissionList list = new PlusPermissionList();
			return list;
		}

		/// <summary>
		/// Gets the resource identifying this permission provider class.
		/// </summary>
		/// <value>The permission resource.</value>
		public PlusPermissionResource PermissionResource
		{
			get
			{
				return null;
			}
		}

		#endregion IPlusFunctionalPermissionProvider Members
	}
}
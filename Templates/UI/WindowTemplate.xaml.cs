using System;
using System.Windows;
using Dcx.Plus.UI.FAT;
using Dcx.Plus.UI.WPF.FW.Shell.ViewModels;

namespace Dcx.Plus.UI.WPF.Modules.$Product$.Windows.$Dialog$
{
	/// <summary>
	/// Represents the view for this dialog.
	/// </summary>
	/// <company>abat+ GmbH</company>
	/// <seealso cref="Dcx.Plus.UI.WPF.FW.Shell.Views.PlusRootWindow" />
	public partial class $Dialog$Window
	{
		#region Construction / Finalization

		/// <summary>
		/// Initializes a new instance of the <see cref="$Dialog$Window"/> class.
		/// </summary>
		public $Dialog$Window() : base()
		{
			InitializeComponent();
		}

		#endregion Construction / Finalization

		#region Initialize

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		/// <param name="viewModel">The view model.</param>
		public override void Initialize(PlusRootViewModel viewModel)
		{
			base.Initialize(viewModel);
			ModalMode = PlusModalModeType.PlusModeless;
		}

		#endregion Initialize
	}
}
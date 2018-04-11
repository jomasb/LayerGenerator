using Dcx.Plus.UI.WPF.FW.Shell.Infrastructure.ViewModels;
using Dcx.Plus.UI.WPF.FW.Shell.Interfaces;

namespace Dcx.Plus.UI.WPF.Modules.$Product$.Windows.$Dialog$.Regions.Toolbar
{
	/// <summary>
	/// Definition of the view model for the toolbar.
	/// </summary>
	/// <seealso cref="RegionViewModelBase" />
	/// <author></author>
	/// <company>abat+ GmbH</company>
	/// <date></date>
	public class ToolbarViewModel : ToolbarViewModelBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ToolbarViewModelBase"/> class.
		/// </summary>
		/// <param name="baseServices">The base services.</param>
		public ToolbarViewModel(IViewModelBaseServices baseServices)
			: base(baseServices)
		{
		}
	}
}
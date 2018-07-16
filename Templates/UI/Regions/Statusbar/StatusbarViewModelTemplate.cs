using Dcx.Plus.UI.FAT;
using Dcx.Plus.UI.WPF.FW.Shell.Infrastructure.ViewModels;
using Dcx.Plus.UI.WPF.FW.Shell.Interfaces;
using Prism.Commands;

namespace Dcx.Plus.UI.WPF.Modules.$Product$.Windows.$Dialog$.Regions.Statusbar
{
	/// <summary>
	/// This class represents the view model of a user interface part that will be shown at the
	/// bottom of the dialog.
	/// </summary>
	/// <author></author>
	/// <company>abat+ GmbH</company>
	/// <date></date>
	public class StatusbarViewModel : StatusbarViewModelBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="StatusbarViewModel"/> class.
		/// </summary>
		/// <param name="baseServices">The base services.</param>
		/// <param name="messageListener">The message listener.</param>
		public StatusbarViewModel(IViewModelBaseServices baseServices, IMessageListener messageListener)
			: base(baseServices, messageListener)
		{
			$specialContent1$
		}
		
		#region Properties
		
		$specialContent2$
		
		#endregion Properties
		
		#region Command Handlers
		
		$specialContent3$
		
		#endregion Command Handlers
	}
}
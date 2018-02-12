using System.Windows;

namespace Dcx.Plus.UI.WPF.Modules.$Product$.Windows.$Dialog$.Regions.Master
{
    /// <summary>
    /// Interaction logic for $Dialog$MasterView.xaml
    /// </summary>
	/// <author></author>
	/// <company>abat+ GmbH</company>
	/// <date></date>
    public partial class $Dialog$MasterView
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="$Dialog$MasterView"/> class.
		/// </summary>
        public $Dialog$MasterView()
        {
            InitializeComponent();
			Loaded += OnLoaded;
		}

		/// <summary>
		///     When [loaded] GridViews ar injected as IColumnProvider into the ViewModel.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			var viewModel = DataContext as $Dialog$MasterViewModel;
			if (viewModel != null)
			{
				$specialContent1$
			}
		}
    }
}
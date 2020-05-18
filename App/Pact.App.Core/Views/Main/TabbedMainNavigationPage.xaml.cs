namespace Pact.App.Core.Views.Main
{
  using Xamarin.Forms;
  using Xamarin.Forms.Xaml;

  /// <summary>
  /// The tabbed main navigation page.
  /// </summary>
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class TabbedMainNavigationPage : TabbedPage
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="TabbedMainNavigationPage"/> class.
    /// </summary>
    public TabbedMainNavigationPage()
    {
      this.InitializeComponent();
      this.Children.Add(new DashboardPage { Title = "Dashboard" });
      this.Children.Add(new BotChatPage { Title = "Florence" });
    }
  }
}
using Pact.App.Core.ViewModels;

namespace Pact.App.Core.Views.Main
{
  using System.Windows.Input;

  using Pact.App.Core.Commands;

  using Xamarin.Forms;
  using Xamarin.Forms.Xaml;

  /// <summary>
  /// The master detail main page master.
  /// </summary>
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class MasterDetailMainPageMaster : ContentPage
  {
    /// <summary>
    /// The list view.
    /// </summary>
    public ListView ListView;

    /// <summary>
    /// Initializes a new instance of the <see cref="MasterDetailMainPageMaster"/> class.
    /// </summary>
    public MasterDetailMainPageMaster()
    {
      InitializeComponent();
      this.ListView = listView;
      this.BindingContext = new MasterDetailMainPageMasterViewModel();
    }
  }

  /// <summary>
  /// The master detail main page master view model.
  /// </summary>
  public class MasterDetailMainPageMasterViewModel : BaseViewModel
  {
    /// <summary>
    /// The logout.
    /// </summary>
    public ICommand Logout => new Logout();
  }
}
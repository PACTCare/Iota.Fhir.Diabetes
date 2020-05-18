namespace Pact.App.Core.Views.Main
{
  using System;
  using System.Windows.Input;

  using Pact.App.Core.Commands;

  using Xamarin.Forms;
  using Xamarin.Forms.Xaml;

  /// <summary>
  /// The master detail main page.
  /// </summary>
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class MasterDetailMainPage : MasterDetailPage
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="MasterDetailMainPage"/> class.
    /// </summary>
    public MasterDetailMainPage()
    {
      InitializeComponent();
      MasterPage.ListView.ItemSelected += this.ListViewItemSelected;
      this.MasterBehavior = MasterBehavior.Popover;
    }

    /// <summary>
    /// The list view_ item selected.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void ListViewItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
      if (!(e.SelectedItem is MasterDetailMainPageMenuItem item))
      {
        return;
      }

      var page = (Page)Activator.CreateInstance(item.TargetType);
      page.Title = item.Title;

      this.Detail = new NavigationPage(page);
      this.IsPresented = false;

      MasterPage.ListView.SelectedItem = null;
    }
  }
}
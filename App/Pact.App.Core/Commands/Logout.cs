using Xamarin.Essentials;

namespace Pact.App.Core.Commands
{
  using System;
  using System.Linq;
  using System.Windows.Input;

  using Pact.App.Core.Views;

  using Tangle.Net.Utils;

  using Xamarin.Forms;

  /// <summary>
  /// The logout.
  /// </summary>
  public class Logout : ICommand
  {
    /// <inheritdoc />
    public bool CanExecute(object parameter)
    {
      return true;
    }

    /// <inheritdoc />
    public void Execute(object parameter)
    {
      SecureStorage.RemoveAll();

      foreach (var key in (from property in Application.Current.Properties where InputValidator.IsTrytes(property.Key) select property.Key).ToList())
      {
        Application.Current.Properties.Remove(key);
      }

      Application.Current.MainPage = new NavigationPage(new AssistedLoginPage());
    }

    /// <inheritdoc />
    public event EventHandler CanExecuteChanged;
  }
}
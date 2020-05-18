namespace Pact.App.Core.ViewModels
{
  using System.ComponentModel;
  using System.Runtime.CompilerServices;

  using Pact.App.Core.Annotations;

  using Xamarin.Forms;

  public class BaseViewModel : INotifyPropertyChanged
  {
    private bool isBusy;

    public BaseViewModel()
    {
      this.Navigation = Application.Current.NavigationProxy;
    }

    public BaseViewModel(INavigation navigation)
    {
      this.Navigation = navigation;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public bool IsBusy
    {
      get => this.isBusy;
      set
      {
        this.isBusy = value;
        this.OnPropertyChanged();
      }
    }

    protected INavigation Navigation { get; }

    [NotifyPropertyChangedInvocator]
    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
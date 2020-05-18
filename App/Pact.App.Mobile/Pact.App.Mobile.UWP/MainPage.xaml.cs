namespace Pact.App.Mobile.UWP
{
  using System;
  using System.Threading.Tasks;

  using Windows.ApplicationModel.Background;

  /// <summary>
  /// The main page.
  /// </summary>
  public sealed partial class MainPage
  {
    private const string BackgroundTaskName = "UWPNotifications";

    /// <summary>
    /// Initializes a new instance of the <see cref="MainPage"/> class.
    /// </summary>
    public MainPage()
    {
      this.InitializeComponent();

      this.LoadApplication(new Mobile.App());
      if (this.IsRegistered())
      {
        this.Unregister();
      }

      this.RegisterAsync();
    }

    private bool IsRegistered()
    {
      var taskName = BackgroundTaskName;

      foreach (var task in BackgroundTaskRegistration.AllTasks)
      {
        if (task.Value.Name == taskName)
        {
          return true;
        }
      }

      return false;
    }

    private async void RegisterAsync()
    {
      BackgroundExecutionManager.RemoveAccess();

      var requestAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();

      var builder = new BackgroundTaskBuilder { Name = BackgroundTaskName, TaskEntryPoint = "UWPRuntimeComponent.SystemBackgroundTask" };

      // builder.SetTrigger(new SystemTrigger(SystemTriggerType.InternetAvailable, false));
      if (requestAccessStatus == BackgroundAccessStatus.AlwaysAllowed || requestAccessStatus == BackgroundAccessStatus.AllowedSubjectToSystemPolicy)
      {
        builder.SetTrigger(new TimeTrigger(15, false));
      }
    }

    private void Unregister()
    {
      var taskName = BackgroundTaskName;

      foreach (var task in BackgroundTaskRegistration.AllTasks)
      {
        if (task.Value.Name == taskName)
        {
          task.Value.Unregister(true);
        }
      }
    }
  }
}
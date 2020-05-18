using Android.App;
using Android.Content.PM;
using Android.OS;

namespace Pact.App.Android.Droid
{
  using System;
  using System.Data.SQLite;
  using System.IO;
  using System.Threading.Tasks;

  using global::Android.Content;

  using Pact.App.Android.Droid.Services;
  using Pact.App.Core.Services;
  using Pact.App.Core.Views.DetailPages;
  using Pact.App.Mobile;

  using Xamarin.Essentials;

  using Environment = System.Environment;

  [Activity(
    Label = "Pact.App.Android",
    Icon = "@mipmap/icon",
    Theme = "@style/MainTheme",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
  public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
  {
    protected override void OnCreate(Bundle savedInstanceState)
    {
      AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
      TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;

      TabLayoutResource = Resource.Layout.Tabbar;
      ToolbarResource = Resource.Layout.Toolbar;

      base.OnCreate(savedInstanceState);


      global::Xamarin.Auth.Presenters.XamarinAndroid.AuthenticationConfiguration.Init(this, savedInstanceState);
      global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
      ZXing.Net.Mobile.Forms.Android.Platform.Init();
      ZXing.Mobile.MobileBarcodeScanner.Initialize(this.Application);

      DependencyResolver.LocalStoragePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
      DependencyResolver.Modules.Add(new InjectionModule());
      DependencyResolver.Init();

      this.LoadApplication(new App());
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
    {
      global::ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    }

    private static void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
    {
      var newExc = new Exception("TaskSchedulerOnUnobservedTaskException", unobservedTaskExceptionEventArgs.Exception);
    }

    private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
    {
      var newExc = new Exception("CurrentDomainOnUnhandledException", unhandledExceptionEventArgs.ExceptionObject as Exception);
    }
  }
}
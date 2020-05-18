namespace Pact.App.Android.Droid.Services
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text.RegularExpressions;

  using global::Android.App;
  using global::Android.Content;
  using global::Android.Content.PM;
  using global::Android.OS;

  using Pact.App.Core.Services;
  using Pact.App.Core.Services.Authentication;

  [Activity(Label = "OAuthUrlInterceptor", NoHistory = true, LaunchMode = LaunchMode.SingleTop)]
  [IntentFilter(
    new[] { Intent.ActionView },
    Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
    DataSchemes = new[] { "android.app.pact.com" },
    DataPath = "/oauth2redirect")]
  public class OAuthUrlInterceptor : Activity
  {
    protected override void OnCreate(Bundle savedInstanceState)
    {
      base.OnCreate(savedInstanceState);

      // Convert Android.Net.Url to Uri
      var uri = new Uri(this.Intent.Data.ToString());
      var parameters = GetParams(uri.ToString());

      // Load redirectUrl page
      Authentication.Authenticator.OnPageLoading(uri);
      Authentication.GetAccessToken(parameters["code"]);

      var intent = new Intent(this, typeof(MainActivity));
      intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
      this.StartActivity(intent);

      this.Finish();
    }

    private static Dictionary<string, string> GetParams(string uri)
    {
      var matches = Regex.Matches(uri, @"[\?&](([^&=]+)=([^&=#]*))", RegexOptions.Compiled);
      return matches.ToDictionary(
        m => Uri.UnescapeDataString(m.Groups[2].Value),
        m => Uri.UnescapeDataString(m.Groups[3].Value)
      );
    }
  }
}
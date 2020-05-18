namespace Pact.App.Core.Services.Authentication
{
  using System;

  using RestSharp;

  using Xamarin.Auth;
  using Xamarin.Auth.Presenters;
  using Xamarin.Essentials;

  public static class Authentication
  {
    public static event EventHandler DeviceAdded;

    private static OAuth2Authenticator authenticator;

    public static OAuth2Authenticator Authenticator =>
      authenticator ?? (authenticator = new OAuth2Authenticator(
                          clientId: "",
                          scope: "",
                          authorizeUrl: new Uri("https://sandbox-api.dexcom.com/v2/oauth2/login"),
                          redirectUrl: new Uri("android.app.pact.com:/oauth2redirect"),
                          isUsingNativeUI: true));

    public static void GetAccessToken(string authCode)
    {
      var client = new RestClient("https://sandbox-api.dexcom.com/v2/oauth2/token");
      var request = new RestRequest(Method.POST);
      request.AddHeader("cache-control", "no-cache");
      request.AddHeader("content-type", "application/x-www-form-urlencoded");
      request.AddParameter("application/x-www-form-urlencoded", $"client_secret=xxxx&code={authCode}&grant_type=authorization_code&redirect_uri=android.app.pact.com:/oauth2redirect", ParameterType.RequestBody);
      var response = client.Execute<AccessTokenPayload>(request);

      AuthenticationStorage.SetTokenPayloadAsync(response.Data);
      DeviceAdded?.Invoke("Authentication", null);
    }

    public static void Login()
    {
      var presenter = new OAuthLoginPresenter();
      presenter.Login(Authenticator);
    }
  }
}
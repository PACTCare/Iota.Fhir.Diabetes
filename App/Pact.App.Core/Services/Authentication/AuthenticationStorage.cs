namespace Pact.App.Core.Services.Authentication
{
  using System.Threading.Tasks;

  using Newtonsoft.Json;

  using Xamarin.Essentials;

  public static class AuthenticationStorage
  {
    public static async Task<AccessTokenPayload> GetTokenPayloadAsync()
    {
      var accessTokenPayload = await SecureStorage.GetAsync("AccessToken");
      if (string.IsNullOrEmpty(accessTokenPayload))
      {
        return null;
      }

      return JsonConvert.DeserializeObject<AccessTokenPayload>(accessTokenPayload);
    }

    public static async void SetTokenPayloadAsync(AccessTokenPayload payload)
    {
      await SecureStorage.SetAsync("AccessToken", JsonConvert.SerializeObject(payload));
    }
  }
}
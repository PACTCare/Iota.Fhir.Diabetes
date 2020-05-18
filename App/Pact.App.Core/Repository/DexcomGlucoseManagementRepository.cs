namespace Pact.App.Core.Repository
{
  using System;
  using System.Collections.Generic;
  using System.Threading.Tasks;

  using Pact.App.Core.Models.Dexcom;
  using Pact.App.Core.Services.Authentication;

  using RestSharp;

  public class DexcomGlucoseManagementRepository : IGlucoseManagementRepository
  {
    public DexcomGlucoseManagementRepository(IRestClient client)
    {
      this.Client = client;
    }

    private IRestClient Client { get; }

    /// <inheritdoc />
    public async Task<GlucoseMeasurementList> LoadGlucoseDataAsync(DateTime @from, DateTime to)
    {
      var request = new RestRequest("/v2/users/self/egvs", Method.GET);
      request.AddParameter("startDate", from.ToString("s"));
      request.AddParameter("endDate", to.ToString("s"));
      await SetAuthorization(request);

      var response = await this.Client.ExecuteTaskAsync<GlucoseMeasurementList>(request);
      if (response.IsSuccessful)
      {
        var measurementList = response.Data;
        measurementList.DeviceName = "Dexcom";
        return measurementList;
      }

      return new GlucoseMeasurementList { DeviceName = "Dexcom", Egvs = new List<GlucoseMeasurement>() };
    }

    private static async Task SetAuthorization(RestRequest request)
    {
      var accessTokenPayload = await AuthenticationStorage.GetTokenPayloadAsync();
      request.AddHeader("authorization", $"Bearer {accessTokenPayload.AccessToken}");
    }
  }
}
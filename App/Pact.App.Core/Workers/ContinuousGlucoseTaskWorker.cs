namespace Pact.App.Core.Workers
{
  using System;

  using Pact.App.Core.Repository;
  using Pact.App.Core.Services.Authentication;
  using Pact.App.Core.Services.Glucose;

  using Xamarin.Essentials;

  public class ContinuousGlucoseTaskWorker : ITaskWorker
  {
    private FhirGlucoseService GlucoseService { get; }
    private IGlucoseManagementRepository GlucoseRepository { get; }

    public ContinuousGlucoseTaskWorker(FhirGlucoseService glucoseService, IGlucoseManagementRepository glucoseRepository)
    {
      this.GlucoseService = glucoseService;
      this.GlucoseRepository = glucoseRepository;
    }

    /// <inheritdoc />
    public async void ProcessTask()
    {
      var shouldSyncGlucose = false;

      var syncGlucose = await SecureStorage.GetAsync("SyncGlucose");
      if (!string.IsNullOrEmpty(syncGlucose))
      {
        shouldSyncGlucose = bool.Parse(syncGlucose);
      }

      if (await AuthenticationStorage.GetTokenPayloadAsync() == null || !shouldSyncGlucose)
      {
        return;
      }

      var lastPocRun = await SecureStorage.GetAsync("LastGlucoseSyncPoCRun");
      if (string.IsNullOrEmpty(lastPocRun))
      {
        lastPocRun = DateTime.Now.AddMinutes(-6).ToString("s");
      }

      if (DateTime.Parse(lastPocRun) < DateTime.Now.AddMinutes(-5))
      {
        var lastSync = await SecureStorage.GetAsync("LastGlucoseSync");
        if (string.IsNullOrEmpty(lastSync))
        {
          lastSync = new DateTime(2015, 12, 12, 8, 0, 0).ToString("s");
        }

        var lastRun = DateTime.Parse(lastSync);
        var measurements = await this.GlucoseRepository.LoadGlucoseDataAsync(lastRun.AddMinutes(-5), lastRun.AddMinutes(10));

        await this.GlucoseService.UploadGlucoseDataAsync(measurements);

        await SecureStorage.SetAsync("LastGlucoseSync", lastRun.AddMinutes(10).ToString("s"));
        await SecureStorage.SetAsync("LastGlucoseSyncPoCRun", DateTime.Now.ToString("s"));
      }
    }
  }
}
using System;

namespace Pact.App.Core.Workers
{
  using Pact.App.Core.Repository;
  using Pact.App.Core.Services.Authentication;
  using Pact.App.Core.Services.Glucose;

  using Xamarin.Essentials;

  public class AggregateGlucoseTaskWorker : ITaskWorker
  {
    private FhirGlucoseService GlucoseService { get; }
    private IGlucoseManagementRepository GlucoseRepository { get; }

    public AggregateGlucoseTaskWorker(FhirGlucoseService glucoseService, IGlucoseManagementRepository glucoseRepository)
    {
      this.GlucoseService = glucoseService;
      this.GlucoseRepository = glucoseRepository;
    }

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

      var lastPocRun = await SecureStorage.GetAsync("LastDailyGlucoseSyncPoCRun");
      if (string.IsNullOrEmpty(lastPocRun))
      {
        lastPocRun = DateTime.Now.AddMinutes(-6).ToString("s");
      }

      if (DateTime.Parse(lastPocRun) < DateTime.Now.AddMinutes(-5))
      {
        var lastSync = await SecureStorage.GetAsync("LastDailyGlucoseSync");
        if (string.IsNullOrEmpty(lastSync))
        {
          lastSync = new DateTime(2015, 12, 12, 0, 0, 0).ToString("s");
        }

        var lastRun = DateTime.Parse(lastSync);
        var measurements = await this.GlucoseRepository.LoadGlucoseDataAsync(lastRun, lastRun.AddDays(1));

        await this.GlucoseService.UploadDaylieGlucoseReportAsync(measurements);

        await SecureStorage.SetAsync("LastDailyGlucoseSync", lastRun.AddDays(1).ToString("s"));
        await SecureStorage.SetAsync("LastDailyGlucoseSyncPoCRun", DateTime.Now.ToString("s"));
      }
    }
  }
}

namespace Pact.App.Core.Services.Glucose
{
  using System;
  using System.Linq;

  using Hl7.Fhir.Model;
  using Hl7.Fhir.Serialization;

  using Pact.App.Core.Models.Dexcom;
  using Pact.Fhir.Core.Repository;
  using Pact.Fhir.Core.Usecase.CreateResource;
  using Pact.Fhir.Core.Usecase.ReadResource;
  using Pact.Fhir.Core.Usecase.UpdateResource;

  using Xamarin.Essentials;

  using Task = System.Threading.Tasks.Task;

  public class FhirGlucoseService
  {
    public FhirGlucoseService(
      CreateResourceInteractor createResourceInteractor,
      UpdateResourceInteractor updateResourceInteractor,
      ReadResourceInteractor readResourceInteractor,
      ISearchRepository searchRepository)
    {
      this.CreateResourceInteractor = createResourceInteractor;
      this.UpdateResourceInteractor = updateResourceInteractor;
      this.ReadResourceInteractor = readResourceInteractor;
      this.SearchRepository = searchRepository;
    }

    private CreateResourceInteractor CreateResourceInteractor { get; }
    private ReadResourceInteractor ReadResourceInteractor { get; }
    private ISearchRepository SearchRepository { get; }
    private UpdateResourceInteractor UpdateResourceInteractor { get; }

    public async Task UploadDaylieGlucoseReportAsync(GlucoseMeasurementList measurementList)
    {
      var glucoseStreamId = await SecureStorage.GetAsync(measurementList.DeviceName + "Daily");
      var patients = await this.SearchRepository.FindResourcesByTypeAsync("Patient");
      var patient = patients.First();

      Observation latestEntry;
      var dailyReport = measurementList.ToDailyReport(patient.Id);

      if (string.IsNullOrEmpty(glucoseStreamId))
      {
        var response = await this.CreateResourceInteractor.ExecuteAsync(
                         new CreateResourceRequest { ResourceJson = dailyReport.ToJson() });
        latestEntry = response.Resource as Observation;

        await SecureStorage.SetAsync(measurementList.DeviceName + "Daily", latestEntry?.Id);
        return;
      }
      else
      {
        var response = await this.ReadResourceInteractor.ExecuteAsync(
                         new ReadResourceRequest { ResourceId = glucoseStreamId, ResourceType = "Observation" });
        latestEntry = response.Resource as Observation;
      }

      if (latestEntry?.Issued != null && dailyReport.Issued?.DateTime.Ticks > latestEntry?.Issued.Value.DateTime.Ticks)
      {
        dailyReport.Id = latestEntry?.Id;
        await this.UpdateResourceInteractor.ExecuteAsync(
          new UpdateResourceRequest { ResourceId = latestEntry?.Id, ResourceJson = dailyReport.ToJson() });
      }
    }

    public async Task UploadGlucoseDataAsync(GlucoseMeasurementList measurementList)
    {
      var glucoseStreamId = await SecureStorage.GetAsync(measurementList.DeviceName);
      var patients = await this.SearchRepository.FindResourcesByTypeAsync("Patient");
      var patient = patients.First();

      Observation latestEntry;

      if (string.IsNullOrEmpty(glucoseStreamId))
      {
        var response = await this.CreateResourceInteractor.ExecuteAsync(
                         new CreateResourceRequest { ResourceJson = measurementList.Egvs.First().ToObservation(patient.Id).ToJson() });
        latestEntry = response.Resource as Observation;

        await SecureStorage.SetAsync(measurementList.DeviceName, latestEntry?.Id);
      }
      else
      {
        var response = await this.ReadResourceInteractor.ExecuteAsync(
                         new ReadResourceRequest { ResourceId = glucoseStreamId, ResourceType = "Observation" });
        latestEntry = response.Resource as Observation;

        await SecureStorage.SetAsync(measurementList.DeviceName, latestEntry?.Id);
      }

      var measurementsToUpload = measurementList.Egvs.Where(e => latestEntry?.Issued != null && e.DisplayTime.Ticks > latestEntry?.Issued.Value.DateTime.Ticks).ToList();
      foreach (var measurement in measurementsToUpload)
      {
        var observation = measurement.ToObservation(patient.Id);
        observation.Id = latestEntry?.Id;

        await this.UpdateResourceInteractor.ExecuteAsync(
          new UpdateResourceRequest { ResourceId = latestEntry?.Id, ResourceJson = observation.ToJson() });
      }
    }
  }
}
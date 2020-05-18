namespace Pact.Fhir.Api.Controllers
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.Linq;
  using System.Threading.Tasks;

  using Hl7.Fhir.Model;

  using Microsoft.AspNetCore.Cors;
  using Microsoft.AspNetCore.Mvc;

  using Pact.Fhir.Api.Models;
  using Pact.Fhir.Api.Services;
  using Pact.Fhir.Core.Usecase;
  using Pact.Fhir.Core.Usecase.ReadResourceHistory;
  using Pact.Fhir.Core.Usecase.SearchResources;

  [EnableCors("Development")]
  [ApiController]
  public class GlucoseController : Controller
  {
    public GlucoseController(SearchResourcesInteractor searchResourcesInteractor, ReadResourceHistoryInteractor readResourceHistoryInteractor)
    {
      this.SearchResourcesInteractor = searchResourcesInteractor;
      this.ReadResourceHistoryInteractor = readResourceHistoryInteractor;
      this.Cache = new SqlLiteGlucoseCache();
    }

    private ReadResourceHistoryInteractor ReadResourceHistoryInteractor { get; }
    private SqlLiteGlucoseCache Cache { get; }
    private SearchResourcesInteractor SearchResourcesInteractor { get; }

    [Route("api/glucose/daily/{patientId}")]
    [HttpGet]
    public async Task<IActionResult> GetGlucoseDailyAsync(string patientId)
    {
      var observations = await this.GetObservations(patientId);
      if (observations == null)
      {
        return this.BadRequest("NoData");
      }

      var observationsInRange = observations.Skip(observations.Count - 15).Take(15);
      var report = (from observation in observationsInRange
                    let average = ((SimpleQuantity)observation.Extension[0].Value).Value
                    select new GlucoseDailyReport
                             {
                               Date = observation.Issued?.ToString("O"),
                               AverageGlucose = average.HasValue ? average.Value : 0M,
                               Indicator = average.HasValue ? this.CalculateIndicator(average.Value) : "On Target"
                             }).ToList();

      return this.Ok(report);
    }

    private string CalculateIndicator(decimal average)
    {
      if (average > 250)
      {
        return "Very High";
      }

      if (average > 180)
      {
        return "High";
      }

      if (average > 69)
      {
        return "On Target";
      }

      if (average > 49)
      {
        return "Low";
      }

      return "Very Low";
    }

    [Route("api/glucose/summary/{patientId}")]
    [HttpGet]
    public async Task<IActionResult> GetGlucoseSummaryAsync(
      string patientId,
      [FromQuery(Name = "from")] DateTime? from,
      [FromQuery(Name = "to")] DateTime? to)
    {
      var observationsInRange = FilterObservations(@from, to, await this.GetObservations(patientId));
      if (observationsInRange == null)
      {
        return this.BadRequest("NoData");
      }

      var averageGlucose = observationsInRange?.Average(o => ((SimpleQuantity)o.Extension[0].Value).Value);

      if (averageGlucose != null)
      {
        var veryHigh = observationsInRange.Sum(o => ((SimpleQuantity)o.Extension[2].Value).Value);
        var high = observationsInRange.Sum(o => ((SimpleQuantity)o.Extension[3].Value).Value);
        var onTarget = observationsInRange.Sum(o => ((SimpleQuantity)o.Extension[4].Value).Value);
        var low = observationsInRange.Sum(o => ((SimpleQuantity)o.Extension[5].Value).Value);
        var veryLow = observationsInRange.Sum(o => ((SimpleQuantity)o.Extension[6].Value).Value);

        var total = veryHigh + high + onTarget + low + veryLow;

        var fromDateTime = @from ?? observationsInRange.Select(o => o.Issued).Min().Value;
        var toDateTime = @to ?? observationsInRange.Select(o => o.Issued).Max().Value;

        var summary = new GlucoseSummary
                        {
                          FromDateTime = fromDateTime.ToString("yyyy-MM-dd"),
                          ToDateTime = toDateTime.ToString("yyyy-MM-dd"),
                          AverageGlucose = $"{averageGlucose.Value:F} mg/dL",
                          GlucoseManagementIndicator = $"{3.31M + 0.02392M * averageGlucose.Value:F} %",
                          Days = ((toDateTime - fromDateTime).TotalDays + 1).ToString(CultureInfo.InvariantCulture),
                          VeryHigh = $"{veryHigh / total:P1}",
                          High = $"{high / total:P1}",
                          OnTarget = $"{onTarget / total:P1}",
                          Low = $"{low / total:P1}",
                          VeryLow = $"{veryLow / total:P1}",
                          ReadingTimeDayOnTarget = FormatReadingTimeDay(onTarget.Value, total, "70%", 0.7M),
                          ReadingTimeDayLow = FormatReadingTimeDay(low.Value, total, "4%", 0.04M),
                          ReadingTimeDayVeryLow = FormatReadingTimeDay(veryLow.Value, total, "1%", 0.01M),
                          ReadingTimeDayVeryHigh = FormatReadingTimeDay(veryHigh.Value, total, "5%", 0.05M),
                        };

        return this.Ok(summary);
      }

      return this.BadRequest("GlucoseNull");
    }

    private async Task<List<Observation>> GetObservations(string patientId)
    {
      var searchResult = await this.SearchResourcesInteractor.ExecuteAsync(
                           new SearchResourcesRequest { Parameters = $"reference={patientId}", ResourceType = "Observation" });

      if (searchResult.Code != ResponseCode.Success)
      {
        return null;
      }

      var resource = ((Bundle)searchResult.Resource).Entry
        .FirstOrDefault(e => e.Resource is Observation observation && observation.Code.Text == "Dexcom Daily Glucose Measurement")?.Resource;
      if (!(resource is Observation glucoseObservation))
      {
        return null;
      }

      var historyBundle = await this.Cache.ReadDataAsync(resource.Id);
      if (historyBundle == null)
      {
        var observationHistoryResult = await this.ReadResourceHistoryInteractor.ExecuteAsync(
                                         new ReadResourceHistoryRequest
                                           {
                                             ResourceId = glucoseObservation.Id, ResourceType = glucoseObservation.TypeName
                                           });

        if (observationHistoryResult.Code != ResponseCode.Success)
        {
          return null;
        }

        historyBundle = observationHistoryResult.Resource as Bundle;
        await this.Cache.WriteDataAsync(resource.Id, historyBundle);
      }

      return historyBundle?.Entry.Select(e => e.Resource as Observation).ToList();
    }

    private static string FormatReadingTimeDay(decimal measurementAmount, decimal? total, string percentageFormatted, decimal percentage)
    {
      var readingTimeInMinutes = TimeSpan.FromMinutes((double)(measurementAmount * 5));
      var readingTimeFormatted = $"{readingTimeInMinutes:%d}d {readingTimeInMinutes:%h}hr {readingTimeInMinutes:%m} min";
      return measurementAmount / total > percentage
               ? $"Greater than {percentageFormatted} ({readingTimeFormatted})"
               : $"Less than {percentageFormatted} ({readingTimeFormatted})";
    }

    private static List<Observation> FilterObservations(DateTime? @from, DateTime? to, List<Observation> observations)
    {
      var fromDateTime = @from ?? observations.Select(o => o.Issued).Min().Value;
      var toDateTime = @to ?? observations.Select(o => o.Issued).Max().Value;

      if (to.HasValue && @from.HasValue)
      {
        observations = observations.Where(o => o.Issued >= fromDateTime && o.Issued <= toDateTime).ToList();
      }
      else if (!to.HasValue && from.HasValue)
      {
        observations = observations.Where(o => o.Issued >= fromDateTime).ToList();
      }

      return observations;
    }
  }
}
namespace Pact.Fhir.Api.Models
{
  public class GlucoseSummary
  {
    public string FromDateTime { get; set; }
    public string ToDateTime { get; set; }
    public string AverageGlucose { get; set; }
    public string Days { get; set; }
    public string GlucoseManagementIndicator { get; set; }
    public string High { get; set; }
    public string Low { get; set; }
    public string OnTarget { get; set; }
    public string VeryHigh { get; set; }
    public string VeryLow { get; set; }
    public string ReadingTimeDayOnTarget { get; set; }
    public string ReadingTimeDayLow { get; set; }
    public string ReadingTimeDayVeryLow { get; set; }
    public string ReadingTimeDayVeryHigh { get; set; }
  }
}
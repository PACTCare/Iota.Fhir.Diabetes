namespace Pact.Fhir.Api.Models
{
  public class GlucoseDailyReport
  {
    public decimal AverageGlucose { get; set; }
    public string Date { get; set; }
    public string Indicator { get; set; }
  }
}
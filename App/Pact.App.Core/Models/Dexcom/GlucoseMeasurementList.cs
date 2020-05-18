namespace Pact.App.Core.Models.Dexcom
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  using Hl7.Fhir.Model;

  using Newtonsoft.Json;

  public class GlucoseMeasurementList
  {
    public string DeviceName { get; set; }

    [JsonProperty("egvs")]
    public List<GlucoseMeasurement> Egvs { get; set; }

    [JsonProperty("rateUnit")]
    public string RateUnit { get; set; }

    [JsonProperty("unit")]
    public string Unit { get; set; }

    public Observation ToDailyReport(string patientId)
    {
      return new Observation
               {
                 Identifier =
                   new List<Identifier>
                     {
                       new Identifier
                         {
                           System = "http://www.bmc.nl/zorgportal/identifiers/observations", Value = "6323", Use = Identifier.IdentifierUse.Official
                         }
                     },
                 Status = ObservationStatus.Final,
                 Subject = new ResourceReference($"did:iota:{patientId}"),
                 Code =
                   new CodeableConcept
                     {
                       Coding = new List<Coding>
                                  {
                                    new Coding { System = "http://loinc.org", Code = "15074-8", Display = "Daily Glucose CGM Report" }
                                  },
                       Text = "Dexcom Daily Glucose Measurement"
                     },
                 Issued =
                   new DateTimeOffset(
                     new DateTime(this.Egvs.First().DisplayTime.Year, this.Egvs.First().DisplayTime.Month, this.Egvs.First().DisplayTime.Day)),
                 Effective =
                   new Period
                     {
                       StartElement = new FhirDateTime(
                         new DateTimeOffset(
                           new DateTime(this.Egvs.First().DisplayTime.Year, this.Egvs.First().DisplayTime.Month, this.Egvs.First().DisplayTime.Day)))
                     },
                 Extension = new List<Extension>
                               {
                                 new Extension
                                   {
                                     Value = new SimpleQuantity
                                               {
                                                 Value = this.CalculateAverageGlucose(),
                                                 Unit = "mg/dL",
                                                 UnitElement = new FhirString("Average Glucose")
                                               }
                                   },
                                 new Extension
                                   {
                                     Value = new SimpleQuantity
                                               {
                                                 Value = this.CalculateGlucoseManagementIndicator(),
                                                 Unit = "%",
                                                 UnitElement = new FhirString("Glucose Management Indicator")
                                               }
                                   },
                                 new Extension
                                   {
                                     Value = new SimpleQuantity
                                               {
                                                 Value = this.Egvs.Count(e => e.Value > 250f),
                                                 Unit = ">250 mg/dL",
                                                 UnitElement = new FhirString("Very High")
                                               }
                                   },
                                 new Extension
                                   {
                                     Value = new SimpleQuantity
                                               {
                                                 Value = this.Egvs.Count(e => e.Value > 180f && e.Value < 251f), UnitElement = new FhirString("High")
                                               }
                                   },
                                 new Extension
                                   {
                                     Value = new SimpleQuantity
                                               {
                                                 Value = this.Egvs.Count(e => e.Value > 70f && e.Value < 181f),
                                                 Unit = "70 - 180 mg/dL",
                                                 UnitElement = new FhirString("On Target")
                                               }
                                   },
                                 new Extension
                                   {
                                     Value = new SimpleQuantity
                                               {
                                                 Value = this.Egvs.Count(e => e.Value > 55f && e.Value < 70f), UnitElement = new FhirString("Low")
                                               }
                                   },
                                 new Extension
                                   {
                                     Value = new SimpleQuantity
                                               {
                                                 Value = this.Egvs.Count(e => e.Value < 54f),
                                                 Unit = "<54 mg/dL",
                                                 UnitElement = new FhirString("Very Low")
                                               }
                                   },
                               }
               };
    }

    private decimal CalculateAverageGlucose()
    {
      return (decimal)this.Egvs.Select(e => e.Value).Average();
    }

    private decimal CalculateGlucoseManagementIndicator()
    {
      return 3.31M + 0.02392M * this.CalculateAverageGlucose();
    }
  }
}
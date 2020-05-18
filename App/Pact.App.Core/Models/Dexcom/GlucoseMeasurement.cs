namespace Pact.App.Core.Models.Dexcom
{
  using System;
  using System.Collections.Generic;

  using Hl7.Fhir.Model;

  using Newtonsoft.Json;

  public class GlucoseMeasurement
  {
    [JsonProperty("displayTime")]
    public DateTime DisplayTime { get; set; }

    public string DisplayTimeFormatted => this.DisplayTime.ToString("t");

    [JsonProperty("realtimeValue")]
    public float RealtimeValue { get; set; }

    [JsonProperty("smoothedValue")]
    public float? SmoothedValue { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("systemTime")]
    public DateTime SystemTime { get; set; }

    [JsonProperty("trend")]
    public string Trend { get; set; }

    [JsonProperty("trendRate")]
    public float TrendRate { get; set; }

    [JsonProperty("value")]
    public float Value { get; set; }

    public Observation ToObservation(string patientId)
    {
      return new Observation
               {
                 Identifier =
                   new List<Identifier>
                     {
                       new Identifier
                         {
                           System = "http://www.bmc.nl/zorgportal/identifiers/observations",
                           Value = "6323",
                           Use = Identifier.IdentifierUse.Official
                         }
                     },
                 Status = ObservationStatus.Final,
                 Subject = new ResourceReference($"did:iota:{patientId}"),
                 Code = new CodeableConcept
                          {
                            Coding = new List<Coding>
                                       {
                                         new Coding
                                           {
                                             System = "http://loinc.org", Code = "15074-8", Display = "Glucose [Moles/volume] in Blood"
                                           }
                                       },
                            Text = "Dexcom Glucose Measurement"
                          },
                 Issued = this.DisplayTime,
                 Value = new SimpleQuantity
                           {
                             Value = (decimal)this.RealtimeValue, Unit = "mmol/l", System = "http://unitsofmeasure.org", Code = "mmol/l"
                           },
                 Effective = new Period { StartElement = new FhirDateTime(new DateTimeOffset(this.DisplayTime)) },
                 Extension = new List<Extension> { new Extension { Value = new FhirString("DexcomData")} }
               };
    }
  }
}
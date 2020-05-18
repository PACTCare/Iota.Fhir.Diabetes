namespace Pact.App.Core.Models
{
  using System;

  using Hl7.Fhir.Model;

  public class ImmunizationDetail
  {
    public ImmunizationDetail(Immunization immunization)
    {
      this.Immunization = immunization;
    }

    public string Date => this.Immunization.DateElement?.ToDateTimeOffset().Date.ToShortDateString();

    public string Display => this.Immunization.VaccineCode.Text;

    private Immunization Immunization { get; }
  }
}
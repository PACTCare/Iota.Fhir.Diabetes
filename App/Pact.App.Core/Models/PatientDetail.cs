using System.Linq;
using Hl7.Fhir.Model;

namespace Pact.App.Core.Models
{
  using System;

  using Telerik.XamarinForms.Common.DataAnnotations;

  /// <summary>
  /// The patient detail view model.
  /// </summary>
  public class PatientDetail
  {
    private Patient Patient { get; }

    public PatientDetail(Patient patient)
    {
      this.Patient = patient;
    }

    [DisplayOptions(Group = "Overview", Header = "Birthday", Position = 3)]
    public DateTime? Birthday => this.Patient.BirthDateElement.ToDateTimeOffset()?.DateTime;

    [DisplayOptions(Group = "Address", PlaceholderText = "Country", Header = "Country", Position = 5, ColumnPosition = 2)]
    public string Country => this.Patient.Address.First().Country;

    [DisplayOptions(Group = "Address", PlaceholderText = "City", Header = "City", Position = 5, ColumnPosition = 1)]
    public string City => this.Patient.Address.First().City;

    [DisplayOptions(Group = "Address", PlaceholderText = "Postcode", Header = "Postcode", Position = 5, ColumnPosition = 0)]
    public string Postcode => this.Patient.Address.First().PostalCode;

    [DisplayOptions(Group = "Overview", PlaceholderText = "First name", Header = "First Name", Position = 2)]
    public string Firstname => this.Patient.Name.First().Given.First();

    [DisplayOptions(Group = "Overview", PlaceholderText = "Last name", Header = "Last Name", Position = 1)]
    public string Lastname => this.Patient.Name.First().Family;

    [DisplayOptions(Group = "Address", PlaceholderText = "Street", Header = "Street", Position = 4, ColumnSpan = 3)]
    public string Street => this.Patient.Address.First().Line.First();
  }
}
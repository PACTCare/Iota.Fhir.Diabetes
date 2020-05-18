namespace Pact.App.Core.Models
{
  using System.Collections.Generic;

  public class MedicalInstitution
  {
    public string AuthToken { get; set; }
    public string City { get; set; }
    public string Endpoint { get; set; }
    public string Name { get; set; }
    public string Postcode { get; set; }
    public List<Sharable> SharedContent { get; set; }
    public string Street { get; set; }
  }
}
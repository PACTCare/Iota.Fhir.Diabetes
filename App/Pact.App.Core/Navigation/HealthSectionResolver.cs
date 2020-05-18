namespace Pact.App.Core.Navigation
{
  using System;
  using System.Linq;
  using System.Threading.Tasks;

  using Hl7.Fhir.Model;

  using Pact.App.Core.Entity;
  using Pact.App.Core.Models;
  using Pact.App.Core.Services;
  using Pact.App.Core.Views.DetailPages;
  using Pact.Fhir.Core.Usecase.SearchResources;

  using Xamarin.Forms;

  using Condition = Hl7.Fhir.Model.Condition;

  public static class HealthSectionResolver
  {
    public static async Task<ContentPage> ResolveAsync(HealthSection section)
    {
      var searchInteractor = DependencyResolver.Resolve<SearchResourcesInteractor>();

      switch (section.Type)
      {
        case HealthSectionType.Patient:
          {
            var result = await searchInteractor.ExecuteAsync(new SearchResourcesRequest { ResourceType = "Patient" });

            if (result.Resource is Bundle resultBundle && resultBundle.Entry.Count > 0)
            {
              return new PatientDetailPage(resultBundle.Entry.First().Resource as Patient);
            }

            return null; 
          }

        case HealthSectionType.Immunization:
          {
            var result = await searchInteractor.ExecuteAsync(new SearchResourcesRequest { ResourceType = "Immunization" });

            if (result.Resource is Bundle resultBundle && resultBundle.Entry.Count > 0)
            {
              return new ImmunizationOverview(resultBundle.Entry.Select(e => new ImmunizationDetail(e.Resource as Immunization)).ToList());
            }

            return null; 
          }

        case HealthSectionType.Conditions:
          {
            var result = await searchInteractor.ExecuteAsync(new SearchResourcesRequest { ResourceType = "Condition" });

            if (result.Resource is Bundle resultBundle && resultBundle.Entry.Count > 0)
            {
              return new ConditionsOverview(resultBundle.Entry.Select(e => new ConditionDetail(e.Resource as Condition)).ToList());
            }

            return null;
          }
        case HealthSectionType.Shared:
          return new SharedDataOverview();
        case HealthSectionType.Devices:
          return new DevicesOverview();
        case HealthSectionType.Default:
        default:
          return null;
      }
    }
  }
}
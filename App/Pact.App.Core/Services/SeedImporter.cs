namespace Pact.App.Core.Services
{
  using System.Linq;

  using Hl7.Fhir.Model;

  using Pact.Fhir.Iota.Events;
  using Pact.Fhir.Iota.Services;

  using Tangle.Net.Entity;

  using Xamarin.Essentials;

  using Task = System.Threading.Tasks.Task;

  public static class SeedImporter
  {
    static SeedImporter()
    {
      ResourceImporter.ResourceAdded += ResourceImporterOnResourceAdded;
    }

    private static async void ResourceImporterOnResourceAdded(object sender, ResourceAddedEventArgs e)
    {
      if (!(e.Resource is Observation observation))
      {
        return;
      }

      if (observation.Code.Text == "Dexcom Daily Glucose Measurement")
      {
        await SecureStorage.SetAsync("DexcomDaily", e.Resource.Id);
        return;
      }

      var extension = observation.Extension.FirstOrDefault(ex => ex.Value is FhirString);
      if (extension != null && ((FhirString)extension.Value).Value == "DexcomData")
      {
        await SecureStorage.SetAsync("Dexcom", e.Resource.Id);
        await SecureStorage.SetAsync("Device", "Dexcom");
      }
    }

    public static async Task ImportSeedAsync(Seed seed)
    {
      if (DependencyResolver.Resolve<ISeedManager>() is DeterministicSeedManager credentialProvider)
      {
        await credentialProvider.SyncAsync(seed);
      }
    }
  }
}
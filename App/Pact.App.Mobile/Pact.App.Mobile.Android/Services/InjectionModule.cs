namespace Pact.App.Android.Droid.Services
{
  using System.Collections.Generic;

  using Autofac;

  using Hl7.Fhir.Serialization;

  using Pact.App.Core.Repository;
  using Pact.App.Core.Services;
  using Pact.App.Core.Services.Authentication;
  using Pact.App.Core.Services.Glucose;
  using Pact.App.Core.Workers;
  using Pact.Fhir.Core.Repository;
  using Pact.Fhir.Core.Usecase.CreateResource;
  using Pact.Fhir.Core.Usecase.ReadResource;
  using Pact.Fhir.Core.Usecase.SearchResources;
  using Pact.Fhir.Core.Usecase.UpdateResource;
  using Pact.Fhir.Core.Usecase.ValidateResource;
  using Pact.Fhir.Iota.Repository;
  using Pact.Fhir.Iota.Serializer;
  using Pact.Fhir.Iota.Services;
  using Pact.Fhir.Iota.SqlLite.Encryption;
  using Pact.Fhir.Mobile.Repository;

  using RestSharp;

  using Tangle.Net.Cryptography;
  using Tangle.Net.Cryptography.Curl;
  using Tangle.Net.Cryptography.Signing;
  using Tangle.Net.Mam.Merkle;
  using Tangle.Net.Mam.Services;
  using Tangle.Net.ProofOfWork;
  using Tangle.Net.Repository;
  using Tangle.Net.Repository.Client;

  public class InjectionModule : Module
  {
    /// <inheritdoc />
    protected override void Load(ContainerBuilder builder)
    {
      var iotaRepository = new RestIotaRepository(
        new FallbackIotaClient(new List<string> { "https://nodes.devnet.thetangle.org:443" }, 5000),
        new PoWService(new CpuPearlDiver()));

      var channelFactory = new MamChannelFactory(CurlMamFactory.Default, CurlMerkleTreeFactory.Default, iotaRepository);
      var subscriptionFactory = new MamChannelSubscriptionFactory(iotaRepository, CurlMamParser.Default, CurlMask.Default);

      var encryption = new RijndaelEncryption("somenicekey", "somenicesalt");
      var resourceTracker = new ResourceTracker(
        channelFactory,
        subscriptionFactory,
        encryption,
        $"{DependencyResolver.LocalStoragePath}\\resources.sqlite");

      var seedManager = new SeedManager(
        resourceTracker,
        new IssSigningHelper(new Curl(), new Curl(), new Curl()),
        new AddressGenerator(),
        iotaRepository,
        encryption,
        $"{DependencyResolver.LocalStoragePath}\\seedmanager.sqlite");

      var fhirRepository = new IotaFhirRepository(iotaRepository, new FhirJsonTryteSerializer(), resourceTracker, seedManager);
      var fhirParser = new FhirJsonParser();
      var searchRepository = new SearchRepository($"{DependencyResolver.LocalStoragePath}\\search.sqlite");

      var createInteractor = new CreateResourceInteractor(fhirRepository, fhirParser, searchRepository);
      var readInteractor = new ReadResourceInteractor(fhirRepository, searchRepository);
      var validationInteractor = new ValidateResourceInteractor(fhirRepository, fhirParser);
      var searchInteractor = new SearchResourcesInteractor(fhirRepository, searchRepository);

      var resourceImporter = new ResourceImporter(searchRepository, fhirRepository, seedManager);

      builder.RegisterInstance(searchRepository).As<ISearchRepository>();
      builder.RegisterInstance(resourceTracker).As<IResourceTracker>();

      builder.RegisterInstance(createInteractor);
      builder.RegisterInstance(readInteractor);
      builder.RegisterInstance(validationInteractor);
      builder.RegisterInstance(searchInteractor);
      builder.RegisterInstance(resourceImporter);
      builder.RegisterInstance(seedManager).As<ISeedManager>();
      builder.RegisterInstance(subscriptionFactory);
      builder.RegisterInstance(fhirRepository).As<IFhirRepository>();
      builder.RegisterInstance(new AndroidLogout()).As<ILogout>();

      var backgroundWorker = new BackgroundWorkerService();

      var glucoseService = new FhirGlucoseService(
        new CreateResourceInteractor(fhirRepository, fhirParser, searchRepository),
        new UpdateResourceInteractor(fhirRepository, fhirParser),
        new ReadResourceInteractor(fhirRepository, searchRepository),
        searchRepository);
      var glucoseRepository = new DexcomGlucoseManagementRepository(new RestClient("https://sandbox-api.dexcom.com"));

      //backgoundWorker.RegisterTaskWorker(new ContinuousGlucoseTaskWorker(glucoseService, glucoseRepository));
      backgroundWorker.RegisterTaskWorker(new AggregateGlucoseTaskWorker(glucoseService, glucoseRepository));
    }
  }
}
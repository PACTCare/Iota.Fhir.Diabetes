namespace Pact.App.Mobile.UWP.Services
{
  using System.Collections.Generic;

  using Autofac;

  using Hl7.Fhir.Serialization;

  using Pact.App.Core.Services;
  using Pact.Fhir.Core.SqlLite.Repository;
  using Pact.Fhir.Core.Usecase.CreateResource;
  using Pact.Fhir.Core.Usecase.ReadResource;
  using Pact.Fhir.Core.Usecase.SearchResources;
  using Pact.Fhir.Core.Usecase.ValidateResource;
  using Pact.Fhir.Iota.Repository;
  using Pact.Fhir.Iota.Serializer;
  using Pact.Fhir.Iota.Services;
  using Pact.Fhir.Iota.SqlLite.Encryption;
  using Pact.Fhir.Iota.SqlLite.Services;

  using Tangle.Net.Cryptography;
  using Tangle.Net.Cryptography.Curl;
  using Tangle.Net.Cryptography.Signing;
  using Tangle.Net.Mam.Merkle;
  using Tangle.Net.Mam.Services;
  using Tangle.Net.ProofOfWork.Service;
  using Tangle.Net.Repository.Client;

  public class InjectionModule : Module
  {
    /// <inheritdoc />
    protected override void Load(ContainerBuilder builder)
    {
      var connectionSupplier = new DefaultDbConnectionSupplier();
      var iotaRepository = new CachedIotaRestRepository(
        new FallbackIotaClient(new List<string> { "https://nodes.devnet.thetangle.org:443" }, 5000),
        new PoWSrvService(),
        null,
        connectionSupplier,
        $"{DependencyResolver.LocalStoragePath}\\iotacache.sqlite");

      var channelFactory = new MamChannelFactory(CurlMamFactory.Default, CurlMerkleTreeFactory.Default, iotaRepository);
      var subscriptionFactory = new MamChannelSubscriptionFactory(iotaRepository, CurlMamParser.Default, CurlMask.Default);

      var encryption = new RijndaelEncryption("somenicekey", "somenicesalt");
      var resourceTracker = new SqlLiteResourceTracker(
        channelFactory,
        subscriptionFactory,
        encryption,
        connectionSupplier,
        $"{DependencyResolver.LocalStoragePath}\\iotafhir.sqlite");

      var seedManager = new SqlLiteDeterministicSeedManager(
        resourceTracker,
        new IssSigningHelper(new Curl(), new Curl(), new Curl()),
        new AddressGenerator(),
        iotaRepository,
        encryption,
        connectionSupplier,
        $"{DependencyResolver.LocalStoragePath}\\iotafhir.sqlite");

      var fhirRepository = new IotaFhirRepository(iotaRepository, new FhirJsonTryteSerializer(), resourceTracker, seedManager);
      var fhirParser = new FhirJsonParser();
      var searchRepository = new SqlLiteSearchRepository(fhirParser, connectionSupplier, $"{DependencyResolver.LocalStoragePath}\\resources.sqlite");

      var createInteractor = new CreateResourceInteractor(fhirRepository, fhirParser, searchRepository);
      var readInteractor = new ReadResourceInteractor(fhirRepository, searchRepository);
      var validationInteractor = new ValidateResourceInteractor(fhirRepository, fhirParser);
      var searchInteractor = new SearchResourcesInteractor(fhirRepository, searchRepository);

      var resourceImporter = new ResourceImporter(searchRepository, fhirRepository, seedManager);

      builder.RegisterInstance(createInteractor);
      builder.RegisterInstance(readInteractor);
      builder.RegisterInstance(validationInteractor);
      builder.RegisterInstance(searchInteractor);
      builder.RegisterInstance(resourceImporter);
      builder.RegisterInstance(seedManager).As<ISeedManager>();
      builder.RegisterInstance(subscriptionFactory);
    }
  }
}
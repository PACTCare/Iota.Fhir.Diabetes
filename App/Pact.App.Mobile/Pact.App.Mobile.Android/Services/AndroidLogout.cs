using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Pact.App.Android.Droid.Services
{
  using System.Threading.Tasks;

  using Pact.App.Core.Services;
  using Pact.App.Core.Services.Authentication;
  using Pact.Fhir.Core.Repository;
  using Pact.Fhir.Iota.Services;
  using Pact.Fhir.Mobile.Repository;

  using Xamarin.Essentials;

  public class AndroidLogout : ILogout
  {
    /// <inheritdoc />
    public async Task LogoutAsync()
    {
      var search = DependencyResolver.Resolve<ISearchRepository>() as SearchRepository;
      var resource = DependencyResolver.Resolve<IResourceTracker>() as ResourceTracker;
      var seed = DependencyResolver.Resolve<ISeedManager>() as SeedManager;

      if (search != null) await search.DeleteAllAsync();
      if (resource != null) await resource.DeleteAllAsync();
      if (seed != null) await seed.DeleteAllAsync();

      SecureStorage.RemoveAll();
    }
  }
}
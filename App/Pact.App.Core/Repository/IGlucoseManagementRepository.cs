namespace Pact.App.Core.Repository
{
  using System;
  using System.Threading.Tasks;

  using Pact.App.Core.Models.Dexcom;

  public interface IGlucoseManagementRepository
  {
    Task<GlucoseMeasurementList> LoadGlucoseDataAsync(DateTime from, DateTime to);
  }
}
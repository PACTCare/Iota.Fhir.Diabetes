namespace Pact.App.Core.Workers
{
  using System.Threading.Tasks;

  /// <summary>
  /// The JobWorker interface.
  /// </summary>
  public interface IObjectWorker
  {
    Task ProcessObjectAsync(object job);
  }
}
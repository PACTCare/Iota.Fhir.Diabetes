namespace Pact.App.Core.Views.Main
{
  using System;

  /// <summary>
  /// The master detail main page menu item.
  /// </summary>
  public class MasterDetailMainPageMenuItem
  {
    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the target type.
    /// </summary>
    public Type TargetType { get; set; }

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    public string Title { get; set; }
  }
}
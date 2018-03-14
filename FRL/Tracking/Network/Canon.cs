namespace FRL.Network {
  /// <summary>
  /// Class for canonical/reserved network identifiers.
  /// </summary>
  public class Canon {

    /// <summary>
    /// Returns a string representing the machine identifier.
    /// </summary>
    /// <returns>The string specified.</returns>
    public static string Origin() {
      return System.Environment.UserName + "@" + System.Environment.MachineName;
    }
  }
}

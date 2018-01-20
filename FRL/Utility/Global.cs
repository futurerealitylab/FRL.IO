using UnityEngine;
/// <summary>
/// Like a Singleton: enforces single instance, but no direct instance access is allowed
/// (exposed via static functions/properties).
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Global<T> : MonoBehaviour where T : MonoBehaviour {
  static T t;
  static UnityEngine.Object lockObject = new UnityEngine.Object();

  /// <summary>
  /// Instance reference, only accessible within subclasses.
  /// <returns>Global instance of type T.</returns>
  /// </summary>
  protected static T global {
    get {
      lock (lockObject) {
        if (t == null) {
          T[] objects = FindObjectsOfType(typeof(T)) as T[];
          if (objects.Length == 0) {
            Debug.LogWarning("Global: " +
                "No instances of " + typeof(T) + " in scene! (static access failure)"
            );
            return null;
          }
          t = objects[0];
          if (objects.Length > 1) {
            Debug.LogWarning("Global: " +
                "More than one instance (" + objects.Length + ") of " + typeof(T) + " in scene! " +
                "(expect undefined behavior)", t
            );
          }
        }
        return t;
      }
    }
  }
}

using UnityEngine;
namespace HelixJump
{
 // Only generated ring roots receive this marker; never Helix, tower or permanent anchors.
 [DisallowMultipleComponent]
 public sealed class BreakablePlatform : MonoBehaviour
 {
  public bool Breaking { get; private set; }
  public bool TryBreak()
  {
   if (Breaking || GetComponentInChildren<GoalController>(true) != null || GetComponent<HelixController>() != null) return false;
   Breaking=true;
   foreach(var collider in GetComponentsInChildren<Collider>(true)) collider.enabled=false;
   if(Application.isPlaying) Destroy(gameObject,0.2f);
   else DestroyImmediate(gameObject);
   return true;
  }
 }
}

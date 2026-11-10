using UnityEngine;
namespace HelixJump
{
    [RequireComponent(typeof(Renderer))]
    public sealed class DeathPart : MonoBehaviour
    {
        void OnEnable()
        {
            var renderer = GetComponent<Renderer>();
            var properties = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(properties);
            properties.SetColor("_BaseColor", Color.red);
            properties.SetColor("_Color", Color.red);
            renderer.SetPropertyBlock(properties);
        }
    }
}

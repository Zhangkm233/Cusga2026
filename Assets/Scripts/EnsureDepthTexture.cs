using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
[RequireComponent(typeof(Camera))]
public class EnsureDepthTexture : MonoBehaviour
{
    void OnEnable()
    {
        var cam = GetComponent<Camera>();
        if (cam != null)
            cam.depthTextureMode |= DepthTextureMode.Depth;
    }

    void Start()
    {
        var cam = GetComponent<Camera>();
        if (cam != null)
            cam.depthTextureMode |= DepthTextureMode.Depth;
    }

#if UNITY_EDITOR
    void Update()
    {
        // Keep it enabled in editor in case something clears it
        var cam = GetComponent<Camera>();
        if (cam != null && (cam.depthTextureMode & DepthTextureMode.Depth) == 0)
            cam.depthTextureMode |= DepthTextureMode.Depth;
    }
#endif
}

using UnityEngine;
using Unity.Netcode;

/// <summary>
/// Local player camera - Sadece owner'ın camera'sı aktif olur
/// Diğer oyuncuların camera'ları disable edilir
/// </summary>
[RequireComponent(typeof(Camera))]
public class LocalPlayerCamera : NetworkBehaviour
{
    private Camera cam;
    private AudioListener audioListener;
    
    private void Awake()
    {
        cam = GetComponent<Camera>();
        audioListener = GetComponent<AudioListener>();
    }
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        // Parent'taki NetworkBehaviour'u bul (Player)
        NetworkBehaviour parentNetworkBehaviour = GetComponentInParent<NetworkBehaviour>();
        
        if (parentNetworkBehaviour == null)
        {
            Debug.LogError("[LocalPlayerCamera] Parent NetworkBehaviour bulunamadı!", this);
            return;
        }
        
        bool isOwner = parentNetworkBehaviour.IsOwner;
        
        // Sadece owner'ın camera'sı aktif
        cam.enabled = isOwner;
        
        if (audioListener != null)
        {
            audioListener.enabled = isOwner;
        }
        
        Debug.Log($"[LocalPlayerCamera] Camera {(isOwner ? "ENABLED" : "DISABLED")} - IsOwner: {isOwner}", this);
    }
}

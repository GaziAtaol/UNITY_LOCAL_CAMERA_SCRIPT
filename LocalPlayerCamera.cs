using UnityEngine;
using Unity.Netcode;

/// <summary>
/// Manages the local player camera and audio listener in a Unity Netcode for GameObjects
/// multiplayer session. Only the owning client's camera and audio listener are enabled;
/// all remote players' cameras are disabled automatically.
/// </summary>
[RequireComponent(typeof(Camera))]
[DisallowMultipleComponent]
public class LocalPlayerCamera : NetworkBehaviour
{
    private Camera cam;
    private AudioListener audioListener;

    private void Awake()
    {
        cam = GetComponent<Camera>();

        // Search on this object first; fall back to parent hierarchy in case the
        // AudioListener lives on the player root rather than the camera child.
        audioListener = GetComponent<AudioListener>();
        if (audioListener == null)
        {
            audioListener = GetComponentInParent<AudioListener>();
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // IsOwner is provided directly by NetworkBehaviour and correctly reflects
        // whether this client owns the spawned object.
        SetCameraState(IsOwner);

        Debug.Log($"[LocalPlayerCamera] Camera {(IsOwner ? "ENABLED" : "DISABLED")} - IsOwner: {IsOwner}", this);
    }

    public override void OnNetworkDespawn()
    {
        // Ensure the camera and audio listener are disabled when the network object
        // is despawned (e.g., player disconnects or the session ends).
        SetCameraState(false);
        base.OnNetworkDespawn();
    }

    private void SetCameraState(bool active)
    {
        if (cam != null)
        {
            cam.enabled = active;
        }
        else
        {
            Debug.LogWarning("[LocalPlayerCamera] Camera component not found on this GameObject!", this);
        }

        if (audioListener != null)
        {
            audioListener.enabled = active;
        }
    }
}

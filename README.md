# LocalPlayerCamera

A Unity C# script that automatically enables the **Camera** and **AudioListener** only for the locally owned player in a [Unity Netcode for GameObjects](https://docs-multiplayer.unity3d.com/netcode/current/about/) multiplayer session. Every remote player's camera is disabled, preventing split-screen rendering and multiple active `AudioListener` warnings.

---

## Requirements

| Requirement | Version |
|-------------|---------|
| Unity | 2021.3 LTS or newer (recommended) |
| Netcode for GameObjects | 1.0.0 or newer |

> **Install Netcode for GameObjects** via the Unity Package Manager:  
> `Window → Package Manager → + → Add package by name → com.unity.netcode.gameobjects`

---

## Setup

### 1. Player Prefab Hierarchy

The script is designed to sit on a **child Camera GameObject** beneath your networked Player prefab:

```
PlayerPrefab  [NetworkObject]
└── CameraRoot
    └── PlayerCamera  [Camera] [AudioListener] [LocalPlayerCamera]  ← attach here
```

> The `AudioListener` can alternatively remain on the `PlayerPrefab` root — the script will find it automatically by searching up the parent hierarchy.

### 2. Attach the Script

1. Select the **Camera** child GameObject inside your Player prefab.
2. In the **Inspector**, click **Add Component** and search for `LocalPlayerCamera`.
3. Because the script is decorated with `[RequireComponent(typeof(Camera))]`, a `Camera` component is required on the same object. A `[DisallowMultipleComponent]` attribute also prevents duplicate instances.

### 3. No Extra Configuration

The script requires no serialized fields — everything is resolved automatically at network spawn time.

---

## How It Works

| Event | Behaviour |
|-------|-----------|
| `OnNetworkSpawn` | Calls `SetCameraState(IsOwner)` — enables camera/audio for the local owner, disables them for every remote client |
| `OnNetworkDespawn` | Calls `SetCameraState(false)` — safely disables the camera when the session ends or the player disconnects |

### Key Design Decisions

* **`IsOwner` used directly** — `LocalPlayerCamera` is itself a `NetworkBehaviour`, so `IsOwner` already reflects the correct ownership status without any parent hierarchy lookup.
* **AudioListener fallback** — The script first looks for an `AudioListener` on the same GameObject; if none is found it walks up the hierarchy, so you can keep the listener on the player root if preferred.
* **`[DisallowMultipleComponent]`** — Prevents accidentally attaching the script twice, which would cause conflicting enable/disable calls.

---

## Example Log Output

When a session starts you will see one of the following in the Console:

```
[LocalPlayerCamera] Camera ENABLED - IsOwner: True    // local player
[LocalPlayerCamera] Camera DISABLED - IsOwner: False  // remote player
```

---

## License

This project is licensed under the [MIT License](LICENSE).

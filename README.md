# Checkpoint System for S&box

Place and manage checkpoints in your S&box game with a built-in editor tool. Left-click to place checkpoints directly in the scene — no manual component wiring required.

---

## How It Works

Three pieces work together:

- **`CheckpointPlacer`** — an editor tool (toolbar icon: `add_location`) that lets you left-click to place checkpoints in your scene. It auto-creates a `CheckpointManager` if one doesn't exist, and auto-increments the checkpoint index as you place each one.
- **`CheckpointRing`** — the runtime component on each checkpoint. Detects any GameObject tagged `player` entering its radius and fires events. Supports sequential or free-order modes.
- **`CheckpointManager`** — singleton that tracks overall progress (`NextCheckpoint`), enforces sequential ordering, and exposes events your gamemode can subscribe to.

---

## Installation

Add the package into the S&box editor:

```
1. Select the Asset Browser tab in the S&box editor.
2. Navigate to the Asset Browser tab -> Find the cloud asset store
3. Search for Checkpoint System from rwtrcsc

https://sbox.game/rwtrcsc/checkpoint_system/
```

Clone into your S&box project directory:

```
git clone https://github.com/JoshuaPaulsen/sbox-addon-checkpoint-system.git
```

Open your project in S&box. The **Checkpoint Placer** tool will appear in the editor toolbar.

---

## Placing Checkpoints

1. Open your scene in S&box Editor
2. Select **Checkpoint Placer** from the toolbar (map pin icon)
3. Left-click anywhere in the scene — a checkpoint is placed at that position
4. Repeat for each checkpoint. Indices are assigned automatically (`Checkpoint_0`, `Checkpoint_1`, etc.)
5. To set a **finish line**, select the last `CheckpointRing` in the inspector and enable `Is Finish Line`
6. Optionally assign a `Color Cube` (ModelRenderer) to `CheckpointManager` — it turns green when the player finishes

---

## Inspector Properties

### CheckpointManager
| Property | Type | Description |
|---|---|---|
| `Sequential` | bool | If true, checkpoints must be hit in order (default: true) |
| `ColorCube` | ModelRenderer | Optional — turns green when finish is reached |
| `FinishLine` | CheckpointRing | Reference to the finish line checkpoint |
| `LastCheckpoint` | CheckpointRing | Reference to the final checkpoint before the finish |

### CheckpointRing
| Property | Type | Description |
|---|---|---|
| `CheckpointIndex` | int | Order index of this checkpoint |
| `IsFinishLine` | bool | Marks this as the finish line |
| `IsLastCheckpoint` | bool | Marks this as the last checkpoint |
| `DetectionRadius` | float | Proximity radius for player detection (default: 150) |

---

## Events

Subscribe to these from your gamemode:

```csharp
// Fires when any checkpoint is hit — passes the checkpoint index
CheckpointManager.Instance.OnCheckpointReachedEvent += ( int index ) =>
{
    Log.Info( $"Checkpoint {index} reached!" );
};

// Fires when the finish line is hit (all checkpoints must be cleared first)
CheckpointManager.Instance.OnFinishReachedEvent += () =>
{
    Log.Info( "Player finished!" );
};

// Static events on CheckpointRing — listen without a manager reference
CheckpointRing.OnCheckpointHit += ( int index ) => { };
CheckpointRing.OnFinishHit += () => { };
```

---

## Resetting

```csharp
// Reset all checkpoints back to untriggered, NextCheckpoint back to 0
CheckpointManager.Instance.ResetAll();
```

---

## Sequential vs Free Order

With `Sequential = true` (default), a player must hit `Checkpoint_0` before `Checkpoint_1`, and so on. The finish line is blocked until all checkpoints are cleared.

Set `Sequential = false` to allow checkpoints to be hit in any order — the finish line still requires all checkpoints to be hit first.

---

## Requirements

- [S&box](https://sbox.game) (latest build)
- .NET 8

---

## Project Structure

```
/
├── Assets/scenes/           # Example scene
├── Code/
│   ├── CheckpointManager.cs # Singleton — tracks progress, exposes events
│   └── CheckpointRing.cs    # Per-checkpoint component — proximity detection
├── Editor/
│   └── CheckpointPlacer.cs  # Editor tool — click-to-place in scene
├── ProjectSettings/
└── checkpoint_system.sbproj
```

---

## License

MIT — use it in anything.

---

Built by [Joshua Paulsen](https://www.linkedin.com/in/joshuapaulsen31/)

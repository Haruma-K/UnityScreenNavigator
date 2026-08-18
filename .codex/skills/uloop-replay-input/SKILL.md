---
name: uloop-replay-input
description: "Replay recorded PlayMode keyboard and mouse input. Use for exact gameplay reproduction, E2E runs, or consistent demos from JSON recordings."
---

# npx --yes uloop-cli@2.2.0 replay-input

Replay recorded keyboard and mouse input during PlayMode. Loads a JSON recording and injects input frame-by-frame via Input System with zero CLI overhead. Supports looping and progress monitoring.

## Usage

```bash
# Start replay (auto-detect latest recording)
npx --yes uloop-cli@2.2.0 replay-input --action Start

# Start replay with specific file
npx --yes uloop-cli@2.2.0 replay-input --action Start --input-path scripts/my-play.json

# Start replay with looping
npx --yes uloop-cli@2.2.0 replay-input --action Start --loop true

# Check replay progress
npx --yes uloop-cli@2.2.0 replay-input --action Status

# Stop replay
npx --yes uloop-cli@2.2.0 replay-input --action Stop
```

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `--action` | enum | `Start` | `Start`, `Stop`, `Status` |
| `--input-path` | string | auto | JSON path. Auto-detects latest in `.uloop/outputs/InputRecordings/` |
| `--show-overlay` | boolean | `true` | Show replay progress overlay |
| `--loop` | boolean | `false` | Loop continuously |

## Deterministic Replay

Replay injects the exact same input frame-by-frame, but the game must also be deterministic to produce identical results. If replay output must be compared across runs, read [references/deterministic-replay.md](references/deterministic-replay.md) before interpreting failures.

## Prerequisites

- Unity must be in **PlayMode**
- **Input System package** must be installed (`com.unity.inputsystem`)
- Use this only when the project already uses the New Input System.

## Output

Returns JSON with:
- `Success`: Whether the operation succeeded
- `Message`: Status message
- `Action`: Echoes which action was executed (`Start`, `Stop`, or `Status`)
- `InputPath`: Path to recording file (nullable string; populated on `Start` only)
- `CurrentFrame`: Current replay frame index (nullable int)
- `TotalFrames`: Total frames in the recording (nullable int)
- `Progress`: Replay progress (nullable float in 0.0 – 1.0)
- `IsReplaying`: Whether replay is currently active (nullable bool)

These are the only eight fields. There is no `LoopCount`, `ElapsedSeconds`, `OverlayVisible`, or per-frame inspection data in the response.

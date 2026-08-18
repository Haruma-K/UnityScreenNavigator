---
name: uloop-control-play-mode
description: "Control Unity Editor Play Mode. Use to start, stop, or pause Play Mode for runtime behavior checks and frame inspection."
---

# npx --yes uloop-cli@2.2.0 control-play-mode

Control Unity Editor play mode (play/stop/pause).

## Usage

```bash
npx --yes uloop-cli@2.2.0 control-play-mode [options]
```

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `--action` | string | `Play` | Action to perform: `Play`, `Stop`, `Pause` |

## Global Options

| Option | Description |
|--------|-------------|
| `--project-path <path>` | Optional. Use only when the target Unity project is not the current directory. |

## Examples

```bash
# Start play mode
npx --yes uloop-cli@2.2.0 control-play-mode --action Play

# Stop play mode
npx --yes uloop-cli@2.2.0 control-play-mode --action Stop

# Pause play mode
npx --yes uloop-cli@2.2.0 control-play-mode --action Pause
```

## Output

Returns JSON with the current play mode state:
- `IsPlaying`: Whether Unity is currently in play mode
- `IsPaused`: Whether play mode is paused
- `Message`: Description of the action performed

## Notes

- Play action starts the game in the Unity Editor (also resumes from pause)
- Stop action exits play mode and returns to edit mode
- Pause action pauses the game while remaining in play mode
- Useful for automated testing workflows

- PlayMode entry may complete on the next editor frame. If a PlayMode-dependent command reports "PlayMode is not active" immediately after `--action Play`, wait briefly and retry.

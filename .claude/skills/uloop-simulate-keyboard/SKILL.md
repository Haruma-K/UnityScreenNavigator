---
name: uloop-simulate-keyboard
description: "Simulate keyboard input in PlayMode through Unity Input System. Use for key presses, holds, releases, and game controls such as WASD or Space."
context: fork
---

# Task

Simulate keyboard input on Unity PlayMode: $ARGUMENTS

## Workflow

1. Ensure Unity is in PlayMode (use `npx --yes uloop-cli@2.2.0 control-play-mode --action Play` if not)
2. Execute the appropriate `npx --yes uloop-cli@2.2.0 simulate-keyboard` command
3. Take a screenshot to verify the result: `npx --yes uloop-cli@2.2.0 screenshot --capture-mode rendering`
4. Report what happened

## Tool Reference

```bash
npx --yes uloop-cli@2.2.0 simulate-keyboard --action <action> --key <key> [options]
```

### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `--action` | enum | `Press` | `Press`, `KeyDown`, `KeyUp` |
| `--key` | string | (required) | Key name matching Input System Key enum (e.g. `W`, `Space`, `LeftShift`, `A`, `Enter`). Case-insensitive. |
| `--duration` | number | `0` | Hold duration in seconds for Press action (0 = one-shot tap). Ignored by KeyDown/KeyUp. |

### Actions

| Action | Behavior | Use Case |
|--------|----------|----------|
| `Press` | KeyDown → wait → KeyUp | One-shot tap (jump, use item) |
| `KeyDown` | KeyDown only (held until KeyUp) | Start continuous movement, hold sprint |
| `KeyUp` | KeyUp only (release held key) | Stop movement, release sprint |

### KeyDown/KeyUp Rules

- `KeyDown` fails if the key is already held
- `KeyUp` fails if the key is not currently held
- Multiple keys can be held simultaneously (e.g. W + LeftShift for sprint)
- All held keys are automatically released when PlayMode exits
- To hold a key for a fixed duration, prefer `--action Press --duration <seconds>` (one-shot, blocks until release). For multi-key holds (e.g. Shift+W), issue separate `KeyDown` calls, then `sleep <seconds>` between them and the `KeyUp` calls.

### Global Options

| Option | Description |
|--------|-------------|
| `--project-path <path>` | Optional. Use only when the target Unity project is not the current directory. |

## Examples

```bash
# One-shot key press (tap W once)
npx --yes uloop-cli@2.2.0 simulate-keyboard --action Press --key W

# Jump (tap Space)
npx --yes uloop-cli@2.2.0 simulate-keyboard --action Press --key Space

# Hold W for 2 seconds (move forward)
npx --yes uloop-cli@2.2.0 simulate-keyboard --action Press --key W --duration 2.0

# Sprint forward (hold Shift + W, then release)
npx --yes uloop-cli@2.2.0 simulate-keyboard --action KeyDown --key LeftShift
npx --yes uloop-cli@2.2.0 simulate-keyboard --action KeyDown --key W
npx --yes uloop-cli@2.2.0 screenshot --capture-mode rendering
npx --yes uloop-cli@2.2.0 simulate-keyboard --action KeyUp --key W
npx --yes uloop-cli@2.2.0 simulate-keyboard --action KeyUp --key LeftShift
```

## Output

Returns JSON with:
- `Success` (boolean): Whether the action succeeded (e.g. `KeyDown` on a not-yet-held key, `KeyUp` on a currently-held key, or `Press` round-trip)
- `Message` (string): Description of what happened or why it failed
- `Action` (string): The `--action` value that was applied (`Press`, `KeyDown`, or `KeyUp`)
- `KeyName` (string, nullable): The key that was acted on; may be `null` when the action could not resolve a key

## Prerequisites

- Unity must be in **PlayMode**
- **Input System package** must be installed (`com.unity.inputsystem`)
- Use this only when the project already uses the New Input System.
- Game code must read input via Input System API (e.g. `Keyboard.current[Key.W].isPressed`), not legacy `Input.GetKey()`

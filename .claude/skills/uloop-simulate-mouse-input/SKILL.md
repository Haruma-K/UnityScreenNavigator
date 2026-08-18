---
name: uloop-simulate-mouse-input
description: "Simulate Mouse.current input in PlayMode through Unity Input System. Use for gameplay clicks, mouse delta, or scroll; use simulate-mouse-ui for EventSystem UI elements."
context: fork
---

# Task

Simulate mouse input via Input System in Unity PlayMode: $ARGUMENTS

## Workflow

1. Ensure Unity is in PlayMode (use `npx --yes uloop-cli@2.2.0 control-play-mode --action Play` if not)
2. For Click/LongPress: determine the target Game View input position from annotated `SimX`/`SimY`, raycast-grid `InputX`/`InputY`, or raw image pixels converted with `ScreenshotToInputFormula`
3. Execute the appropriate `npx --yes uloop-cli@2.2.0 simulate-mouse-input` command
4. Take a screenshot to verify the result: `npx --yes uloop-cli@2.2.0 screenshot --capture-mode rendering`
5. Report what happened

## Tool Reference

```bash
npx --yes uloop-cli@2.2.0 simulate-mouse-input --action <action> [options]
```

### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `--action` | enum | `Click` | `Click`, `LongPress`, `MoveDelta`, `SmoothDelta`, `Scroll` |
| `--x` | number | `0` | Target X position in Game View pixels (origin: top-left). Used by Click and LongPress. Use `AnnotatedElements[].SimX`, `RaycastGridPoints[].InputX`, or raw image pixels converted with `ScreenshotToInputFormula`. |
| `--y` | number | `0` | Target Y position in Game View pixels (origin: top-left). Used by Click and LongPress. Use `AnnotatedElements[].SimY`, `RaycastGridPoints[].InputY`, or raw image pixels converted with `ScreenshotToInputFormula`. |
| `--button` | enum | `Left` | Mouse button: `Left`, `Right`, `Middle`. Used by Click and LongPress. |
| `--duration` | number | `0` | Hold duration for LongPress, or interpolation duration for SmoothDelta (seconds). For Click, 0 = one-shot tap. |
| `--delta-x` | number | `0` | Delta X in pixels for MoveDelta/SmoothDelta. Positive = right. |
| `--delta-y` | number | `0` | Delta Y in pixels for MoveDelta/SmoothDelta. Positive = up. |
| `--scroll-x` | number | `0` | Horizontal scroll delta for Scroll action. |
| `--scroll-y` | number | `0` | Vertical scroll delta for Scroll action. Typically 120 per notch. |

### Actions

| Action | What it injects | Description |
|--------|----------------|-------------|
| `Click` | Mouse.current button press → release | Inject a button click so game logic detects `wasPressedThisFrame` |
| `LongPress` | Mouse.current button press → hold → release | Hold a button for `--duration` seconds |
| `MoveDelta` | Mouse.current.delta | Inject mouse movement delta one-shot (e.g. for FPS camera look) |
| `SmoothDelta` | Mouse.current.delta (per-frame) | Inject mouse delta smoothly over `--duration` seconds (human-like camera pan) |
| `Scroll` | Mouse.current.scroll | Inject scroll wheel input (e.g. for hotbar or zoom) |

### Global Options (optional)

| Option | Description |
|--------|-------------|
| `--project-path <path>` | Optional. Use only when the target Unity project is not the current directory. |


## When to use this vs simulate-mouse-ui

| Scenario | Tool |
|----------|------|
| Click a Unity UI Button (IPointerClickHandler) | `simulate-mouse-ui` |
| Destroy a block in Minecraft (reads `Mouse.current.leftButton`) | `simulate-mouse-input` when the project uses the New Input System |
| Place a block with right-click | `simulate-mouse-input --button Right` when the project uses the New Input System |
| Drag a UI slider | `simulate-mouse-ui --action Drag` |
| Look around with mouse (FPS camera) | `simulate-mouse-input --action MoveDelta` when the project uses the New Input System |
| Scroll hotbar slots | `simulate-mouse-input --action Scroll` when the project uses the New Input System |

## Examples

```bash
# Left-click at the Game View center (for game logic)
npx --yes uloop-cli@2.2.0 simulate-mouse-input --action Click --x 400 --y 300

# Right-click at screen center (e.g. place block)
npx --yes uloop-cli@2.2.0 simulate-mouse-input --action Click --x 400 --y 300 --button Right

# Hold left-click for 2 seconds (e.g. mine block)
npx --yes uloop-cli@2.2.0 simulate-mouse-input --action LongPress --x 400 --y 300 --duration 2.0

# Look right (FPS camera)
npx --yes uloop-cli@2.2.0 simulate-mouse-input --action MoveDelta --delta-x 100 --delta-y 0

# Scroll up (e.g. previous hotbar slot)
npx --yes uloop-cli@2.2.0 simulate-mouse-input --action Scroll --scroll-y 120

# Scroll down (e.g. next hotbar slot)
npx --yes uloop-cli@2.2.0 simulate-mouse-input --action Scroll --scroll-y -120

# Smooth camera pan right over 0.5 seconds
npx --yes uloop-cli@2.2.0 simulate-mouse-input --action SmoothDelta --delta-x 300 --delta-y 0 --duration 0.5
```

## Coordinate System

- `--x` / `--y` use **top-left Game View coordinates**.
- Raw image pixels from `npx --yes uloop-cli@2.2.0 screenshot --capture-mode rendering` must be converted with `ScreenshotToInputFormula`.
- `AnnotatedElements[].SimX/SimY` and `RaycastGridPoints[].InputX/InputY` can be passed directly to this tool.
- Do not flip Y in the caller. The tool converts internally for Unity Input System:

```text
unity_x = input_x
unity_y = gameViewHeight - input_y
```

- `Mouse.current.position` uses bottom-left Unity coordinates, so the value read inside Unity may show the converted Y.

## Prerequisites

- Unity must be in **PlayMode**
- **Input System package** must be installed (`com.unity.inputsystem`)
- Game code must read input via Input System API (e.g. `Mouse.current.leftButton.wasPressedThisFrame`)
- Use this only when the project already uses the New Input System.

## Output

Returns JSON with:
- `Success`: Whether the operation succeeded
- `Message`: Status message
- `Action`: Echoes which action was executed (`Click`, `LongPress`, `MoveDelta`, `SmoothDelta`, or `Scroll`)
- `Button`: Which button was used (nullable string; populated for `Click` / `LongPress`, null otherwise)
- `PositionX` / `PositionY`: Target top-left Game View coordinates (nullable float; populated for `Click` / `LongPress`)
- `InputCoordinateSystem`: `"top-left-game-view"` for click/long-press coordinates
- `UnityCoordinateSystem`: `"bottom-left-game-view"` for the injected `Mouse.current.position`
- `GameViewWidth` / `GameViewHeight`: Game View size used for conversion
- `InputPositionX` / `InputPositionY`: Coordinates received from the caller
- `InjectedUnityPositionX` / `InjectedUnityPositionY`: Coordinates injected into `Mouse.current.position`
- `CoordinateConversionFormula`: Conversion formula used by the tool

Verify visual outcome with a follow-up screenshot.

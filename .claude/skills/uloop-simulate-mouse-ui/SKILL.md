---
name: uloop-simulate-mouse-ui
description: "Simulate PlayMode EventSystem UI mouse actions using top-left Game View coordinates. Use for UI clicks, long-presses, or drags from annotated screenshots."
context: fork
---

# Task

Simulate mouse interaction on Unity PlayMode UI: $ARGUMENTS

## Workflow

1. Ensure Unity is in PlayMode (use `npx --yes uloop-cli@2.2.0 control-play-mode --action Play` if not)
2. Get UI element info: `npx --yes uloop-cli@2.2.0 screenshot --capture-mode rendering --annotate-elements true --elements-only true`
3. Use the `AnnotatedElements` array to find the target element by `Label`, `Name`, or `Path` (A=frontmost, B=next, ...). Use `Interaction` to distinguish click targets from drag/drop/text targets, then use `SimX`/`SimY` directly as `--x`/`--y` coordinates.
4. Execute the appropriate `npx --yes uloop-cli@2.2.0 simulate-mouse-ui` command
5. Take a screenshot to verify the result: `npx --yes uloop-cli@2.2.0 screenshot --capture-mode rendering --annotate-elements true`
6. Report what happened

## Tool Reference

```bash
npx --yes uloop-cli@2.2.0 simulate-mouse-ui --action <action> --x <x> --y <y> [options]
```

### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `--action` | enum | `Click` | `Click`, `Drag`, `DragStart`, `DragMove`, `DragEnd`, `LongPress` |
| `--x` | number | `0` | Target X position in Game View pixels (origin: top-left). Used by Click, LongPress, DragStart, DragMove, and DragEnd; for Drag, this is the destination. |
| `--y` | number | `0` | Target Y position in Game View pixels (origin: top-left). Used by Click, LongPress, DragStart, DragMove, and DragEnd; for Drag, this is the destination. |
| `--from-x` | number | `0` | Start X position in Game View pixels for Drag action. Drag starts here and moves to x,y. |
| `--from-y` | number | `0` | Start Y position in Game View pixels for Drag action. Drag starts here and moves to x,y. |
| `--drag-speed` | number | `2000` | Drag speed in pixels per second (0 for instant). 2000 is fast (default), 200 is slow enough to watch. Applies to Drag, DragMove, and DragEnd actions. |
| `--duration` | number | `0.5` | Hold duration in seconds for LongPress action. |
| `--button` | enum | `Left` | Mouse button. `Click` and `LongPress` support `Left`, `Right`, and `Middle`. Drag actions support `Left` only; other buttons return an error. |
| `--bypass-raycast` | boolean | `false` | For `Click`, `LongPress`, `Drag`, and `DragStart`, bypass EventSystem raycast and dispatch pointer events directly to `--target-path`. Use when a raycast-blocking overlay visually covers the intended target. |
| `--target-path` | string | `""` | Hierarchy path of the target GameObject, for example `Canvas/Panel/Button`. Required when `--bypass-raycast true` is used with `Click`, `LongPress`, `Drag`, or `DragStart`; prefer `AnnotatedElements[].Path` from screenshot JSON. |
| `--drop-target-path` | string | `""` | Optional hierarchy path of a drop target for `Drag` or `DragEnd`, for example `Canvas/DropZone`. Use this when the drop zone is also behind a raycast blocker. |

### Actions

| Action | Event Fired | Description |
|--------|-------------|-------------|
| `Click` | PointerDown → PointerUp → PointerClick | Click at (x, y) with the selected `--button` |
| `LongPress` | PointerDown → (hold) → PointerUp | Press and hold at (x, y) for `--duration` seconds, then release. No PointerClick is fired. |
| `Drag` | BeginDrag → Drag×N → EndDrag | One-shot drag from (fromX, fromY) to (x, y) at the specified speed |
| `DragStart` | BeginDrag | Begin drag at (x, y) and hold |
| `DragMove` | Drag×N | Animate from current position to (x, y) at the specified speed |
| `DragEnd` | Drag×N → EndDrag | Animate to (x, y) at the specified speed, then release drag |

### Split Drag Rules

- `DragStart` must be called before `DragMove` or `DragEnd`
- `DragEnd` must be called to release an active drag — failing to call it leaves drag state stuck
- Calling `DragMove` or `DragEnd` without an active drag returns an error
- `Drag`, `DragStart`, `DragMove`, and `DragEnd` only support `--button Left`

### Global Options (all optional, mutually exclusive)

| Option | Description |
|--------|-------------|
| `--project-path <path>` | Optional. Use only when the target Unity project is not the current directory. |


## Coordinate System

- Origin is **top-left** (0, 0)
- All positions are in **top-left Game View pixels**
- Get coordinates from `AnnotatedElements` JSON (`SimX`/`SimY`) — do NOT look up GameObject positions
- Clicking or long-pressing on empty space (no UI element) still succeeds with a message indicating no element was hit
- Dragging on empty space (no draggable UI element) returns `Success = false`
- `--bypass-raycast true` still uses coordinates for pointer event positions, but chooses the clicked, long-pressed, or dragged GameObject by `--target-path`
- If `--target-path` or `--drop-target-path` matches multiple active GameObjects, the command fails instead of choosing an arbitrary duplicate

## Examples

```bash
# Click a button at a Game View position
npx --yes uloop-cli@2.2.0 simulate-mouse-ui --action Click --x 400 --y 300

# Force-click a button behind a raycast blocker by path
npx --yes uloop-cli@2.2.0 simulate-mouse-ui --action Click --x 400 --y 300 --bypass-raycast true --target-path "Canvas/Panel/Button"

# Force-long-press a button behind a raycast blocker by path
npx --yes uloop-cli@2.2.0 simulate-mouse-ui --action LongPress --x 400 --y 300 --duration 3.0 --bypass-raycast true --target-path "Canvas/Panel/Button"

# Force-drag an item behind a raycast blocker by path
npx --yes uloop-cli@2.2.0 simulate-mouse-ui --action Drag --from-x 400 --from-y 300 --x 600 --y 300 --bypass-raycast true --target-path "Canvas/Item"

# Force-drag and dispatch Drop to a blocked drop zone
npx --yes uloop-cli@2.2.0 simulate-mouse-ui --action Drag --from-x 400 --from-y 300 --x 600 --y 300 --bypass-raycast true --target-path "Canvas/Item" --drop-target-path "Canvas/DropZone"

# Long-press a button for 3 seconds
npx --yes uloop-cli@2.2.0 simulate-mouse-ui --action LongPress --x 400 --y 300 --duration 3.0

# One-shot drag (start to end in one call)
npx --yes uloop-cli@2.2.0 simulate-mouse-ui --action Drag --from-x 400 --from-y 300 --x 600 --y 300

# Slow drag for visual inspection
npx --yes uloop-cli@2.2.0 simulate-mouse-ui --action Drag --from-x 400 --from-y 300 --x 600 --y 300 --drag-speed 200

# Split drag with hold (for inspection between steps)
npx --yes uloop-cli@2.2.0 simulate-mouse-ui --action DragStart --x 400 --y 300
npx --yes uloop-cli@2.2.0 screenshot --window-name Game
npx --yes uloop-cli@2.2.0 simulate-mouse-ui --action DragMove --x 500 --y 300
npx --yes uloop-cli@2.2.0 simulate-mouse-ui --action DragEnd --x 600 --y 300
```

## Prerequisites

- Unity must be in **PlayMode**
- Target scene must have an **EventSystem** GameObject
- UI elements must have a **GraphicRaycaster** on their Canvas
- If you need gameplay mouse input rather than UI pointer events, `simulate-mouse-input` assumes the project uses the New Input System; otherwise prefer `execute-dynamic-code`

## Output

Returns JSON with:
- `Success`: Whether the operation succeeded
- `Message`: Status message (e.g. "Hit element: ButtonStart" or "No UI element under (x, y)")
- `Action`: Echoes which action was executed (`Click`, `Drag`, `DragStart`, `DragMove`, `DragEnd`, or `LongPress`)
- `HitGameObjectName`: Name of the topmost UI element under the pointer (nullable string; null if nothing was hit)
- `PositionX`: Target X coordinate that was used
- `PositionY`: Target Y coordinate that was used
- `EndPositionX`: Drag end X coordinate (nullable float; populated for drag actions only)
- `EndPositionY`: Drag end Y coordinate (nullable float; populated for drag actions only)

These are the only eight fields. There is no `Button`, `Duration`, `DragSpeed`, raycast list, or pointer-event log in the response — verify the visual outcome with a follow-up `npx --yes uloop-cli@2.2.0 screenshot --capture-mode rendering --annotate-elements true`.

Note: Click and LongPress on empty space (no UI element) still return `Success = true` with `HitGameObjectName = null`. Drag actions on empty space return `Success = false`.

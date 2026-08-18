---
name: uloop-raycast
description: "Raycast from Camera.main through a Game View coordinate. Use when you need to check what a screenshot coordinate would hit in 3D physics before clicking or long-pressing with simulate-mouse-input."
---

# npx --yes uloop-cli@2.2.0 raycast

Check what a top-left Game View coordinate hits in 3D physics.

## Usage

```bash
npx --yes uloop-cli@2.2.0 raycast --x <x> --y <y> [--layer-mask <mask>] [--max-distance <distance>]
```

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `--x` | number | `0` | Target X position in Game View pixels (origin: top-left). Use `AnnotatedElements[].SimX`, `RaycastGridPoints[].InputX`, or raw image pixels converted with `ScreenshotToInputFormula`. |
| `--y` | number | `0` | Target Y position in Game View pixels (origin: top-left). Use `AnnotatedElements[].SimY`, `RaycastGridPoints[].InputY`, or raw image pixels converted with `ScreenshotToInputFormula`. |
| `--layer-mask` | number | Unity default raycast layers | Physics layer mask used by the raycast. |
| `--max-distance` | number | `1000` | Maximum raycast distance in world units. |

## Coordinate System

- `--x` / `--y` use the same top-left Game View input coordinates as `simulate-mouse-input`.
- Raw image pixels from `npx --yes uloop-cli@2.2.0 screenshot --capture-mode rendering` must be converted with `ScreenshotToInputFormula`.
- `AnnotatedElements[].SimX/SimY` and `RaycastGridPoints[].InputX/InputY` can be passed directly to this tool.
- Do not flip Y in the caller. The tool converts internally:

```text
unity_x = input_x
unity_y = gameViewHeight - input_y
```

## Examples

```bash
# Check what is under a screenshot coordinate
npx --yes uloop-cli@2.2.0 raycast --x 960 --y 540

# Check only specific layers
npx --yes uloop-cli@2.2.0 raycast --x 960 --y 540 --layer-mask 1
```

## Output

Returns JSON with:
- `Success`: Whether the command completed
- `Message`: Status message
- `Hit`: Whether physics hit anything
- `HitGameObjectName` / `HitGameObjectPath`: Hit object identity when `Hit` is true
- `HitLayer` / `HitLayerName`: Hit object layer when `Hit` is true
- `Distance`, `HitPointX/Y/Z`, `HitNormalX/Y/Z`: Hit details when `Hit` is true
- `InputCoordinateSystem`, `UnityCoordinateSystem`, `GameViewWidth/Height`, `InputPositionX/Y`, `InjectedUnityPositionX/Y`, `CoordinateConversionFormula`: Coordinate conversion details

## Notes

- Requires an active `Camera.main`.
- Uses Unity Physics raycasts, not UI EventSystem raycasts.

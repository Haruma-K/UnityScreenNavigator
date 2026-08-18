---
name: uloop-focus-window
description: "Bring the Unity Editor window to front. Use when Unity must be visible for visual checks or user-facing interaction."
---

# npx --yes uloop-cli@2.2.0 focus-window

Bring Unity Editor window to front using OS-level commands.

## Usage

```bash
npx --yes uloop-cli@2.2.0 focus-window
```

## Parameters

None.

## Global Options

| Option | Description |
|--------|-------------|
| `--project-path <path>` | Optional. Use only when the target Unity project is not the current directory. |

## Examples

```bash
# Focus Unity Editor
npx --yes uloop-cli@2.2.0 focus-window
```

## Output

Returns JSON with:
- `Success`: Whether the focus operation succeeded
- `Message`: Status message (e.g. `Unity Editor window focused (PID: 12345)`, or the failure reason such as `Unity project not found` / `No running Unity process found for this project` / `Failed to focus Unity window: <reason>`)

These are the only two fields. There is no PID, window-handle, or platform field returned to the caller.

## Notes

- **Works even when Unity is busy** (compiling, domain reload, etc.)
- Uses OS-level commands (osascript on macOS, PowerShell on Windows)
- Useful before `npx --yes uloop-cli@2.2.0 capture-unity-window` to ensure the target window is visible
- Brings the main Unity Editor window to the foreground

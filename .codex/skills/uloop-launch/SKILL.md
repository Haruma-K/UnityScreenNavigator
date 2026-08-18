---
name: uloop-launch
description: "Use when Unity Editor is not running or needs a clean restart."
---

# npx --yes uloop-cli@2.2.0 launch

Launch Unity Editor with the correct version for a project.

`npx --yes uloop-cli@2.2.0 launch` is not fire-and-forget. When Unity needs to start or restart, the command waits
until Unity is actually ready for CLI operations before it exits.

## Usage

```bash
npx --yes uloop-cli@2.2.0 launch [project-path] [options]
```

## Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `project-path` | string | Optional. Use only when the target Unity project is not in the current directory. |
| `-r, --restart` | boolean | Kill running Unity and restart |
| `-p, --platform <P>` | string | Build target (e.g., StandaloneOSX, Android, iOS) |
| `--max-depth <N>` | number | Search depth when project-path is omitted (default: 3, -1 for unlimited) |
| `-a, --add-unity-hub` | boolean | Add to Unity Hub only (does not launch) |
| `-f, --favorite` | boolean | Add to Unity Hub as favorite (does not launch) |

## Examples

```bash
# Search for Unity project in current directory and launch
npx --yes uloop-cli@2.2.0 launch

# Launch specific project
npx --yes uloop-cli@2.2.0 launch /path/to/project

# Restart Unity (kill existing and relaunch)
npx --yes uloop-cli@2.2.0 launch -r

# Launch with build target
npx --yes uloop-cli@2.2.0 launch -p Android

# Add project to Unity Hub without launching
npx --yes uloop-cli@2.2.0 launch -a
```

## Output

- Prints detected Unity version
- Prints project path
- If Unity is already running, focuses the existing window
- If launching, waits until Unity finishes startup and the CLI can connect to the project

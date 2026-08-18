---
name: uloop-find-game-objects
description: "Find or inspect Unity GameObjects, especially objects the user currently selected in the Hierarchy. Use for details, components, tags, layers, or name/path searches."
---

# npx --yes uloop-cli@2.2.0 find-game-objects

Find GameObjects with search criteria or get details for currently selected Hierarchy objects.

Use this before `execute-dynamic-code` when identifying or inspecting selected GameObjects. Use `get-hierarchy` instead when you need the child tree, parent-child structure, or descendants under the selection.

## Usage

```bash
npx --yes uloop-cli@2.2.0 find-game-objects [options]
```

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `--name-pattern` | string | - | Name pattern to search |
| `--search-mode` | string | `Exact` | Search mode: `Exact`, `Path`, `Regex`, `Contains`, `Selected` |
| `--required-components` | array | - | Required components |
| `--tag` | string | - | Tag filter |
| `--layer` | integer | - | Layer filter (layer number) |
| `--max-results` | integer | `20` | Maximum number of results |
| `--include-inactive` | boolean | `false` | Include inactive GameObjects |
| `--include-inherited-properties` | boolean | `false` | Include inherited properties in results |

## Search Modes

| Mode | Description |
|------|-------------|
| `Exact` | Exact name match (default) |
| `Path` | Hierarchy path search (e.g., `Canvas/Button`) |
| `Regex` | Regular expression pattern |
| `Contains` | Partial name match |
| `Selected` | Get currently selected GameObjects in Unity Editor |

## Global Options

| Option | Description |
|--------|-------------|
| `--project-path <path>` | Optional. Use only when the target Unity project is not the current directory. |

## Examples

```bash
# Find by name
npx --yes uloop-cli@2.2.0 find-game-objects --name-pattern "Player"

# Find with component
npx --yes uloop-cli@2.2.0 find-game-objects --required-components Rigidbody

# Find by tag
npx --yes uloop-cli@2.2.0 find-game-objects --tag "Enemy"

# Regex search
npx --yes uloop-cli@2.2.0 find-game-objects --name-pattern "UI_.*" --search-mode Regex

# Get selected GameObjects
npx --yes uloop-cli@2.2.0 find-game-objects --search-mode Selected

# Get selected including inactive
npx --yes uloop-cli@2.2.0 find-game-objects --search-mode Selected --include-inactive
```

## Output

Returns JSON with:
- `results` (array): Matching GameObjects, each containing:
  - `name` (string): GameObject name
  - `path` (string): Hierarchy path (e.g., `Canvas/Panel/Button`)
  - `isActive` (boolean): Active state in hierarchy
  - `tag` (string): GameObject tag
  - `layer` (number): Layer index
  - `components` (array): Each entry has `type` (short name, e.g., `Rigidbody`), `fullTypeName` (e.g., `UnityEngine.Rigidbody`), and `properties` (array of Inspector-visible `{name, type, value}` pairs)
- `totalFound` (number): Number of results returned inline, or number exported for multi-selection file output. For search modes, this is after `--max-results` clipping and serialization.
- `errorMessage` (string): Top-level failure summary (empty on success)
- `processingErrors` (array): Selected-mode per-GameObject serialization failures, each `{gameObjectName, gameObjectPath, error}`. Omitted/null or empty on clean runs.

### Multi-selection file export

For `Selected` mode with **multiple** successfully serialized GameObjects, inline `results` is not populated and the data is written to a file instead. Two extra fields appear:
- `resultsFilePath` (string): Relative path under `.uloop/outputs/FindGameObjectsResults/`
- `message` (string): Human-readable summary (e.g., "5 GameObjects exported")

Single-selection and search-mode calls (`Exact`, `Path`, `Regex`, `Contains`) always return inline. No selection (`Selected` mode with empty selection) returns empty `results` plus a `message`.

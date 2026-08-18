---
name: uloop-clear-console
description: "Clear Unity Console entries. Use before compile, tests, or debugging when stale logs would hide the current result."
---

# npx --yes uloop-cli@2.2.0 clear-console

Clear Unity console logs.

## Usage

```bash
npx --yes uloop-cli@2.2.0 clear-console [--add-confirmation-message]
```

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `--add-confirmation-message` | boolean | `false` | Add confirmation message after clearing |

## Global Options

| Option | Description |
|--------|-------------|
| `--project-path <path>` | Optional. Use only when the target Unity project is not the current directory. |

## Examples

```bash
# Clear console
npx --yes uloop-cli@2.2.0 clear-console

# Clear with confirmation
npx --yes uloop-cli@2.2.0 clear-console --add-confirmation-message
```

## Output

Returns JSON with:
- `Success` (boolean): Whether the clear operation succeeded
- `ClearedLogCount` (number): Total number of log entries that were cleared
- `ClearedCounts` (object): Breakdown by log type
  - `ErrorCount` (number): Errors cleared
  - `WarningCount` (number): Warnings cleared
  - `LogCount` (number): Info logs cleared
- `Message` (string): Description of the result; carries the failure summary when the operation fails (e.g. `"Failed to clear console: ..."`)
- `ErrorMessage` (string): Currently always empty for this tool — read `Message` for failure details

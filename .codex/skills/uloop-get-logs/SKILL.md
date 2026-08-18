---
name: uloop-get-logs
description: "Read current Unity Console entries from a running Editor. Use during bug investigation after compile, tests, PlayMode, or dynamic code to inspect logs, warnings, errors, and stack traces."
---

# npx --yes uloop-cli@2.2.0 get-logs

Retrieve logs from Unity Console.

## Usage

```bash
npx --yes uloop-cli@2.2.0 get-logs [options]
```

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `--log-type` | string | `All` | Log type filter: `Error`, `Warning`, `Log`, `All` |
| `--max-count` | integer | `100` | Maximum number of logs to retrieve |
| `--search-text` | string | - | Text to search within logs |
| `--include-stack-trace` | boolean | `false` | Include stack trace in output |
| `--use-regex` | boolean | `false` | Use regex for search |
| `--search-in-stack-trace` | boolean | `false` | Search within stack trace |

## Global Options

| Option | Description |
|--------|-------------|
| `--project-path <path>` | Optional. Use only when the target Unity project is not the current directory. |

## Examples

```bash
# Get all logs
npx --yes uloop-cli@2.2.0 get-logs

# Get only errors
npx --yes uloop-cli@2.2.0 get-logs --log-type Error

# Search for specific text
npx --yes uloop-cli@2.2.0 get-logs --search-text "NullReference"

# Regex search
npx --yes uloop-cli@2.2.0 get-logs --search-text "Missing.*Component" --use-regex
```

## Output

Returns JSON with:
- `TotalCount` (number): Total logs available before max-count clipping
- `DisplayedCount` (number): Logs returned in this response (≤ `--max-count`)
- `LogType` (string): The `--log-type` filter that was applied
- `MaxCount` (number): The `--max-count` cap that was applied
- `SearchText` (string): The `--search-text` filter that was applied (empty when omitted)
- `IncludeStackTrace` (boolean): Whether stack traces are included in `Logs[]`
- `Logs` (array): Each entry has:
  - `Type` (string): `"Error"`, `"Warning"`, or `"Log"`
  - `Message` (string): Log message body
  - `StackTrace` (string): Stack trace text. Empty when `--include-stack-trace` is `false`.

# PlayMode UI Controls

Code examples for manipulating UI controls at runtime using `execute-dynamic-code`.
These examples interact with Unity UI components while the game is running.

## Set InputField Text

```csharp
using UnityEngine.UI;

InputField input = GameObject.Find("Canvas/NameInput")?.GetComponent<InputField>();
if (input == null) return "InputField not found";

input.text = "Player1";
input.onEndEdit.Invoke(input.text);
return $"Set InputField text to: {input.text}";
```

## Set Slider Value

```csharp
using UnityEngine.UI;

Slider slider = GameObject.Find("Canvas/VolumeSlider")?.GetComponent<Slider>();
if (slider == null) return "Slider not found";

float oldValue = slider.value;
slider.value = 0.75f;
return $"Slider changed from {oldValue} to {slider.value}";
```

## Set Slider Value with Bounds Check

```csharp
using UnityEngine.UI;
using System.Linq;

Slider[] sliders = Object.FindObjectsByType<Slider>(FindObjectsSortMode.None);
Slider target = sliders.FirstOrDefault(s => s.gameObject.name == "HealthBar");
if (target == null) return $"HealthBar not found. Available: {string.Join(", ", sliders.Select(s => s.gameObject.name))}";

float targetValue = 0.5f;
float clamped = Mathf.Clamp(targetValue, target.minValue, target.maxValue);
target.value = clamped;
return $"Set {target.gameObject.name} to {clamped} (range: {target.minValue}-{target.maxValue})";
```

## Toggle On/Off

```csharp
using UnityEngine.UI;

Toggle toggle = GameObject.Find("Canvas/MuteToggle")?.GetComponent<Toggle>();
if (toggle == null) return "Toggle not found";

bool oldValue = toggle.isOn;
toggle.isOn = !toggle.isOn;
return $"Toggle changed from {oldValue} to {toggle.isOn}";
```

## Set Toggle to Specific Value

```csharp
using UnityEngine.UI;
using System.Linq;

Toggle[] toggles = Object.FindObjectsByType<Toggle>(FindObjectsSortMode.None);
Toggle target = toggles.FirstOrDefault(t => t.gameObject.name == "SoundToggle");
if (target == null) return $"SoundToggle not found. Available: {string.Join(", ", toggles.Select(t => t.gameObject.name))}";

target.isOn = true;
return $"Set {target.gameObject.name} to {target.isOn}";
```

## Select Dropdown Item by Index

```csharp
using UnityEngine.UI;

Dropdown dropdown = GameObject.Find("Canvas/DifficultyDropdown")?.GetComponent<Dropdown>();
if (dropdown == null) return "Dropdown not found";

if (dropdown.options.Count == 0) return "Dropdown has no options";

int targetIndex = 2;
if (targetIndex >= dropdown.options.Count)
    return $"Index {targetIndex} out of range. Options count: {dropdown.options.Count}";

dropdown.value = targetIndex;
return $"Selected: {dropdown.options[targetIndex].text} (index {targetIndex})";
```

## Select Dropdown Item by Text

```csharp
using UnityEngine.UI;
using System.Linq;

Dropdown dropdown = GameObject.Find("Canvas/DifficultyDropdown")?.GetComponent<Dropdown>();
if (dropdown == null) return "Dropdown not found";

string targetText = "Hard";
int index = dropdown.options.FindIndex(o => o.text == targetText);
if (index < 0)
    return $"'{targetText}' not found. Available: {string.Join(", ", dropdown.options.Select(o => o.text))}";

dropdown.value = index;
return $"Selected: {targetText} (index {index})";
```

## List All Dropdown Options

```csharp
using UnityEngine.UI;
using System.Text;

Dropdown dropdown = GameObject.Find("Canvas/DifficultyDropdown")?.GetComponent<Dropdown>();
if (dropdown == null) return "Dropdown not found";

StringBuilder sb = new StringBuilder();
sb.AppendLine($"Dropdown '{dropdown.gameObject.name}' - current: {dropdown.value}");
for (int i = 0; i < dropdown.options.Count; i++)
{
    string marker = i == dropdown.value ? " [selected]" : "";
    sb.AppendLine($"  [{i}] {dropdown.options[i].text}{marker}");
}
return sb.ToString();
```

## Simulate Drag on UI Element

```csharp
using UnityEngine.EventSystems;
using System.Collections.Generic;

GameObject target = GameObject.Find("Canvas/DraggableItem");
if (target == null) return "DraggableItem not found";

PointerEventData pointerData = new PointerEventData(EventSystem.current)
{
    position = new Vector2(Screen.width / 2f, Screen.height / 2f)
};

// Verify target handles drag events
if (!ExecuteEvents.CanHandleEvent<IDragHandler>(target))
    return $"{target.name} does not handle drag events";

// Begin drag
ExecuteEvents.Execute(target, pointerData, ExecuteEvents.beginDragHandler);

// Move (simulate drag to a new position)
pointerData.position = new Vector2(Screen.width / 2f + 100f, Screen.height / 2f);
ExecuteEvents.Execute(target, pointerData, ExecuteEvents.dragHandler);

// End drag
ExecuteEvents.Execute(target, pointerData, ExecuteEvents.endDragHandler);
return $"Dragged {target.name} 100px to the right";
```

## List All UI Controls in Scene

```csharp
using UnityEngine.UI;
using System.Text;

StringBuilder sb = new StringBuilder();

Slider[] sliders = Object.FindObjectsByType<Slider>(FindObjectsSortMode.None);
Toggle[] toggles = Object.FindObjectsByType<Toggle>(FindObjectsSortMode.None);
Dropdown[] dropdowns = Object.FindObjectsByType<Dropdown>(FindObjectsSortMode.None);
InputField[] inputs = Object.FindObjectsByType<InputField>(FindObjectsSortMode.None);
ScrollRect[] scrolls = Object.FindObjectsByType<ScrollRect>(FindObjectsSortMode.None);

sb.AppendLine($"Sliders ({sliders.Length}):");
foreach (Slider s in sliders) sb.AppendLine($"  {s.gameObject.name} = {s.value} ({s.minValue}-{s.maxValue})");

sb.AppendLine($"Toggles ({toggles.Length}):");
foreach (Toggle t in toggles) sb.AppendLine($"  {t.gameObject.name} = {t.isOn}");

sb.AppendLine($"Dropdowns ({dropdowns.Length}):");
foreach (Dropdown d in dropdowns) sb.AppendLine($"  {d.gameObject.name} = {(d.options.Count > 0 ? d.options[d.value].text : "(empty)")} (index {d.value})");

sb.AppendLine($"InputFields ({inputs.Length}):");
foreach (InputField i in inputs) sb.AppendLine($"  {i.gameObject.name} = \"{i.text}\"");

sb.AppendLine($"ScrollRects ({scrolls.Length}):");
foreach (ScrollRect sr in scrolls) sb.AppendLine($"  {sr.gameObject.name} pos={sr.normalizedPosition}");

return sb.ToString();
```

# Selection Operations

Code examples for Selection operations using `execute-dynamic-code`.

## Get Selected GameObjects

```csharp
using UnityEditor;
using System.Collections.Generic;

GameObject[] selected = Selection.gameObjects;
if (selected.Length == 0)
{
    return "No GameObjects selected";
}

List<string> names = new List<string>();
foreach (GameObject obj in selected)
{
    names.Add(obj.name);
}
return $"Selected: {string.Join(", ", names)}";
```

## Get Active (Last Selected) GameObject

```csharp
using UnityEditor;

GameObject active = Selection.activeGameObject;
if (active == null)
{
    return "No active GameObject";
}
return $"Active: {active.name}";
```

## Set Selection Programmatically

```csharp
using UnityEditor;

GameObject obj = GameObject.Find("Player");
if (obj == null)
{
    return "GameObject 'Player' not found";
}

Selection.activeGameObject = obj;
return $"Selected {obj.name}";
```

## Select Multiple GameObjects

```csharp
using UnityEditor;

GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
if (enemies.Length == 0)
{
    return "No enemies found";
}

Selection.objects = enemies;
return $"Selected {enemies.Length} enemies";
```

## Get Top-Level Transforms Only

```csharp
using UnityEditor;
using System.Collections.Generic;

Transform[] transforms = Selection.GetTransforms(SelectionMode.TopLevel);
if (transforms.Length == 0)
{
    return "No transforms selected";
}

List<string> names = new List<string>();
foreach (Transform t in transforms)
{
    names.Add(t.name);
}
return $"Top-level: {string.Join(", ", names)}";
```

## Get Deep Selection (Including Children)

```csharp
using UnityEditor;

Transform[] transforms = Selection.GetTransforms(SelectionMode.Deep);
if (transforms.Length == 0)
{
    return "No transforms selected";
}

return $"Deep selection count: {transforms.Length}";
```

## Get Editable Objects Only

```csharp
using UnityEditor;
using System.Collections.Generic;

Transform[] transforms = Selection.GetTransforms(SelectionMode.Editable);
if (transforms.Length == 0)
{
    return "No editable transforms selected";
}

List<string> names = new List<string>();
foreach (Transform t in transforms)
{
    names.Add(t.name);
}
return $"Editable: {string.Join(", ", names)}";
```

## Get Selected Assets

```csharp
using UnityEditor;
using System.Collections.Generic;

Object[] selectedAssets = Selection.GetFiltered<Object>(SelectionMode.Assets);
if (selectedAssets.Length == 0)
{
    return "No assets selected";
}

List<string> paths = new List<string>();
foreach (Object asset in selectedAssets)
{
    paths.Add(AssetDatabase.GetAssetPath(asset));
}
return $"Assets: {string.Join(", ", paths)}";
```

## Get Selected Asset GUIDs

```csharp
using UnityEditor;
using System.Collections.Generic;

string[] guids = Selection.assetGUIDs;
if (guids.Length == 0)
{
    return "No assets selected";
}

List<string> paths = new List<string>();
foreach (string guid in guids)
{
    paths.Add(AssetDatabase.GUIDToAssetPath(guid));
}
return $"Selected assets: {string.Join(", ", paths)}";
```

## Select All Children of Selected Object

```csharp
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

GameObject parent = Selection.activeGameObject;
if (parent == null)
{
    return "No GameObject selected";
}

List<GameObject> children = new List<GameObject>();
foreach (Transform child in parent.GetComponentsInChildren<Transform>())
{
    if (child != parent.transform)
    {
        children.Add(child.gameObject);
    }
}

if (children.Count == 0)
{
    return "No children found";
}

Selection.objects = children.ToArray();
return $"Selected {children.Count} children";
```

## Filter Selection by Component

```csharp
using UnityEditor;
using System.Collections.Generic;

GameObject[] selected = Selection.gameObjects;
List<GameObject> withRigidbody = new List<GameObject>();

foreach (GameObject obj in selected)
{
    if (obj.GetComponent<Rigidbody>() != null)
    {
        withRigidbody.Add(obj);
    }
}

if (withRigidbody.Count == 0)
{
    return "No objects with Rigidbody in selection";
}

Selection.objects = withRigidbody.ToArray();
return $"Filtered to {withRigidbody.Count} objects with Rigidbody";
```

## Check if Object is Selected

```csharp
using UnityEditor;

GameObject player = GameObject.Find("Player");
if (player == null)
{
    return "Player not found";
}

bool isSelected = Selection.Contains(player);
return $"Player is {(isSelected ? "" : "not ")}selected";
```

## Clear Selection

```csharp
using UnityEditor;

Selection.activeObject = null;
return "Selection cleared";
```

## Select Objects by Layer

```csharp
using UnityEditor;
using System.Collections.Generic;

int layer = LayerMask.NameToLayer("UI");
if (layer == -1)
{
    return "Layer 'UI' not found";
}

GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
List<GameObject> layerObjects = new List<GameObject>();

foreach (GameObject obj in allObjects)
{
    if (obj.layer == layer)
    {
        layerObjects.Add(obj);
    }
}

if (layerObjects.Count == 0)
{
    return "No objects found on UI layer";
}

Selection.objects = layerObjects.ToArray();
return $"Selected {layerObjects.Count} objects on UI layer";
```

## Ping Object in Hierarchy/Project

```csharp
using UnityEditor;

GameObject obj = GameObject.Find("Player");
if (obj == null)
{
    return "Player not found";
}

EditorGUIUtility.PingObject(obj);
return $"Pinged {obj.name} in Hierarchy";
```

## Focus on Selected Object in Scene View

```csharp
using UnityEditor;

if (Selection.activeGameObject == null)
{
    return "No GameObject selected";
}

SceneView.FrameLastActiveSceneView();
return "Focused on selected object";
```

# Scene Operations

Code examples for Scene and Hierarchy operations using `execute-dynamic-code`.

## Create GameObject

```csharp
GameObject obj = new GameObject("MyObject");
obj.transform.position = new Vector3(0, 1, 0);
return $"Created {obj.name}";
```

## Create UI GameObject (under Canvas)

```csharp
using UnityEngine.UI;

// UI objects require RectTransform, which is auto-added when parented to Canvas
GameObject canvas = GameObject.Find("Canvas");
if (canvas == null)
{
    return "Canvas not found in scene";
}

GameObject uiObj = new GameObject("MyUIElement");
uiObj.transform.SetParent(canvas.transform, false);
uiObj.AddComponent<RectTransform>();
uiObj.AddComponent<Image>();
return $"Created UI element: {uiObj.name}";
```

## Create Primitive

```csharp
GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
cube.name = "MyCube";
cube.transform.position = new Vector3(2, 0, 0);
return $"Created {cube.name}";
```

## Add Component to GameObject

```csharp
GameObject selected = Selection.activeGameObject;
if (selected == null)
{
    return "No GameObject selected";
}

Rigidbody rb = selected.AddComponent<Rigidbody>();
rb.mass = 2f;
rb.useGravity = true;
return $"Added Rigidbody to {selected.name}";
```

## Find GameObject by Name

```csharp
GameObject obj = GameObject.Find("Player");
if (obj == null)
{
    return "GameObject 'Player' not found";
}
return $"Found: {obj.name} at {obj.transform.position}";
```

## Find GameObjects by Tag

```csharp
GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
return $"Found {enemies.Length} GameObjects with tag 'Enemy'";
```

## Set Parent

```csharp
GameObject child = GameObject.Find("Child");
GameObject parent = GameObject.Find("Parent");

if (child == null || parent == null)
{
    return "Child or Parent not found";
}

child.transform.SetParent(parent.transform);
return $"Set {child.name}'s parent to {parent.name}";
```

## Get All Children

```csharp
GameObject parent = Selection.activeGameObject;
if (parent == null)
{
    return "No GameObject selected";
}

List<string> children = new List<string>();
foreach (Transform child in parent.transform)
{
    children.Add(child.name);
}
return $"Children: {string.Join(", ", children)}";
```

## Wire Component References

```csharp
using UnityEditor;

GameObject player = GameObject.Find("Player");
GameObject target = GameObject.Find("Target");

if (player == null || target == null)
{
    return "Player or Target not found";
}

MonoBehaviour script = player.GetComponent("PlayerController") as MonoBehaviour;
if (script == null)
{
    return "PlayerController not found on Player";
}

SerializedObject serializedScript = new SerializedObject(script);
SerializedProperty targetProp = serializedScript.FindProperty("target");

if (targetProp != null)
{
    targetProp.objectReferenceValue = target.transform;
    serializedScript.ApplyModifiedProperties();
    return "Target reference wired";
}
return "Property 'target' not found";
```

## Load Scene (Editor)

```csharp
using UnityEditor.SceneManagement;

string scenePath = "Assets/Scenes/MainMenu.unity";
EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
return $"Loaded scene: {scenePath}";
```

## Save Current Scene

```csharp
using UnityEditor.SceneManagement;

UnityEngine.SceneManagement.Scene scene = EditorSceneManager.GetActiveScene();
EditorSceneManager.SaveScene(scene);
return $"Saved scene: {scene.name}";
```

## Create New Scene

```csharp
using UnityEditor.SceneManagement;

UnityEngine.SceneManagement.Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
return $"Created new scene: {newScene.name}";
```

## Get All Root GameObjects in Scene

```csharp
UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
GameObject[] roots = scene.GetRootGameObjects();

List<string> names = new List<string>();
foreach (GameObject root in roots)
{
    names.Add(root.name);
}
return $"Root objects: {string.Join(", ", names)}";
```

## Destroy GameObject

```csharp
GameObject obj = GameObject.Find("OldObject");
if (obj == null)
{
    return "GameObject not found";
}

Object.DestroyImmediate(obj);
return "GameObject destroyed";
```

## Duplicate GameObject

```csharp
GameObject selected = Selection.activeGameObject;
if (selected == null)
{
    return "No GameObject selected";
}

GameObject copy = Object.Instantiate(selected);
copy.name = selected.name + "_Copy";
copy.transform.position = selected.transform.position + Vector3.right * 2;
return $"Created duplicate: {copy.name}";
```

## Set Active/Inactive

```csharp
GameObject obj = GameObject.Find("MyObject");
if (obj == null)
{
    return "GameObject not found";
}

obj.SetActive(!obj.activeSelf);
return $"{obj.name} is now {(obj.activeSelf ? "active" : "inactive")}";
```

## Modify Transform

```csharp
GameObject selected = Selection.activeGameObject;
if (selected == null)
{
    return "No GameObject selected";
}

selected.transform.position = new Vector3(0, 5, 0);
selected.transform.rotation = Quaternion.Euler(0, 45, 0);
selected.transform.localScale = new Vector3(2, 2, 2);
return "Transform modified";
```

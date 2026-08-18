# Undo Operations

Code examples for Undo-supported operations using `execute-dynamic-code`.

## Record Property Change (Undo.RecordObject)

```csharp
using UnityEditor;

GameObject selected = Selection.activeGameObject;
if (selected == null)
{
    return "No GameObject selected";
}

Undo.RecordObject(selected.transform, "Move Object");
selected.transform.position = new Vector3(0, 5, 0);
return $"Moved {selected.name} (Undo available)";
```

## Record Multiple Objects

```csharp
using UnityEditor;

GameObject[] selectedObjects = Selection.gameObjects;
if (selectedObjects.Length == 0)
{
    return "No GameObjects selected";
}

Object[] transforms = new Object[selectedObjects.Length];
for (int i = 0; i < selectedObjects.Length; i++)
{
    transforms[i] = selectedObjects[i].transform;
}

Undo.RecordObjects(transforms, "Move Multiple Objects");
foreach (GameObject obj in selectedObjects)
{
    obj.transform.position += Vector3.up * 2;
}
return $"Moved {selectedObjects.Length} objects (Undo available)";
```

## Complete Object Undo (For Complex Changes)

```csharp
using UnityEditor;

GameObject selected = Selection.activeGameObject;
if (selected == null)
{
    return "No GameObject selected";
}

Undo.RegisterCompleteObjectUndo(selected, "Complete Object Change");
selected.name = "RenamedObject";
selected.layer = LayerMask.NameToLayer("Default");
selected.tag = "Untagged";
return $"Modified object completely (Undo available)";
```

## Add Component with Undo

```csharp
using UnityEditor;

GameObject selected = Selection.activeGameObject;
if (selected == null)
{
    return "No GameObject selected";
}

Rigidbody rb = Undo.AddComponent<Rigidbody>(selected);
rb.mass = 2f;
rb.useGravity = true;
return $"Added Rigidbody to {selected.name} (Undo available)";
```

## Set Parent with Undo

```csharp
using UnityEditor;

GameObject child = GameObject.Find("Child");
GameObject parent = GameObject.Find("Parent");

if (child == null || parent == null)
{
    return "Child or Parent not found";
}

Undo.SetTransformParent(child.transform, parent.transform, "Set Parent");
return $"Set {child.name}'s parent to {parent.name} (Undo available)";
```

## Create GameObject with Undo

```csharp
using UnityEditor;

GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
cube.name = "UndoableCube";
cube.transform.position = new Vector3(0, 1, 0);
Undo.RegisterCreatedObjectUndo(cube, "Create Cube");
return $"Created {cube.name} (Undo available)";
```

## Destroy GameObject with Undo

```csharp
using UnityEditor;

GameObject obj = GameObject.Find("ObjectToDelete");
if (obj == null)
{
    return "GameObject not found";
}

Undo.DestroyObjectImmediate(obj);
return "GameObject destroyed (Undo available)";
```

## Named Undo Group

```csharp
using UnityEditor;

GameObject selected = Selection.activeGameObject;
if (selected == null)
{
    return "No GameObject selected";
}

Undo.SetCurrentGroupName("Complex Transform Operation");

Undo.RecordObject(selected.transform, "");
selected.transform.position = Vector3.zero;
selected.transform.rotation = Quaternion.identity;
selected.transform.localScale = Vector3.one;

return "Reset transform (Single undo step)";
```

## Collapse Multiple Operations into One Undo

```csharp
using UnityEditor;

int undoGroup = Undo.GetCurrentGroup();
Undo.SetCurrentGroupName("Batch Operation");

GameObject cube1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
cube1.name = "Cube1";
Undo.RegisterCreatedObjectUndo(cube1, "");

GameObject cube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
cube2.name = "Cube2";
cube2.transform.position = Vector3.right * 2;
Undo.RegisterCreatedObjectUndo(cube2, "");

GameObject cube3 = GameObject.CreatePrimitive(PrimitiveType.Cube);
cube3.name = "Cube3";
cube3.transform.position = Vector3.right * 4;
Undo.RegisterCreatedObjectUndo(cube3, "");

Undo.CollapseUndoOperations(undoGroup);
return "Created 3 cubes (Single undo step)";
```

## Modify ScriptableObject with Undo

```csharp
using UnityEditor;

string path = "Assets/Data/GameSettings.asset";
ScriptableObject so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
if (so == null)
{
    return $"Asset not found at {path}";
}

Undo.RecordObject(so, "Modify Settings");
SerializedObject serializedObj = new SerializedObject(so);
SerializedProperty prop = serializedObj.FindProperty("maxHealth");
if (prop != null)
{
    prop.intValue = 200;
    serializedObj.ApplyModifiedProperties();
}
return "Modified ScriptableObject (Undo available)";
```

## Modify Material with Undo

```csharp
using UnityEditor;

string path = "Assets/Materials/MyMaterial.mat";
Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
if (mat == null)
{
    return $"Material not found at {path}";
}

Undo.RecordObject(mat, "Change Material Color");
mat.color = Color.red;
return "Changed material color (Undo available)";
```

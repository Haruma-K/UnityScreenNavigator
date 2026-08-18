# Cleanup Operations

Code examples for project cleanup operations using `execute-dynamic-code`.

## Detect Missing Scripts on GameObject

```csharp
using UnityEditor;

GameObject selected = Selection.activeGameObject;
if (selected == null)
{
    return "No GameObject selected";
}

int missingCount = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(selected);
return $"{selected.name} has {missingCount} missing script(s)";
```

## Remove Missing Scripts from GameObject

```csharp
using UnityEditor;

GameObject selected = Selection.activeGameObject;
if (selected == null)
{
    return "No GameObject selected";
}

int removedCount = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(selected);
return $"Removed {removedCount} missing script(s) from {selected.name}";
```

## Scan Scene for Missing Scripts

```csharp
using UnityEditor;

GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
List<string> objectsWithMissing = new List<string>();

foreach (GameObject obj in allObjects)
{
    int count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(obj);
    if (count > 0)
    {
        objectsWithMissing.Add($"{obj.name} ({count})");
    }
}

if (objectsWithMissing.Count == 0)
{
    return "No missing scripts found in scene";
}

return $"Objects with missing scripts: {string.Join(", ", objectsWithMissing)}";
```

## Remove All Missing Scripts from Scene

```csharp
using UnityEditor;

GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
int totalRemoved = 0;

int undoGroup = Undo.GetCurrentGroup();
Undo.SetCurrentGroupName("Remove All Missing Scripts");

foreach (GameObject obj in allObjects)
{
    int removed = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(obj);
    totalRemoved += removed;
}

Undo.CollapseUndoOperations(undoGroup);
return $"Removed {totalRemoved} missing scripts from scene";
```

## Detect Missing References in Component

```csharp
using UnityEditor;

GameObject selected = Selection.activeGameObject;
if (selected == null)
{
    return "No GameObject selected";
}

List<string> missingRefs = new List<string>();

Component[] components = selected.GetComponents<Component>();
foreach (Component comp in components)
{
    if (comp == null) continue;

    SerializedObject so = new SerializedObject(comp);
    SerializedProperty prop = so.GetIterator();

    while (prop.NextVisible(true))
    {
        if (prop.propertyType == SerializedPropertyType.ObjectReference)
        {
            if (prop.objectReferenceValue == null && prop.objectReferenceInstanceIDValue != 0)
            {
                missingRefs.Add($"{comp.GetType().Name}.{prop.name}");
            }
        }
    }
}

if (missingRefs.Count == 0)
{
    return "No missing references found";
}

return $"Missing references: {string.Join(", ", missingRefs)}";
```

## Scan Scene for Missing References

```csharp
using UnityEditor;

GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
List<string> results = new List<string>();

foreach (GameObject obj in allObjects)
{
    Component[] components = obj.GetComponents<Component>();
    foreach (Component comp in components)
    {
        if (comp == null) continue;

        SerializedObject so = new SerializedObject(comp);
        SerializedProperty prop = so.GetIterator();

        while (prop.NextVisible(true))
        {
            if (prop.propertyType == SerializedPropertyType.ObjectReference)
            {
                if (prop.objectReferenceValue == null && prop.objectReferenceInstanceIDValue != 0)
                {
                    results.Add($"{obj.name}/{comp.GetType().Name}.{prop.name}");
                }
            }
        }
    }
}

if (results.Count == 0)
{
    return "No missing references found in scene";
}

return $"Missing references ({results.Count}): {string.Join(", ", results.Take(10))}...";
```

## Find Unused Materials in Project

```csharp
using UnityEditor;
using System.Collections.Generic;

string[] materialGuids = AssetDatabase.FindAssets("t:Material");
HashSet<string> usedMaterials = new HashSet<string>();

string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
foreach (string guid in prefabGuids)
{
    string path = AssetDatabase.GUIDToAssetPath(guid);
    string[] deps = AssetDatabase.GetDependencies(path, true);
    foreach (string dep in deps)
    {
        if (dep.EndsWith(".mat"))
        {
            usedMaterials.Add(dep);
        }
    }
}

// This scan only checks prefab dependencies. Verify scene and other asset
// references manually before deleting any reported materials.
List<string> unusedMaterials = new List<string>();
foreach (string guid in materialGuids)
{
    string path = AssetDatabase.GUIDToAssetPath(guid);
    if (!usedMaterials.Contains(path))
    {
        unusedMaterials.Add(path);
    }
}

return $"Found {unusedMaterials.Count} materials not referenced by prefabs. Verify scene and other asset references manually before deleting.";
```

## Find Empty GameObjects

```csharp
using UnityEditor;

GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
List<string> emptyObjects = new List<string>();

foreach (GameObject obj in allObjects)
{
    Component[] components = obj.GetComponents<Component>();
    if (components.Length == 1 && obj.transform.childCount == 0)
    {
        emptyObjects.Add(obj.name);
    }
}

if (emptyObjects.Count == 0)
{
    return "No empty GameObjects found";
}

return $"Empty objects ({emptyObjects.Count}): {string.Join(", ", emptyObjects.Take(20))}";
```

## Find Duplicate Names in Hierarchy

```csharp
using UnityEditor;

GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
Dictionary<string, int> nameCounts = new Dictionary<string, int>();

foreach (GameObject obj in allObjects)
{
    if (nameCounts.ContainsKey(obj.name))
    {
        nameCounts[obj.name]++;
    }
    else
    {
        nameCounts[obj.name] = 1;
    }
}

List<string> duplicates = new List<string>();
foreach (KeyValuePair<string, int> kvp in nameCounts)
{
    if (kvp.Value > 1)
    {
        duplicates.Add($"{kvp.Key} ({kvp.Value})");
    }
}

if (duplicates.Count == 0)
{
    return "No duplicate names found";
}

return $"Duplicate names: {string.Join(", ", duplicates.Take(15))}";
```

## Check for Broken Prefab Instances

```csharp
using UnityEditor;

GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
List<string> brokenPrefabs = new List<string>();

foreach (GameObject obj in allObjects)
{
    if (PrefabUtility.IsPartOfPrefabInstance(obj))
    {
        GameObject prefabAsset = PrefabUtility.GetCorrespondingObjectFromSource(obj);
        if (prefabAsset == null)
        {
            brokenPrefabs.Add(obj.name);
        }
    }
}

if (brokenPrefabs.Count == 0)
{
    return "No broken prefab instances found";
}

return $"Broken prefab instances: {string.Join(", ", brokenPrefabs)}";
```

## Find Objects with Negative Scale

```csharp
using UnityEditor;

GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
List<string> negativeScale = new List<string>();

foreach (GameObject obj in allObjects)
{
    Vector3 scale = obj.transform.localScale;
    if (scale.x < 0 || scale.y < 0 || scale.z < 0)
    {
        negativeScale.Add($"{obj.name} ({scale})");
    }
}

if (negativeScale.Count == 0)
{
    return "No objects with negative scale found";
}

return $"Negative scale objects: {string.Join(", ", negativeScale.Take(10))}";
```

## Remove Empty Leaf GameObjects

```csharp
using UnityEditor;

GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);

int undoGroup = Undo.GetCurrentGroup();
Undo.SetCurrentGroupName("Remove Empty Leaves");

int removedCount = 0;
foreach (GameObject obj in allObjects)
{
    if (obj == null) continue;

    Component[] components = obj.GetComponents<Component>();
    if (components.Length == 1 && obj.transform.childCount == 0)
    {
        Undo.DestroyObjectImmediate(obj);
        removedCount++;
    }
}

Undo.CollapseUndoOperations(undoGroup);
return $"Removed {removedCount} empty leaf GameObjects";
```

## Find Large Meshes

```csharp
using UnityEditor;

string[] meshGuids = AssetDatabase.FindAssets("t:Mesh");
List<string> largeMeshes = new List<string>();
int threshold = 10000;

foreach (string guid in meshGuids)
{
    string path = AssetDatabase.GUIDToAssetPath(guid);
    Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
    if (mesh != null && mesh.vertexCount > threshold)
    {
        largeMeshes.Add($"{path} ({mesh.vertexCount} verts)");
    }
}

if (largeMeshes.Count == 0)
{
    return $"No meshes with more than {threshold} vertices found";
}

return $"Large meshes: {string.Join(", ", largeMeshes.Take(10))}";
```

## Validate Asset References

```csharp
using UnityEditor;

string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { "Assets/Data" });
List<string> invalidRefs = new List<string>();

foreach (string guid in guids)
{
    string path = AssetDatabase.GUIDToAssetPath(guid);
    ScriptableObject so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
    if (so == null) continue;

    SerializedObject serializedObj = new SerializedObject(so);
    SerializedProperty prop = serializedObj.GetIterator();

    while (prop.NextVisible(true))
    {
        if (prop.propertyType == SerializedPropertyType.ObjectReference)
        {
            if (prop.objectReferenceValue == null && prop.objectReferenceInstanceIDValue != 0)
            {
                invalidRefs.Add($"{path}: {prop.name}");
            }
        }
    }
}

if (invalidRefs.Count == 0)
{
    return "All asset references are valid";
}

return $"Invalid references ({invalidRefs.Count}): {string.Join(", ", invalidRefs.Take(10))}";
```

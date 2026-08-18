# Prefab Operations

Code examples for Prefab operations using `execute-dynamic-code`.

## Create a Prefab from GameObject

```csharp
using UnityEditor;

GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
cube.name = "MyCube";
string path = "Assets/Prefabs/MyCube.prefab";
PrefabUtility.SaveAsPrefabAsset(cube, path);
Object.DestroyImmediate(cube);
return $"Prefab created at {path}";
```

## Instantiate a Prefab

```csharp
using UnityEditor;

string prefabPath = "Assets/Prefabs/MyCube.prefab";
GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
if (prefab == null)
{
    return $"Prefab not found at {prefabPath}";
}

GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
instance.transform.position = new Vector3(0, 1, 0);
return $"Instantiated {instance.name}";
```

## Add Component to Prefab

```csharp
using UnityEditor;

string prefabPath = "Assets/Prefabs/MyCube.prefab";
GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
if (prefab == null)
{
    return $"Prefab not found at {prefabPath}";
}
string assetPath = AssetDatabase.GetAssetPath(prefab);

using (PrefabUtility.EditPrefabContentsScope scope = new PrefabUtility.EditPrefabContentsScope(assetPath))
{
    GameObject root = scope.prefabContentsRoot;
    if (root.GetComponent<Rigidbody>() == null)
    {
        root.AddComponent<Rigidbody>();
    }
}
return "Added Rigidbody to prefab";
```

## Modify Prefab Properties

```csharp
using UnityEditor;

string prefabPath = "Assets/Prefabs/MyCube.prefab";
GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
if (prefab == null)
{
    return $"Prefab not found at {prefabPath}";
}

using (PrefabUtility.EditPrefabContentsScope scope = new PrefabUtility.EditPrefabContentsScope(prefabPath))
{
    GameObject root = scope.prefabContentsRoot;
    root.transform.localScale = new Vector3(2, 2, 2);

    MeshRenderer renderer = root.GetComponent<MeshRenderer>();
    if (renderer != null)
    {
        renderer.receiveShadows = false;
    }
}
return "Modified prefab properties";
```

## Wire SerializedField Reference in Prefab

```csharp
using UnityEditor;

string prefabPath = "Assets/Prefabs/Player.prefab";
string weaponPrefabPath = "Assets/Prefabs/Weapon.prefab";
GameObject weaponPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(weaponPrefabPath);
if (weaponPrefab == null)
{
    return $"Asset not found at {weaponPrefabPath}";
}

GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
if (prefab == null)
{
    return $"Prefab not found at {prefabPath}";
}

using (PrefabUtility.EditPrefabContentsScope scope = new PrefabUtility.EditPrefabContentsScope(prefabPath))
{
    GameObject root = scope.prefabContentsRoot;
    MonoBehaviour script = root.GetComponent("PlayerController") as MonoBehaviour;
    if (script == null)
    {
        return "PlayerController not found on prefab root";
    }

    SerializedObject so = new SerializedObject(script);
    SerializedProperty prop = so.FindProperty("weaponPrefab");
    if (prop == null)
    {
        return "Property 'weaponPrefab' not found";
    }

    prop.objectReferenceValue = weaponPrefab;
    so.ApplyModifiedProperties();
}
return "Wired weaponPrefab reference in Player prefab";
```

## Wire Multiple References in Prefab

```csharp
using UnityEditor;

string prefabPath = "Assets/Prefabs/Enemy.prefab";
GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
if (prefab == null)
{
    return $"Prefab not found at {prefabPath}";
}

int wiredCount = 0;

using (PrefabUtility.EditPrefabContentsScope scope = new PrefabUtility.EditPrefabContentsScope(prefabPath))
{
    GameObject root = scope.prefabContentsRoot;
    MonoBehaviour script = root.GetComponent("EnemyController") as MonoBehaviour;
    if (script == null)
    {
        return "EnemyController not found";
    }

    SerializedObject so = new SerializedObject(script);

    SerializedProperty matProp = so.FindProperty("bodyMaterial");
    Material mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/EnemyBody.mat");
    if (matProp != null && mat != null)
    {
        matProp.objectReferenceValue = mat;
        wiredCount++;
    }

    SerializedProperty clipProp = so.FindProperty("deathSound");
    AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Death.wav");
    if (clipProp != null && clip != null)
    {
        clipProp.objectReferenceValue = clip;
        wiredCount++;
    }

    so.ApplyModifiedProperties();
}
return $"Wired {wiredCount}/2 references in Enemy prefab";
```

## Find All Prefab Instances in Scene

```csharp
using UnityEditor;
using System.Collections.Generic;

string prefabPath = "Assets/Prefabs/MyCube.prefab";
GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
if (prefab == null)
{
    return $"Prefab not found at {prefabPath}";
}

List<GameObject> instances = new List<GameObject>();

foreach (GameObject obj in Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
{
    if (PrefabUtility.GetCorrespondingObjectFromSource(obj) == prefab)
    {
        instances.Add(obj);
    }
}
return $"Found {instances.Count} instances of {prefab.name}";
```

## Apply Prefab Overrides

```csharp
using UnityEditor;

GameObject selected = Selection.activeGameObject;
if (selected == null)
{
    return "No GameObject selected";
}

if (!PrefabUtility.IsPartOfPrefabInstance(selected))
{
    return "Selected object is not a prefab instance";
}

GameObject instanceRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(selected);
if (instanceRoot == null)
{
    return "Could not resolve prefab instance root";
}

PrefabUtility.ApplyPrefabInstance(instanceRoot, InteractionMode.UserAction);
return $"Applied overrides from {instanceRoot.name} to prefab";
```

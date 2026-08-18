# Asset Operations

Code examples for AssetDatabase operations using `execute-dynamic-code`.

## Find Assets by Type

```csharp
using UnityEditor;
using System.Collections.Generic;

string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
List<string> paths = new List<string>();

foreach (string guid in prefabGuids)
{
    paths.Add(AssetDatabase.GUIDToAssetPath(guid));
}
return $"Found {paths.Count} prefabs";
```

## Find Assets by Name

```csharp
using UnityEditor;
using System.Collections.Generic;

string searchName = "Player";
string[] guids = AssetDatabase.FindAssets(searchName);
List<string> paths = new List<string>();

foreach (string guid in guids)
{
    paths.Add(AssetDatabase.GUIDToAssetPath(guid));
}
return $"Found {paths.Count} assets matching '{searchName}'";
```

## Find Assets in Folder

```csharp
using UnityEditor;
using System.Collections.Generic;

string folder = "Assets/Prefabs";
string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { folder });
List<string> paths = new List<string>();

foreach (string guid in guids)
{
    paths.Add(AssetDatabase.GUIDToAssetPath(guid));
}
return $"Found {paths.Count} prefabs in {folder}";
```

## Duplicate Asset

```csharp
using UnityEditor;

string sourcePath = "Assets/Materials/MyMaterial.mat";
string destPath = "Assets/Materials/MyMaterial_Backup.mat";

bool success = AssetDatabase.CopyAsset(sourcePath, destPath);
return success ? $"Copied to {destPath}" : "Copy failed";
```

## Move Asset

```csharp
using UnityEditor;

string sourcePath = "Assets/OldFolder/MyAsset.asset";
string destPath = "Assets/NewFolder/MyAsset.asset";

string error = AssetDatabase.MoveAsset(sourcePath, destPath);
return string.IsNullOrEmpty(error) ? $"Moved to {destPath}" : $"Error: {error}";
```

## Rename Asset

```csharp
using UnityEditor;

string assetPath = "Assets/Materials/OldName.mat";
string newName = "NewName";

string error = AssetDatabase.RenameAsset(assetPath, newName);
return string.IsNullOrEmpty(error) ? $"Renamed to {newName}" : $"Error: {error}";
```

## Rename Asset (Undo-supported)

```csharp
using UnityEditor;

// ObjectNames.SetNameSmart() supports Undo (AssetDatabase.RenameAsset() does NOT)
Object selected = Selection.activeObject;
if (selected == null)
{
    return "No asset selected";
}

string oldName = selected.name;
ObjectNames.SetNameSmart(selected, "NewName");
AssetDatabase.SaveAssets();
return $"Renamed {oldName} to {selected.name}";
```

## Get Asset Path from Object

```csharp
using UnityEditor;

GameObject selected = Selection.activeGameObject;
if (selected == null)
{
    return "No object selected";
}

string path = AssetDatabase.GetAssetPath(selected);
if (string.IsNullOrEmpty(path))
{
    return "Selected object is not an asset (scene object)";
}
return $"Asset path: {path}";
```

## Load Asset at Path

```csharp
using UnityEditor;

string path = "Assets/Prefabs/Player.prefab";
GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);

if (asset == null)
{
    return $"Asset not found at {path}";
}
return $"Loaded: {asset.name}";
```

## Get All Assets of Type

```csharp
using UnityEditor;

string[] scriptGuids = AssetDatabase.FindAssets("t:MonoScript");
int count = 0;

foreach (string guid in scriptGuids)
{
    string path = AssetDatabase.GUIDToAssetPath(guid);
    if (path.StartsWith("Assets/"))
    {
        count++;
    }
}
return $"Found {count} scripts in Assets folder";
```

## Check if Asset Exists

```csharp
using UnityEditor;

string path = "Assets/Prefabs/Player.prefab";
string guid = AssetDatabase.AssetPathToGUID(path);

bool exists = !string.IsNullOrEmpty(guid);
return exists ? $"Asset exists: {path}" : $"Asset not found: {path}";
```

## Get Asset Dependencies

```csharp
using UnityEditor;

string assetPath = "Assets/Prefabs/Player.prefab";
string[] dependencies = AssetDatabase.GetDependencies(assetPath, true);

return $"Asset has {dependencies.Length} dependencies";
```

## Refresh AssetDatabase

Use this after terminal-based asset file changes when Unity needs to import them and generate `.meta` files.

```csharp
using UnityEditor;

AssetDatabase.Refresh();
return "AssetDatabase refreshed";
```

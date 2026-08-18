# ScriptableObject Operations

Code examples for ScriptableObject operations using `execute-dynamic-code`.

## Create ScriptableObject Instance

```csharp
using UnityEditor;

ScriptableObject so = ScriptableObject.CreateInstance<ScriptableObject>();
string path = "Assets/Data/MyData.asset";
AssetDatabase.CreateAsset(so, path);
AssetDatabase.SaveAssets();
return $"ScriptableObject created at {path}";
```

## Create Custom ScriptableObject

```csharp
using UnityEditor;

ScriptableObject so = ScriptableObject.CreateInstance("MyCustomSO");
if (so == null)
{
    return "Type 'MyCustomSO' not found. Ensure the class exists.";
}

string path = "Assets/Data/MyCustomData.asset";
AssetDatabase.CreateAsset(so, path);
AssetDatabase.SaveAssets();
return $"Created {so.GetType().Name} at {path}";
```

## Modify ScriptableObject with SerializedObject

```csharp
using UnityEditor;

string path = "Assets/Data/MyData.asset";
ScriptableObject so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);

if (so == null)
{
    return $"Asset not found at {path}";
}

SerializedObject serializedObj = new SerializedObject(so);
SerializedProperty prop = serializedObj.FindProperty("myField");

if (prop != null)
{
    prop.stringValue = "New Value";
    serializedObj.ApplyModifiedProperties();
    EditorUtility.SetDirty(so);
    AssetDatabase.SaveAssets();
    return "Property updated";
}
return "Property 'myField' not found";
```

## Set Int/Float/Bool Properties

```csharp
using UnityEditor;

string path = "Assets/Data/GameSettings.asset";
ScriptableObject so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
if (so == null)
{
    return $"Asset not found at {path}";
}

SerializedObject serializedObj = new SerializedObject(so);

SerializedProperty intProp = serializedObj.FindProperty("maxHealth");
if (intProp != null) intProp.intValue = 100;

SerializedProperty floatProp = serializedObj.FindProperty("moveSpeed");
if (floatProp != null) floatProp.floatValue = 5.5f;

SerializedProperty boolProp = serializedObj.FindProperty("isEnabled");
if (boolProp != null) boolProp.boolValue = true;

serializedObj.ApplyModifiedProperties();
EditorUtility.SetDirty(so);
AssetDatabase.SaveAssets();
return "Properties updated";
```

## Set Reference Properties

```csharp
using UnityEditor;

string soPath = "Assets/Data/CharacterData.asset";
string prefabPath = "Assets/Prefabs/Player.prefab";

ScriptableObject so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(soPath);
GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

if (so == null)
{
    return $"ScriptableObject not found at {soPath}";
}
if (prefab == null)
{
    return $"Prefab not found at {prefabPath}";
}

SerializedObject serializedObj = new SerializedObject(so);
SerializedProperty prop = serializedObj.FindProperty("playerPrefab");

if (prop != null)
{
    prop.objectReferenceValue = prefab;
    serializedObj.ApplyModifiedProperties();
    EditorUtility.SetDirty(so);
    AssetDatabase.SaveAssets();
    return "Reference set successfully";
}
return "Property not found";
```

## Set Array/List Properties

```csharp
using UnityEditor;

string path = "Assets/Data/ItemDatabase.asset";
ScriptableObject so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);

if (so == null)
{
    return $"Asset not found at {path}";
}

SerializedObject serializedObj = new SerializedObject(so);
SerializedProperty arrayProp = serializedObj.FindProperty("items");

if (arrayProp != null && arrayProp.isArray)
{
    arrayProp.ClearArray();
    arrayProp.InsertArrayElementAtIndex(0);
    arrayProp.GetArrayElementAtIndex(0).stringValue = "Sword";
    arrayProp.InsertArrayElementAtIndex(1);
    arrayProp.GetArrayElementAtIndex(1).stringValue = "Shield";

    serializedObj.ApplyModifiedProperties();
    EditorUtility.SetDirty(so);
    AssetDatabase.SaveAssets();
    return "Array updated with 2 items";
}
return "Array property not found";
```

## Find All ScriptableObjects of Type

```csharp
using UnityEditor;
using System.Collections.Generic;

string typeName = "GameSettings";
string[] guids = AssetDatabase.FindAssets($"t:{typeName}");
List<string> paths = new List<string>();

foreach (string guid in guids)
{
    paths.Add(AssetDatabase.GUIDToAssetPath(guid));
}
return $"Found {paths.Count} {typeName} assets";
```

## Duplicate ScriptableObject

```csharp
using UnityEditor;

string sourcePath = "Assets/Data/Template.asset";
string destPath = "Assets/Data/NewInstance.asset";

ScriptableObject source = AssetDatabase.LoadAssetAtPath<ScriptableObject>(sourcePath);
if (source == null)
{
    return $"Source asset not found at {sourcePath}";
}

ScriptableObject copy = Object.Instantiate(source);
AssetDatabase.CreateAsset(copy, destPath);
AssetDatabase.SaveAssets();
return $"Duplicated to {destPath}";
```

## List All Properties of ScriptableObject

```csharp
using UnityEditor;
using System.Collections.Generic;

string path = "Assets/Data/MyData.asset";
ScriptableObject so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);

if (so == null)
{
    return $"Asset not found at {path}";
}

SerializedObject serializedObj = new SerializedObject(so);
SerializedProperty prop = serializedObj.GetIterator();

List<string> properties = new List<string>();
while (prop.NextVisible(true))
{
    properties.Add($"{prop.name} ({prop.propertyType})");
}
return string.Join(", ", properties);
```

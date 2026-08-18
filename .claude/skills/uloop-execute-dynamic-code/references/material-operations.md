# Material Operations

Code examples for Material operations using `execute-dynamic-code`.

## Create a New Material

```csharp
using UnityEditor;

Shader shader = Shader.Find("Standard");
Material mat = new Material(shader);
mat.name = "MyMaterial";
string path = "Assets/Materials/MyMaterial.mat";
AssetDatabase.CreateAsset(mat, path);
AssetDatabase.SaveAssets();
return $"Material created at {path}";
```

## Set Material Color

```csharp
using UnityEditor;

string matPath = "Assets/Materials/MyMaterial.mat";
Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
if (mat == null)
{
    return $"Material not found at {matPath}";
}

mat.SetColor("_Color", new Color(1f, 0.5f, 0f, 1f));
EditorUtility.SetDirty(mat);
AssetDatabase.SaveAssets();
return "Material color set to orange";
```

## Set Material Properties (Float, Vector)

```csharp
using UnityEditor;

string matPath = "Assets/Materials/MyMaterial.mat";
Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);

mat.SetFloat("_Metallic", 0.8f);
mat.SetFloat("_Glossiness", 0.6f);
mat.SetVector("_EmissionColor", new Vector4(1, 1, 0, 1));

EditorUtility.SetDirty(mat);
AssetDatabase.SaveAssets();
return "Material properties updated";
```

## Assign Texture to Material

```csharp
using UnityEditor;

string matPath = "Assets/Materials/MyMaterial.mat";
string texPath = "Assets/Textures/MyTexture.png";

Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);
if (mat == null)
{
    return $"Material not found at {matPath}";
}
if (tex == null)
{
    return $"Texture not found at {texPath}";
}

mat.SetTexture("_MainTex", tex);
EditorUtility.SetDirty(mat);
AssetDatabase.SaveAssets();
return $"Assigned {tex.name} to material";
```

## Assign Material to GameObject

```csharp
using UnityEditor;

string matPath = "Assets/Materials/MyMaterial.mat";
Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);

GameObject selected = Selection.activeGameObject;
if (selected == null)
{
    return "No GameObject selected";
}

Renderer renderer = selected.GetComponent<Renderer>();
if (renderer == null)
{
    return "Selected object has no Renderer";
}

renderer.sharedMaterial = mat;
EditorUtility.SetDirty(selected);
return $"Assigned {mat.name} to {selected.name}";
```

## Enable/Disable Material Keywords

```csharp
using UnityEditor;

string matPath = "Assets/Materials/MyMaterial.mat";
Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);

mat.EnableKeyword("_EMISSION");
mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;

EditorUtility.SetDirty(mat);
AssetDatabase.SaveAssets();
return "Emission enabled on material";
```

## Find All Materials Using a Shader

```csharp
using UnityEditor;
using System.Collections.Generic;

string shaderName = "Standard";
string[] guids = AssetDatabase.FindAssets("t:Material");
List<string> matchingMaterials = new List<string>();

foreach (string guid in guids)
{
    string path = AssetDatabase.GUIDToAssetPath(guid);
    Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
    if (mat != null && mat.shader != null && mat.shader.name == shaderName)
    {
        matchingMaterials.Add(path);
    }
}
return $"Found {matchingMaterials.Count} materials using {shaderName}";
```

## Duplicate Material

```csharp
using UnityEditor;

string sourcePath = "Assets/Materials/MyMaterial.mat";
string destPath = "Assets/Materials/MyMaterial_Copy.mat";

Material source = AssetDatabase.LoadAssetAtPath<Material>(sourcePath);
if (source == null)
{
    return $"Material not found at {sourcePath}";
}

Material copy = new Material(source);
AssetDatabase.CreateAsset(copy, destPath);
AssetDatabase.SaveAssets();
return $"Material duplicated to {destPath}";
```

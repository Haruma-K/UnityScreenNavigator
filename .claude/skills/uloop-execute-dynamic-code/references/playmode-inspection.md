# PlayMode Inspection

Code examples for inspecting and verifying game state at runtime using `execute-dynamic-code`.
These examples read or check runtime state for debugging and automated testing.

## Get Active Scene Info

```csharp
using UnityEngine.SceneManagement;

Scene scene = SceneManager.GetActiveScene();
return $"Scene: {scene.name}, path: {scene.path}, buildIndex: {scene.buildIndex}, isLoaded: {scene.isLoaded}, rootCount: {scene.rootCount}";
```

## Check Loaded Scenes

```csharp
using UnityEngine.SceneManagement;
using System.Text;

int count = SceneManager.sceneCount;
StringBuilder sb = new StringBuilder();
sb.AppendLine($"Loaded scenes ({count}):");

for (int i = 0; i < count; i++)
{
    Scene scene = SceneManager.GetSceneAt(i);
    sb.AppendLine($"  [{i}] {scene.name} (buildIndex={scene.buildIndex}, isLoaded={scene.isLoaded})");
}
return sb.ToString();
```

## Read Component Field via Reflection

```csharp
using System.Reflection;

GameObject target = GameObject.Find("Player");
if (target == null) return "Player not found";

MonoBehaviour script = target.GetComponent("PlayerController") as MonoBehaviour;
if (script == null) return "PlayerController not found";

FieldInfo hpField = script.GetType().GetField("hp", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
if (hpField == null) return "hp field not found";

object value = hpField.GetValue(script);
return $"Player hp = {value}";
```

## Read Multiple Fields via Reflection

```csharp
using System.Reflection;
using System.Text;

GameObject target = GameObject.Find("Player");
if (target == null) return "Player not found";

MonoBehaviour script = target.GetComponent("PlayerController") as MonoBehaviour;
if (script == null) return "PlayerController not found";

StringBuilder sb = new StringBuilder();
sb.AppendLine($"Fields on {script.GetType().Name}:");

FieldInfo[] fields = script.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
foreach (FieldInfo field in fields)
{
    if (field.DeclaringType == script.GetType())
    {
        object value = field.GetValue(script);
        sb.AppendLine($"  {field.Name} ({field.FieldType.Name}) = {value}");
    }
}
return sb.ToString();
```

## Read Property via Reflection

```csharp
using System.Reflection;

GameObject target = GameObject.Find("GameManager");
if (target == null) return "GameManager not found";

MonoBehaviour script = target.GetComponent("GameManager") as MonoBehaviour;
if (script == null) return "GameManager component not found";

PropertyInfo prop = script.GetType().GetProperty("Score", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
if (prop == null) return "Score property not found";

object value = prop.GetValue(script);
return $"Score = {value}";
```

## Check If GameObject Exists (Spawned/Destroyed)

```csharp
using System.Linq;

string targetName = "Enemy";
GameObject[] all = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
GameObject[] matches = all.Where(g => g.name.Contains(targetName)).ToArray();

if (matches.Length == 0) return $"No GameObjects containing '{targetName}' found";

return $"Found {matches.Length} objects matching '{targetName}': {string.Join(", ", matches.Select(g => g.name))}";
```

## Verify Object Position/Rotation

```csharp
GameObject target = GameObject.Find("Player");
if (target == null) return "Player not found";

Transform t = target.transform;
return $"{target.name}: pos={t.position}, rot={t.rotation.eulerAngles}, scale={t.localScale}";
```

## Check Rigidbody State

```csharp
Rigidbody rb = GameObject.Find("TargetCube")?.GetComponent<Rigidbody>();
if (rb == null) return "Rigidbody not found on TargetCube";

return $"velocity={rb.velocity}, angularVelocity={rb.angularVelocity}, isKinematic={rb.isKinematic}, useGravity={rb.useGravity}, isSleeping={rb.IsSleeping()}";
```

## Raycast from Camera Center

```csharp
Camera cam = Camera.main;
if (cam == null) return "Main camera not found";

Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
if (Physics.Raycast(ray, out RaycastHit hit, 100f))
{
    return $"Hit: {hit.collider.gameObject.name} at {hit.point}";
}
return "No hit";
```

## Find GameObjects by Tag

```csharp
using System.Linq;

string tag = "Enemy";
GameObject[] enemies = GameObject.FindGameObjectsWithTag(tag);
return $"Found {enemies.Length} objects with tag '{tag}': {string.Join(", ", enemies.Select(e => $"{e.name} at {e.transform.position}"))}";
```

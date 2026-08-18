# Annotated Elements and Coordinates

Read this when using `npx --yes uloop-cli@2.2.0 screenshot --capture-mode rendering --annotate-elements true` or clustered `--annotate-raycast-grid true --raycast-layer-mask <layers>` output to find coordinates for `simulate-mouse-ui` or `simulate-mouse-input`.

## AnnotatedElements Fields

`AnnotatedElements` is empty unless `--annotate-elements true` is used, or unless `--annotate-raycast-grid true --raycast-layer-mask <layers>` adds clustered 3D collider candidates. UI entries are sorted by z-order, frontmost first. Each item contains:

- `Label`: Index label in JSON (`A` = frontmost, `B` = next, ...). Screenshot labels also include the interaction hint, such as `A / CLICK` or `B / DRAG`.
- `Name`: Element name
- `Path`: Hierarchy path from the scene root, for example `Canvas/Panel/Button`. Use this as `simulate-mouse-ui --target-path` when bypassing raycast blockers.
- `Type`: Element type (`Button`, `Toggle`, `Slider`, `Dropdown`, `InputField`, `Scrollbar`, `Draggable`, `DropTarget`, `Selectable`, `PhysicsCollider`)
- `Interaction`: Derived interaction category (`Click`, `Drag`, `Drop`, `Text`) or `Raycast` for clustered physics collider entries. Use this to choose between `simulate-mouse-ui --action Click`, drag actions, or `simulate-mouse-input`/`raycast`.
- `Layer`: Physics layer name for `PhysicsCollider` entries. Empty for UI entries.
- `Components`: Collider and MonoBehaviour component type names from the hit GameObject for `PhysicsCollider` entries. Empty for UI entries.
- `SimX`, `SimY`: Target click position in top-left Game View coordinates. For UI entries this is the element center; for `PhysicsCollider` entries this is a representative sampled hit. Use these directly with `simulate-mouse-ui --x/--y`, `simulate-mouse-input --x/--y`, or `raycast --x/--y`.
- `BoundsMinX`, `BoundsMinY`, `BoundsMaxX`, `BoundsMaxY`: Bounding box in simulate-mouse coordinates. For `PhysicsCollider` entries, this is the axis-aligned sampled-cell coverage box from reachable raycast hits, not a guarantee that every interior point is clickable.
- `SortingOrder`: Canvas sorting order. Higher values are in front.
- `SiblingIndex`: Transform sibling index under the element's direct parent. Do not use it as a reliable z-order signal across nested UI hierarchies.

## RaycastLayerSummaries Fields

`RaycastLayerSummaries` is populated when `--annotate-raycast-grid true` is used without `--raycast-layer-mask`. It is built from dense raycast samples, while `RaycastGridPoints` remains the coarse 5x5 annotated grid.

- `Layer`: Physics layer name to pass to `--raycast-layer-mask`
- `LayerIndex`: Unity physics layer index
- `HitCount`: Dense raycast hit count for the layer
- `RepresentativeObjectPath`: Hierarchy path for the object with the most hits on that layer. Ties are resolved alphabetically by path.

Entries are sorted by `HitCount` descending, then `LayerIndex` ascending.

## Coordinate Conversion

When `ImageCoordinateSystem` is `"top-left-game-view"`, convert raw image pixel coordinates from `screenshot --capture-mode rendering` with the formula returned in `ScreenshotToInputFormula`:

```text
input_x = image_x / resolutionScale
input_y = image_y / resolutionScale + imageToInputOffsetY
```

When `ResolutionScale` is `1.0` and `ImageToInputOffsetY` is `0` for rendering captures, raw image pixel coordinates already match mouse-input coordinates. `AnnotatedElements[].SimX/SimY` and `RaycastGridPoints[].InputX/InputY` are already mouse-input coordinates in that mode, so pass those values directly.

For `PhysicsCollider` entries, `SimX/SimY` is a real sampled raycast hit nearest to the reachable cluster centroid. This avoids synthetic center points that may fall into empty space for L-shaped or ring-shaped collider coverage. Always use `SimX/SimY` for clicking; use `BoundsMinX/Y` and `BoundsMaxX/Y` only as a sampled coverage guide.

`--raycast-layer-mask` filters by the requested physics layers and Camera.main.cullingMask. A layer that is requested but hidden from the active camera is treated as not visible and will not produce `PhysicsCollider` entries.

For clustered `PhysicsCollider` entries, points where the frontmost EventSystem hit comes from a `GraphicRaycaster` UI element are treated as covered by UI. This includes world-space Canvas UI. PhysicsRaycaster and other non-uGUI hits are not treated as UI occlusion. Bounds, screenshot outlines, and `SimX/SimY` are derived from the remaining reachable samples; if every sampled hit in that collider cluster is covered, the collider is omitted from `AnnotatedElements`.

`PhysicsCollider` bounds expand each reachable sample by half the dense raycast sampling step in X and Y, then clamp the result to the captured Game View area. The screenshot overlay draws only the outer edges of those reachable sample cells, so angled, L-shaped, separated, and partially UI-covered hit regions do not become one large rectangle. `BoundsMinX/Y` and `BoundsMaxX/Y` are still an axis-aligned bbox for JSON consumers, may extend up to half a sample step past the visible collider edge, and do not guarantee that every interior point is clickable.

The mouse input tools convert internally to Unity Input System coordinates:

```text
unity_x = input_x
unity_y = gameViewHeight - input_y
```

Do not flip Y in the caller.

## Annotation Readability

Annotated screenshots compensate border thickness for `ResolutionScale`, so the saved PNG keeps the intended outline width after downscaling. The neutral contrast borders are 2 output pixels each, and the colored middle border is 4 output pixels. Label outlines are also compensated and are separated from element borders by a 4 output pixel gap.

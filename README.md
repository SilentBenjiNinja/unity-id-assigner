# ID Assigner for Unity

Provides a base Editor window and interface for assigning and managing unique integer IDs across ScriptableObject assets. Extend it once per asset type and get automatic detection of missing and duplicate IDs.

## Requirements

- Unity 2020.3+
- [Odin Inspector](https://odininspector.com) *(optional)*

## Features

- **`IIdContainer` interface** — implement on any ScriptableObject to opt in to ID management
- **Base `IdAssigner<T>` window** — one-line subclass gives a full management window for any type
- **Unassigned ID detection** — surfaces assets missing an ID and assigns them incrementally in one click
- **Duplicate ID detection** — pings the first conflicting asset in the Project window
- **Odin Inspector support** — rich UI with `[Button]`, `[ShowIf]`, and `[InfoBox]` when Odin is present; falls back to standard IMGUI otherwise

## Setup

1. Implement `IIdContainer` on your ScriptableObject class
2. In an `Editor` folder, create a subclass of `IdAssigner<T>` and add a `[MenuItem]` to open it

## Full Example

```csharp
// Runtime — your ScriptableObject
[CreateAssetMenu(menuName = "MyGame/Item Data", fileName = "Item_")]
public class SO_ItemData : ScriptableObject, IIdContainer
{
    // Both fields must be serialized so IDs persist with the asset.
    // _id can be edited manually in the Inspector to resolve duplicate ID issues,
    // though this is generally discouraged — use the assigner window instead.
    [SerializeField] private int _id;
    [SerializeField, HideInInspector] private bool _idSet;

    public int Id
    {
        get => _id;
        set { _id = value; _idSet = true; }
    }
    public bool IdSet => _idSet;
}

// Editor — the ID management window
public class ItemIdAssigner : IdAssigner<SO_ItemData>
{
    [MenuItem("Tools/Item ID Assigner")]
    private static void OpenWindow() => GetWindow<ItemIdAssigner>().Show();
}
```

## Usage

1. Open the window via your `[MenuItem]` path
2. Click **Refresh** to load all assets of the managed type into the list
3. If unassigned assets are found, a **Fix** button appears — click it to assign IDs incrementally from the current highest
4. If duplicates are found, a **Highlight** button appears — click it to ping the first conflicting asset in the Project window

## Notes

- **Always Refresh before making changes** — `FetchAllAssets` uses `Resources.FindObjectsOfTypeAll`, which only sees assets currently loaded in memory
- IDs are assigned by incrementing from the highest existing ID; gaps are not filled
- The window does not persist state between sessions

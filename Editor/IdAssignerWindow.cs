using System.Linq;
using bnj.id_assigner.Runtime;
using UnityEditor;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
#endif

namespace bnj.id_assigner.Editor
{
    /// <summary>
    /// Base Editor window for managing unique integer IDs on <typeparamref name="T"/> assets.
    /// Subclass it, add a <c>[MenuItem]</c>, and call <c>GetWindow</c> to open it. See the README for a full example.
    /// </summary>
    // TODO: actually check that the non-Odin editor window works as intended!
    // TODO: auto-refresh? separate fetch-in-use vs fetch all buttons? some way to show which assets are unassigned/duplicated without Odin's info boxes?
    // TODO: IDs from duplicated assets should be set to 0 by default (on creation and duplication) to avoid confusion and make it easier to find them with the "Fix" button.
    // TODO: Assets with duplicate IDs should be highlighted in the list (red text?) to make them easier to find. Duplicate entries could also have a button to reset their ID to 0 for reassignment which would allow for readonly ID in the inspector. Also can probably be optimized by only checking for duplicates once and caching the result, instead of re-checking every time we draw the GUI.
    // TODO: tests
#if ODIN_INSPECTOR
    public abstract class IdAssignerWindow<T> : OdinEditorWindow where T : SO_IdContainer
#else
    public abstract class IdAssignerWindow<T> : EditorWindow where T : SO_IdContainer
#endif
    {
#if ODIN_INSPECTOR
        [ReadOnly, ListDrawerSettings(ShowPaging = false)]
#endif
        [SerializeField] private T[] _items = new T[] { };

        private int HighestAssignedId => _items.Max(x => x.Id);

        // used to show/hide buttons and warnings about unassigned/duplicate IDs.
        private bool HasUnassignedIds => _items.Count(x => !x.IdSet) > 0;
        private bool HasDuplicateIds
        {
            get
            {
                var assigned = _items.Where(x => x.IdSet);
                var assignedDistinct = assigned
                    .Select(x => x.Id)
                    .Distinct();
                return assigned.Count() > assignedDistinct.Count();
            }
        }

#if ODIN_INSPECTOR
        [PropertyOrder(-3)]
        [InfoBox("Make sure when assigning IDs that all items are fetched (in case items were added or removed)!")]
        [Button("Refresh", ButtonSizes.Medium)]
#endif
        private void FetchAllAssets()
        {
            _items = Resources
                .FindObjectsOfTypeAll(typeof(T))
                .Select(x => x as T)
                .ToArray();
        }

#if ODIN_INSPECTOR
        [PropertyOrder(-2)]
        [InfoBox("Found unassigned IDs")]
        [ShowIf("HasUnassignedIds")]
        [Button("Fix", ButtonSizes.Medium)]
#endif
        private void SetUnassignedIds()
        {
            var unassigned = _items.Where(x => !x.IdSet).ToArray();
            foreach (var data in unassigned)
            {
                data.Id = HighestAssignedId + 1;
                EditorUtility.SetDirty(data);
                Debug.Log($"Assigned ID {data.Id} to {data.name}");
            }

            Debug.Log($"Assigned IDs for {unassigned.Length} items");
        }

#if ODIN_INSPECTOR
        [PropertyOrder(-1)]
        [InfoBox("Found duplicate ID!")]
        [ShowIf("HasDuplicateIds")]
        [Button("Highlight", ButtonSizes.Medium)]
#endif
        private void HighlightDuplicateIds()
        {
            var distinct = _items.Select(x => x.Id).Distinct().ToArray();

            for (int i = 0; i < _items.Length; i++)
            {
                var item = _items[i];
                if (distinct.Length > i && item.Id == distinct[i]) continue;

                EditorGUIUtility.PingObject(item);
                break;
            }
        }

#if !ODIN_INSPECTOR
        private Vector2 _scrollPosition;

        private void OnGUI()
        {
            EditorGUILayout.HelpBox(
                "Make sure when assigning IDs that all items are fetched (in case items were added or removed)!",
                MessageType.Info);

            if (GUILayout.Button("Refresh"))
                FetchAllAssets();

            if (HasUnassignedIds)
            {
                EditorGUILayout.HelpBox("Found unassigned IDs", MessageType.Warning);
                if (GUILayout.Button("Fix"))
                    SetUnassignedIds();
            }

            if (HasDuplicateIds)
            {
                EditorGUILayout.HelpBox("Found duplicate ID!", MessageType.Error);
                if (GUILayout.Button("Highlight"))
                    HighlightDuplicateIds();
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            foreach (var item in _items)
                EditorGUILayout.ObjectField(item?.name ?? "(null)", item, typeof(T), false);
            EditorGUILayout.EndScrollView();
        }
#endif
    }
}

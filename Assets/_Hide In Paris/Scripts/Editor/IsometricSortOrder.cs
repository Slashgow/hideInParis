// Place this file anywhere inside an Editor folder, e.g.:
// Assets/Editor/IsometricSortOrder.cs

using UnityEditor;
using UnityEngine;

/// <summary>
/// Adds sort order controls in two places:
///
///  1. Top menu bar:  Isometric > Sort Order > ...
///  2. Hierarchy right-click context menu (same options)
///
/// Select one or more GameObjects with a SpriteRenderer, then use any of the commands.
/// All operations are Undo-able.
/// </summary>
public static class IsometricSortOrder
{
    // ── Shared validation ──────────────────────────────────────────────────────

    private static bool HasSelection() =>
        Selection.gameObjects.Length > 0 &&
        System.Array.Exists(Selection.gameObjects, go => go.GetComponent<SpriteRenderer>() != null);

    // ══════════════════════════════════════════════════════════════════════════
    // TOP MENU BAR  —  Isometric / Sort Order / ...
    // ══════════════════════════════════════════════════════════════════════════

    [MenuItem("Isometric/Sort Order/Increase by 1  _F3", true)]
    [MenuItem("Isometric/Sort Order/Decrease by 1  _F2", true)]
    [MenuItem("Isometric/Sort Order/Set Order...", true)]
    [MenuItem("Isometric/Sort Order/Auto-Sort Selection by Y", true)]
    private static bool ValidateMenuItems() => HasSelection();

    [MenuItem("Isometric/Sort Order/Increase by 1  _F3", priority = 1)]
    private static void MenuIncrease() => NudgeSelected(+1);

    [MenuItem("Isometric/Sort Order/Decrease by 1  _F2", priority = 2)]
    private static void MenuDecrease() => NudgeSelected(-1);

    [MenuItem("Isometric/Sort Order/Set Order...", priority = 3)]
    private static void MenuSetOrder() => OpenSetOrderDialog();

    [MenuItem("Isometric/Sort Order/Auto-Sort Selection by Y", priority = 4)]
    private static void MenuAutoSort() => AutoSortByY();

    // ══════════════════════════════════════════════════════════════════════════
    // HIERARCHY CONTEXT MENU  —  right-click on any GameObject
    // ══════════════════════════════════════════════════════════════════════════

    [MenuItem("GameObject/Sort Order/Increase by 1", false, 49)]
    private static void ContextIncrease() => NudgeSelected(+1);

    [MenuItem("GameObject/Sort Order/Increase by 1", true)]
    private static bool ValidateContextIncrease() => HasSelection();

    [MenuItem("GameObject/Sort Order/Decrease by 1", false, 49)]
    private static void ContextDecrease() => NudgeSelected(-1);

    [MenuItem("GameObject/Sort Order/Decrease by 1", true)]
    private static bool ValidateContextDecrease() => HasSelection();

    [MenuItem("GameObject/Sort Order/Set Order...", false, 49)]
    private static void ContextSetOrder() => OpenSetOrderDialog();

    [MenuItem("GameObject/Sort Order/Set Order...", true)]
    private static bool ValidateContextSetOrder() => HasSelection();

    [MenuItem("GameObject/Sort Order/Auto-Sort Selection by Y", false, 49)]
    private static void ContextAutoSort() => AutoSortByY();

    [MenuItem("GameObject/Sort Order/Auto-Sort Selection by Y", true)]
    private static bool ValidateContextAutoSort() => HasSelection();

    // ══════════════════════════════════════════════════════════════════════════
    // OPERATIONS
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>Nudge every selected SpriteRenderer's sort order by delta.</summary>
    private static void NudgeSelected(int delta)
    {
        var renderers = GetSelectedRenderers();
        if (renderers.Length == 0) return;

        Undo.RecordObjects(renderers, delta > 0 ? "Increase Sort Order" : "Decrease Sort Order");

        foreach (var sr in renderers)
        {
            sr.sortingOrder += delta;
            EditorUtility.SetDirty(sr);
        }
    }

    /// <summary>
    /// Opens a small modal dialog to type an explicit sort order value.
    /// If multiple objects are selected, all are set to the same value.
    /// </summary>
    private static void OpenSetOrderDialog()
    {
        var renderers = GetSelectedRenderers();
        if (renderers.Length == 0) return;

        // Pre-fill with the current value if only one object is selected
        int current = renderers[0].sortingOrder;
        SetOrderDialog.Open(current, newOrder =>
        {
            Undo.RecordObjects(renderers, "Set Sort Order");
            foreach (var sr in renderers)
            {
                sr.sortingOrder = newOrder;
                EditorUtility.SetDirty(sr);
            }
        });
    }

    /// <summary>
    /// Assigns sequential sort orders to the selection based on world Y position.
    /// Highest Y (furthest back in isometric view) gets the lowest order number.
    /// </summary>
    private static void AutoSortByY()
    {
        var renderers = GetSelectedRenderers();
        if (renderers.Length == 0) return;

        var sorted = System.Array.FindAll(renderers, r => r != null);
        System.Array.Sort(sorted, (a, b) =>
            b.transform.position.y.CompareTo(a.transform.position.y)); // descending Y

        int baseOrder = renderers[0].sortingOrder; // anchor to first selected

        Undo.RecordObjects(sorted, "Auto-Sort by Y");

        for (int i = 0; i < sorted.Length; i++)
        {
            sorted[i].sortingOrder = baseOrder + i;
            EditorUtility.SetDirty(sorted[i]);
        }
    }

    // ── Helpers ────────────────────────────────────────────────────────────────

    private static SpriteRenderer[] GetSelectedRenderers()
    {
        var list = new System.Collections.Generic.List<SpriteRenderer>();
        foreach (var go in Selection.gameObjects)
        {
            var sr = go.GetComponent<SpriteRenderer>();
            if (sr != null) list.Add(sr);
        }
        return list.ToArray();
    }
}

/// <summary>
/// Minimal modal dialog for entering an explicit sort order value.
/// </summary>
public class SetOrderDialog : EditorWindow
{
    private int _order;
    private System.Action<int> _onConfirm;

    public static void Open(int currentOrder, System.Action<int> onConfirm)
    {
        var win = CreateInstance<SetOrderDialog>();
        win.titleContent = new GUIContent("Set Sort Order");
        win._order       = currentOrder;
        win._onConfirm   = onConfirm;
        win.minSize      = new Vector2(220, 70);
        win.maxSize      = new Vector2(220, 70);
        win.ShowModal();
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Sort Order:", GUILayout.Width(80));
        GUI.SetNextControlName("OrderField");
        _order = EditorGUILayout.IntField(_order);
        EditorGUILayout.EndHorizontal();

        GUI.FocusControl("OrderField");

        GUILayout.Space(6);
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Apply") || (Event.current.type == EventType.KeyDown &&
                                          Event.current.keyCode == KeyCode.Return))
        {
            _onConfirm?.Invoke(_order);
            Close();
        }

        if (GUILayout.Button("Cancel")) Close();

        EditorGUILayout.EndHorizontal();
    }
}

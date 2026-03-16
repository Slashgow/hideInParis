// Place this file anywhere inside an Editor folder, e.g.:
// Assets/Editor/IsometricMover.cs

using UnityEditor;
using UnityEngine;

/// <summary>
/// Moves selected GameObjects along the 4 isometric diagonal axes plus straight
/// up/down (Procreate 30 degree grid).
///
/// Two ways to use it:
///  1. Top menu bar:   Isometric > Move > ...
///  2. Scene view overlay panel
///
/// The 6 directions:
///
///              Up  ( 0, +1 )        PageUp
///         NW        NE
///           SW    SE
///             Down ( 0, -1 )        PageDown
///
///   NE  ( sqrt3/2, +0.5 )   right arrow
///   SW  (-sqrt3/2, -0.5 )   left arrow   (true opposite of NE)
///   NW  (-sqrt3/2, +0.5 )   up arrow
///   SE  ( sqrt3/2, -0.5 )   down arrow   (true opposite of NW)
///   Up  (  0,      +1.0 )   Page Up
///   Down(  0,      -1.0 )   Page Down
///
/// All operations are Undo-able.
/// Snap value is saved between sessions.
/// </summary>
[InitializeOnLoad]
public static class IsometricMover
{
    // ── Iso axis vectors (30 degrees) ─────────────────────────────────────────

    private static readonly float Cos30 = UnityEngine.Mathf.Sqrt(3f) / 2f; // ~0.866
    private static readonly float Sin30 = 0.5f;

    private static Vector2 IsoNE => new(Cos30, Sin30);   // Right diagonal
    private static Vector2 IsoSW => new(-Cos30, -Sin30);   // Left  diagonal (opposite NE)
    private static Vector2 IsoNW => new(-Cos30, Sin30);   // Up    diagonal
    private static Vector2 IsoSE => new(Cos30, -Sin30);   // Down  diagonal (opposite NW)
    private static Vector2 IsoUp => new(0f, 1f);   // Straight up
    private static Vector2 IsoDown => new(0f, -1f);   // Straight down

    // ── Snap ──────────────────────────────────────────────────────────────────

    private const string PrefKey = "IsoMover_SnapValue";
    public static float SnapValue
    {
        get => EditorPrefs.GetFloat(PrefKey, 0.5f);
        set => EditorPrefs.SetFloat(PrefKey, value);
    }

    // ── Init ──────────────────────────────────────────────────────────────────

    static IsometricMover()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    // ══════════════════════════════════════════════════════════════════════════
    // TOP MENU BAR
    // ══════════════════════════════════════════════════════════════════════════

    [MenuItem("Isometric/Move/NE — Right     _RIGHT", true)]
    [MenuItem("Isometric/Move/SW — Left      _LEFT", true)]
    [MenuItem("Isometric/Move/NW — Up-left   _UP", true)]
    [MenuItem("Isometric/Move/SE — Down-right _DOWN", true)]
    [MenuItem("Isometric/Move/Up             _PAGEUP", true)]
    [MenuItem("Isometric/Move/Down           _PAGEDOWN", true)]
    private static bool ValidateMove() => Selection.gameObjects.Length > 0;

    [MenuItem("Isometric/Move/NE — Right     _RIGHT", false, 10)]
    private static void MenuNE() => MoveSelected(IsoNE);

    [MenuItem("Isometric/Move/SW — Left      _LEFT", false, 11)]
    private static void MenuSW() => MoveSelected(IsoSW);

    [MenuItem("Isometric/Move/NW — Up-left   _UP", false, 12)]
    private static void MenuNW() => MoveSelected(IsoNW);

    [MenuItem("Isometric/Move/SE — Down-right _DOWN", false, 13)]
    private static void MenuSE() => MoveSelected(IsoSE);

    [MenuItem("Isometric/Move/Up             _PAGEUP", false, 20)]
    private static void MenuUp() => MoveSelected(IsoUp);

    [MenuItem("Isometric/Move/Down           _PAGEDOWN", false, 21)]
    private static void MenuDown() => MoveSelected(IsoDown);

    // ══════════════════════════════════════════════════════════════════════════
    // SCENE VIEW OVERLAY PANEL
    // ══════════════════════════════════════════════════════════════════════════

    private static bool _showPanel = true;

    private static void OnSceneGUI(SceneView sceneView)
    {
        Handles.BeginGUI();

        Rect toggleRect = new Rect(8, 8, 110, 22);
        _showPanel = GUI.Toggle(toggleRect, _showPanel, "Iso Mover", EditorStyles.miniButton);

        if (_showPanel)
            DrawPanel();

        Handles.EndGUI();

        HandleKeyboardShortcuts();
    }

    private static void DrawPanel()
    {
        float panelWidth = 160f;
        float panelHeight = 210f;
        float x = 8f;
        float y = 34f;

        GUI.Box(new Rect(x, y, panelWidth, panelHeight), GUIContent.none, EditorStyles.helpBox);

        // ── Snap ──
        GUILayout.BeginArea(new Rect(x + 6, y + 6, panelWidth - 12, 24));
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Snap:", GUILayout.Width(36));
        float newSnap = EditorGUILayout.FloatField(SnapValue, GUILayout.Width(50));
        if (!Mathf.Approximately(newSnap, SnapValue) && newSnap > 0f)
            SnapValue = newSnap;
        GUILayout.Label("u", EditorStyles.miniLabel);
        EditorGUILayout.EndHorizontal();
        GUILayout.EndArea();

        // ── Button layout ──
        //
        //        [   Up   ]          straight up   (PageUp)
        //        [ NW ][ NE ]        iso diagonals
        //        [ SW ][ SE ]        iso diagonals
        //        [  Down  ]          straight down (PageDown)

        float btnW = 62f;
        float btnH = 28f;
        float wideW = btnW * 2 + 4f;   // full-width button spans both columns
        float left = x + 8f;
        float right = left + btnW + 4f;
        float row0 = y + 38f;
        float row1 = row0 + btnH + 4f;
        float row2 = row1 + btnH + 4f;
        float row3 = row2 + btnH + 4f;

        if (GUI.Button(new Rect(left, row0, wideW, btnH), "↑  Up")) MoveSelected(IsoUp);
        if (GUI.Button(new Rect(left, row1, btnW, btnH), "↖  NW")) MoveSelected(IsoNW);
        if (GUI.Button(new Rect(right, row1, btnW, btnH), "NE  ↗")) MoveSelected(IsoNE);
        if (GUI.Button(new Rect(left, row2, btnW, btnH), "↙  SW")) MoveSelected(IsoSW);
        if (GUI.Button(new Rect(right, row2, btnW, btnH), "SE  ↘")) MoveSelected(IsoSE);
        if (GUI.Button(new Rect(left, row3, wideW, btnH), "↓  Down")) MoveSelected(IsoDown);

        // ── Info ──
        GUILayout.BeginArea(new Rect(x + 4, y + panelHeight - 26, panelWidth - 8, 22));
        int count = Selection.gameObjects.Length;
        string info = count == 0
            ? "No selection"
            : $"{count} object{(count > 1 ? "s" : "")} selected";
        GUILayout.Label(info, EditorStyles.centeredGreyMiniLabel);
        GUILayout.EndArea();
    }

    // ── Keyboard shortcuts in the Scene view ──────────────────────────────────

    private static void HandleKeyboardShortcuts()
    {
        Event e = Event.current;
        if (e.type != EventType.KeyDown) return;
        if (Selection.gameObjects.Length == 0) return;

        switch (e.keyCode)
        {
            case KeyCode.RightArrow: MoveSelected(IsoNE); e.Use(); break;
            case KeyCode.LeftArrow: MoveSelected(IsoSW); e.Use(); break;
            case KeyCode.UpArrow: MoveSelected(IsoNW); e.Use(); break;
            case KeyCode.DownArrow: MoveSelected(IsoSE); e.Use(); break;
            case KeyCode.PageUp: MoveSelected(IsoUp); e.Use(); break;
            case KeyCode.PageDown: MoveSelected(IsoDown); e.Use(); break;
        }
    }

    // ══════════════════════════════════════════════════════════════════════════
    // CORE MOVE LOGIC
    // ══════════════════════════════════════════════════════════════════════════

    private static void MoveSelected(Vector2 isoDirection)
    {
        var targets = Selection.gameObjects;
        if (targets.Length == 0) return;

        Vector3 delta = new Vector3(
            isoDirection.x * SnapValue,
            isoDirection.y * SnapValue,
            0f);

        Undo.RecordObjects(
            System.Array.ConvertAll(targets, go => (Object)go.transform),
            "Move Iso");

        foreach (var go in targets)
            go.transform.position += delta;
    }
}
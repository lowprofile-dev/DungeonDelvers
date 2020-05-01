using Sirenix.OdinInspector.Editor;
using UnityEditor;

public class GameSettingsEditorWindow : OdinEditorWindow
{
    [MenuItem("Dungeon Delver/Game Settings")]
    private static void OpenWindow()
    {
        GetWindow<GameSettingsEditorWindow>().Show();
    }

    protected override object GetTarget()
    {
        return GameSettings.Instance;
    }
}

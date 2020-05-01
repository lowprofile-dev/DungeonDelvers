using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameSettings : ScriptableObject
{
    private static GameSettings instance;
    public static GameSettings Instance
    {
        get {
            if (instance == null)
            {
                instance = Resources.Load<GameSettings>("GameSettings");
                if (instance == null)
                {
#if UNITY_EDITOR
                    instance = CreateInstance<GameSettings>();
                    AssetDatabase.CreateAsset(instance, "Assets/Resources/GameSettings.asset");
#else
                Debug.LogError("Game Settings Missing.");
                Application.Quit();
#endif
                }

            }
            return instance;
        }
    }
}

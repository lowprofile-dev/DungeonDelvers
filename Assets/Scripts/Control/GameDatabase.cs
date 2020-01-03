using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Database")]
public class GameDatabase : SerializedScriptableObject
{
    private static GameDatabase _instance;
    public static GameDatabase Instance
    {
        get
        {
            if (_instance == null)
            {
                var gameDatabase = Resources.Load<GameDatabase>("GameDatabase");
                _instance = gameDatabase;
            }
            return _instance;
        }
    }
    
    public string DatabaseVersion;

    #region Databases
    
    public List<CharacterBase> CharacterBases;
    
    #endregion

#if UNITY_EDITOR
    [Button("Build Database", ButtonSizes.Gigantic), PropertyOrder(-999)]
    public void BuildDatabase()
    {
        DatabaseVersion = Application.version;

        var characterList = new List<CharacterBase>();
        var characters = Resources.LoadAll<CharacterBase>("");

        foreach (var character in characters)
        {
            if (!character.databaseIgnore)
                characterList.Add(character);
        }
        
        
        
    }
#endif
}

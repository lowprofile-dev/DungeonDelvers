using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
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

    [SerializeField] private List<Database> _databases;
    [SerializeField] public int DatabaseVersion { get; private set; }

    public static Database Database =>
        Instance._databases.FirstOrDefault(database => database.DatabaseVersion == Instance.DatabaseVersion);
    
#if UNITY_EDITOR
    [Button("Build Database", ButtonSizes.Gigantic), PropertyOrder(-999)]
    public void BuildDatabase()
    {
        try
        {
            var database = new Database();
            if (_databases.Any())
            {
                database.DatabaseVersion = _databases.OrderByDescending(db => db.DatabaseVersion).First().DatabaseVersion + 1;
            }
            else
            {
                database.DatabaseVersion = 1;
            }
            
            var characters = Resources.LoadAll<CharacterBase>("").Where(character => !character.databaseIgnore)
                .ToArray();
            for (int i = 0; i < characters.Length; i++)
            {
                characters[i].uniqueIdentifier = i.ToString();
            }

            database.CharacterBases.AddRange(characters);

            var items = Resources.LoadAll<ItemBase>("").Where(item => !item.databaseIgnore).ToArray();
            for (int i = 0; i < items.Length; i++)
            {
                items[i].uniqueIdentifier = i.ToString();
            }

            database.Items.AddRange(items);

            var passives = Resources.LoadAll<Passive>("").Where(passive => !passive.databaseIgnore).ToArray();
            for (int i = 0; i < passives.Length; i++)
            {
                passives[i].uniqueIdentifier = i.ToString();
            }

            database.Passives.AddRange(passives);

            var skills = Resources.LoadAll<Skill>("").Where(skill => !skill.databaseIgnore).ToArray();
            for (int i = 0; i < skills.Length; i++)
            {
                skills[i].uniqueIdentifier = i.ToString();
            }

            database.Skills.AddRange(skills);

            var monsters = Resources.LoadAll<Monster>("").Where(monster => !monster.databaseIgnore).ToArray();
            for (int i = 0; i < monsters.Length; i++)
            {
                monsters[i].uniqueIdentifier = i.ToString();
            }

            database.Monsters.AddRange(monsters);

            _databases.Add(database);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        
    }
#endif
}

[Serializable]
public class Database
{
    public int DatabaseVersion;
    public List<CharacterBase> CharacterBases = new List<CharacterBase>();
    public List<ItemBase> Items = new List<ItemBase>();
    public List<Passive> Passives = new List<Passive>();
    public List<Skill> Skills = new List<Skill>();
    public List<Monster> Monsters = new List<Monster>();
}
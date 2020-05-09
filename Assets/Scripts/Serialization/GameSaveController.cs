using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using EncryptStringSample;
using Newtonsoft.Json;
using SkredUtils;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class GameSaveController
{
    private const string Key = "*G-KaPdSgVkYp3s6v9y/B?E(H+MbQeTh";
    private static readonly string SaveName = Application.version;
    private const string Extension = "ddgs";
    private static readonly string FullSaveName = $"{SaveName}.{Extension}";
    private static readonly string FolderPath = Application.persistentDataPath;
    private static readonly string FullSavePath = Path.Combine(FolderPath, FullSaveName);

    private string GetFullSavePath(string saveName) =>
        $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}{saveName}.{Extension}";
    
    
    public static void Save()
    {
        var stopwatch = Stopwatch.StartNew();
        var player = PlayerController.Instance;

        Save save = new Save
        {
            SaveVersion = Application.version,
            Items = player.Inventory,
            Characters = player.Party,
            Globals = GameController.Instance.Globals,
            Seeds = GameController.Instance.Seeds,
            CurrentSceneIndex = SceneManager.GetActiveScene().buildIndex,
            ScenePosition = player.transform.position,
            CurrentExp = player.CurrentExp,
            CurrentGold = player.CurrentGold,
            CurrentLevel = player.PartyLevel
        };

        var json = JsonConvert.SerializeObject(save, Formatting.Indented);
        var encryptedJson = StringCipher.Encrypt(json, Key);
        
        File.WriteAllText(FullSavePath, encryptedJson);
        File.WriteAllText(FullSavePath+"unenc", json);
        
        stopwatch.Stop();
        
        Debug.Log($"Saved to {FullSavePath}");
        Debug.Log($"Save gerado em {stopwatch.ElapsedMilliseconds}ms");
    }

    public static bool SaveExists => File.Exists(FullSavePath);
    public static void Load()
    {
        if (!SaveExists)
        {
            throw new NullReferenceException();
        }

        var encryptedJson = File.ReadAllText(FullSavePath);
        var json = StringCipher.Decrypt(encryptedJson, Key);

        var save = JsonConvert.DeserializeObject<Save>(json);

        PlayerController.Instance.Party = save.Characters;
        PlayerController.Instance.Inventory = save.Items;
        GameController.Instance.Globals = save.Globals;
        GameController.Instance.Seeds = save.Seeds;
        PlayerController.Instance.CurrentGold = save.CurrentGold;
        PlayerController.Instance.CurrentExp = save.CurrentExp;
        PlayerController.Instance.PartyLevel = save.CurrentLevel;
        PlayerController.Instance.transform.position = save.ScenePosition;
        
        PlayerController.Instance.Party.ForEach(p => p.Regenerate());

        SceneManager.LoadScene(save.CurrentSceneIndex);
    }
}


[Serializable]
public class Save
{
    public string SaveVersion { get; set; }
    [JsonConverter(typeof(PartyConverter))] public List<Character> Characters { get; set; }
    [JsonConverter(typeof(InventoryConverter))] public List<Item> Items { get; set; }
    public Dictionary<string,int> Globals { get; set; }
    public Dictionary<int,int> Seeds { get; set; }
    public int CurrentGold { get; set; }
    public int CurrentExp { get; set; }
    public int CurrentLevel { get; set; }
    public int CurrentSceneIndex { get; set; }
    public Vector3 ScenePosition { get; set; }
    //seeds dos mapas
}


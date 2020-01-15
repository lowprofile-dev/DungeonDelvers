using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using EncryptStringSample;
using Newtonsoft.Json;
using SkredUtils;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class GameSaveController
{
    private const string Key = "ABCDEFGHIJKLMNOPKRSTUVWXYZ1234567890";
    private static readonly string SaveName = Application.version;
    private const string Extension = "dds";
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
            Characters = null,
            Globals = null
        };

        var json = JsonConvert.SerializeObject(save, Formatting.Indented);

        json = StringCipher.Encrypt(json, Key);
        
        File.WriteAllText(FullSavePath, json);
        
        stopwatch.Stop();
        
        Debug.Log($"Saved to {FullSavePath}");
        Debug.Log($"Save gerado em {stopwatch.ElapsedMilliseconds}ms");
    }

    public void Load(string saveName)
    {
//        var player = PlayerController.Instance;
//        if (player == null)
//            return;
//
//        var encryptedJson = File.ReadAllText(GetFullSavePath(saveName));
//        var decryptedJson = StringCipher.Decrypt(encryptedJson, Key);
//
//        var save = JsonConvert.DeserializeObject<Save>(decryptedJson);
//
//        if (save.SaveVersion != Application.version)
//        {
//            throw new NotImplementedException();
//        }
//
//        player.Inventory = save.ItemSaves.EachDo(ItemInstanceBuilder.BuildInstance);
//        player.Party = save.CharacterSaves.EachDo(characterSave => new Character(characterSave));
    }
}


[Serializable]
public class Save
{
    public string SaveVersion { get; set; }
    [JsonConverter(typeof(PartyConverter))]
    public List<Character> Characters { get; set; }
    [JsonConverter(typeof(InventoryConverter))]
    public List<Item> Items { get; set; }
    public Dictionary<string,int> Globals { get; set; }
    public int CurrentGold { get; set; }
    public int CurrentExp { get; set; }
    public int CurrentLevel { get; set; }
    public int CurrentSceneIndex { get; set; }
    public Vector3 ScenePosition { get; set; }
    //seeds dos mapas
}


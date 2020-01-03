using System;
using System.Collections.Generic;
using System.IO;
using EncryptStringSample;
using Newtonsoft.Json;
using SkredUtils;
using UnityEngine;

public class GameSaveController
{
    private const string Key = "ABCDEFGHIJKLMNOPKRSTUVWXYZ1234567890";
    private const string Extension = "gsv";

    private string GetFullSavePath(string saveName) =>
        $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}{saveName}.{Extension}";
    
    
    public void Save(string saveName)
    {
        var player = PlayerController.Instance;
        if (player == null)
            return;
        
        var save = new Save()
        {
            SaveVersion = Application.version,
            CharacterSaves = player.Party.EachDo(character => character.Serialize()),
            ItemSaves = player.Inventory.EachDo(item => item.Serialize())
        };

        var json = JsonConvert.SerializeObject(save,Formatting.Indented);
        var encryptedJson = StringCipher.Encrypt(json, Key);

        File.WriteAllText(GetFullSavePath(saveName), encryptedJson);
    }

    public void Load(string saveName)
    {
        var player = PlayerController.Instance;
        if (player == null)
            return;

        var encryptedJson = File.ReadAllText(GetFullSavePath(saveName));
        var decryptedJson = StringCipher.Decrypt(encryptedJson, Key);

        var save = JsonConvert.DeserializeObject<Save>(decryptedJson);

        if (save.SaveVersion != Application.version)
        {
            throw new NotImplementedException();
        }

        player.Inventory = save.ItemSaves.EachDo(ItemInstanceBuilder.BuildInstance);
        player.Party = save.CharacterSaves.EachDo(characterSave => new Character(characterSave));
    }
}


[Serializable]
public struct Save
{
    public string SaveVersion { get; set; }
    public List<CharacterSave> CharacterSaves { get; set; }
    public List<ItemSave> ItemSaves { get; set; }
    public List<KeyValuePair<string,int>> Globals { get; set; }
}


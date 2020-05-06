using System.Collections.Generic;
using Sirenix.OdinInspector;
using SkredUtils;
using UnityEngine;


// [CreateAssetMenu(fileName = "CharacterDatabase", menuName = "Character/CharacterDatabase")]
// public class CharacterDatabase : SerializedScriptableObject
// {
//     public string DatabaseVersion = "";
//     public List<CharacterBase> CharacterBases = new List<CharacterBase>();
//
//     public static CharacterDatabase Instance
//     {
//         get
//         {
//             var resource = Resources.Load("Characters/CharacterDatabase");
//
//             if (resource is CharacterDatabase database)
//                 return database;
//             return null;
//         }
//     }
//
// #if UNITY_EDITOR
//     [Button(Name = "Load All")] void LoadAllCharacters()
//     {
//         var resources = Resources.LoadAll("", typeof(CharacterBase));
//         CharacterBases = new List<CharacterBase>();
//
//         foreach (var resource in resources)
//             CharacterBases.Include(resource as CharacterBase);
//     }
// #endif
// }
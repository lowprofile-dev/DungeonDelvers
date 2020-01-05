using System;
using Boo.Lang;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

[CreateAssetMenu(menuName = "Character/Character", fileName = "New Character")]
public class CharacterBase : SerializableAsset
{
    [PropertyOrder(-100)] public string CharacterName = "";
    [PropertyOrder(-99)] public Sprite CharacterFace = null;
    [PropertyOrder(-98)] public GameObject CharacterPrefab = null;
    [PropertyOrder(-97)] public GameObject BattlerPrefab = null;

    [FoldoutGroup("Stats")] public int BaseMaxHp;
    [FoldoutGroup("Stats")] public int MaxHpGrowth;
    [FoldoutGroup("Stats")] public int BaseInitialEp;
    [FoldoutGroup("Stats")] public int BaseEpGain;
    [FoldoutGroup("Stats")] public int BasePhysAtk;
    [FoldoutGroup("Stats")] public int PhysAtkGrowth;
    [FoldoutGroup("Stats")] public int BaseMagAtk;
    [FoldoutGroup("Stats")] public int MagAtkGrowth;
    [FoldoutGroup("Stats")] public int BasePhysDef;
    [FoldoutGroup("Stats")] public int PhysDefGrowth;
    [FoldoutGroup("Stats")] public int BaseMagDef;
    [FoldoutGroup("Stats")] public int MagDefGrowth;
    [FoldoutGroup("Stats")] public int BaseSpeed;
    [FoldoutGroup("Stats")] public int SpeedGrowth;
    [FoldoutGroup("Stats"), PropertyRange(0,1)] public float BaseAccuracy;
    [FoldoutGroup("Stats"), PropertyRange(0,1)] public float BaseEvasion;
    [FoldoutGroup("Stats"), PropertyRange(0,1)] public float BaseCritChance;
    [FoldoutGroup("Stats"), PropertyRange(0,1)] public float BaseCritAvoid;

    [FoldoutGroup("Equips")] public List<WeaponBase.WeaponType> WeaponTypes = new List<WeaponBase.WeaponType>();
    [FoldoutGroup("Equips")] public List<EquippableBase.ArmorType> ArmorTypes = new List<EquippableBase.ArmorType>();
    
    [FoldoutGroup("Equips")] public WeaponBase Weapon;
    [FoldoutGroup("Equips")] public HeadBase Head;
    [FoldoutGroup("Equips")] public BodyBase Body;
    [FoldoutGroup("Equips")] public HandBase Hand;
    [FoldoutGroup("Equips")] public FeetBase Feet;
    [FoldoutGroup("Equips")] public AccessoryBase Accessory1;
    [FoldoutGroup("Equips")] public AccessoryBase Accessory2;
    [FoldoutGroup("Equips")] public AccessoryBase Accessory3;
    
#if UNITY_EDITOR
    [InfoBox("$LevelMessage", InfoMessageType.Info), FoldoutGroup("Stats"), PropertyOrder(-1), Range(1,50)] 
    public int Level;
    public string LevelMessage
    {
        get
        {
            var message = $"Level {Level}\n";
            message += $"Max HP: {BaseMaxHp + MaxHpGrowth * Level}\n";
            message += $"Phys Atk: {BasePhysAtk + PhysAtkGrowth * Level}\n";
            message += $"Mag Atk: {BaseMagAtk + MagAtkGrowth * Level}\n";
            message += $"Phys Def: {BasePhysDef + PhysDefGrowth * Level}\n";
            message += $"Mag Def: {BaseMagDef + MagDefGrowth * Level}\n";
            message += $"Speed: {BaseSpeed + SpeedGrowth * Level}\n";
            return message;
        }
    }
#endif
}

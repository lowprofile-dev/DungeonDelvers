using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

[CreateAssetMenu(menuName = "Character/Character", fileName = "New Character")]
public class CharacterBase : SerializableAsset
{
    #region Data

    [PropertyOrder(-100)] public string CharacterName = "";
    [PropertyOrder(-98)] public GameObject CharacterPrefab = null;
    [PropertyOrder(-97)] public GameObject BattlerPrefab = null;

    [PropertyOrder(-96)]
    public List<BattlerAnimationController> BattlerAnimationControllers = new List<BattlerAnimationController>();

    [FoldoutGroup("Stats")] public Stats Bases;
    [FoldoutGroup("Stats")] public Stats Growths;

    [FoldoutGroup("Equips")] public List<WeaponBase.WeaponType> WeaponTypes = new List<WeaponBase.WeaponType>();

    [FoldoutGroup("Equips")] public List<EquippableBase.ArmorType> ArmorTypes = new List<EquippableBase.ArmorType>();

    [FoldoutGroup("Equips")] public WeaponBase Weapon;
    [FoldoutGroup("Equips")] public HeadBase Head;
    [FoldoutGroup("Equips")] public BodyBase Body;
    [FoldoutGroup("Equips")] public HandBase Hand;
    [FoldoutGroup("Equips")] public FeetBase Feet;
    [FoldoutGroup("Equips")] public AccessoryBase Accessory;

    // [ListDrawerSettings(ListElementLabelName = "_masteryElementName")]
    // public List<_Mastery> Masteries = new List<_Mastery>();
    
    public GameObject MasteryGrid;

    public List<PlayerSkill> BaseSkills = new List<PlayerSkill>();
    public List<Passive> BasePassives = new List<Passive>();
    public PlayerSkill DefaultDefendSkill;
    public SoundInfo HitSound;

    #endregion

    public struct BattlerAnimationController
    {
        public WeaponBase.WeaponType? WeaponType;
        public RuntimeAnimatorController AnimatorController;
    }

#if UNITY_EDITOR

    [InfoBox("$LevelMessage", InfoMessageType.Info), FoldoutGroup("Stats"), PropertyOrder(-1), Range(1, 50)]
    public int Level;

    public string LevelMessage
    {
        get
        {
            var message = $"Level {Level}\n";
            message += $"Max HP: {Bases.MaxHp + Growths.MaxHp * Level}\n";
            message += $"Phys Atk: {Bases.PhysAtk + Growths.PhysAtk * Level}\n";
            message += $"Mag Atk: {Bases.MagAtk + Growths.MagAtk * Level}\n";
            message += $"Phys Def: {Bases.PhysDef + Growths.PhysDef * Level}\n";
            message += $"Mag Def: {Bases.MagDef + Growths.MagDef * Level}\n";
            message += $"Speed: {Bases.Speed + Growths.Speed * Level}\n";
            return message;
        }
    }
#endif
}
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
    [FoldoutGroup("Stats"), PropertyRange(0, 1)] public float BaseAccuracy;
    [FoldoutGroup("Stats"), PropertyRange(0, 1)] public float BaseEvasion;
    [FoldoutGroup("Stats"), PropertyRange(0, 1)] public float BaseCritChance;
    [FoldoutGroup("Stats"), PropertyRange(0, 1)] public float BaseCritAvoid;

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

    [ListDrawerSettings(ListElementLabelName = "_masteryElementName")]public List<Mastery> Masteries = new List<Mastery>();

    #endregion


#if UNITY_EDITOR
    [Button("Generate Generic Masteries")]
    public void _generateGenericMasteries()
    {
        //Arrumar pra apagar se já existir
        Func<int, string> indexToLevel = (index) =>
        {
            switch (index)
            {
                case 0:
                    return "I";
                case 1:
                    return "II";
                case 2:
                    return "III";
                case 3:
                    return "IV";
                case 4:
                    return "V";
                default:
                    return "";
            }
        };

        Mastery previous = null;
        for (int i = 0; i < 5; i++)
        {
            if (Masteries.All(mastery => mastery.MasteryName != $"Hit Point Training {indexToLevel(i)}"))
            {
                var mastery = new Mastery();
                mastery.MasteryName = $"Hit Point Training {indexToLevel(i)}";
                var effect = new BaseStatMasteryEffect();
                mastery.MasteryMaxLevel = 10;
                mastery.MPCost = 1 + 1*i;
                effect.Stats = new Stats
                {
                    MaxHp = 4 + 2*i
                };
                mastery.Effects.Add(effect);
                if (previous != null)
                {
                    var condition = new HasMasteryMasteryCondition();
                    condition.Mastery = previous;
                    condition.RequiredLevel = 10;
                    mastery.Conditions.Add(condition);
                }
                previous = mastery;
                Masteries.Add(mastery);
            }
        }

        previous = null;
        for (int i = 0; i < 5; i++)
        {
            if (Masteries.All(mastery => mastery.MasteryName != $"Physical Attack Training {indexToLevel(i)}"))
            {
                var mastery = new Mastery();
                mastery.MasteryName = $"Physical Attack Training {indexToLevel(i)}";
                var effect = new BaseStatMasteryEffect();
                mastery.MasteryMaxLevel = 10;
                mastery.MPCost = 1 + 1 * i;
                effect.Stats = new Stats
                {
                    PhysAtk = 2 + 1 * i
                };
                mastery.Effects.Add(effect);
                if (previous != null)
                {
                    var condition = new HasMasteryMasteryCondition();
                    condition.Mastery = previous;
                    condition.RequiredLevel = 10;
                    mastery.Conditions.Add(condition);
                }
                previous = mastery;
                Masteries.Add(mastery);
            }
        }

        previous = null;
        for (int i = 0; i < 5; i++)
        {
            if (Masteries.All(mastery => mastery.MasteryName != $"Magical Attack Training {indexToLevel(i)}"))
            {
                var mastery = new Mastery();
                mastery.MasteryName = $"Magical Attack Training {indexToLevel(i)}";
                var effect = new BaseStatMasteryEffect();
                mastery.MasteryMaxLevel = 10;
                mastery.MPCost = 1 + 1*i;
                effect.Stats = new Stats
                {
                    MagAtk = 2 + 1*i
                };
                mastery.Effects.Add(effect);
                if (previous != null)
                {
                    var condition = new HasMasteryMasteryCondition();
                    condition.Mastery = previous;
                    condition.RequiredLevel = 10;
                    mastery.Conditions.Add(condition);
                }

                previous = mastery;
                Masteries.Add(mastery);
            }
        }

        previous = null;
        for(int i = 0; i < 5; i++)
        {
            if (Masteries.All(mastery => mastery.MasteryName != $"Physical Defense Training {indexToLevel(i)}"))
            {
                var mastery = new Mastery();
                mastery.MasteryName = $"Physical Defense Training {indexToLevel(i)}";
                var effect = new BaseStatMasteryEffect();
                mastery.MasteryMaxLevel = 10;
                mastery.MPCost = 1 + 1*i;
                effect.Stats = new Stats
                {
                    PhysDef = 2 + 1*i
                };
                mastery.Effects.Add(effect);
                if (previous != null)
                {
                    var condition = new HasMasteryMasteryCondition();
                    condition.Mastery = previous;
                    condition.RequiredLevel = 10;
                    mastery.Conditions.Add(condition);
                }
                previous = mastery;
                Masteries.Add(mastery);
            }
        }

        previous = null;
        for (int i = 0; i<5; i++)
        {
            if (Masteries.All(mastery => mastery.MasteryName != $"Magical Defense Training {indexToLevel(i)}"))
            {
                var mastery = new Mastery();
                mastery.MasteryName = $"Magical Defense Training {indexToLevel(i)}";
                var effect = new BaseStatMasteryEffect();
                mastery.MasteryMaxLevel = 10;
                mastery.MPCost = 1+1*i;
                effect.Stats = new Stats
                {
                    MagDef = 2+1*i
                };
                mastery.Effects.Add(effect);
                if (previous != null)
                {
                    var condition = new HasMasteryMasteryCondition();
                    condition.Mastery = previous;
                    condition.RequiredLevel = 10;
                    mastery.Conditions.Add(condition);
                }
                previous = mastery;
                Masteries.Add(mastery);
            }
        }

        previous = null;
        for (int i = 0; i < 5; i++)
        {
            if (Masteries.All(mastery => mastery.MasteryName != $"Speed Training {indexToLevel(i)}"))
            {
                var mastery = new Mastery();
                mastery.MasteryName = $"Speed Training {indexToLevel(i)}";
                var effect = new BaseStatMasteryEffect();
                mastery.MasteryMaxLevel = 10;
                mastery.MPCost = 1 + 1*i;
                effect.Stats = new Stats
                {
                    Speed = 2 + 1*i
                };
                mastery.Effects.Add(effect);
                if (previous != null)
                {
                    var condition = new HasMasteryMasteryCondition();
                    condition.Mastery = previous;
                    condition.RequiredLevel = 10;
                    mastery.Conditions.Add(condition);
                }
                previous = mastery;
                Masteries.Add(mastery);
            }
        }
    }
    
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

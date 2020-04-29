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
    [PropertyOrder(-96)] public List<BattlerAnimationController> BattlerAnimationControllers = new List<BattlerAnimationController>();

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

    [ListDrawerSettings(ListElementLabelName = "_masteryElementName")]public List<Mastery> Masteries = new List<Mastery>();
    public List<PlayerSkill> BaseSkills = new List<PlayerSkill>();
    public List<Passive> BasePassives = new List<Passive>();
    public PlayerSkill DefaultDefendSkill;

    #endregion

    public struct BattlerAnimationController
    {
        public WeaponBase.WeaponType? WeaponType;
        public RuntimeAnimatorController AnimatorController;
    }
    
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

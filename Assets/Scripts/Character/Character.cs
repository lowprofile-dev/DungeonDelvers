using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;

public class Character
{
    public CharacterBase Base { get; set; }

    #region Instancing

    public CharacterSave Serialize()
    {
        var save = new CharacterSave()
        {
            baseUid = Base.uniqueIdentifier,
            //MaxHP = MaxHP,
            //CurrentHP = CurrentHP
        };

        return save;
    }

    public Character(CharacterBase characterBase)
    {
        Base = characterBase;
        Weapon = ItemInstanceBuilder.BuildInstance(Base.Weapon) as Equippable;
        Head = ItemInstanceBuilder.BuildInstance(Base.Head) as Equippable;
        Body = ItemInstanceBuilder.BuildInstance(Base.Body) as Equippable;
        Hand = ItemInstanceBuilder.BuildInstance(Base.Hand) as Equippable;
        Feet = ItemInstanceBuilder.BuildInstance(Base.Feet) as Equippable;
        Accessory = ItemInstanceBuilder.BuildInstance(Base.Accessory) as Equippable;
        MasteryGroup = new MasteryGroup(this);

        Regenerate();

        CurrentHp = Stats.MaxHp;
    }

    public Character(CharacterSave save)
    {
        Base = CharacterDatabase.Instance.CharacterBases.Find(x => x.uniqueIdentifier == save.baseUid);
        //MaxHP = save.MaxHP;
        //CurrentHP = save.CurrentHP;
    }

    #endregion

    #region Stats

    private int CurrentLevel => PlayerController.Instance.PartyLevel;

    public int CurrentMp;

    [FoldoutGroup("Stats"), ShowInInspector, Sirenix.OdinInspector.ReadOnly] private int currentHp;

    public int CurrentHp {
        get => currentHp;
        set {
            currentHp = value;
            currentHp = Mathf.Clamp(currentHp, 0, Stats.MaxHp);
        }
    }

    public bool Fainted => CurrentHp == 0;

    [FoldoutGroup("Stats"), Sirenix.OdinInspector.ReadOnly] public Stats BaseStats;
    [FoldoutGroup("Stats"), Sirenix.OdinInspector.ReadOnly] public Stats BonusStats;
    [FoldoutGroup("Stats"), Sirenix.OdinInspector.ReadOnly] public Stats Stats;

    [FoldoutGroup("Equips")] public Equippable Weapon;
    [FoldoutGroup("Equips")] public Equippable Head;
    [FoldoutGroup("Equips")] public Equippable Body;
    [FoldoutGroup("Equips")] public Equippable Hand;
    [FoldoutGroup("Equips")] public Equippable Feet;
    [FoldoutGroup("Equips")] public Equippable Accessory;

    //[FoldoutGroup("Passives")] public List<StatPassive> StatPassives;

    [ShowInInspector] public List<PlayerSkill> Skills { get; private set; }

    [ShowInInspector] public MasteryGroup MasteryGroup { get; private set; }

    [ShowInInspector] public List<Passive> Passives { get; private set; }

    public IEnumerable<Equippable> Equipment {
        get {
            return new Equippable[] { Weapon, Head, Body, Hand, Feet, Accessory }.Where(equippable => equippable != null);
        }
    }

    #endregion

    #region Updating

    public void LevelUp()
    {
        CurrentMp += (CurrentLevel / 5) + 1;
        Regenerate();
    }
    public void Regenerate()
    {
        var stopwatch = Stopwatch.StartNew();

        RecalculateBases();
        RecalculateBonus();
        LoadSkills();
        LoadPassives();
        LoadMasteries();

        Stats = BaseStats + BonusStats;

        stopwatch.Stop();

        Debug.Log($"Recalculated {Base.CharacterName} stats: {stopwatch.ElapsedMilliseconds}ms");
    }

    private void RecalculateBases()
    {
        BaseStats = Base.Bases + (Base.Growths * CurrentLevel);
    }

    private void RecalculateBonus()
    {
        BonusStats = new Stats();

        foreach (var equipInstance in Equipment)
        {
            var equip = equipInstance.EquippableBase;
            BonusStats += equip.Stats;
        }
    }

    private void LoadSkills()
    {
        Skills = new List<PlayerSkill>();
        foreach (var equippable in Equipment)
        {
            Skills.AddRange(equippable.EquippableBase.Skills);
        }
    }

    private void LoadPassives()
    {
        Passives = new List<Passive>();

        foreach (var equippable in Equipment)
        {
            Passives.AddRange(equippable.EquippableBase.Passives);
        }
    }

    private void LoadMasteries()
    {
        var masteries = MasteryGroup.Masteries.Values;

        foreach (var mastery in masteries)
        {
            mastery.ApplyMastery();
        }
    }

    public void Equip(Equippable equippable)
    {
        if (equippable == null || equippable.EquippableBase == null)
            throw new NullReferenceException();

        var slot = equippable.EquippableBase.Slot;

        if (!CanBeEquipped(equippable.EquippableBase))
        {
            Debug.LogError($"{Base.CharacterName} não pode equipar {equippable.EquippableBase.itemName}");
            return;
        }

        switch (slot)
        {
            case EquippableBase.EquippableSlot.Accessory:
                {
                    var old = Accessory;
                    Accessory = equippable;
                    OnUnequip.Invoke(old);
                    OnEquip.Invoke(Accessory);
                    PlayerController.Instance.Inventory.Add(old);
                    break;
                }
            case EquippableBase.EquippableSlot.Body:
                {
                    var old = Body;
                    Body = equippable;
                    OnUnequip.Invoke(old);
                    OnEquip.Invoke(Body);
                    PlayerController.Instance.Inventory.Add(old);
                    break;
                }
            case EquippableBase.EquippableSlot.Feet:
                {
                    var old = Feet;
                    Feet = equippable;
                    OnUnequip.Invoke(old);
                    OnEquip.Invoke(Feet);
                    PlayerController.Instance.Inventory.Add(old);
                    break;
                }
            case EquippableBase.EquippableSlot.Hand:
                {
                    var old = Hand;
                    Hand = equippable;
                    OnUnequip.Invoke(old);
                    OnEquip.Invoke(Hand);
                    PlayerController.Instance.Inventory.Add(old);
                    break;
                }
            case EquippableBase.EquippableSlot.Head:
                {
                    var old = Head;
                    Head = equippable;
                    OnUnequip.Invoke(old);
                    OnEquip.Invoke(Head);
                    PlayerController.Instance.Inventory.Add(old);
                    break;
                }
            case EquippableBase.EquippableSlot.Weapon:
                {
                    var old = Weapon;
                    Weapon = equippable;
                    OnUnequip.Invoke(old);
                    OnEquip.Invoke(Weapon);
                    PlayerController.Instance.Inventory.Add(old);
                    break;
                }
        }

        PlayerController.Instance.Inventory.Remove(equippable);

        Regenerate();
    }

    private bool CanBeEquipped(EquippableBase equippable)
    {
        if (equippable is WeaponBase weaponBase)
        {
            return Base.WeaponTypes.Contains(weaponBase.weaponType);
        }
        else if (equippable is IArmorTypeEquipment armorBase)
        {
            return Base.ArmorTypes.Contains(armorBase.ArmorType);
        }
        else
            return true;
    }

    #endregion

    #region Events

    public EquipEvent OnEquip = new EquipEvent();
    public EquipEvent OnUnequip = new EquipEvent();

    #endregion

#if UNITY_EDITOR
    [FoldoutGroup("Equips"), PropertyOrder(-1), ShowInInspector, OnValueChanged("_EquipBase")]
    private EquippableBase _ToEquip;

    private void _EquipBase()
    {
        if (_ToEquip == null)
            return;
        var equip = ItemInstanceBuilder.BuildInstance(_ToEquip) as Equippable;
        Equip(equip);
        _ToEquip = null;
    }
#endif

    public Equippable GetSlot(EquippableBase.EquippableSlot slot)
    {
        switch (slot)
        {
            case EquippableBase.EquippableSlot.Weapon:
                return Weapon;
            case EquippableBase.EquippableSlot.Hand:
                return Hand;
            case EquippableBase.EquippableSlot.Head:
                return Head;
            case EquippableBase.EquippableSlot.Body:
                return Body;
            case EquippableBase.EquippableSlot.Feet:
                return Feet;
            case EquippableBase.EquippableSlot.Accessory:
                return Accessory;
            default:
                throw new ArgumentException();
        }
    }
}

[Serializable]
public class EquipEvent : UnityEvent<Equippable>
{
    public EquipEvent() { }
}

[Serializable]
public class CharacterEvent : UnityEvent<Character>
{
}
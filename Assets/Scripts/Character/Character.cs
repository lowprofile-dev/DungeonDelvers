using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.Collections;
using UnityEngine;
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
        Accessory1 = ItemInstanceBuilder.BuildInstance(Base.Accessory1) as Equippable;
        Accessory2 = ItemInstanceBuilder.BuildInstance(Base.Accessory2) as Equippable;
        Accessory3 = ItemInstanceBuilder.BuildInstance(Base.Accessory3) as Equippable;
        Regenerate();

        CurrentHp = Stats.MaxHp;
        MasteryGroup = new MasteryGroup(this);
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
    
    [FoldoutGroup("Stats"), ShowInInspector, Sirenix.OdinInspector.ReadOnly]private int currentHp;

    public int CurrentHp
    {
        get => currentHp;
        set
        {
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
    [FoldoutGroup("Equips")] public Equippable Accessory1;
    [FoldoutGroup("Equips")] public Equippable Accessory2;
    [FoldoutGroup("Equips")] public Equippable Accessory3;

    //[FoldoutGroup("Passives")] public List<StatPassive> StatPassives;
    
    [ShowInInspector] public List<PlayerSkill> Skills { get; private set; }

    [ShowInInspector] public MasteryGroup MasteryGroup { get; private set; }

    [ShowInInspector] public List<Passive> Passives { get; private set; }
    
    public IEnumerable<Equippable> Equipment
    {
        get
        {
            return new Equippable[] {Weapon, Head, Body, Hand, Feet, Accessory1, Accessory2, Accessory3}.Where(equippable => equippable != null);
        }
    }

    #endregion

    #region Updating

    public void Regenerate()
    {
        var stopwatch = Stopwatch.StartNew();
        
        RecalculateBases();
        RecalculateBonus();
        LoadSkills();
        LoadPassives();
        
        Stats = BaseStats + BonusStats;
        
        stopwatch.Stop();
        
        Debug.Log($"Recalculated {Base.CharacterName} stats: {stopwatch.ElapsedMilliseconds}ms");
    }

    private void RecalculateBases()
    {
        BaseStats = new Stats()
        {
            MaxHp = Base.BaseMaxHp + Base.MaxHpGrowth * CurrentLevel,
            InitialEp = Base.BaseInitialEp,
            EpGain = Base.BaseEpGain,
            PhysAtk = Base.BasePhysAtk + Base.PhysAtkGrowth * CurrentLevel,
            MagAtk = Base.BaseMagAtk + Base.MagAtkGrowth * CurrentLevel,
            PhysDef = Base.BasePhysDef + Base.PhysDefGrowth * CurrentLevel,
            MagDef = Base.BaseMagDef + Base.MagDefGrowth * CurrentLevel,
            Speed = Base.BaseSpeed + Base.SpeedGrowth * CurrentLevel
        };
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

    public void Equip(Equippable equippable, int accessorySlot = 0)
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
                if (accessorySlot == 0)
                {
                    var oldAccessory = Accessory1;
                    Accessory1 = equippable;
                    OnUnequip(oldAccessory);
                    OnEquip(Accessory1);
                } else if (accessorySlot == 1)
                {
                    var oldAccessory = Accessory2;
                    Accessory2 = equippable;
                    OnUnequip(oldAccessory);
                    OnEquip(Accessory2);
                } else if (accessorySlot == 2)
                {
                    var oldAccessory = Accessory2;
                    Accessory2 = equippable;
                    OnUnequip(oldAccessory);
                    OnEquip(Accessory2);
                }
                else
                {
                    throw new IndexOutOfRangeException();
                }
                break;
            case EquippableBase.EquippableSlot.Body:
                var old = Body;
                Body = equippable;
                OnUnequip(old);
                OnEquip(Body);
                break;
            case EquippableBase.EquippableSlot.Feet:
                old = Feet;
                Feet = equippable;
                OnUnequip(old);
                OnEquip(Feet);
                break;
            case EquippableBase.EquippableSlot.Hand:
                old = Hand;
                Hand = equippable;
                OnUnequip(old);
                OnEquip(Hand);
                break;
            case EquippableBase.EquippableSlot.Head:
                old = Head;
                Head = equippable;
                OnUnequip(old);
                OnEquip(Head);
                break;
            case EquippableBase.EquippableSlot.Weapon:
                old = Weapon;
                Weapon = equippable;
                OnUnequip(old);
                OnEquip(Weapon);
                break;
        }
        
        Regenerate();
    }

    private bool CanBeEquipped(EquippableBase equippable)
    {
        if (equippable is WeaponBase weaponBase)
        {
            return Base.WeaponTypes.Contains(weaponBase.weaponType);
        } else if (equippable is IArmorTypeEquipment armorBase)
        {
            return Base.ArmorTypes.Contains(armorBase.ArmorType);
        }
        else
            return true;
    }

    #endregion

    #region Events

    private void OnEquip(Equippable equip)
    {
        
    }

    private void OnUnequip(Equippable equip)
    {
        
    }

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
}
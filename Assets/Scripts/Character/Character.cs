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

    public CharacterBase Base { get; set; }

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
    }

    public Character(CharacterSave save)
    {
        Base = CharacterDatabase.Instance.CharacterBases.Find(x => x.uniqueIdentifier == save.baseUid);
        //MaxHP = save.MaxHP;
        //CurrentHP = save.CurrentHP;
    }

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
    
    [ShowInInspector] public List<Skill> Skills { get; private set; }
    
    public IEnumerable<Equippable> Equipment
    {
        get
        {
            return new Equippable[] {Weapon, Head, Body, Hand, Feet, Accessory1, Accessory2, Accessory3}.Where(equippable => equippable != null);
        }
    }

    public void Regenerate()
    {
        var stopwatch = Stopwatch.StartNew();
        
        RecalculateBases();
        RecalculateBonus();
        LoadSkills();
        
        Stats = BaseStats + BonusStats;
        
        stopwatch.Stop();
        
        Debug.Log($"Recalculated {Base.CharacterName} stats: {stopwatch.ElapsedMilliseconds}ms");
    }

    public void RecalculateBases()
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

    public void RecalculateBonus()
    {
        BonusStats = new Stats();
        
        foreach (var equipInstance in Equipment)
        {
            var equip = equipInstance.EquippableBase;
            BonusStats += equip.Stats;
        }
    }

    public void LoadSkills()
    {
        Skills = new List<Skill>();
        //Skills dos niveis e tal
        foreach (var equippable in Equipment)
        {
            Skills.AddRange(equippable.EquippableBase.Skills);
        }
    }
    
    private int CurrentLevel => PlayerController.Instance.PartyLevel;

    public void Equip(Equippable equippable, int accessorySlot = 0)
    {
        if (equippable == null || equippable.EquippableBase == null)
            throw new NullReferenceException();

        var slot = equippable.EquippableBase.Slot;
        
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

    private void OnEquip(Equippable equip)
    {
        
    }

    private void OnUnequip(Equippable equip)
    {
        
    }

    
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

    //QUANDO UPAM PERSONAGENS GANHAM PONTOS PRA FAZER UPGRADE
    //TODOS OS PERSONAGENS TEM UMA LISTA DE UPGRADES, QUE PODEM TER REQUERIMENTOS -> Lista de um objeto, esse objeto de uma lista de requerimentos, que é algo abstrato que pode ser ex. Ter já X upgrade, não ter X upgrade, ser nivel X, etc. 
    //UPGRADES TEM NIVEIS MAXIMOS, SÓ PODEM SER UPDADOS QUANDO NIVEL ATUAL < MAX E REQUERIMENTOS PASSAM
}
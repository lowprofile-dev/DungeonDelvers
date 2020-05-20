using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using SkredUtils;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;


public class Character
{
    public CharacterBase Base { get; private set; }

    #region Instancing

    public CharacterSave Serialize()
    {
        var save = new CharacterSave()
        {
            baseUid = GameSettings.Instance.CharacterDatabase.GetId(Base).Value,
            currentHp = currentHp,
            serializedTechInstances = Base.TechGroup.Serialize(TechInstances),
            Equipment = EquippableSaves(),
            masteryPoints = MasteryPoints,
        };

        return save;
    }

    public Character(CharacterBase characterBase)
    {
        Base = characterBase;
        Weapon = ItemInstanceBuilder.BuildInstance(Base.Weapon, true) as Equippable;
        Head = ItemInstanceBuilder.BuildInstance(Base.Head, true) as Equippable;
        Body = ItemInstanceBuilder.BuildInstance(Base.Body, true) as Equippable;
        Hand = ItemInstanceBuilder.BuildInstance(Base.Hand, true) as Equippable;
        Feet = ItemInstanceBuilder.BuildInstance(Base.Feet, true) as Equippable;
        Accessory = ItemInstanceBuilder.BuildInstance(Base.Accessory, true) as Equippable;

        TechInstances = Base.TechGroup.Initialize();
        
        Regenerate();

        CurrentHp = Stats.MaxHp;
    }

    public Character(CharacterSave save)
    {
        try
        {
            var baseExists = GameSettings.Instance.CharacterDatabase.Content.TryGetValue(save.baseUid, out var characterBase);
            if (!baseExists)
            {
                Debug.LogError("Missing character base");
                throw new Exception();
            }

            Base = characterBase;
            TechInstances = Base.TechGroup.Deserialize(save.serializedTechInstances);

            Weapon = ItemInstanceBuilder.BuildInstance(save.Equipment[0]) as Equippable;
            Head = ItemInstanceBuilder.BuildInstance(save.Equipment[1]) as Equippable;
            Body = ItemInstanceBuilder.BuildInstance(save.Equipment[2]) as Equippable;
            Hand = ItemInstanceBuilder.BuildInstance(save.Equipment[3]) as Equippable;
            Feet = ItemInstanceBuilder.BuildInstance(save.Equipment[4]) as Equippable;
            Accessory = ItemInstanceBuilder.BuildInstance(save.Equipment[5]) as Equippable;

            Regenerate();

            currentHp = save.currentHp;
            MasteryPoints = save.masteryPoints;
        }
        catch (Exception e)
        {
            throw new DeserializationFailureException(typeof(Character));
        }
    }

    #endregion

    #region Stats

    public int CurrentLevel => PlayerController.Instance.PartyLevel;
    public int MasteryPoints;

    [FoldoutGroup("Stats"), ShowInInspector, Sirenix.OdinInspector.ReadOnly]
    private int currentHp;

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

    [FoldoutGroup("Stats"), Sirenix.OdinInspector.ReadOnly]
    public Stats BaseStats;

    [FoldoutGroup("Stats"), Sirenix.OdinInspector.ReadOnly]
    public Stats BonusStats;

    [FoldoutGroup("Stats"), Sirenix.OdinInspector.ReadOnly]
    public Stats Stats;

    [FoldoutGroup("Equips")] public Equippable Weapon;
    [FoldoutGroup("Equips")] public Equippable Head;
    [FoldoutGroup("Equips")] public Equippable Body;
    [FoldoutGroup("Equips")] public Equippable Hand;
    [FoldoutGroup("Equips")] public Equippable Feet;
    [FoldoutGroup("Equips")] public Equippable Accessory;

    [ShowInInspector] public List<PlayerSkill> Skills { get; private set; }
    [ShowInInspector] public List<PlayerSkill> EquippedSkills { get; private set; }

    [ShowInInspector] public List<Passive> Passives { get; private set; }
    [ShowInInspector] public List<Passive> EquippedPassives { get; set; }

    public void ValidateEquippedSkillsAndPassives()
    {
        var invalidSkills = EquippedSkills.Except(Skills);
        var invalidPassives = EquippedPassives.Except(Passives);
        invalidSkills.EachDo(EquippedSkills.Remove);
        invalidPassives.EachDo(EquippedPassives.Remove);

        if (EquippedSkills.Count > 5)
            EquippedSkills = EquippedSkills.GetRange(0, 5);
        if (EquippedPassives.Count > 5)
            EquippedPassives = EquippedPassives.GetRange(0, 5);
    }
    
    public TechInstance[] TechInstances;
    
    [ShowInInspector] public Tech[] LearnedTechs => TechInstances
        .Where(tI => tI.Acquired)
        .Select(tI => tI.Tech)
        .ToArray();
    
    public IEnumerable<Equippable> Equipment
    {
        get
        {
            return new Equippable[] {Weapon, Head, Body, Hand, Feet, Accessory}.Where(equippable =>
                equippable != null);
        }
    }

    private EquippableSave[] EquippableSaves()
    {
        return new Equippable[] {Weapon, Head, Body, Hand, Feet, Accessory}.Select(equipment =>
        {
            if (equipment == null)
                return new EquippableSave
                {
                    baseUid = -1
                };

            return equipment.Serialize();
        }).Cast<EquippableSave>().ToArray();
    }

    #endregion

    #region Updating

    public void LevelUp()
    {
        MasteryPoints += (CurrentLevel / 5) + 1;
        Regenerate();
    }

    public void Regenerate()
    {
        //Fazer validações em tudo, eg. ver se tudo que tá equipado é valido, masteries tem nivel valido (>= 0, <= max)
        var stopwatch = Stopwatch.StartNew();

        LoadSkills();
        LoadPassives();
        InitializeBases();
        ApplyTechs();
        InitializeBonus();

        Stats = BaseStats + BonusStats;

        stopwatch.Stop();
        Debug.Log($"{Base.CharacterName} regenerado em {stopwatch.ElapsedMilliseconds}ms");
    }

    private void InitializeBases()
    {
        BaseStats = Base.Bases + (Base.Growths * CurrentLevel);
    }

    private void InitializeBonus()
    {
        BonusStats = new Stats();

        foreach (var equipInstance in Equipment)
        {
            BonusStats += equipInstance.GetStats;
        }

        var bonusStatPassives = Passives
            .SelectMany(p => p.Effects)
            .Where(pE => pE is ICharacterCalculateBonusStatsListener)
            .Cast<ICharacterCalculateBonusStatsListener>();

        foreach (var bonusStats in bonusStatPassives)
        {
            bonusStats.Apply(this,ref BonusStats);
        }
    }

    private void LoadSkills()
    {
        Skills = new List<PlayerSkill>();
        Skills.AddRange(Base.BaseSkills);

        foreach (var equippable in Equipment)
        {
            Skills.AddRange(equippable.GetSkills);
        }
    }

    private void LoadPassives()
    {
        Passives = new List<Passive>();
        Passives.AddRange(Base.BasePassives);

        foreach (var equippable in Equipment)
        {
            Passives.AddRange(equippable.GetPassives);
        }
    }

    private void ApplyTechs()
    {
        foreach (var tech in LearnedTechs)
        {
            tech.Apply(this);
        }
    }

    public void Equip(Equippable equippable)
    {
        if (equippable.EquippableBase == null)
            throw new NullReferenceException();

        var slot = equippable.EquippableBase.Slot;

        if (!CanEquip(equippable.EquippableBase))
        {
            Debug.LogError($"{Base.CharacterName} não pode equipar {equippable.EquippableBase.itemName}");
            return;
        }

        switch (slot)
        {
            case EquippableBase.EquippableSlot.Accessory:
            {
                var old = Unequip(EquippableBase.EquippableSlot.Accessory);
                Accessory = equippable;
                OnEquip.Invoke(Accessory);
                break;
            }
            case EquippableBase.EquippableSlot.Body:
            {
                Unequip(EquippableBase.EquippableSlot.Body);
                Body = equippable;
                OnEquip.Invoke(Body);
                break;
            }
            case EquippableBase.EquippableSlot.Feet:
            {
                Unequip(EquippableBase.EquippableSlot.Feet);
                Feet = equippable;
                OnEquip.Invoke(Feet);
                break;
            }
            case EquippableBase.EquippableSlot.Hand:
            {
                Unequip(EquippableBase.EquippableSlot.Hand);
                Hand = equippable;
                OnEquip.Invoke(Hand);
                break;
            }
            case EquippableBase.EquippableSlot.Head:
            {
                Unequip(EquippableBase.EquippableSlot.Head);
                Head = equippable;
                OnEquip.Invoke(Head);
                break;
            }
            case EquippableBase.EquippableSlot.Weapon:
            {
                Unequip(EquippableBase.EquippableSlot.Weapon);
                Weapon = equippable;
                OnEquip.Invoke(Weapon);
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }

        PlayerController.Instance.Inventory.Remove(equippable);

        Regenerate();
    }

    public Equippable Unequip(EquippableBase.EquippableSlot slot)
    {
        Equippable old;

        switch (slot)
        {
            case EquippableBase.EquippableSlot.Weapon:
                old = Weapon;
                Weapon = null;
                break;
            case EquippableBase.EquippableSlot.Head:
                old = Head;
                Head = null;
                break;
            case EquippableBase.EquippableSlot.Body:
                old = Body;
                Body = null;
                break;
            case EquippableBase.EquippableSlot.Hand:
                old = Hand;
                Hand = null;
                break;
            case EquippableBase.EquippableSlot.Feet:
                old = Feet;
                Feet = null;
                break;
            case EquippableBase.EquippableSlot.Accessory:
                old = Accessory;
                Accessory = null;
                break;
            default:
                throw new ArgumentException();
        }

        if (old != null)
        {
            PlayerController.Instance.Inventory.Add(old);
            OnUnequip.Invoke(old);
            Regenerate();
        }

        return old;
    }

    public WeaponBase.WeaponType? EquippedWeaponType => (Weapon?.EquippableBase as WeaponBase)?.weaponType;
    
    public bool CanEquip(EquippableBase equippable)
    {
        if (equippable is WeaponBase weaponBase)
        {
            return Base.WeaponTypes.Contains(weaponBase.weaponType);
        }
        else if (equippable is BodyBase bodyBase)
        {
            return Base.ArmorTypes.Contains(bodyBase.ArmorType);
        }
        else
            return true;
    }

    public bool CanEquip(Equippable equippable) => CanEquip(equippable.EquippableBase);

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

[Serializable] public class EquipEvent : UnityEvent<Equippable>
{
    public EquipEvent()
    {
    }
}

[Serializable] public class CharacterEvent : UnityEvent<Character>
{
}

public interface ICharacterCalculateBonusStatsListener
{
    void Apply(Character character, ref Stats bonusStats);
}
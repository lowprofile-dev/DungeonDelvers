using System;
using Sirenix.OdinInspector;


[Serializable] public struct Stats
{
    public int MaxHp;
    public int InitialEp;
    public int EpGain;
    public int PhysAtk;
    public int MagAtk;
    public int PhysDef;
    public int MagDef;
    public int Speed;
    [PropertyRange(0, 1)] public float Accuracy;
    [PropertyRange(-1, 1)] public float Evasion;
    [PropertyRange(0, 1)] public float CritChance;
    [PropertyRange(-1, 1)] public float CritAvoid;
    public ElementalResistance ElementalResistance;

    public static Stats operator +(Stats a, Stats b)
    {
        return new Stats()
        {
            MaxHp = a.MaxHp + b.MaxHp,
            InitialEp = a.InitialEp + b.InitialEp,
            EpGain = a.EpGain + b.EpGain,
            PhysAtk = a.PhysAtk + b.PhysAtk,
            MagAtk = a.MagAtk + b.MagAtk,
            PhysDef = a.PhysDef + b.PhysDef,
            MagDef = a.MagDef + b.MagDef,
            Speed = a.Speed + b.Speed,
            Accuracy = a.Accuracy + b.Accuracy,
            Evasion = a.Evasion + b.Evasion,
            CritChance = a.CritChance + b.CritChance,
            CritAvoid = a.CritAvoid + b.CritAvoid,
            ElementalResistance = a.ElementalResistance + b.ElementalResistance
        };
    }

    public static Stats operator -(Stats a, Stats b)
    {
        return new Stats()
        {
            MaxHp = a.MaxHp - b.MaxHp,
            InitialEp = a.InitialEp - b.InitialEp,
            EpGain = a.EpGain - b.EpGain,
            PhysAtk = a.PhysAtk - b.PhysAtk,
            MagAtk = a.MagAtk - b.MagAtk,
            PhysDef = a.PhysDef - b.PhysDef,
            MagDef = a.MagDef - b.MagDef,
            Speed = a.Speed - b.Speed,
            Accuracy = a.Accuracy - b.Accuracy,
            Evasion = a.Evasion - b.Evasion,
            CritChance = a.CritChance - b.CritChance,
            CritAvoid = a.CritAvoid - b.CritAvoid,
            ElementalResistance = a.ElementalResistance - b.ElementalResistance
        };
    }

    public static Stats operator *(Stats a, Stats b)
    {
        return new Stats()
        {
            MaxHp = a.MaxHp * b.MaxHp,
            InitialEp = a.InitialEp * b.InitialEp,
            EpGain = a.EpGain * b.EpGain,
            PhysAtk = a.PhysAtk * b.PhysAtk,
            MagAtk = a.MagAtk * b.MagAtk,
            PhysDef = a.PhysDef * b.PhysDef,
            MagDef = a.MagDef * b.MagDef,
            Speed = a.Speed * b.Speed,
            Accuracy = a.Accuracy * b.Accuracy,
            Evasion = a.Evasion * b.Evasion,
            CritChance = a.CritChance * b.CritChance,
            CritAvoid = a.CritAvoid * b.CritAvoid,
            ElementalResistance = a.ElementalResistance * b.ElementalResistance
        };
    }

    public static Stats operator *(Stats a, int b)
    {
        return new Stats()
        {
            MaxHp = a.MaxHp * b,
            InitialEp = a.InitialEp * b,
            EpGain = a.EpGain * b,
            PhysAtk = a.PhysAtk * b,
            MagAtk = a.MagAtk * b,
            PhysDef = a.PhysDef * b,
            MagDef = a.MagDef * b,
            Speed = a.Speed * b,
            Accuracy = a.Accuracy * b,
            Evasion = a.Evasion * b,
            CritChance = a.CritChance * b,
            CritAvoid = a.CritAvoid * b,
            ElementalResistance = a.ElementalResistance * b
        };
    }
}

[Serializable] public struct ElementalResistance
{
    [PropertyRange(-1, 1)] public float EarthResistance;
    [PropertyRange(-1, 1)] public float FireResistance;
    [PropertyRange(-1, 1)] public float HolyResistance;
    [PropertyRange(-1, 1)] public float DarkResistance;
    [PropertyRange(-1, 1)] public float IceResistance;
    [PropertyRange(-1, 1)] public float LightningResistance;
    [PropertyRange(-1, 1)] public float WaterResistance;
    [PropertyRange(-1, 1)] public float WindResistance;

    public float this[Element element]
    {
        get
        {
            switch (element)
            {
                case Element.None:
                    return 1f;
                case Element.Earth:
                    return 1 - EarthResistance;
                case Element.Fire:
                    return 1 - FireResistance;
                case Element.Holy:
                    return 1 - HolyResistance;
                case Element.Dark:
                    return 1 - DarkResistance;
                case Element.Ice:
                    return 1 - IceResistance;
                case Element.Lightning:
                    return 1 - LightningResistance;
                case Element.Water:
                    return 1 - WaterResistance;
                case Element.Wind:
                    return 1 - WindResistance;
                default:
                    throw new ArgumentOutOfRangeException(nameof(element), element, null);
            }
        }
    }

    public static ElementalResistance operator +(ElementalResistance a, ElementalResistance b)
    {
        return new ElementalResistance
        {
            EarthResistance = a.EarthResistance + b.EarthResistance,
            FireResistance = a.FireResistance + b.FireResistance,
            HolyResistance = a.HolyResistance + b.HolyResistance,
            DarkResistance = a.DarkResistance + b.DarkResistance,
            IceResistance = a.IceResistance + b.IceResistance,
            LightningResistance = a.LightningResistance + b.LightningResistance,
            WaterResistance = a.WaterResistance + b.WaterResistance,
            WindResistance = a.WaterResistance + b.WaterResistance
        };
    }

    public static ElementalResistance operator -(ElementalResistance a, ElementalResistance b)
    {
        return new ElementalResistance
        {
            EarthResistance = a.EarthResistance - b.EarthResistance,
            FireResistance = a.FireResistance - b.FireResistance,
            HolyResistance = a.HolyResistance - b.HolyResistance,
            DarkResistance = a.DarkResistance - b.DarkResistance,
            IceResistance = a.IceResistance - b.IceResistance,
            LightningResistance = a.LightningResistance - b.LightningResistance,
            WaterResistance = a.WaterResistance - b.WaterResistance,
            WindResistance = a.WaterResistance - b.WaterResistance
        };
    }

    public static ElementalResistance operator *(ElementalResistance a, ElementalResistance b)
    {
        return new ElementalResistance
        {
            EarthResistance = a.EarthResistance * b.EarthResistance,
            FireResistance = a.FireResistance * b.FireResistance,
            HolyResistance = a.HolyResistance * b.HolyResistance,
            DarkResistance = a.DarkResistance * b.DarkResistance,
            IceResistance = a.IceResistance * b.IceResistance,
            LightningResistance = a.LightningResistance * b.LightningResistance,
            WaterResistance = a.WaterResistance * b.WaterResistance,
            WindResistance = a.WaterResistance * b.WaterResistance
        };
    }

    public static ElementalResistance operator /(ElementalResistance a, ElementalResistance b)
    {
        return new ElementalResistance
        {
            EarthResistance = a.EarthResistance / b.EarthResistance,
            FireResistance = a.FireResistance / b.FireResistance,
            HolyResistance = a.HolyResistance / b.HolyResistance,
            DarkResistance = a.DarkResistance / b.DarkResistance,
            IceResistance = a.IceResistance / b.IceResistance,
            LightningResistance = a.LightningResistance / b.LightningResistance,
            WaterResistance = a.WaterResistance / b.WaterResistance,
            WindResistance = a.WaterResistance / b.WaterResistance
        };
    }

    public static ElementalResistance operator *(ElementalResistance a, float b)
    {
        return new ElementalResistance
        {
            EarthResistance = a.EarthResistance * b,
            FireResistance = a.FireResistance * b,
            HolyResistance = a.HolyResistance * b,
            DarkResistance = a.DarkResistance * b,
            IceResistance = a.IceResistance * b,
            LightningResistance = a.LightningResistance * b,
            WaterResistance = a.WaterResistance * b,
            WindResistance = a.WaterResistance * b
        };
    }
}
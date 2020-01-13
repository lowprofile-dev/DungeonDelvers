using System;
using Sirenix.OdinInspector;

[Serializable]
public struct Stats
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
    [PropertyRange(0, 1)] public float Evasion;
    [PropertyRange(0, 1)] public float CritChance;
    [PropertyRange(0, 1)] public float CritAvoid;

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
            CritAvoid = a.CritAvoid + b.CritAvoid
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
            CritAvoid = a.CritAvoid - b.CritAvoid
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
            CritAvoid = a.CritAvoid * b.CritAvoid
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
            CritAvoid = a.CritAvoid * b
        };
    }
}

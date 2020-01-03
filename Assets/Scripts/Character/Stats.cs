using System;

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
    public float Accuracy;
    public float Evasion;
    public float CritChance;
    public float CritAvoid;

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
}

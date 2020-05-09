﻿using System;
using Sirenix.OdinInspector;
using SkredUtils;
using UnityEngine;
using Random = UnityEngine.Random;


[Serializable] public struct Stats
{
    public int MaxHp;
    public int PhysAtk;
    public int MagAtk;
    public int PhysDef;
    public int MagDef;
    public int Speed;
    [PropertyRange(-1, 1)] public double Accuracy;
    [PropertyRange(-1, 1)] public double Evasion;
    [PropertyRange(-1, 1)] public double CritChance;
    [PropertyRange(-1, 1)] public double CritAvoid;
    public ElementalResistance ElementalResistance;

    public static Stats operator +(Stats a, Stats b)
    {
        return new Stats()
        {
            MaxHp = a.MaxHp + b.MaxHp,
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

[Serializable] public struct StatsRange
{
    public Vector2Int MaxHp;
    public Vector2Int PhysAtk;
    public Vector2Int MagAtk;
    public Vector2Int PhysDef;
    public Vector2Int MagDef;
    public Vector2Int Speed;
    [Space]
    public Vector2 Accuracy;
    public Vector2 Evasion;
    public Vector2 CritChance;
    public Vector2 CritAvoid;
    public ElementalResistanceRange ElementalResistanceRange;

    public Stats Roll()
    {
        return new Stats
        {
            MaxHp = Random.Range(MaxHp.x, MaxHp.y),
            PhysAtk = Random.Range(PhysAtk.x, PhysAtk.y),
            MagAtk = Random.Range(MagAtk.x, MagAtk.y),
            PhysDef = Random.Range(PhysDef.x, PhysDef.y),
            MagDef = Random.Range(MagDef.x, MagDef.y),
            Speed = Random.Range(Speed.x, Speed.y),
            Accuracy = Random.Range(Accuracy.x, Accuracy.y),
            Evasion = Random.Range(Evasion.x, Evasion.y),
            CritChance = Random.Range(CritChance.x, CritChance.y),
            CritAvoid = Random.Range(CritAvoid.x, CritAvoid.y),
            ElementalResistance = ElementalResistanceRange.Roll()
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

[Serializable] public struct ElementalResistanceRange
{
    public Vector2 EarthResistance;
    public Vector2 FireResistance;
    public Vector2 HolyResistance;
    public Vector2 DarkResistance;
    public Vector2 IceResistance;
    public Vector2 LightningResistance;
    public Vector2 WaterResistance;
    public Vector2 WindResistance;

    public ElementalResistance Roll()
    {
        return new ElementalResistance
        {
            EarthResistance = Random.Range(EarthResistance.x, EarthResistance.y),
            FireResistance = Random.Range(FireResistance.x, FireResistance.y),
            HolyResistance = Random.Range(HolyResistance.x, HolyResistance.y),
            DarkResistance = Random.Range(DarkResistance.x, DarkResistance.y),
            IceResistance = Random.Range(IceResistance.x, IceResistance.y),
            LightningResistance = Random.Range(LightningResistance.x, LightningResistance.y),
            WaterResistance = Random.Range(WaterResistance.x, WaterResistance.y),
            WindResistance = Random.Range(WindResistance.x, WindResistance.y)
        };
    }
}
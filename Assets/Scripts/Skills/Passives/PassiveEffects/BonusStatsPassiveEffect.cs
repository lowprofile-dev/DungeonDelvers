using System;
using Sirenix.OdinInspector;

public class BonusStatsPassiveEffect : PassiveEffect, ICharacterCalculateBonusStatsListener, IMonsterCalculateBonusStatsListener
{
    public BonusStatsMode Mode = BonusStatsMode.Additive;
    [ShowIf("isAdditive")] public Stats BonusStats;
    [HideIf("isAdditive")] public StatsMultiplier StatsMultiplier;

    public BonusStatsPassiveEffect()
    {
        StatsMultiplier = new StatsMultiplier
        {
            Accuracy = 1,
            CritAvoid = 1,
            CritChance = 1,
            ElementalResistance = new ElementalResistance
            {
                FireResistance = 1,
                EarthResistance = 1,
                DarkResistance = 1,
                HolyResistance = 1,
                IceResistance = 1,
                LightningResistance = 1,
                WaterResistance = 1,
                WindResistance = 1
            },
            PhysAtk = 1,
            MaxHp = 1,
            MagAtk = 1,
            PhysDef = 1,
            Speed = 1,
            Evasion = 1,
            MagDef = 1,
        };
    }
    
    private void ApplyBonusStats(Stats bases, ref Stats stats)
    {
        switch (Mode)
        {
            case BonusStatsMode.Additive:
                stats += BonusStats;
                break;
            case BonusStatsMode.Multiplicative:
                stats += bases*StatsMultiplier.Offset;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void Apply(Character character, ref Stats bonusStats) => ApplyBonusStats(character.BaseStats, ref bonusStats);
    public void Apply(MonsterBattler monster, ref Stats stats) => ApplyBonusStats(monster.MonsterBase.Stats, ref stats);
    
    public enum BonusStatsMode { Additive, Multiplicative }

#if UNITY_EDITOR
    private bool isAdditive() => Mode == BonusStatsMode.Additive;
#endif
}

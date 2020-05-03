

public class BonusStatPassiveEffect : PassiveEffect, IStatModifierPassiveEffect
{
    public bool IsMultiplier = false;
    public Stats Stats;
    
    public void AddBonuses(Battler battler, ref Stats bonuses)
    {
        // if (IsMultiplier)
        // {
        //     var bases = battler.BaseStats;
        //     
        // }
    }
}

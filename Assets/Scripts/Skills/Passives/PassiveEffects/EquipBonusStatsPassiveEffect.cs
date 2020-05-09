using System.Collections.Generic;

public class EquipBonusStatsPassiveEffect : PassiveEffect, ICharacterCalculateBonusStatsListener
{
    public List<WeaponBase.WeaponType> Types;
    public BonusStatsPassiveEffect BonusStatsPassiveEffect;


    public void Apply(Character character, ref Stats bonusStats)
    {
        BonusStatsPassiveEffect.Apply(character,ref bonusStats);
    }
}

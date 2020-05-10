using System.Collections.Generic;

public class EquipBonusStatsPassiveEffect : PassiveEffect, ICharacterCalculateBonusStatsListener
{
    public List<WeaponBase.WeaponType> Types;
    public BonusStatsPassiveEffect BonusStatsPassiveEffect;


    public void Apply(Character character, ref Stats bonusStats)
    {
        var weaponType = (character.Weapon?.Base as WeaponBase)?.weaponType;
        if (weaponType.HasValue && Types.Contains(weaponType.Value))
            BonusStatsPassiveEffect.Apply(character,ref bonusStats);
    }
}

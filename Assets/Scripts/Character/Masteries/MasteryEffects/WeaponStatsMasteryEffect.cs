public class WeaponStatsMasteryEffect : MasteryEffect
{
    public WeaponBase.WeaponType WeaponType;
    public Stats Stats;

    public override void ApplyEffect(Character owner)
    {
        var weaponType = owner.EquippedWeaponType;
        if (weaponType.HasValue && weaponType.Value == WeaponType)
            owner.BaseStats += Stats;
    }
}
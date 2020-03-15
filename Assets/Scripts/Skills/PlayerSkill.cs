using UnityEngine;

[CreateAssetMenu(menuName = "Skill/PlayerSkill")]
public class PlayerSkill : Skill
{
    public CharacterBattler.CharacterBattlerAnimation AnimationType;
    public WeaponBase.WeaponType? RequiredWeaponType;

    public bool HasRequiredWeapon(Character character)
    {
        if (RequiredWeaponType == null)
            return true;

        var weapon = (character.Weapon?.EquippableBase as WeaponBase)?.weaponType ;

        return weapon == RequiredWeaponType;
    }
}
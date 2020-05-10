using System.Collections.Generic;
using DD.Skill.Animation;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/PlayerSkill")]
public class PlayerSkill : Skill
{
    [TabGroup("Use Animations")] public List<IPlayerSkillAnimation> Animations = new List<IPlayerSkillAnimation>(); 
    public WeaponBase.WeaponType? RequiredWeaponType;

    public bool HasRequiredWeapon(Character character)
    {
        if (RequiredWeaponType == null)
            return true;

        var weapon = (character.Weapon?.EquippableBase as WeaponBase)?.weaponType ;

        return weapon == RequiredWeaponType;
    }
}
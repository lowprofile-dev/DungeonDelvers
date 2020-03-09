using UnityEngine;

public class MageCharacterBattlerAnimator : CharacterBattlerAnimator
{
    protected override string GetStateNameFromAnimation(CharacterBattler.CharacterBattlerAnimation characterBattlerAnimation)
    {
        if (characterBattlerAnimation == CharacterBattler.CharacterBattlerAnimation.Attack || characterBattlerAnimation == CharacterBattler.CharacterBattlerAnimation.Idle)
        {
            var element = Animator.GetInteger("MageSwordElement");
            var baseCommand = base.GetStateNameFromAnimation(characterBattlerAnimation);

            switch (element)
            {
                case 0:
                    return baseCommand;
                case 1:
                    return $"{baseCommand}_Fire";
                case 2:
                    return $"{baseCommand})_Water";
                case 3:
                    return $"{baseCommand}_Earth";
            }
        }
    
        return base.GetStateNameFromAnimation(characterBattlerAnimation);
    }
}

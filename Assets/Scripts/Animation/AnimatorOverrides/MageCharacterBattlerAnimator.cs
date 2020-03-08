using UnityEngine;

public class MageCharacterBattlerAnimator : CharacterBattlerAnimator
{
    protected override string GetStateNameFromAnimation(CharacterBattler.CharacterBattlerAnimation characterBattlerAnimation)
    {
        if (characterBattlerAnimation == CharacterBattler.CharacterBattlerAnimation.Attack)
        {
            var hasElement = AnimatorValues.TryGetValue("MageSwordElement", out var element);

            if (!hasElement)
                element = 0;
            
            return $"Attack_{element}";
        }

        return base.GetStateNameFromAnimation(characterBattlerAnimation);
    }
}

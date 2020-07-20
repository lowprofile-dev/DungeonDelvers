public class ChangeAggroPassiveEffect : PassiveEffect, IOnApplyPassiveEffect
{
    public int AggroModifier = 0;
    
    public void OnApply(Battler battler)
    {
        if (battler is CharacterBattler characterBattler)
        {
            characterBattler.Aggro += AggroModifier;
        }
    }

    public void OnUnapply(Battler battler)
    {
        if (battler is CharacterBattler characterBattler)
        {
            characterBattler.Aggro -= AggroModifier;
        }
    }
}

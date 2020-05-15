using UnityEngine;

public class ChanceEffect : Effect
{
    [Range(0f, 1f)] public float Chance;
    public Effect Effect;
    
    public override EffectResult ExecuteEffect(SkillInfo skillInfo)
    {
        var rng = GameController.Instance.Random.NextDouble();
        if (rng < Chance) return Effect.ExecuteEffect(skillInfo);
        return new EffectResult();
    }

    public override object Clone()
    {
        return new ChanceEffect
        {
            Chance = Chance,
            Effect = (Effect)Effect.Clone(),
            ElementOverride = ElementOverride
        };
    }
}

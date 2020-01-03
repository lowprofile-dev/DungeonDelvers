using UnityEngine;

public abstract class Effect
{
    //public abstract EffectType effectType { get; }

    public abstract EffectResult ExecuteEffect(BattleController battle, Skill effectSource, IBattler source, IBattler target);
    //Ver a necessidade de mais
//    public enum EffectType
//    {
//        Damage,
//        Heal,
//        Status
//    }
}
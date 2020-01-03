//Fazer um heal pra poção.
//Ver como vai ficar, dano fixo (ex. sempre 50), dano escalavel (o normal), % vida, set vida a 1, o que mais for.

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DamageEffect : Effect
{
    public float DamageFactor = 1.0f;

    public override EffectResult ExecuteEffect(BattleController battle, Skill effectSource, IBattler source, IBattler target)
    {
        //Ainda mais coisa a arrumar conforme for necessario.

        var damage = battle.DamageCalculation(source, target, this);

        Debug.Log($"Levando {damage} de dano");

        target.CurrentHp -= (int)damage;
        return new DamageEffectResult()
        {
            DamageDealt = (int)damage,
            Skill = effectSource,
            Source = source,
            Target = target
        };
    }

    //Vai criar uma instancia disso aqui, a isso passa por todas as passivas em ordem de prioridade
    //e vai modificando. o battle.DamageCalculation recebe um disso.
    public struct DamageCalculation
    {
        private IBattler source;
        private IBattler target;
        
        //Tudo o que precisar
        
        private float DamageMultiplier;
        public List<Effect> ExtraEffects; //Ver como parar infinite loop. Criar um execute effect separado pra quando é um efeito adicional?
    }
    
    public class DamageEffectResult : EffectResult
    {
        //tipo de dano
        public int DamageDealt;
    }
}
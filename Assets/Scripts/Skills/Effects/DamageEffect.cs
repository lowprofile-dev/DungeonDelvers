using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DamageEffect : Effect
{
    //Ver como vai ficar, dano fixo (ex. sempre 50), dano escalavel (o normal), % vida, set vida a 1, o que mais for.
    public float DamageFactor = 1.0f;

    public override EffectResult ExecuteEffect(SkillInfo skillInfo)
    {
        //Ainda mais coisa a arrumar conforme for necessario.
        //Dá pra ver se a fonte de um efeito é uma skill ou não vendo se effectSource é nulo ou não. Assim pode evitar loops infinitos
        //eg. reflete dano, só reflete se a fonte é uma skill, manda o efeito com skill sendo nulo

        var damage = (int)Mathf.Max(0,BattleController.Instance.DamageCalculation(skillInfo.Source, skillInfo.Target, this));

        Debug.Log($"Levando {damage} de dano");

        skillInfo.Target.CurrentHp -= damage;
        return new DamageEffectResult()
        {
            DamageDealt = damage,
            skillInfo = skillInfo
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

    //interface de override de dano aqui
}
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using SkredUtils;
using UnityEngine;

public interface IBattler
{
    string Name { get; }
    int CurrentHp { get; set; }
    int CurrentEp { get; set; }
    bool Fainted { get; }
    
    //Botar aqui coisas tipo passivas e tal, tipos elementais, etc. etc.
    //Remover o BattleController dos métodos. Quando a batlha inicia, parte do setup é botar uma referencia ao controller em todos os IBattlers.
    //Pois todos os métodos vão precisar do battlecontroller mesmo.
    
    Stats Stats { get; }
    //List<BattlePassive> Passives { get; }
    RectTransform RectTransform { get; }
    
    Task TurnStart();
    Task TurnEnd();
    
    Task<Turn> GetTurn();
    Task ExecuteTurn(IBattler source, Skill skill, IEnumerable<IBattler> targets);
    Task<IEnumerable<EffectResult>> ReceiveSkill(IBattler source, Skill skill);
    Task<EffectResult> ReceiveEffect(IBattler source, Skill skillSource, Effect effect);
    Task AfterSkill(IEnumerable<EffectResult> result);
}
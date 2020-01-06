using System.Collections.Generic;

public abstract class MonsterAI
{
    public abstract Turn BuildTurn(MonsterBattler battler);
    //Botar mais funções depois (escolher alvo também)
}
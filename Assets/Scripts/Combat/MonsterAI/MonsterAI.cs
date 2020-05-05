using System;
using System.Collections.Generic;

[Obsolete]
public abstract class MonsterAI
{
    public abstract Turn BuildTurn(MonsterBattler battler);
    //Botar mais funções depois (escolher alvo também)
}
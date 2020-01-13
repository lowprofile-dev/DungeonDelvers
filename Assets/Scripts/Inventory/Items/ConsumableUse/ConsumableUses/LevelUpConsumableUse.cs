using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class LevelUpConsumableUse : ConsumableUse
{
    public override IEnumerator ApplyUse()
    {
        yield return null;
        var neededExp = PlayerController.Instance.ExpToNextLevel - PlayerController.Instance.CurrentExp;

        PlayerController.Instance.GainEXP(neededExp);
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Encounter : MonoBehaviour {
    public List<MonsterBattler> Monsters;

    //Virar um range?
    public int ExpReward;
    public int GoldReward;

    //Refazer pra ser coisas genericas (tipo effect e interaction) que tem condições de aparecer (ex. item só aparece se o player não tem, ou aparece com 50% de chance, etc.)
    public List<ItemBase> ItemReward;
}
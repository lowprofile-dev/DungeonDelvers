using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class Encounter : SerializedMonoBehaviour {
    public List<MonsterBattler> Monsters;
    
    //Virar um range?
    public int ExpReward;
    public int GoldReward;

    //Refazer pra ser coisas genericas (tipo effect e interaction) que tem condições de aparecer (ex. item só aparece se o player não tem, ou aparece com 50% de chance, etc.)
    public List<ItemBase> ItemReward;
    
#if UNITY_EDITOR
    [SerializeField, OnValueChanged("_CreateMonster")] private Monster CreateMonster;
    private void _CreateMonster()
    {
        if (CreateMonster == null)
            return;

        var emptyBase = new GameObject();
        var monsterBattlerObject = Instantiate<GameObject>(emptyBase,transform);
        DestroyImmediate(emptyBase);
        monsterBattlerObject.AddComponent<RectTransform>();
        monsterBattlerObject.transform.parent = transform;

        var monsterBattler = monsterBattlerObject.AddComponent<MonsterBattler>();
        monsterBattler.MonsterBase = CreateMonster;
        monsterBattler.Encounter = this;
        monsterBattler.LoadBase();
        Monsters.Add(monsterBattler);
        CreateMonster = null;
    }
#endif
}
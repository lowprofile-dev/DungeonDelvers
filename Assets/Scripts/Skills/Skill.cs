using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Skill/Skill")]
public class Skill : SerializableAsset
{
    public string SkillName = "";
    [AssetIcon] public Sprite SkillIcon;
    public int EpCost;
    [TextArea] public string SkillDescription;
    public TargetType Target;
    public List<Effect> Effects = new List<Effect>();
    
    //Ver se necessario mais depois, eg. Raise-equivalent -> one dead ally
    public enum TargetType
    {
        Any,
        OneEnemy,
        OneAlly,
        AllEnemies,
        AllAllies,
        All
    }
//

//    
//    public enum DamageType
//    {
//        Physical,
//        Magical,
//        True //outro nome dps
//    }
//
//    //Fazer depois
//    public enum Element
//    {
//        None,
//        Fire,
//        Ice,
//        Light,
//        Darkness
//    }
}
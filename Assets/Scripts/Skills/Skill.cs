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
    public CharacterBattler.CharacterBattlerAnimation AnimationType;
    public SkillAnimation SkillAnimation = null;
    
    //Ver se necessario mais depois, eg. Raise-equivalent -> one dead ally
    public enum TargetType
    {
        Any,
        OneEnemy,
        OneAlly,
        AllEnemies,
        AllAllies,
        All,
        Self
    }
}
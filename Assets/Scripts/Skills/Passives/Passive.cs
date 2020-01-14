using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Passive")]
public class Passive : SerializableAsset
{
    [AssetIcon] public Sprite AssetIcon;
    public string PassiveName;
    [TextArea] public string PassiveDescription;
    public List<PassiveEffect> Effects = new List<PassiveEffect>();

    //Fazer classe separada para statuseffects. também tem uma lista de passiveeffects, mas eles tem também métodos para ver quando para o statuseffect,
    //e um enum se é positivo/negativo/neutro, para fazer efeitos tipo "remova todos os efeitos negativos"
    //

    //enum PassiveType
    //{
    //    Passive,
    //    StatusEffect
    //}
}
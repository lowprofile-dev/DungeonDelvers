using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

[CreateAssetMenu(menuName = "Passive")]
public class Passive : SerializableAsset
{
    [AssetIcon] public Sprite AssetIcon;
    public string PassiveName;
    [TextArea] public string PassiveDescription;
    [OnValueChanged("SetEffectOrigin")] public List<PassiveEffect> Effects = new List<PassiveEffect>();
}
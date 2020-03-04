using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

[CreateAssetMenu(menuName = "Passive")]
public class Passive : SerializableAsset, IPassiveEffectSource
{
    [AssetIcon] public Sprite AssetIcon;
    public string PassiveName;
    public string GetName => PassiveName;
    [TextArea] public string PassiveDescription;
    [OnValueChanged("SetEffectOrigin")] public List<PassiveEffect> Effects = new List<PassiveEffect>();
    public List<PassiveEffect> GetEffects => Effects;
    
    
    #if UNITY_EDITOR
    private void SetEffectOrigin()
    {
        foreach (var effect in Effects)
        {
            effect.PassiveSource = this;
        }
    }
    #endif
}
using UnityEngine;

[CreateAssetMenu(menuName = "Passive")]
public class Passive : SerializableAsset
{
    [AssetIcon] public Sprite AssetIcon;
    public string PassiveName;
    [TextArea] public string PassiveDescription;
}
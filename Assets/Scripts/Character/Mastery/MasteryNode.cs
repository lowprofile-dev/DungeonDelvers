using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using Unity.Collections;
using UnityEngine;
using XNode;

[ShowOdinSerializedPropertiesInInspector, NodeWidth(416)]
public class MasteryNode : Node, IMasteryPrerequisite, ISerializationCallbackReceiver
{
    [Sirenix.OdinInspector.ReadOnly] public int Id;
    [OnValueChanged("renameAsset")] public string MasteryName;
    public Sprite MasterySprite;
    [TextArea] public string MasteryDescription;
    [OdinSerialize] public List<MasteryEffect> MasteryEffects = new List<MasteryEffect>();
    public int MasteryMaxLevel = 1;
    public int MasteryPointCost = 1;
    [Input(ShowBackingValue.Never)] public MasteryNode Prerequisites;
    [Output(ShowBackingValue.Never)] public MasteryNode RequirementOf;

    public IMasteryPrerequisite[] GetPrerequisites() => GetInputValues("Prerequisites", new IMasteryPrerequisite[]{ });
    
    public bool PrerequisiteAchieved(Character context)
    {
        var instance = context.MasteryInstances[Id];
        return instance.Maxed;
    }

    public override object GetValue(NodePort port)
    {
        if (port.fieldName == "RequirementOf") return this;
        return null;
    }

    #region Odin Serialization

    [SerializeField, HideInInspector]
    private SerializationData serializationData;

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        UnitySerializationUtility.DeserializeUnityObject(this, ref this.serializationData);
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        UnitySerializationUtility.SerializeUnityObject(this, ref this.serializationData);
    }

    #endregion

    #if UNITY_EDITOR
    private void renameAsset()
    {
        name = MasteryName;
    }
    #endif
}

using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Character/Tech Group")]
public class MasteryGroup : SerializedScriptableObject
{
    public List<Mastery> Masteries = new List<Mastery>();

    public SerializedMasteryInstance[] Serialize(MasteryInstance[] instances) =>
        instances.Select(i => 
            new SerializedMasteryInstance
            {
                TechIndex = Masteries.IndexOf(i.Mastery),
                Acquired = i.Acquired
            }).ToArray();
    
    public MasteryInstance[] Deserialize(SerializedMasteryInstance[] serializedTechInstances) =>
        serializedTechInstances.Select(sti => new MasteryInstance
        {
            Mastery = Masteries[sti.TechIndex],
            Acquired = sti.Acquired
        }).ToArray();
    
    public MasteryInstance[] Initialize() => Masteries.Select(t => new MasteryInstance(t)).ToArray();
}
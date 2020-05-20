using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Character/Tech Group")]
public class TechGroup : SerializedScriptableObject
{
    public List<Tech> Techs = new List<Tech>();

    public SerializedTechInstance[] Serialize(TechInstance[] instances) =>
        instances.Select(i => 
            new SerializedTechInstance
            {
                TechIndex = Techs.IndexOf(i.Tech),
                Acquired = i.Acquired
            }).ToArray();


    public TechInstance[] Deserialize(SerializedTechInstance[] serializedTechInstances) =>
        serializedTechInstances.Select(sti => new TechInstance
        {
            Tech = Techs[sti.TechIndex],
            Acquired = sti.Acquired
        }).ToArray();
    
    public TechInstance[] Initialize() => Techs.Select(t => new TechInstance(t)).ToArray();
}

public class TechInstance
{
    public Tech Tech;
    public bool Acquired;

    public TechInstance()
    {
        
    }
    
    public TechInstance(Tech tech)
    {
        Tech = tech;
        Acquired = false;
    }
}

public struct SerializedTechInstance
{
    public int TechIndex;
    public bool Acquired;
}

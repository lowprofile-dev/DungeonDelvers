using System;
using System.Collections.Generic;
using System.Linq;
using MasteriesV3;
using Sirenix.OdinInspector;
using SkredUtils;
using UnityEngine;

public class MasteryGrid : MonoBehaviour
{
    [FoldoutGroup("Settings")] public GameObject ConnectorPrefab;
    [FoldoutGroup("Settings")] public GameObject MasteryPrefab;
    
    public List<Mastery> Masteries = new List<Mastery>();
    public List<MasteryConnector> Connections = new List<MasteryConnector>();
    private List<IMasteryGridSubscriber> Subscribers = new List<IMasteryGridSubscriber>();

    public RectTransform MasteryRect;
    public RectTransform ConnectorRect;

    public float GridStep = 180;
    public float minScale = 1;
    public float maxScale = 10;
    public float scaleSpeed = 1;

    public void Subscribe(IMasteryGridSubscriber subscriber) => Subscribers.Add(subscriber);
    public void Unsubscribe(IMasteryGridSubscriber subscriber) => Subscribers.Remove(subscriber);
    public void ClearSubscriptions() => Subscribers.Clear();
    
    
    private void Update()
    {
        var wheelMovement = Input.mouseScrollDelta;
        if (wheelMovement != Vector2.zero)
        {
            var oldScale = transform.localScale.x;
            var newScale = oldScale + (wheelMovement.y * scaleSpeed);
            var clampedNewScale = Mathf.Clamp(newScale, minScale, maxScale);
            
            transform.localScale = new Vector3(clampedNewScale,clampedNewScale);
        }
    }

    public void MasteryButtonClicked(Mastery mastery)
    {
        Subscribers.ForEach(s => s.OnClick(mastery));
    }

    public MasteryConnector BuildConnection(Mastery from, Mastery to)
    {
        var connectionObject = Instantiate(ConnectorPrefab, ConnectorRect);
        connectionObject.name = $"{from.MasteryName}->{to.MasteryName}";
        var connectionComponent = connectionObject.Ensure<MasteryConnector>();
        
        connectionComponent.Setup(from,to);
        return connectionComponent;
    }

    public MasteriesV3.MasteryInstance[] Initialize() => Masteries.Select(m
        => new MasteriesV3.MasteryInstance(m)).ToArray();

    public void Load(IEnumerable<MasteriesV3.MasteryInstance> instances)
    {
        foreach (var instance in instances)
        {
            var mastery = Masteries[instance.Id];
            mastery.SetStatus(instance.Status, false);
        }
    }

    public List<MasteriesV3.MasteryInstance> Save()
    {
        var instances = new List<MasteriesV3.MasteryInstance>();
        foreach (var mastery in Masteries)
        {
            mastery.ValidateStatus();
            instances.Add(mastery.Save());
        }
        return instances;
    }

    public void Apply(Character context, IEnumerable<MasteriesV3.MasteryInstance> instances)
    {
        foreach (var instance in instances.Where(i => i.Status == Mastery.MasteryStatus.Learned))
        {
            var mastery = Masteries[instance.Id];
            mastery.MasteryEffects.ForEach(mE => mE.ApplyEffect(context));
        }
    }

    [Button("Build Mastery")]
    private void BuildMastery()
    {
        var masteryObject = Instantiate(MasteryPrefab, MasteryRect);
        var masteryComponent = masteryObject.Ensure<Mastery>();
        masteryComponent.Owner = this;
        Masteries.Add(masteryComponent);
    }
    
    [Button("Validate")]
    private void ValidateAllMasteries()
    {
        Masteries.Clear();

        int counter = 0;

        foreach (Transform obj in MasteryRect)
        {
            if (obj.TryGetComponent<Mastery>(out var mastery))
            {
                mastery.Id = counter++;
                Masteries.Add(mastery);
            }
        }

        foreach (var connection in Connections)
        {
            DestroyImmediate(connection.gameObject);
        }
        
        Connections.Clear();
        
        foreach (var mastery in Masteries)
        {
            var preRequisites = mastery.MasteryPrerequisites;
            foreach (var preRequisite in preRequisites)
                Connections.Add(BuildConnection(preRequisite,mastery));    
        }
    }
}

public interface IMasteryGridSubscriber
{
    void OnClick(Mastery mastery);
}

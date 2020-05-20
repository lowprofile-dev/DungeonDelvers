using System;
using System.Collections.Generic;
using System.Linq;
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

    public void Subscribe(IMasteryGridSubscriber subscriber) => Subscribers.Add(subscriber);
    public void Unsubscribe(IMasteryGridSubscriber subscriber) => Subscribers.Remove(subscriber);
    public void ClearSubscriptions() => Subscribers.Clear();

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

    public List<MasteryInstance> Initialize() => Masteries.Select(m
        => new MasteryInstance(m)).ToList();

    public void Load(IEnumerable<MasteryInstance> instances)
    {
        Masteries.ForEach(m => m.SetStatus(Mastery.MasteryStatus.Locked,false));
        
        foreach (var instance in instances)
        {
            var mastery = Masteries[instance.Id];
            mastery.SetStatus(instance.Status, false);
        }

        foreach (var mastery in Masteries)
        {
            mastery.ValidateStatus();
        }
    }

    public List<MasteryInstance> Save()
    {
        var instances = new List<MasteryInstance>();
        foreach (var mastery in Masteries)
        {
            mastery.ValidateStatus();
            instances.Add(mastery.Save());
        }
        return instances;
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

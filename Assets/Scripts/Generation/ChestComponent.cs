using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using SkredUtils;
using UnityEngine;
using Random = UnityEngine.Random;

public class ChestComponent : SerializedMonoBehaviour
{
    //public SoundInfo SoundInfo;
    public GameObject MessageBoxPrefab;
    public List<ChestReward> Rewards = new List<ChestReward>();

    private void Awake()
    {
        if (!Rewards.Any() && MapSettings.Instance != null) Rewards.AddRange(MapSettings.Instance.MapChestRewards);
    }

    private void Reset()
    {
        MessageBoxPrefab = GameSettings.Instance.DefaultMessageBox;
    }

    public (string, SoundInfo) RollChest()
    {
        var reward = Rewards.ToArray().WeightedRandom(r => r.Weight);
        return (reward.GivePlayer(), reward.SoundInfo);
    }
}

[Serializable] public abstract class ChestReward
{
    public int Weight;
    public SoundInfo SoundInfo;
    public abstract string GivePlayer();

    public ChestReward()
    {
        Weight = 1;
        SoundInfo = GameSettings.Instance.DefaultChestSound;
    }
}

public class GoldChestReward : ChestReward
{
    public Vector2Int Range;
    
    public override string GivePlayer()
    {
        var value = Random.Range(Range.x, Range.y);
        PlayerController.Instance.CurrentGold += value;
        return $"You found <color={GameSettings.Instance.GoldTextColor.ToHex()}>{value}g</color>!";
    }
}

public class ItemChestReward : ChestReward
{
    public ItemBase Item;
    [ShowIf("itemIsConsumable")]public int Quantity;
    
    #if UNITY_EDITOR
    private bool itemIsConsumable() => Item as ConsumableBase != null;
    #endif
    
    public override string GivePlayer()
    {
        var instance = ItemInstanceBuilder.BuildInstance(Item);
        if (instance is IStackable stackable)
        {
            stackable.Quantity = Quantity;
            PlayerController.Instance.AddItemToInventory(instance);
            return $"You found {stackable.Quantity}x <color={GameSettings.Instance.DefaultItemTextColor.ToHex()}>{instance}</color>!";
        }
        else if (instance is Equippable equippable)
        {
            PlayerController.Instance.AddItemToInventory(instance);
            return $"You found {equippable.TierQualifiedName}!";
        }
        else
        {
            PlayerController.Instance.AddItemToInventory(instance);
            return $"You found <color={GameSettings.Instance.DefaultItemTextColor.ToHex()}>{instance.Base.itemName}</color>!";
        }
        
    }
}

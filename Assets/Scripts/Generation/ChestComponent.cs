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
    public List<ChestReward> Rewards;

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
    public Item Item;
    
    public override string GivePlayer()
    {
        var copy = Item.Copy();
        PlayerController.Instance.AddItemToInventory(copy);
        return $"You found <color={GameSettings.Instance.DefaultItemTextColor.ToHex()}>{copy.InspectorName}</color>!";
    }
}

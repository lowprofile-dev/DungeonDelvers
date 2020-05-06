using System;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Monster")]
public class Monster : SerializableAsset
{
#if UNITY_EDITOR
    [AssetIcon,SerializeField,HideInInspector] private Sprite _assetIcon;
    private void _updateAssetIcon()
    {
        try
        {
            _assetIcon = MonsterBattler.GetComponent<Image>().sprite;
        }
        catch
        {
            // ignored
        }
    }
#endif
    public string MonsterName;
    #if UNITY_EDITOR
    [OnValueChanged("_updateAssetIcon")]
    #endif
    public GameObject MonsterBattler;
    public int BaseLevel;
    // public int LevelVariance;
    public Stats Stats;
    public Stats StatLevelVariance;
    public SoundInfo HitSound;
    public List<MonsterSkill> Skills = new List<MonsterSkill>();
    public List<Passive> Passives = new List<Passive>();
    //public MonsterAI MonsterAi;
    public ISkillSelector SkillAi;
    public ITargetSelector TargeterAi;

    public Vector2Int goldReward;
    public List<IMonsterDrop> MonsterDrops = new List<IMonsterDrop>();

    protected virtual void Reset()
    {
        HitSound = GameSettings.Instance.DefaultMonsterHitSound;
    }

    public virtual MonsterBattler BuildBattler(int level)
    {
        var battler = new GameObject(MonsterName);
        battler.AddComponent<RectTransform>();
        var monsterBattler = battler.AddComponent<MonsterBattler>();
        monsterBattler.LoadMonsterBase(this,level);
        return monsterBattler;
    }
    
    public int RollGold()
    {
        return GameController.Instance.Random.Next(goldReward.x, goldReward.y);
    }

    public IList<Item> RollItems()
    {
        var list = new List<Item>();

        foreach (var monsterDrop in MonsterDrops)
        {
            monsterDrop.Roll(ref list);
        }
        
        return list;
    }

    public interface IMonsterDrop
    {
        void Roll(ref List<Item> items);
    }

    public struct MonsterDrop : IMonsterDrop
    {
        public ItemBase ItemBase;
        [Range(0f,1f)] public float chance;
        
        public void Roll(ref List<Item> items)
        {
            var rng = GameController.Instance.Random.NextDouble();
            if (rng <= chance)
            {
                var item = ItemInstanceBuilder.BuildInstance(ItemBase);
                items.Add(item);
            }
        }
    }

    public struct MonsterStackableDrop : IMonsterDrop
    {
        public ConsumableBase ConsumableBase;
        [Range(0f, 1f)] public float chance;
        public Vector2Int quantity;

        public void Roll(ref List<Item> items)
        {
            var rng = GameController.Instance.Random.NextDouble();
            if (rng <= chance)
            {
                var consumable = ItemInstanceBuilder.BuildInstance(ConsumableBase) as Consumable;
                var quantity = GameController.Instance.Random.Next(this.quantity.x, this.quantity.y+1);
                consumable.Quantity = quantity;
                items.Add(consumable);
            }
        }
    }
}
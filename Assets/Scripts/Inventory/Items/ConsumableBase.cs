using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/ConsumableBase", fileName = "ConsumableBase")]
public class ConsumableBase : ItemBase, IStackableBase
{
    [SerializeField] private int maxStack = 9;
    public ItemBase ItemBase => this;
    public int MaxStack => maxStack;
}

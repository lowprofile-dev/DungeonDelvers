using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/MiscItemBase", fileName = "MiscItemBase")]
public class MiscItemBase : ItemBase, IStackableBase
{
    [SerializeField] private int maxStack = 9;
    public ItemBase ItemBase => this;
    public int MaxStack => maxStack;
}

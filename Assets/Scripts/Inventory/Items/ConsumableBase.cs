using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/ConsumableBase", fileName = "ConsumableBase")]
public class ConsumableBase : ItemBase, IStackableBase
{
    [SerializeField] private int maxStack = 9;
    public ItemBase ItemBase => this;
    public int MaxStack => maxStack;
    
    //Ver ainda como fazer pra usar fora da batalha
    [ValidateInput("_skillRemovesItem", "Skill de Itens precisa remover o Item.")]
    public PlayerSkill ItemSkill;
    
#if UNITY_EDITOR
    private bool _skillRemovesItem(PlayerSkill ItemSkill)
    {
        if (ItemSkill == null)
            return true;

        return ItemSkill.Effects.Find(effect => effect is RemoveItemEffect removeItemEffect && removeItemEffect.Item == this) != null;
    }
#endif
}

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

    [ValidateInput("_validateSkill", "Skill de Itens precisa remover o Item, e ter o mesmo sprite")]
    public PlayerSkill ItemSkill;

    public List<ConsumableUse> ConsumableUses;
    
#if UNITY_EDITOR

    [Button("Fix Skill"), ShowIf("_canFixSkill")]
    private void _fixSkill()
    {
        var hasRemoveEffect = ItemSkill.Effects.Find(effect =>
                                    effect is RemoveItemEffect removeItemEffect && removeItemEffect.Item == ItemBase) 
                                    != null;

        if (!hasRemoveEffect)
        {
            var removeItemEffect = new RemoveItemEffect {Item = ItemBase};
            ItemSkill.Effects.Add(removeItemEffect);
        }

        var hasSameName = ItemSkill.SkillName == itemName;

        if (!hasSameName)
        {
            ItemSkill.SkillName = itemName;
        }

        var hasSameSprite = ItemSkill.SkillIcon == itemIcon;

        if (!hasSameSprite)
        {
            ItemSkill.SkillIcon = itemIcon;
        }
    }

    private bool _canFixSkill()
    {
        return !_validateSkill(ItemSkill);
    }
    
    private bool _validateSkill(PlayerSkill ItemSkill)
    {
        if (ItemSkill == null)
            return true;

        return ItemSkill.Effects.Find(effect =>
                   effect is RemoveItemEffect removeItemEffect && removeItemEffect.Item == this) != null
               && ItemSkill.SkillIcon == itemIcon
               && ItemSkill.SkillName == itemName;
    }
#endif
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using SkredUtils;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/ConsumableBase", fileName = "ConsumableBase")]
public class ConsumableBase : ItemBase, IStackableBase
{
    [SerializeField] private int maxStack = 9;
    public ItemBase ItemBase => this;
    public int MaxStack => maxStack;

    [ValidateInput("_validateSkill", "Skill de Itens precisa remover o Item e ter o mesmo sprite")]
    public PlayerSkill ItemSkill;

    public SoundInfo SoundInfo;
    public List<ConsumableUse> ConsumableUses;

    public bool RequiresTarget => ConsumableUses.Any(cU => cU is TargetedConsumableUse);

    public IEnumerator UseCoroutine()
    {
        GameController.Instance.SfxPlayer.PlayOneShot(SoundInfo);
        foreach (var consumableUse in ConsumableUses)
        {
            yield return consumableUse.ApplyUse();
        }
    }

    public IEnumerator TargetedUseCoroutine(Character target)
    {
        GameController.Instance.SfxPlayer.PlayOneShot(SoundInfo);
        foreach (var consumableUse in ConsumableUses)
        {
            if (consumableUse is TargetedConsumableUse targetedConsumableUse)
                targetedConsumableUse.Target = target;
            yield return consumableUse.ApplyUse();
        }
    }
    
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
               && ItemSkill.SkillIcon == itemIcon;
    }
#endif
}

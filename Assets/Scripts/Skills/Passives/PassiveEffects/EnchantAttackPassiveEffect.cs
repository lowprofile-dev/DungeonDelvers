using UnityEngine;

public class EnchantAttackPassiveEffect : PassiveEffect, DamageEffect.IDamageCalculationInfoOverride
{
    public Element Element;
    public DamageType DamageType;
    
    public void OverrideDamageCalculationInfo(ref DamageEffect.DamageCalculationInfo damageCalculationInfo)
    {
        Debug.Log($"Overriden element to {Element.ToString()}");
        damageCalculationInfo.DamageElement = Element;
        damageCalculationInfo.DamageType = DamageType;
    }
}

using System;
using System.Linq;
using UnityEngine;

public class SetAnimatorVariablePassiveEffect : PassiveEffect, IOnApplyPassiveEffect
{
    public string Name;
    public string ApplyValue;
    public string UnapplyValue;
    // public int? ApplyValue;
    // public int? UnapplyValue;
    
    // public void OnApply(Battler battler)
    // {
    //     if (string.IsNullOrWhiteSpace(Name) || !ApplyValue.HasValue)
    //         return;
    //
    //     var character = battler as CharacterBattler;
    //
    //     character.Animator.AnimatorValues[Name] = ApplyValue.Value;
    // }
    //
    // public void OnUnapply(Battler battler)
    // {
    //     if (string.IsNullOrWhiteSpace(Name) || !UnapplyValue.HasValue)
    //         return;
    //
    //     var character = battler as CharacterBattler;
    //
    //     character.Animator.AnimatorValues[Name] = UnapplyValue.Value;
    // }
    
    public void OnApply(Battler battler)
    {
        if (string.IsNullOrWhiteSpace(ApplyValue))
            return;
        
        var hasAnimator = battler.TryGetComponent<Animator>(out var animator);
        
        if (!hasAnimator)
            return;

        var parameter = animator.parameters.FirstOrDefault(p => p.name == Name);

        if (parameter != null)
        {
            var type = parameter.type;

            switch (type)
            {
                case AnimatorControllerParameterType.Float:
                {
                    var couldParseValue = float.TryParse(ApplyValue, out var value);
                    if (couldParseValue)
                    {
                        animator.SetFloat(Name, value);
                    }
                    else
                    {
                        Debug.LogError($"Could not parse {ApplyValue} as {type}.");
                    }
                    break;
                }
                case AnimatorControllerParameterType.Int:
                {
                    var couldParseValue = int.TryParse(ApplyValue, out var value);
                    if (couldParseValue)
                    {
                        animator.SetInteger(Name, value);
                    }
                    else
                    {
                        Debug.LogError($"Could not parse {ApplyValue} as {type}.");
                    }
                    break;
                }
                case AnimatorControllerParameterType.Bool:
                {
                    var couldParseValue = bool.TryParse(ApplyValue, out var value);
                    if (couldParseValue)
                    {
                        animator.SetBool(Name,value);
                    }
                    else
                    {
                        Debug.LogError($"Could not parse {ApplyValue} as {type}.");
                    }
                    break;
                }
                case AnimatorControllerParameterType.Trigger:
                    throw new NotImplementedException();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        else
        {
            Debug.LogWarning($"Parameter named {Name} not found on {battler.BattlerName}.");
        }
    }

    public void OnUnapply(Battler battler)
    {
        if (string.IsNullOrWhiteSpace(UnapplyValue))
            return;
        
        var hasAnimator = battler.TryGetComponent<Animator>(out var animator);
        
        if (!hasAnimator)
            return;

        var parameter = animator.parameters.FirstOrDefault(p => p.name == Name);

        if (parameter != null)
        {
            var type = parameter.type;

            switch (type)
            {
                case AnimatorControllerParameterType.Float:
                {
                    var couldParseValue = float.TryParse(UnapplyValue, out var value);
                    if (couldParseValue)
                    {
                        animator.SetFloat(Name, value);
                        if (battler is CharacterBattler cb)
                        {
                            cb.UpdateAnimator();
                        }
                    }
                    else
                    {
                        Debug.LogError($"Could not parse {UnapplyValue} as {type}.");
                    }
                    break;
                }
                case AnimatorControllerParameterType.Int:
                {
                    var couldParseValue = int.TryParse(UnapplyValue, out var value);
                    if (couldParseValue)
                    {
                        animator.SetInteger(Name, value);
                        if (battler is CharacterBattler cb)
                        {
                            cb.UpdateAnimator();
                        }
                    }
                    else
                    {
                        Debug.LogError($"Could not parse {UnapplyValue} as {type}.");
                    }
                    break;
                }
                case AnimatorControllerParameterType.Bool:
                {
                    var couldParseValue = bool.TryParse(UnapplyValue, out var value);
                    if (couldParseValue)
                    {
                        animator.SetBool(Name,value);
                        if (battler is CharacterBattler cb)
                        {
                            cb.UpdateAnimator();
                        }
                    }
                    else
                    {
                        Debug.LogError($"Could not parse {UnapplyValue} as {type}.");
                    }
                    break;
                }
                case AnimatorControllerParameterType.Trigger:
                    throw new NotImplementedException();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        else
        {
            Debug.LogWarning($"Parameter named {Name} not found on {battler.BattlerName}.");
        }
    }

}
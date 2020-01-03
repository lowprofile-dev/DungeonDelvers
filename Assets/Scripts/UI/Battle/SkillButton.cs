using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillButton : SerializedMonoBehaviour, ISelectHandler, IPointerEnterHandler
{
    [ReadOnly] public SkillActionMenu SkillActionMenu;
    [ShowInInspector, ReadOnly] public Skill Skill { get; private set; }
    public Image SelectedIndicatior;
    public Image SkillIcon;
    public Text SkillName;
    public Text SkillCost;
    public Button Button;

    private bool canBeUsed;
    
    [Obsolete]
    public void BuildSkillButton(Skill skill, SkillActionMenu skillActionMenu)
    {
        SkillIcon.sprite = skill.SkillIcon;
        SkillName.text = skill.SkillName;
        SkillCost.text = $"{skill.EpCost} EP";
        Skill = skill;
        SkillActionMenu = skillActionMenu;
        gameObject.SetActive(true);
        
        if (skillActionMenu.CharacterActionMenu.Battler.CurrentEp < skill.EpCost)
        {
            canBeUsed = false;
            {
                var tempColor = SkillIcon.color;
                tempColor.a = 0.5f;
                SkillIcon.color = tempColor;
            }
            {
                var tempColor = SkillName.color;
                tempColor.a = 0.5f;
                SkillName.color = tempColor;
            }
            {
                var tempColor = SkillCost.color;
                tempColor.a = 0.5f;
                SkillCost.color = tempColor;
            }
        }
        else
        {
            canBeUsed = true;
        }
    }

    public void StartTargetSkill()
    {
        if (!canBeUsed)
            return;
        
        SkillActionMenu.ChooseSkill(Skill);
    }

    public void OnSelect(BaseEventData eventData)
    {
        SkillActionMenu.SelectSkill(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}

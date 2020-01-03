using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillButton : SerializedMonoBehaviour, ISelectHandler
{
    [ReadOnly] public CharacterActionMenu CharacterActionMenu;
    [ShowInInspector, ReadOnly] public Skill Skill { get; private set; }
    public Image SelectedIndicatior;
    public Image SkillIcon;
    public Text SkillName;
    public Text SkillCost;
    public Button Button;

    private bool canBeUsed;
    
    public void BuildSkillButton(Skill skill, CharacterActionMenu characterActionMenu)
    {
        SkillIcon.sprite = skill.SkillIcon;
        SkillName.text = skill.SkillName;
        SkillCost.text = $"{skill.EpCost} EP";
        Skill = skill;
        CharacterActionMenu = characterActionMenu;
        gameObject.SetActive(true);
        
        if (characterActionMenu.Battler.CurrentEp < skill.EpCost)
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
        
        CharacterActionMenu.ChooseSkill(Skill);
    }

    public void OnSelect(BaseEventData eventData)
    {
        CharacterActionMenu.SelectSkill(this);
    }
}

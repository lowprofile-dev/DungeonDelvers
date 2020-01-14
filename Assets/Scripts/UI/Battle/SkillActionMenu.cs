using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.EventSystems;

public class SkillActionMenu : SerializedMonoBehaviour
{
    public GameObject SkillPanel;
    public CharacterActionMenu CharacterActionMenu;
    public RectTransform SkillGridContent;
    public GameObject SkillButtonPrefab;
    public SkillInfoPanel SkillInfoPanel;

    public SkillTargeter SkillTargeter;

    [ShowInInspector, ReadOnly] private SkillButton SelectedSkill;

    //Passar se é skill normal ou itemskill
    public void OpenSkillMenu(SkillMenuMode mode)
    {
        CharacterActionMenu.Panel.SetActive(false);
        SkillPanel.SetActive(true);

        switch (mode)
        {
            case SkillMenuMode.Skills:
            {
                BuildSkills();
                break;
            }
            case SkillMenuMode.Items:
            {
                BuildItems();
                break;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
            CloseSkillMenu();
    }

    public void CloseSkillMenu()
    {
        SkillPanel.SetActive(false);
        CharacterActionMenu.ShowActionMenu();
    }

    public void ResumeSkillMenu()
    {
        SkillPanel.SetActive(true);
    }
    
    private void BuildSkills()
    {
        //Cleanup
        foreach (Transform child in SkillGridContent)
        {
            Destroy(child.gameObject);
        }

        SelectedSkill = null;
        
        var skills = CharacterActionMenu.Battler.Skills.OrderBy(skill => skill.EpCost).ToArray();

        for (var i = 0; i < skills.Length; i++)
        {
            var skillButtonObject = Instantiate(SkillButtonPrefab, SkillGridContent);
            var skillButton = skillButtonObject.GetComponent<SkillButton>();
            skillButton.BuildSkillButton(skills[i], this);

            if (i == 0)
            {
                ShowSkillInfo(skills[0]);
                SelectedSkill = skillButton;
                EventSystem.current.SetSelectedGameObject(skillButton.Button.gameObject);
            }
        }
        
        
    }

    private void BuildItems()
    {
        //Cleanup
        foreach (Transform child in SkillGridContent)
        {
            Destroy(child.gameObject);
        }

        SelectedSkill = null;

        var consumables = PlayerController.Instance.Inventory
            .Where(item => item is Consumable).Cast<Consumable>()
            .Where(consumable => consumable.ConsumableBase.ItemSkill != null)
            .Select(consumable => consumable.ConsumableBase)
            .Distinct()
            .ToArray();

        for (var i = 0; i < consumables.Length; i++)
        {
            var skillButtonObject = Instantiate(SkillButtonPrefab, SkillGridContent);
            var skillButton = skillButtonObject.GetComponent<SkillButton>();
            skillButton.BuildSkillButton(consumables[i], this);
            
            if (i == 0)
            {
                ShowSkillInfo(consumables[0].ItemSkill);
                SelectedSkill = skillButton;
                EventSystem.current.SetSelectedGameObject(skillButton.Button.gameObject);
            }
        }
    }

    private void ShowSkillInfo(PlayerSkill skill)
    {
        SkillInfoPanel.BuildSkillInfo(skill);
    }
    
    public void ChooseSkill(PlayerSkill skill)
    {
        SkillPanel.SetActive(false);
        SkillTargeter.StartTarget(skill, this);
    }

    public void SelectSkill(SkillButton skillObject)
    {
        //Deselect Old
        if (SelectedSkill != null) 
            SelectedSkill.SelectedIndicatior.enabled = false;
        
        //Select new
        skillObject.SelectedIndicatior.enabled = true;
        ShowSkillInfo(skillObject.Skill);
        SelectedSkill = skillObject;
    }

    public enum SkillMenuMode
    {
        Skills,
        Items
    }
}

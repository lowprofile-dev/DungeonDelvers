using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillActionMenu : SerializedMonoBehaviour
{
    public GameObject SkillPanel;
    public CharacterActionMenu CharacterActionMenu;
    public RectTransform SkillGridContent;
    public GameObject SkillButtonPrefab;
    public SkillInfo SkillInfoPanel;

    public SkillTargeter SkillTargeter;

    [ShowInInspector, ReadOnly] private SkillButton SelectedSkill;

    //Passar se é skill normal ou itemskill
    public void OpenSkillMenu(IEnumerable<Skill> skills)
    {
        CharacterActionMenu.Panel.SetActive(false);
        SkillPanel.SetActive(true);
        BuildSkills(skills);
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
    
    private void BuildSkills(IEnumerable<Skill> Skills)
    {
        //Cleanup
        foreach (Transform child in SkillGridContent)
        {
            Destroy(child.gameObject);
        }

        SelectedSkill = null;

        //var skills = CharacterActionMenu.Battler.Skills.OrderBy(skill => skill.EpCost);
        var skills = Skills.OrderBy(skill => skill.EpCost);

        foreach (var skill in skills)
        {
            var skillButtonObject = Instantiate(SkillButtonPrefab, SkillGridContent);
            var skillButton = skillButtonObject.GetComponent<SkillButton>();
            skillButton.BuildSkillButton(skill, this);

            if (skill == skills.First())
            {
                ShowSkillInfo(skill);
                SelectedSkill = skillButton;
                EventSystem.current.SetSelectedGameObject(skillButton.Button.gameObject);
            }
        }
    }

    private void ShowSkillInfo(Skill skill)
    {
        SkillInfoPanel.BuildSkillInfo(skill);
    }
    
    public void ChooseSkill(Skill skill)
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
}

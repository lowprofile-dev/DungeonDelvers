using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillInfoPanel : MonoBehaviour
{
    public Text SkillName;
    public Image SkillIcon;
    public Text SkillCost;
    public Text SkillDescription;

    public void BuildSkillInfo(Skill skill)
    {
        SkillName.text = skill.SkillName;
        SkillIcon.sprite = skill.SkillIcon;
        SkillCost.text = skill.EpCost.ToString(); //=> Ver depois quando tiver custos de HP ou etc.
        SkillDescription.text = skill.SkillDescription;
        gameObject.SetActive(true);
    }
}

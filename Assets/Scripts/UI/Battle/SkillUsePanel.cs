using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class SkillUsePanel : SerializedMonoBehaviour
{
    public GameObject Panel;
    public Text Text;

    public async Task ShowSkillInfoDuration(Skill skill, int duration = 2000)
    {
        GameController.Instance.QueueAction(() => ShowSkillInfo(skill));
        await Task.Delay(duration);
        GameController.Instance.QueueAction(() => HideSkillInfo());
        
    }

    public void ShowSkillInfo(Skill skill)
    {
        Text.text = skill.SkillName;
        Panel.SetActive(true);
    }

    public void HideSkillInfo()
    {
        Panel.SetActive(false);
    }
}

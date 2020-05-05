using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using SkredUtils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CharacterActionMenu : SerializedMonoBehaviour
{
    public BattleCanvas BattleCanvas;
    public GameObject Panel;
    
    public Button ActionButton;
    public Button ItemButton;

    public SkillActionMenu SkillActionMenu;

    [SerializeField] private List<StatusText> StatusTexts;
    [SerializeField] private LifeColor LifeColors;
    [ShowInInspector] public CharacterBattler Battler { get; private set; }

    public void DisplayActionMenu(CharacterBattler character)
    {
        Battler = character;
        
        for (int i = 0; i < 4; i++)
        {
            var battler = BattleController.Instance.Party[i];
            var statusText = StatusTexts[i];

            statusText.Name.text = battler.Character.Base.CharacterName;

            if (battler == Battler)
            {
                statusText.Name.text = ">" + statusText.Name.text;
                statusText.Panel.enabled = true;
            }
            else
            {
                statusText.Panel.enabled = false;
            }
                

            statusText.Life.text = $"{battler.CurrentHp}/{battler.Stats.MaxHp} - {battler.CurrentAp} AP";
            statusText.Life.color = LifeToColor(battler.CurrentHp, battler.Stats.MaxHp);
        }
        
        ShowActionMenu();
    }

    public void ShowActionMenu()
    {
        Panel.SetActive(true);
        
        ActionButton.interactable = Battler.AvailableSkills.Any();
        
        var hasUsableConsumables = PlayerController.Instance.Inventory
            .Any(item =>
            {
                if (item is Consumable cons)
                {
                    return cons.ConsumableBase.ItemSkill != null;
                }
                return false;
            });
        
        ItemButton.interactable = hasUsableConsumables;
        
        BattleCanvas.BindActionArrow(Battler.RectTransform);
        EventSystem.current.SetSelectedGameObject(ActionButton.gameObject);
    }
    
    public void FinishTurn(Turn turn)
    {
        Panel.SetActive(false);
        BattleCanvas.FinishBuildingTurn(turn);
    }

    private Color LifeToColor(int current, int max)
    {
        var percentage = (float)current / max;
        
        if (current == 0)
            return LifeColors.FaintedHealth;
        if (percentage < 0.25f)
            return LifeColors.LowHealth;
        if (percentage < 0.6f)
            return LifeColors.MidHealth;
        else
            return LifeColors.HighHealth;
    }
    
    struct StatusText
    {
        public TMP_Text Name;
        public TMP_Text Life;
        public Image Panel;
    }

    struct LifeColor
    {
        public Color HighHealth;
        public Color MidHealth;
        public Color LowHealth;
        public Color FaintedHealth;
    }

    public void OpenSkillMenu()
    {
        SkillActionMenu.OpenSkillMenu(SkillActionMenu.SkillMenuMode.Skills);
    }

    public void OpenItemMenu()
    {
        SkillActionMenu.OpenSkillMenu(SkillActionMenu.SkillMenuMode.Items);
    }

    public void Defend()
    {
        var defendSkill = Battler.Character.Base.DefaultDefendSkill;
        if (defendSkill != null)
        {
            FinishTurn(new Turn
            {
                Skill = defendSkill,
                Targets = new []{Battler}
            });
        }
        else
        {
            FinishTurn(new Turn());
        }
    }

    public void Run()
    {
        BattleController.Instance.ForceEnd();
    }
}

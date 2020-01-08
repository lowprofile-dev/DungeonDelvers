using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using SkredUtils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterActionMenu : SerializedMonoBehaviour
{
    public BattleCanvas BattleCanvas;
    public GameObject Panel;
    public GameObject InitialButton;

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
                

            statusText.Life.text = $"{battler.CurrentHp}/{battler.Stats.MaxHp} - {battler.CurrentEp} EP";
            statusText.Life.color = LifeToColor(battler.CurrentHp, battler.Stats.MaxHp);
        }
        
        ShowActionMenu();
    }

    public void ShowActionMenu()
    {
        Panel.SetActive(true);
        BattleCanvas.BindActionArrow(Battler.RectTransform);
        EventSystem.current.SetSelectedGameObject(InitialButton);
    }
    
    public void FinishTurn(Turn turn)
    {
        Panel.SetActive(false);
        BattleCanvas.FinishBuildingTurn(turn);
    }

    private Color LifeToColor(int current, int max)
    {
        var percentage = current / max;
        
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
        public Text Name;
        public Text Life;
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
        SkillActionMenu.OpenSkillMenu(Battler.Skills);
    }

    public void OpenItemMenu()
    {
        var consumableSkills = PlayerController.Instance.Inventory.Where(item => item is Consumable).Cast<Consumable>()
            .Where(consumable => consumable.ConsumableBase.ItemSkill != null)
            .Select(consumable => consumable.ConsumableBase.ItemSkill)
            .Distinct();
        
        SkillActionMenu.OpenSkillMenu(consumableSkills);
    }

    public void Defend()
    {
        //Por enquanto pula o turno. Botar pra dar um buff de redução de dano.
        FinishTurn(new Turn
        {
            Skill = null,
            Targets = null
        });
    }

    public void Run()
    {
        //Fazer
    }
}

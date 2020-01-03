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

    [FoldoutGroup("Skill Panel")] public GameObject SkillPanel;
    [FoldoutGroup("Skill Panel")] public RectTransform SkillGridContent;
    [FoldoutGroup("Skill Panel")] public GameObject SkillButtonPrefab;
    [FoldoutGroup("Skill Panel")] public SkillInfo SkillInfoPanel;
    
    
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
                statusText.Name.text = ">" + statusText.Name.text;

            statusText.Life.text = $"{battler.CurrentHp}/{battler.Stats.MaxHp} - {battler.CurrentEp}";
            statusText.Life.color = LifeToColor(battler.CurrentHp, battler.Stats.MaxHp);
        }
        
        ShowActionMenu();
        
        EventSystem.current.SetSelectedGameObject(InitialButton);
    }

    public void ShowActionMenu()
    {
        Panel.SetActive(true);
        BattleCanvas.BindActionArrow(Battler.RectTransform);
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
    }

    struct LifeColor
    {
        public Color HighHealth;
        public Color MidHealth;
        public Color LowHealth;
        public Color FaintedHealth;
    }
    
    //Fazer uma state machine e uma transição com os enums.
    #region SkillMenu
    private SkillButton SelectedSkill;
    
    public void OpenSkillMenu()
    {
        Panel.SetActive(false);
        SkillPanel.SetActive(true);
        BuildSkills();
    }

    private void BuildSkills()
    {
        //Cleanup
        foreach (Transform child in SkillGridContent)
        {
            Destroy(child.gameObject);
        }
        SelectedSkill = null;
        
        //Rebuild
        var skills = Battler.Skills.OrderBy(skill => skill.EpCost);

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
    
    public void ShowSkillInfo(Skill skill)
    {
        SkillInfoPanel.BuildSkillInfo(skill);
    }

    public void ChooseSkill(Skill skill)
    {
        SkillPanel.SetActive(false);
        StartTarget(skill, () => {});
    }
    
    #endregion

    #region SkillTargeting
    private Skill Skill;
    private Action OpenPreviousMenu; //Caso venha de Skill ou Items
    private Action Cleanup;
    private bool TargetChosen;
    
    public void StartTarget(Skill skill, Action openPreviousMenu)
    {
        Skill = skill;
        TargetChosen = false;
        OpenPreviousMenu = openPreviousMenu;
        SetupTargets();
    }


    private void SetupTargets()
    {
        
        var targetGroups = new List<IEnumerable<IBattler>>();
        //Por enquanto só one enemy
        switch (Skill.Target)
        {
            case Skill.TargetType.OneEnemy:
                foreach (var enemy in BattleController.Instance.Enemies)
                {
                    if (!enemy.Fainted)
                        targetGroups.Add(new []{enemy});
                }
                break;
            case Skill.TargetType.Any:
                foreach (var battler in BattleController.Instance.Battlers)
                {
                    if (!battler.Fainted)
                        targetGroups.Add(new []{battler});
                }
                break;
        }
        
        //Ordenar esquerda -> direita
        var orderedGroups = targetGroups.OrderBy(targetGroup =>
        {
            var totalRect = new Vector3(0,0,0);

            targetGroup.ForEach(battler => totalRect += battler.RectTransform.position);

            var averageRect = totalRect / targetGroup.Count();

            return averageRect.x;
        });
                
        StartCoroutine(Targeting(orderedGroups.ToList()));
    }

    private IEnumerator Targeting(List<IEnumerable<IBattler>> Groups)
    {
        yield return null;
        var currentIndex = 0;
        BattleCanvas.CleanTargetArrows();
                    
        foreach (var battler in Groups[currentIndex])
        {
            BattleCanvas.BindTargetArrow(battler.RectTransform);
        }

        Action DisplayTargets = () =>
        {
            BattleCanvas.CleanTargetArrows();
            var names = new List<string>();

            foreach (var battler in Groups[currentIndex])
            {
                BattleCanvas.BindTargetArrow(battler.RectTransform);
                names.Add(battler.Name);
            }

            var targetName = string.Join(", ", names);
            BattleCanvas.battleInfoPanel.ShowInfo(targetName);
        };

        DisplayTargets();
        
        while (!TargetChosen)
        {
            //Handle Input
            if (Input.GetButtonDown("Horizontal"))
            {
                var hInput = (int)Input.GetAxisRaw("Horizontal");

                if (hInput != 0)
                {
                    currentIndex += hInput;
                    if (currentIndex == -1)
                        currentIndex = Groups.Count - 1;
                    if (currentIndex == Groups.Count)
                        currentIndex = 0;
                    
                    DisplayTargets();
                }
            }

            if (Input.GetButtonDown("Submit"))
            {
                ChooseTargets(Groups[currentIndex]);
            }

            yield return null;
        }
        
        //Cleanup
        BattleCanvas.battleInfoPanel.HideInfo();
    }

    private void ChooseTargets(IEnumerable<IBattler> targets)
    {
        EndTarget();
        BattleCanvas.CleanTargetArrows();
        TargetChosen = true;
        FinishTurn(new Turn
        {
            Skill = Skill,
            Targets = targets
        });
    }

    private void EndTarget()
    {
        
    }

    #endregion
    
    public void OpenItemMenu()
    {
        //Fazer
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

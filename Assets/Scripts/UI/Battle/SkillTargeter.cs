using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

public class SkillTargeter : SerializedMonoBehaviour
{
    public CharacterActionMenu CharacterActionMenu;
    [ReadOnly] public Skill Skill;
    [ReadOnly] public bool TargetChosen;
    [ReadOnly] public List<IEnumerable<IBattler>> TargetGroups;
    [ShowInInspector, ReadOnly] private BattleCanvas BattleCanvas;
    [ReadOnly] public SkillActionMenu PreviousMenu;
    private Coroutine Targeting;

    public void StartTarget(Skill skill,SkillActionMenu previousMenu)
    {
        Skill = skill;
        PreviousMenu = previousMenu;
        TargetChosen = false;
        BattleCanvas = BattleController.Instance.battleCanvas;
        SetupTargets();
        Targeting = StartCoroutine(TargetingCoroutine());
    }

    private void SetupTargets()
    {
        //Cleanup
        TargetGroups = new List<IEnumerable<IBattler>>();

        switch (Skill.Target)
        {
            case Skill.TargetType.OneEnemy:
                foreach (var enemy in BattleController.Instance.Enemies)
                {
                    if (!enemy.Fainted)
                        TargetGroups.Add(new []{enemy});
                }
                break;
            case Skill.TargetType.Any:
                foreach (var battler in BattleController.Instance.Battlers)
                {
                    if (!battler.Fainted)
                        TargetGroups.Add(new []{battler});
                }
                break;
        }
        
        //Ordernar esquerda -> direita
        var orderedGroups = TargetGroups.OrderBy(targetGroup =>
        { 
            var totalRect = new Vector3(0,0,0);

            targetGroup.ForEach(battler => totalRect += battler.RectTransform.position);

            var averageRect = totalRect / targetGroup.Count();

            return averageRect.x;
        });

        TargetGroups = orderedGroups.ToList();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            CancelTarget();
        }
    }

    private IEnumerator TargetingCoroutine()
    {
        //Wait 1 frame
        yield return null;
        var currentIndex = 0;
        BattleCanvas.CleanTargetArrows();
                    
        foreach (var battler in TargetGroups[currentIndex])
        {
            BattleCanvas.BindTargetArrow(battler.RectTransform);
        }

        Action DisplayTargets = () =>
        {
            BattleCanvas.CleanTargetArrows();
            var names = new List<string>();

            foreach (var battler in TargetGroups[currentIndex])
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
                        currentIndex = TargetGroups.Count - 1;
                    if (currentIndex == TargetGroups.Count)
                        currentIndex = 0;
                    
                    DisplayTargets();
                }
            }

            if (Input.GetButtonDown("Submit"))
            {
                ChooseTargets(TargetGroups[currentIndex]);
            }

            yield return null;
        }
        
        //Cleanup
        BattleCanvas.battleInfoPanel.HideInfo();
    }

    private void ChooseTargets(IEnumerable<IBattler> targets)
    {
        BattleCanvas.CleanTargetArrows();
        TargetChosen = true;
        CharacterActionMenu.FinishTurn(new Turn()
        {
            Skill = Skill,
            Targets = targets
        });
    }

    public void CancelTarget()
    {
        StopCoroutine(Targeting);
        PreviousMenu.ResumeSkillMenu();
        BattleCanvas.battleInfoPanel.HideInfo();
        BattleCanvas.CleanTargetArrows();
    }
}

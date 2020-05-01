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
    [ReadOnly] public List<Battler[]> TargetGroups;
    [ShowInInspector, ReadOnly] private BattleCanvas BattleCanvas;
    [ReadOnly] public SkillActionMenu PreviousMenu;
    private Coroutine Targeting;

    public void StartTarget(Skill skill,SkillActionMenu previousMenu)
    {
        gameObject.SetActive(true);
        Skill = skill;
        PreviousMenu = previousMenu;
        TargetChosen = false;
        BattleCanvas = BattleController.Instance.battleCanvas;
        SetupTargets();
        if (!TargetChosen)
            Targeting = StartCoroutine(TargetingCoroutine());
    }

    private void SetupTargets()
    {
        //Cleanup
        TargetGroups = new List<Battler[]>();

        switch (Skill.Target)
        {
            case Skill.TargetType.OneEnemy:
                BattleController.Instance.Enemies
                    .Where(enemy => !enemy.Fainted)
                    .ForEach(
                        enemy => TargetGroups.Add(new[] {enemy})
                        );
                break;
            case Skill.TargetType.Any:
                BattleController.Instance.Battlers
                    .Where(battler => !battler.Fainted)
                    .ForEach(
                        battler => TargetGroups.Add(new[] {battler})
                    );
                break;
            case Skill.TargetType.Self:
                //TargetGroups.Add(new []{CharacterActionMenu.Battler});
                ChooseTargets(new []{CharacterActionMenu.Battler});
                break;
            case Skill.TargetType.OneAlly:
                BattleController.Instance.Party
                    .Where(partyMember => !partyMember.Fainted)
                    .ForEach(partyMember => TargetGroups.Add(new[] {partyMember}));
                break;
            case Skill.TargetType.AllEnemies:
                TargetGroups.Add(BattleController.Instance.Enemies.Where(enemy => !enemy.Fainted).ToArray());
                //ChooseTargets(BattleController.Instance.Enemies.Where(enemy => !enemy.Fainted).ToArray());
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
        
        //Começar no monstro mais a direita -> o mais perto da party
//        var currentIndex = TargetGroups.FindLastIndex(group => group.Any(battler => battler is MonsterBattler));
        var currentIndex = 0;

        BattleCanvas.CleanTargetArrows();
                    
        foreach (var battler in TargetGroups[currentIndex])
        {
            BattleCanvas.BindTargetArrow(battler.RectTransform);
        }

        void DisplayTargets()
        {
            BattleCanvas.CleanTargetArrows();
            var names = new List<string>();

            foreach (var battler in TargetGroups[currentIndex])
            {
                BattleCanvas.BindTargetArrow(battler.RectTransform);
                names.Add($"Lv. {battler.Level} {battler.BattlerName}");
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
            
            //Handle click
            if (Input.GetMouseButtonDown(0))
            {
                if (TargetGroups.Count >= 2)
                {
                    var mousePosition = Input.mousePosition;

                    int index = 0;
                    float indexDistance = float.PositiveInfinity;
                
                    Func<Battler[], float> GetDistance = (group) =>
                    {
                        var groupPositions =
                            group.Select(target => target.RectTransform.position);

                        var totalPosition = new Vector3(0, 0, 0);

                        foreach (var position in groupPositions)
                        {
                            totalPosition += position;
                        }

                        totalPosition = new Vector3(totalPosition.x,totalPosition.y,0);
                    
                        var averagePosition = totalPosition / group.Length;
                        var deltaPosition = averagePosition - mousePosition;
                        var distance = deltaPosition.magnitude;

                        return distance;
                    };

                    for (int i = 0; i < TargetGroups.Count; i++)
                    {
                        var distance = GetDistance(TargetGroups[i]);

                        if (distance < indexDistance)
                        {
                            index = i;
                            indexDistance = distance;
                        }
                    }

                    if (index != currentIndex)
                    {
                        currentIndex = index;
                        DisplayTargets();
                    }
                    else
                    {
                        ChooseTargets(TargetGroups[index]);
                    }
                }
                else
                {
                    ChooseTargets(TargetGroups.First());
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                CancelTarget();
            }

            yield return null;
        }
        
        //Cleanup
        BattleCanvas.battleInfoPanel.HideInfo();
    }

    private void ChooseTargets(IEnumerable<Battler> targets)
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
        gameObject.SetActive(false);
        StopCoroutine(Targeting);
        PreviousMenu.ResumeSkillMenu();
        BattleCanvas.battleInfoPanel.HideInfo();
        BattleCanvas.CleanTargetArrows();
    }
}
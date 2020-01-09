using System.Linq;
using UnityEngine;

public class MasteryInstance
{
    public MasteryGroup MasteryGroup { get; private set; }
    public Mastery Mastery { get; private set; }

    public int CurrentLevel { get; set; } = 0;

    public MasteryInstance(Mastery mastery, MasteryGroup masteryGroup)
    {
        Mastery = mastery;
        MasteryGroup = masteryGroup;
        CurrentLevel = 0;
    }

    public bool CanLevelUp() =>
        !(Mastery.Conditions.Any(condition => !condition.Achieved(MasteryGroup)) ||
          CurrentLevel == Mastery.MasteryMaxLevel);

    public void LevelUp()
    {
        if (CurrentLevel < Mastery.MasteryMaxLevel)
        {
            CurrentLevel++;
            MasteryGroup.Character.Regenerate();
        }
        else
        {
            Debug.LogWarning($"{Mastery.MasteryName} não pode subir de nível. Atual: {CurrentLevel}, Máximo: {Mastery.MasteryMaxLevel}");
        }
    }
}
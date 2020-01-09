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
        if (CurrentLevel >= Mastery.MasteryMaxLevel)
        {
            Debug.LogWarning($"{Mastery.MasteryName} não pode subir de nível. Atual: {CurrentLevel}, Máximo: {Mastery.MasteryMaxLevel}");
        }
        else if (Mastery.MPCost > MasteryGroup.Character.CurrentMp)
        {
            Debug.LogWarning($"{MasteryGroup.Character.Base.CharacterName} não possui MP suficiente para subir de nível.");
        }
        else
        {
            MasteryGroup.Character.CurrentMp -= Mastery.MPCost;
            CurrentLevel++;
            MasteryGroup.Character.Regenerate();
        }
    }

    public void ApplyMastery()
    {
        foreach (var effect in Mastery.Effects)
        {
            effect.ApplyBonuses(CurrentLevel, MasteryGroup.Character);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MasteryButton : MonoBehaviour
{
    public TMP_Text MasteryNameText;
    public TMP_Text MasteryCostText;
    public Button Button;
    [ReadOnly] public _MasteryInstance Mastery;
    [ReadOnly] public MasteryMenu MasteryMenu;

    public void BuildMasteryButton(_MasteryInstance masteryInstance, MasteryMenu menu)
    {
        Mastery = masteryInstance;
        MasteryMenu = menu;

        MasteryNameText.text =
            $"{masteryInstance.Mastery.MasteryName} - <#c0c0c0ff>{masteryInstance.CurrentLevel}/{masteryInstance.Mastery.MasteryMaxLevel}</color>";

        MasteryCostText.text =
            $"{masteryInstance.Mastery.MPCost}MP";

        Button.onClick.AddListener(LevelUpMastery);
    }

    private void LevelUpMastery()
    {
        Mastery.LevelUp();
        MasteryMenu.RebuildMasteries();
    }
}


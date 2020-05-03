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
    [ReadOnly] public MasteryInstance Mastery;
    [ReadOnly] public MasteryMenu MasteryMenu;

    public void BuildMasteryButton(MasteryInstance masteryInstance, MasteryMenu menu)
    {
        Mastery = masteryInstance;
        MasteryMenu = menu;

        MasteryNameText.text =
            $"{masteryInstance.Node.MasteryName} - <#c0c0c0ff>{masteryInstance.Level}/{masteryInstance.Node.MasteryMaxLevel}</color>";

        MasteryCostText.text =
            $"{masteryInstance.Node.MasteryPointCost}MP";

        Button.onClick.AddListener(LevelUpMastery);
    }

    private void LevelUpMastery()
    {
        Mastery.LevelUp();
        MasteryMenu.RebuildMasteries();
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;

public class MasteryMenu : MonoBehaviour
{
    public CharacterInspector CharacterInspector;
    [ReadOnly] public Character Character;
    public RectTransform MasteryLayout;
    public GameObject MasteryButtonPrefab;
    [ReadOnly] public List<GameObject> MasteryButtons = new List<GameObject>();
    public TMP_Text CurrentMpText;

    public void BuildMasteries(Character character)
    {
        Character = character;
        gameObject.SetActive(true);

        RebuildMasteries();
    }

    public void RebuildMasteries()
    {
        CurrentMpText.text = $"{Character.CurrentMp} MP";

        MasteryButtons.ForEach(Destroy);
        MasteryButtons = new List<GameObject>();


        // var availableMasteries = Character.MasteryGroup.Masteries.Values
        //     .Where(mI => mI.CanLevelUp()).ToArray();

        var availableMasteries = Character.MasteryInstances
            .Where(instance => instance.Available).ToArray();

        var message = "Available Masteries: ";
        availableMasteries.ForEach(aM => { message += $" {aM.Node.MasteryName}({aM.Level})"; });
        Debug.Log(message);

        availableMasteries.ForEach(availableMastery =>
        {
            var masteryButtonObject = Instantiate(MasteryButtonPrefab, MasteryLayout);
            var masteryButton = masteryButtonObject.GetComponent<MasteryButton>();

            masteryButton.BuildMasteryButton(availableMastery, this);

            if (Character.CurrentMp < availableMastery.Node.MasteryPointCost)
                masteryButton.Button.interactable = false;

            MasteryButtons.Add(masteryButtonObject);
        });
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
            CloseMasteryMenu();
    }

    public void CloseMasteryMenu()
    {
        gameObject.SetActive(false);
        CharacterInspector.OpenCharacterInspector(Character);
    }

}


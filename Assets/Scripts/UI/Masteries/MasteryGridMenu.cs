using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MasteryGridMenu : MonoBehaviour//, IMasteryGridSubscriber
{
    // private Action openPrevious;
    // private MasteryGrid grid;
    // public RectTransform GridRect;
    // public ScrollRect ScrollRect;
    //
    // public GameObject MasteryInspector;
    // public TMP_Text MasteryInspectorName;
    // public TMP_Text MasteryInspectorDescription;
    // public Button MasteryInspectorButton;
    //
    // public TMP_Text MasteryPointsText;
    // public Button SaveButton;
    // public Button RevertButton;
    //
    // private int currentMasteryPoints;
    //
    // [ShowInInspector, ReadOnly] public int CurrentMasteryPoints
    // {
    //     get => currentMasteryPoints;
    //     set
    //     {
    //         currentMasteryPoints = value;
    //         MasteryPointsText.text = $"{value} MP";
    //     }
    // }
    // public Character Character;
    // private Mastery inspectedMastery;
    // [ShowInInspector] private List<MasteryInstance> initialState;
    //
    // public void Open(Character character, Action OpenPrevious = null)
    // {
    //     throw new NotImplementedException();
    //     // gameObject.SetActive(true);
    //     // Character = character;
    //     // initialState = character.MasteryInstances;
    //     // openPrevious = OpenPrevious;
    //     // var characterGrid = character.Base.MasteryGrid;
    //     // var gridObject = Instantiate(characterGrid, GridRect);
    //     // ScrollRect.content = gridObject.transform as RectTransform;
    //     // grid = gridObject.GetComponent<MasteryGrid>();
    //     // grid.Subscribe(this);
    //     // grid.Load(initialState);
    //     // CurrentMasteryPoints = character.MasteryPoints;
    //     // SetDirty(false);
    //     // CloseMasteryInspector();
    // }
    //
    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Escape)) Close();
    // }
    //
    // public void Close()
    // {
    //     Destroy(grid.gameObject);
    //     gameObject.SetActive(false);
    //     openPrevious();
    // }
    //
    // public void Revert()
    // {
    //     CurrentMasteryPoints = Character.MasteryPoints;
    //     grid.Load(initialState);
    //     SetDirty(false);
    //     CurrentMasteryPoints = Character.MasteryPoints;
    //     OpenMasteryInspector(inspectedMastery);
    // }
    //
    // private void SetDirty(bool dirty)
    // {
    //     SaveButton.interactable = dirty;
    //     RevertButton.interactable = dirty;
    // }
    //
    // public void Save()
    // {
    //     throw new NotImplementedException();
    //     // var state = grid.Save();
    //     // Character.MasteryInstances = state;
    //     // initialState = state;
    //     // SetDirty(false);
    //     // Character.Regenerate();
    // }
    //
    // public void OnClick(Mastery mastery) => OpenMasteryInspector(mastery);
    //
    // public void OpenMasteryInspector(Mastery mastery)
    // {
    //     inspectedMastery = mastery;
    //     MasteryInspectorName.text = mastery.MasteryName;
    //     MasteryInspectorDescription.text = mastery.MasteryDescription;
    //     MasteryInspectorButton.interactable =
    //         mastery.Status == Mastery.MasteryStatus.Unlocked && CurrentMasteryPoints > 0;
    //
    //     MasteryInspector.gameObject.SetActive(true);
    // }
    //
    // public void CloseMasteryInspector()
    // {
    //     MasteryInspector.gameObject.SetActive(false);
    // }
    //
    // public void LevelUpMastery()
    // {
    //     if (inspectedMastery == null) return;
    //     if (inspectedMastery.Status == Mastery.MasteryStatus.Unlocked && CurrentMasteryPoints > 0)
    //     {
    //         inspectedMastery.SetStatus(Mastery.MasteryStatus.Learned);
    //         OpenMasteryInspector(inspectedMastery);
    //         CurrentMasteryPoints -= 1;
    //         SetDirty(true);
    //     }
    //     else
    //     {
    //         Debug.Log("Não pode aprender mastery.");
    //     }
    // }
}

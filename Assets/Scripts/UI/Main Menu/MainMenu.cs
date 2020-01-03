using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public UnityEvent OnMenuClose = new UnityEvent();
    public Text PartyLevelText;
    public CharacterPanel[] CharacterPanels = new CharacterPanel[4];

    public GameObject MainPanel;
    public InventoryMenu InventoryMenu;
    public CharacterInspector CharacterInspector;
    
    private void Start()
    {
        var party = PlayerController.Instance.Party;
        for (int i = 0; i < party.Count; i++)
        {
            CharacterPanels[i].SetupCharacterPanel(party[i]);
        }

        PartyLevelText.text = $"Party Lv. {PlayerController.Instance.PartyLevel}";
    }

    public void OpenMainMenu()
    {
        MainPanel.SetActive(true);
    }

    private void Update()
    {
        if (MainPanel.activeSelf && Input.GetButtonDown("Cancel"))
        {
            CloseMenu();
        }
    }

    public void CloseMenu()
    {
        OnMenuClose.Invoke();
        Destroy(gameObject);
    }

    public void OpenInventory()
    {
        MainPanel.SetActive(false);
        InventoryMenu.OpenInventory();
    }

    public void OpenCharacterInspector(Character character)
    {
        MainPanel.SetActive(false);
        CharacterInspector.OpenCharacterInspector(character);
    }
}

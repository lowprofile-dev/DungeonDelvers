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
    public CharacterSelector CharacterSelector;

    private static MainMenu _instance;

    public static MainMenu Instance = _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    private void Start()
    {
        RebuildCharacters();
    }

    public void OpenMainMenu()
    {
        MainPanel.SetActive(true);
        RebuildCharacters();
    }

    private void RebuildCharacters()
    {
        var party = PlayerController.Instance.Party;
        for (int i = 0; i < party.Count; i++)
        {
            var index = i;
            CharacterPanels[index].SetupCharacterPanel(party[index]);
            CharacterPanels[index].SetOnClick(() => OpenCharacterInspector(party[index]));
        }

        PartyLevelText.text = $"Party Lv. {PlayerController.Instance.PartyLevel} ({PlayerController.Instance.CurrentExp}/{PlayerController.Instance.ExpToNextLevel})";
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

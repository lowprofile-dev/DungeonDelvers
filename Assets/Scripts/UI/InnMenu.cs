using System;
using SkredUtils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InnMenu : MonoBehaviour
{
    public TMP_Text InnCostText;
    public TMP_Text PlayerGoldText;
    public Button RestButton;
    private int innCost;
    public Ref<bool> Closed;
    
    public Ref<bool> Initialize(int cost)
    {
        InnCostText.text = $"Inn Cost: {cost}g";
        PlayerGoldText.text = $"{PlayerController.Instance.CurrentGold}g";
        innCost = cost;
        Closed = (false).CreateRef();
        if (PlayerController.Instance.CurrentGold < cost) RestButton.interactable = false;
        return Closed;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            CloseMenu();
    }

    public void CloseMenu()
    {
        Destroy(gameObject);
        Closed.Instance = true;
    }
    
    public void Rest()
    {
        if (PlayerController.Instance.CurrentGold < innCost) return;
        PlayerController.Instance.CurrentGold -= innCost;
        foreach (var partyMember in PlayerController.Instance.Party)
            partyMember.CurrentHp = partyMember.Stats.MaxHp;
        CloseMenu();
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopMenu : MonoBehaviour
{
    public TMP_Text GoldText;
    public GameObject ItemPrefab;
    
    private void Start()
    {
        UpdateGoldText();
    }

    private void BuildItems()
    {
        
    }

    private void UpdateGoldText()
    {
        GoldText.text = $"{PlayerController.Instance.CurrentGold}g";
    }
    
    public void CloseMenu()
    {
        Destroy(gameObject);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using SkredUtils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ForgeMenu : MonoBehaviour
{
    public RectTransform InventoryRect;
    public GameObject ItemButtonBase;

    private Ref<bool> Closed;

    public List<GameObject> Buttons;
    public Equippable Inspected;
    public TMP_Text GoldText;
    public TMP_Text ItemNameText;
    public Button EnhanceButton;
    public TMP_Text EnhanceText;
    public Button ReforgeButton;
    public TMP_Text RefogeText;
    public Image ItemImage;
    public GameObject ComparisonLayout;
    public TMP_Text ItemBeforeStats;
    public TMP_Text ItemAfterStats;
    private Dictionary<Equippable, Character> EquippedBy;

    private void Awake()
    {
        Buttons = new List<GameObject>();
        EquippedBy = new Dictionary<Equippable, Character>();
    }

    private void UpdateGoldInfo()
    {
        GoldText.text = $"{PlayerController.Instance.CurrentGold}g";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1)) CloseMenu();
    }

    public Ref<bool> Open()
    {
        RebuildItemButtons();
        Closed = (false).CreateRef();
        Inspect(null);
        return Closed;
    }

    public void CloseMenu()
    {
        Destroy(gameObject);
        Closed.Instance = true;
    }

    private void RebuildItemButtons()
    {
        UpdateGoldInfo();

        foreach (var button in Buttons) Destroy(button);
        Buttons.Clear();

        var equips = PlayerController.Instance.Inventory
            .Cast<Equippable>().ToList();

        BuildEquips(equips);
        
        var equippedEquips = PlayerController.Instance.Party
            .SelectMany(pM => 
                pM.Equipment
                    .Select<Equippable, (Character, Equippable)>(e => 
                        (pM, e))).ToArray();
        
        EquippedBy.Clear();
        BuildEquippedEquips(equippedEquips);
    }

    private void BuildEquips(IEnumerable<Equippable> equips)
    {
        foreach (var equip in equips)
        {
            var itemObject = Instantiate(ItemButtonBase, InventoryRect);
            var button = itemObject.GetComponent<InventoryItemButton>();
            button.Setup(this, equip);
            itemObject.SetActive(true);
            Buttons.Add(itemObject);
        }
    }

    private void BuildEquippedEquips((Character character, Equippable equip)[] equippedEquips)
    {
        foreach (var equipped in equippedEquips)
        {
            var itemObject = Instantiate(ItemButtonBase, InventoryRect);
            var button = itemObject.GetComponent<InventoryItemButton>();
            var text =
                $"{equipped.equip.ColoredInspectorName}\n<color=#C0C0C0><size=22>(Equipped by {equipped.character.Base.CharacterName})</color></size>";
            button.Setup(this,equipped.equip,text);
            itemObject.SetActive(true);
            Buttons.Add(itemObject);
            EquippedBy[equipped.equip] = equipped.character;
        }
    }

    public void Inspect(Equippable equippable)
    {
        Inspected = equippable;
        if (equippable == null)
        {
            ItemNameText.text = "";
            EnhanceText.text = "Enhance";
            RefogeText.text = "Reforge";
            ItemImage.gameObject.SetActive(false);
            ComparisonLayout.SetActive(false);
            return;
        }

        ItemNameText.text = equippable.TierQualifiedName;
        RefogeText.text = $"Reforge ({equippable.Base.goldValue}g)";
        ItemImage.gameObject.SetActive(true);
        ItemImage.sprite = equippable.Base.itemIcon;

        if (equippable.EnhancementCount < equippable.GetEnhancementSlots)
        {
            EnhanceButton.interactable = true;
            var enhancementCost = equippable.GetEnhancementCost;
            EnhanceText.text = $"Enhance ({enhancementCost}g)";
            if (enhancementCost > PlayerController.Instance.CurrentGold) EnhanceButton.interactable = false;
            ComparisonLayout.SetActive(true);
            ItemBeforeStats.text = equippable.StatsDescription;
            ItemAfterStats.text = equippable.EnhancedStatsDescription;
        }
        else
        {
            ComparisonLayout.SetActive(false);
            EnhanceText.text = $"Enhance (Maxed)";
            EnhanceButton.interactable = false;
        }

        if (PlayerController.Instance.CurrentGold < equippable.Base.goldValue) ReforgeButton.interactable = false;
    }

    public void Enhance()
    {
        if (Inspected == null) return;
        var cost = Inspected.GetEnhancementCost;
        if (cost > PlayerController.Instance.CurrentGold) return;
        PlayerController.Instance.CurrentGold -= cost;
        Inspected.Enhance();
        if (EquippedBy.TryGetValue(Inspected,out var character)) character.Regenerate();
        RebuildItemButtons();
        Inspect(Inspected);
    }

    public void Reforge()
    {
        if (Inspected == null) return;
        var cost = Inspected.Base.goldValue;
        if (cost > PlayerController.Instance.CurrentGold) return;
        PlayerController.Instance.CurrentGold -= cost;
        Inspected.Reforge();
        if (EquippedBy.TryGetValue(Inspected,out var character)) character.Regenerate();
        RebuildItemButtons();
        Inspect(Inspected);
    }
}

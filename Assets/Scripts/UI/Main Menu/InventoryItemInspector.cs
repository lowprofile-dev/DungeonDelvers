using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemInspector : MonoBehaviour
{
    public InventoryMenu InventoryMenu;
    public Text ItemName;
    public Image ItemImage;
    public Text ItemDescription;
    
    public void Inspect(Item item)
    {
        if (item == null)
        {
            ItemName.text = "";
            ItemImage.enabled = false;
            ItemDescription.text = "";
            return;
        }
        
        ItemName.text = item.InspectorName;
        ItemImage.enabled = true;
        ItemImage.sprite = item.Base.itemIcon;
        ItemDescription.text = item.InspectorDescription;
        
        //Adicionar botões pra coisas (tipo dropar, equipar (se for um equip), usar (se for um consumível que pode ser usado no inventário))
    }
}

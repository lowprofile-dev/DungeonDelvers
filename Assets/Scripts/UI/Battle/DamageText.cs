using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    public Text Text;

    public void SetupDamageText(string text, Color color, int fontSizeDif = 0)
    {
        Text.text = text;
        Text.color = color;
        Text.fontSize += fontSizeDif;
    }

    //Fazer resto depois
}

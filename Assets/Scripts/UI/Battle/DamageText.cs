using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    public Text Text;

    public void SetupDamageText(string text, Color color)
    {
        Text.text = text;
        Text.color = color;
    }

    //Fazer resto depois
}

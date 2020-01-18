using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    public TMP_Text VersionText;

    private void Start()
    {
        VersionText.text = $"v{Application.version}";
    }
}

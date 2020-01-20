using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainCanvas : MonoBehaviour
{
    public static MainCanvas Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);
    }

    public GameObject MainMenuObject;
    public MainMenu MainMenu;

    public GameObject BattleCanvasObject;
    public BattleCanvas BattleCanvas;

    public Image FadeImage;
}

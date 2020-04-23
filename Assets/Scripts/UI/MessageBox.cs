using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour
{
    private bool _dismissed = false;
    private string fullText;
    private Coroutine typingCoroutine;
    public bool Dismissed => _dismissed;

    [SerializeField] private TMP_Text Text;

    private void Start()
    {
        StartCoroutine(MessageBoxCoroutine());
    }

    public void SetText(string text)
    {
        
    }

    IEnumerator MessageBoxCoroutine()
    {
        //Delay para a caixa de texto não fechar imediatamente
        StartCoroutine(TypingCoroutine());
        yield return new WaitForSeconds(0.15f);

        while (!Dismissed)
        {
            if (!Input.GetButtonDown("Submit"))
            {
                if (typingCoroutine != null)
                {
                    StopCoroutine(typingCoroutine);
                    Text.text = fullText;
                }
                else
                {
                    _dismissed = true;
                }
            }
            yield return null;
        }
    }

    IEnumerator TypingCoroutine()
    {
        typingCoroutine = StartCoroutine(SkredUtils.SkredUtils.TextWriter(Text, fullText));
        yield return typingCoroutine;
        typingCoroutine = null;
    }
}

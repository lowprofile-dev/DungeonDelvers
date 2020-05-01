using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour
{
    [SerializeField] private bool _dismissed = false;
    private string fullText;
    private Coroutine typingCoroutine;
    public bool Dismissed => _dismissed;

    [SerializeField] private TMP_Text Text;

    private void Awake()
    {
        Text.text = "";
    }

    public void SetText(string text)
    {
        fullText = text;
        StartCoroutine(MessageBoxCoroutine());
    }

    IEnumerator MessageBoxCoroutine()
    {
        StartCoroutine(TypingCoroutine());
        //Delay para a caixa de texto não fechar imediatamente
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        while (!Dismissed)
        {
            if (Input.GetButtonDown("Submit"))
            {
                if (typingCoroutine != null)
                {
                    StopCoroutine(typingCoroutine);
                    typingCoroutine = null;
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
        typingCoroutine = StartCoroutine(SkredUtils.SkredUtils.TextWriter(Text, fullText,2));
        yield return typingCoroutine;
        typingCoroutine = null;
    }
}

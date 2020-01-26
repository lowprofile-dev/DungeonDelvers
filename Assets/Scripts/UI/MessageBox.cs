using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour
{
    //Adicionar funcionalidades de digitar letra por letra, interpretar variaveis (ex. -> $coisa) pra Globals.Get(coisa)
    private bool _dismissed = false;
    public bool Dismissed => _dismissed;

    public TMP_Text Text;

    private void Start()
    {
        StartCoroutine(WaitForInput());
    }

    IEnumerator WaitForInput()
    {
        //Delay para a caixa de texto não fechar imediatamente
        yield return new WaitForSeconds(0.15f);
        
        while (!Input.GetButtonDown("Submit"))
            yield return null;

        _dismissed = true;
    }
}

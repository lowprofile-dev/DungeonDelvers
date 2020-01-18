using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class testtogglepixelperfect : MonoBehaviour
{
    public PixelPerfectCamera pcc;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            pcc.enabled = !pcc.enabled;
        }
    }
}

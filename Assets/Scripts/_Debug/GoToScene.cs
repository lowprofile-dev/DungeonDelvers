using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToScene : MonoBehaviour
{
    public int SceneIndex;

    [Button("Warp")]
    public void Warp()
    {
        SceneManager.LoadScene(SceneIndex);
    }
}


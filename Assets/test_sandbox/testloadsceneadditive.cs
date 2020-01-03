using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;


class testloadsceneadditive : MonoBehaviour
{
    public int ToLoadAdditive;
    public Scene currentScene;

    private void Start()
    {
        DontDestroyOnLoad(this);
        currentScene = SceneManager.GetActiveScene();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            DoIt();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            //SceneManager.
        }
    }

    //IEnumerator Test()
    //{
    //    var tempScene
    //}

    void DoIt()
    {

    }
}


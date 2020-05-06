using Sirenix.OdinInspector;
using UnityEngine;

public class generaltest : MonoBehaviour
{
    [Button] private void test()
    {
        var rand = UnityEngine.Random.Range(0, 0);
        Debug.Log(rand);
    }
}

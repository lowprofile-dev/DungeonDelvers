using Sirenix.OdinInspector;
using UnityEngine;

public class playanim : MonoBehaviour
{
    public string animName;

    [Button] private void PlayAnim()
    {
        GetComponent<Animator>().Play(animName);
    }
}

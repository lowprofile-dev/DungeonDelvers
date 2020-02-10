using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events; 
using SkredUtils;

public class RewardPanel : MonoBehaviour
{
    public TMP_Text RewardText;
    public UnityEvent RewardPanelClosed;
    private Coroutine TypeTextCoroutine;
    private string rewardTextString;

    public void ShowRewardPanel(int expGained, int goldGained, IEnumerable<Item> itemsGained)
    {
        //Fazer depois animação de aparecer + animação de digitar.
        gameObject.SetActive(true);
        var text = $"The party gained <color=#00FFFF>{expGained} EXP</color> and <color=#FFFF00>{goldGained}g</color>.";
        if (itemsGained.Any())
            text += $"\nThey also found {string.Join(", ", itemsGained.Select(item => item.InspectorName))}";
        RewardText.text = "";

        rewardTextString = text;
        StartCoroutine(TypeText(text));
    }

    private IEnumerator TypeText(string text)
    {
        TypeTextCoroutine = StartCoroutine(SkredUtils.SkredUtils.TextWriter(RewardText, text, 10));
        yield return TypeTextCoroutine;
        TypeTextCoroutine = null;
    }
    
    private void CloseRewardPanel()
    {
        gameObject.SetActive(false);
        RewardPanelClosed.Invoke();
    }

    private void FinishTyping()
    {
        StopCoroutine(TypeTextCoroutine);
        TypeTextCoroutine = null;
        RewardText.text = rewardTextString;
    }
    
    public void Update()
    {
        if (Input.GetButtonDown("Submit") || Input.GetMouseButtonDown(0))
        {
            if (TypeTextCoroutine != null)
                FinishTyping();
            else
                CloseRewardPanel();    
        }
    }
}

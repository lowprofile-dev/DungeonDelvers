using System.Collections;
using System;
using UnityEngine;
using SkredUtils;

[InteractableNode(defaultNodeName = "Open Chest")]
public class OpenChestInteraction : Interaction
{
    [Input] public AudioSource AudioSource;
    [Input] public ChestComponent ChestComponent;

    public override IEnumerator PerformInteraction(Interactable source)
    {
        var chestComponent = GetInputValue<object>("ChestComponent", ChestComponent) as ChestComponent;
        var audioSource = GetInputValue("AudioSource", AudioSource);

        if (chestComponent == null)
        {
            Debug.LogError("No chest component found");
            yield break;
        }

        (string rewardText, SoundInfo sound) reward = chestComponent.RollChest();
        
        var tempMessageBox = CreateInstance<MessageBoxInteraction>();
        tempMessageBox.Text = reward.rewardText;
        tempMessageBox.MessageBoxPrefab = chestComponent.MessageBoxPrefab;
        tempMessageBox.SoundInfo = GameSettings.Instance.DefaultTypingSound;
        
        audioSource.PlayOneShot(reward.sound);

        yield return tempMessageBox.PerformInteraction(source);
        
        Destroy(tempMessageBox);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CharacterPanel : MonoBehaviour
{
    public RectTransform CharacterImageTransform;
    private GameObject CharacterBattler;
    public Character Character;
    public Text CharacterName;
    public Text CharacterHealth;
    public Button CharacterButton;

    public void SetupCharacterPanel(Character character)
    {
        Character = character;
        if (CharacterBattler != null)
            Destroy(CharacterBattler);

        if (character.Base.BattlerPrefab != null)
        {
            CharacterBattler = Instantiate(character.Base.BattlerPrefab, CharacterImageTransform);
            var battlerRect = CharacterBattler.transform as RectTransform;
            battlerRect.sizeDelta = new Vector2(160, 160);
            var battler = CharacterBattler.GetComponent<CharacterBattler>();
            battler.Character = character;
            var hasWeapon = character.Weapon != null;
            
//            if (character.Fainted)
//                battler.Play(global::CharacterBattler.CharacterBattlerAnimation.Fainted, true);
//            else if (hasWeapon)
//                battler.Play(global::CharacterBattler.CharacterBattlerAnimation.Idle, true);
//            else
//                battler.Play(global::CharacterBattler.CharacterBattlerAnimation.IdleNoWeapon, true);
            if (character.Fainted)
                battler.Play(global::CharacterBattler.CharacterBattlerAnimation.Fainted, true);
            else
            {
                var weaponType = (battler.Character.Weapon?.EquippableBase as WeaponBase)?.weaponType;
                battler.Animator.LoadControllerForWeapon(weaponType);
                battler.Animator.CanTransition = false;
            }
        }
        CharacterName.text = character.Base.CharacterName;
        CharacterHealth.text = $"{character.CurrentHp}/{character.Stats.MaxHp}";
    }

    public void SetOnClick(Action action)
    {
        CharacterButton.onClick.RemoveAllListeners();
        CharacterButton.onClick.AddListener(() => action());
    }
}

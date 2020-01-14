using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInspector : SerializedMonoBehaviour
{
    public MainMenu MainMenu;
    public EquipMenu EquipMenu;
    public MasteryMenu MasteryMenu;

    public Character Character;
    public RectTransform CharacterRectTransform;
    public GameObject CharacterBattler;

    public TMP_Text WeaponName;
    public TMP_Text HeadName;
    public TMP_Text BodyName;
    public TMP_Text HandName;
    public TMP_Text FeetName;
    public TMP_Text AccessoryName;

    public TMP_Text StatsText;
    public TMP_Text MpText;

    public void Update()
    {
        if (Input.GetButtonDown("Cancel"))
            CloseCharacterInspector();
    }

    public void OpenCharacterInspector(Character character)
    {
        gameObject.SetActive(true);
        Character = character;

        if (CharacterBattler != null)
            Destroy(CharacterBattler);

        if (character.Base.BattlerPrefab != null)
        {
            CharacterBattler = Instantiate(character.Base.BattlerPrefab, CharacterRectTransform);
            var battler = CharacterBattler.GetComponent<CharacterBattler>();
            
            if (character.Fainted)
                battler.Play(global::CharacterBattler.CharacterBattlerAnimation.Fainted, true);
            else
                battler.Play(global::CharacterBattler.CharacterBattlerAnimation.Idle, true);
        }

        SetEquip(WeaponName, character.Weapon);
        SetEquip(HeadName, character.Head);
        SetEquip(BodyName, character.Body);
        SetEquip(HandName, character.Hand);
        SetEquip(FeetName, character.Feet);
        SetEquip(AccessoryName, character.Accessory);

        var stats = character.Stats;

        var newText =
            $"HP: {character.CurrentHp}/{stats.MaxHp}\nInitial AP: {stats.InitialEp}\nAP Gain: {stats.EpGain}\nPhysical Attack: {stats.PhysAtk}\nMagical Attack: {stats.MagAtk}\nPhysical Defense: {stats.PhysDef}\nMagical Defense: {stats.MagDef}\nSpeed: {stats.Speed}\nAccuracy: {stats.Accuracy:F}\nEvasion: {stats.Evasion:F}\nCritical Accuracy: {stats.CritChance:F}\nCritical Evasion: {stats.CritAvoid:F}";

        StatsText.text = newText;

        MpText.text = MpText.text.Replace("%mp%", character.CurrentMp.ToString());
    }

    public void SetEquip(TMP_Text text, Equippable equippable)
    {
        if (equippable == null)
            text.text = "";
        else
            text.text = equippable.InspectorName;
    }

    public void CloseCharacterInspector()
    {
        gameObject.SetActive(false);
        MainMenu.OpenMainMenu();
    }

    public void OpenEquipMenu(string slot)
    {
        var parsed =
            EquippableBase.EquippableSlot.TryParse(slot, true, out EquippableBase.EquippableSlot parsedSlot);
        if (!parsed)
            throw new ArgumentException();
        EquipMenu.BuildEquips(Character, parsedSlot);
        gameObject.SetActive(false);
    }

    public void OpenMasteryMenu()
    {
        gameObject.SetActive(false);
        MasteryMenu.BuildMasteries(Character);
    }

    public void OpenPassiveMenu()
    {

    }
}
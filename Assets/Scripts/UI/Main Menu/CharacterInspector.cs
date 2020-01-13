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

        var newText = StatsText.text
            .Replace("%chp%", character.CurrentHp.ToString())
            .Replace("%mhp%", stats.MaxHp.ToString())
            .Replace("%iap%", stats.InitialEp.ToString())
            .Replace("%apg%", stats.InitialEp.ToString())
            .Replace("%pa%", stats.PhysAtk.ToString())
            .Replace("%pd%", stats.PhysDef.ToString())
            .Replace("%ma%", stats.MagAtk.ToString())
            .Replace("%md%", stats.MagDef.ToString())
            .Replace("%spd%", stats.Speed.ToString())
            .Replace("%acc%", stats.Accuracy.ToString("F"))
            .Replace("%eva%", stats.Evasion.ToString("F"))
            .Replace("%cacc%", stats.CritChance.ToString("F"))
            .Replace("%ceva%", stats.CritAvoid.ToString("F"));

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
        var availableMasteries = Character.MasteryGroup.Masteries.Keys.Where(key =>
        {
            return Character.MasteryGroup.Masteries[key].CanLevelUp();
        });

        var message = "Available Masteries: ";
        availableMasteries.ForEach(aM => { message += $" {aM.MasteryName}({Character.MasteryGroup.Masteries[aM].CurrentLevel})"; });
        Debug.Log(message);
    }

    public void OpenPassiveMenu()
    {

    }
}
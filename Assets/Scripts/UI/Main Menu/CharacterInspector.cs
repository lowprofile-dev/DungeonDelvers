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
    public MasteryGridMenu MasteryGridMenu;

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
            battler.Character = Character;
            
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

        //var stats = character.Stats;
        var bases = Character.BaseStats;
        var bonuses = Character.BonusStats;
        var color = "#C0C0C0";
        //<color={color}></color>
        
        //<color=#C0C0C0></color>
        var newText =
            $"HP: {character.CurrentHp}/{bases.MaxHp}<color={color}>{bonuses.MaxHp:+#;-#; ;}</color>\n" +
            $"Phys. Attack: {bases.PhysAtk}<color={color}>{bonuses.PhysAtk:+#;-#; ;}</color>\n" +
            $"Mag. Attack: {bases.MagAtk}<color={color}>{bonuses.MagAtk:+#;-#; ;}</color>\n" +
            $"Phys. Defense: {bases.PhysDef}<color={color}>{bonuses.PhysDef:+#;-#; ;}</color>\n" +
            $"Mag. Defense: {bases.MagDef}<color={color}>{bonuses.MagDef:+#;-#; ;}</color>\n" +
            $"Speed: {bases.Speed}<color={color}>{bonuses.Speed:+#;-#; ;}</color>\n" +
            $"Accuracy: {bases.Accuracy:0.#%}<color={color}>{bonuses.Accuracy:+##.#%;-##.#%; ;}</color>\n" +
            $"Evasion: {bases.Evasion:0.#%}<color={color}>{bonuses.Evasion:+##.#%;-##.#%; ;}</color>\n" +
            $"Critical Accuracy: {bases.CritChance:0.#%}<color={color}>{bonuses.CritChance:+##.#%;-##.#%; ;}</color>\n" +
            $"Critical Evasion: {bases.CritAvoid:0.#%}<color={color}>{bonuses.CritAvoid:+##.#%;-##.#%; ;}</color>";

        StatsText.text = newText;

        MpText.text = $"Mastery Points: {character.CurrentMp} unspent";
        //MpText.text = MpText.text.Replace("%mp%", character.CurrentMp.ToString());
    }

    public void SetEquip(TMP_Text text, Equippable equippable)
    {
        if (equippable == null)
            text.text = "";
        else
            text.text = equippable.ColoredInspectorName;
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
        // MasteryMenu.BuildMasteries(Character);
        MasteryGridMenu.Open(Character, () => OpenCharacterInspector(Character));
    }

    public void OpenPassiveMenu()
    {

    }
}
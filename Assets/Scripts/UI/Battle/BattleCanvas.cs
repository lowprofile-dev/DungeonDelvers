using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class BattleCanvas : SerializedMonoBehaviour
{
    public RectTransform[] PartyBattlerHolders;
    public RectTransform MonsterPanel;
    public Image Battleground;
    public CharacterActionMenu CharacterActionMenu;

    public GameObject DamagePrefab;
    public GameObject ActionArrowPrefab;
    private GameObject actionArrow;
    public GameObject TargetArrowPrefab;
    private List<GameObject> targetArrows = new List<GameObject>();
    public SkillUsePanel SkillUsePanel;
    
    [ReadOnly] public CharacterBattler currentCharacter;
    private readonly EventWaitHandle WaitHandle = new AutoResetEvent(false);
    private Turn Turn;
    private GameObject lastSelect = null;
    
    private void Start()
    {
        actionArrow = Instantiate(ActionArrowPrefab, transform);
        actionArrow.SetActive(false);
    }

    private void Update()
    {
        KeepSelect();
    }

    private void KeepSelect()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(lastSelect);
        }
        else
        {
            lastSelect = EventSystem.current.currentSelectedGameObject;
        }
    }
    
    public List<CharacterBattler> SetupParty(List<Character> characters)
    {
        var characterBattlers = new List<CharacterBattler>();

        var index = 0;
        characters.ForEach((character) =>
        {
            var parentRect = PartyBattlerHolders[index++];
            var characterBattlerObject = Instantiate(character.Base.BattlerPrefab, parentRect);
            ((RectTransform) characterBattlerObject.transform).sizeDelta = new Vector2(200,200);
            var characterBattler = characterBattlerObject.GetComponent<CharacterBattler>();
            characterBattler.Create(character);
            characterBattlers.Add(characterBattler);
        });

        return characterBattlers;
    }

    public List<MonsterBattler> SetupMonsters(GameObject encounterPrefab)
    {
        var encounterObject = Instantiate(encounterPrefab, transform);
        var encounter = encounterObject.GetComponent<Encounter>();
        return encounter.Monsters;
    }

    public void SetupBattleground(Sprite sprite)
    {
        Battleground.sprite = sprite;
        Battleground.type = Image.Type.Tiled;
    }

    public async Task<Turn> GetTurn(CharacterBattler character)
    {
        Turn = null;
        
        GameController.Instance.QueueAction(() =>
        {
            CharacterActionMenu.DisplayActionMenu(character);
            CharacterActionMenu.transform.SetAsLastSibling();
        });
        
        while (Turn == null)
        {
            await Task.Delay(50);
        }

        return Turn;
    }

    public void FinishBuildingTurn(Turn turn)
    {
        Turn = turn;
    }

    public async Task ShowDamage(IBattler battler, int damage)
    {
        GameObject damageObject = null;
        await GameController.Instance.QueueActionAndAwait(() =>
        {
            //Animação pro dano?
            damageObject = Instantiate(DamagePrefab, transform);
            damageObject.transform.position = battler.RectTransform.position + new Vector3(0, 100, 0);

            var damageText = damageObject.GetComponent<DamageText>();
            damageText.Text.text = damage.ToString();
        });

        await Task.Delay(1400);

        await GameController.Instance.QueueActionAndAwait(() =>
        {
            Destroy(damageObject);
        });
    }
    
    public void BindActionArrow(RectTransform rectTransform)
    {
        actionArrow.SetActive(true);
        actionArrow.transform.position = rectTransform.position + new Vector3(0, 100, 0);
    }

    public void UnbindActionArrow()
    {
        actionArrow.SetActive(false);
    }

    public void BindTargetArrow(RectTransform rectTransform)
    {
        var targetArrow = Instantiate(TargetArrowPrefab, transform);
        targetArrow.transform.position = rectTransform.position + new Vector3(0, 100, 0);
        targetArrows.Add(targetArrow);
    }

    public void CleanTargetArrows()
    {
        foreach (var targetArrow in targetArrows)
        {
            Destroy(targetArrow);
        }

        targetArrows = new List<GameObject>();
    }
}

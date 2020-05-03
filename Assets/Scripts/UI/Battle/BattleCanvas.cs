using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DD.Sound.IntroloopTypes;
using E7.Introloop;
using Sirenix.OdinInspector;
using SkredUtils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class BattleCanvas : AsyncMonoBehaviour
{

    private BattleBGMPlayer Player;
    public RectTransform[] PartyBattlerHolders;
    public RectTransform MonsterPanel;
    public Image Battleground;
    public CharacterActionMenu CharacterActionMenu;
    public RewardPanel RewardPanel;
    public MonsterLayoutManager MonsterLayoutManager;

    public GameObject DamagePrefab;
    public GameObject ActionArrowPrefab;
    private GameObject actionArrow;
    public GameObject TargetArrowPrefab;
    private List<GameObject> targetArrows = new List<GameObject>();
    public BattleInfoPanel battleInfoPanel;

    [ReadOnly] public CharacterBattler currentCharacter;
    private readonly EventWaitHandle WaitHandle = new AutoResetEvent(false);
    private Turn Turn;
    private GameObject lastSelect = null;

    private List<GameObject> skillResultObjects = new List<GameObject>();

    private void Awake()
    {
        Player = BattleBGMPlayer.Get;
    }

    private void Start()
    {
        actionArrow = Instantiate(ActionArrowPrefab, transform);
        actionArrow.SetActive(false);
    }

    private void Update()
    {
        KeepSelect();
    }

    private async Task BackgroundAnimation()
    {

    }

    public void StartBattleMusic(IntroloopAudio audio) => Player.Play(audio);

    public void UnpauseBattleMusic() => Player.Resume();

    public void PauseBattleMusic() => Player.Pause();

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
            ((RectTransform) characterBattlerObject.transform).sizeDelta = new Vector2(200, 200);
            var characterBattler = characterBattlerObject.GetComponent<CharacterBattler>();
            characterBattler.Create(character);
            characterBattlers.Add(characterBattler);
        });

        return characterBattlers;
    }

    public List<MonsterBattler> SetupMonsters(EncounterSet encounterSet)
    {
        var monsters = encounterSet.BuildMonsters();

        foreach (var monster in monsters)
        {
            var tf = monster.RectTransform;
            
            tf.SetParent(MonsterPanel,false);
            tf.localRotation = Quaternion.Euler(-55,0,0);
            tf.localPosition = new Vector3(0,0,0);
        }

        MonsterLayoutManager.SetMonsters(monsters);
        MonsterLayoutManager.SetLayout(encounterSet.Layout);
        MonsterLayoutManager.OrderRects();
        
        return monsters;
    }

    public List<MonsterBattler> SetupMonsters(GameObject encounterPrefab, out Encounter encounter)
    {
        var encounterObject = Instantiate(encounterPrefab, transform);
        encounter = encounterObject.GetComponent<Encounter>();
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

    public  Task ShowSkillResultAsync(Battler battler, string info, Color textColor, float duration = 1f) 
        => GameController.Instance.PlayCoroutine(ShowSkillResult(battler, info, textColor, duration));
    

    public IEnumerator ShowSkillResult(Battler battler, string info, Color textColor, float duration = 1f)
    {
        var damageObject = Instantiate(DamagePrefab, transform);
        damageObject.transform.position = battler.RectTransform.position + new Vector3(0, 100, 0);

        var damageText = damageObject.GetComponent<DamageText>();
        damageText.SetupDamageText(info,textColor);
        
        skillResultObjects.Add(damageObject);
        
        yield return new WaitForSeconds(duration);
        
        GameController.Instance.QueueAction(() =>
        {
            skillResultObjects.Remove(damageObject);
            if (damageObject != null) Destroy(damageObject);
        });
    }

    public Task ShowModifiableSkillResultAsync(Battler battler, Ref<(string text, Color color)> info,
        float duration = 1f) => GameController.Instance.PlayCoroutine(ShowModifiableSkillResult(battler, info, duration));
    
    public IEnumerator ShowModifiableSkillResult(Battler battler, Ref<(string text, Color color)> info,
        float duration = 1f)
    {
        var damageObject = Instantiate(DamagePrefab, transform);
        damageObject.transform.position = battler.RectTransform.position + new Vector3(0, 100, 0);

        var damageText = damageObject.GetComponent<DamageText>();
        var lastInfo = info.Instance;
        damageText.SetupDamageText(lastInfo.text,lastInfo.color);
        
        skillResultObjects.Add(damageObject);

        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            yield return null;
            elapsedTime += Time.deltaTime;
            if (info.Instance != lastInfo)
            {
                lastInfo = info.Instance;
                damageText.SetupDamageText(lastInfo.text,lastInfo.color);
            }
        }
        
        GameController.Instance.QueueAction(() =>
        {
            skillResultObjects.Remove(damageObject);
            if (damageObject != null) Destroy(damageObject);
        });
    }

    public void ClearSkillResults()
    {
        foreach (var skillResultObject in skillResultObjects)
        {
            Destroy(skillResultObject);
        }
        skillResultObjects.Clear();
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

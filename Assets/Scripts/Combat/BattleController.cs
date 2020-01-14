using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using SkredUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class BattleController : SerializedMonoBehaviour
{
    public static BattleController Instance { get; private set; }

    [ReadOnly, ShowInInspector] private Encounter _encounter;
    public List<CharacterBattler> Party;
    public List<MonsterBattler> Enemies;

    public IEnumerable<IBattler> Battlers => Party.Concat<IBattler>(Enemies);

    public UnityEvent OnBattleEnd;

    //Mandar isso pro battlecanvas. Tudo gráfico é pra tar lá.
    public GameObject BattleCanvasPrefab;
    public BattleCanvas battleCanvas;

    [ReadOnly] public int CurrentTurn;
    [ReadOnly] public IBattler CurrentBattler;

    private CancellationTokenSource CancelBattle = new CancellationTokenSource();
    
    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(this);
            return;
        }

        Instance = this;
    }

    public void BeginBattle(GameObject encounterPrefab, Sprite backgroundSprite = null)
    {
        //Pega uma referencia ao Encounter
        _encounter = encounterPrefab.GetComponent<Encounter>();
        
        //Pausa o Jogo
        PlayerController.Instance.PauseGame();

        //Caso não tenha chão especificado, tenta pegar o chão que o player está
        if (backgroundSprite == null)
            backgroundSprite = GetPlayerGroundSprite();

        //Monta o BattleCanvas
        battleCanvas = Instantiate(BattleCanvasPrefab).GetComponent<BattleCanvas>();
        battleCanvas.SetupBattleground(backgroundSprite);
        
        //Monta party e inimigos
        Party = battleCanvas.SetupParty(PlayerController.Instance.Party);
        Enemies = battleCanvas.SetupMonsters(encounterPrefab);
        Party.ForEach((partyMember) => { partyMember.UpdateAnimator(); });

        //Inicia o combate
        Task.Run(BattleLoop, CancelBattle.Token);
    }

    private void OnDestroy()
    {
        CancelBattle.Cancel();
    }

    async Task BattleLoop()
    {
        try
        {
            //Inicia Turno atual = 1
            CurrentTurn = 1;

            //Pega a ordem do turno.
            var turnOrder = Battlers.OrderByDescending(battler => battler.Stats.Speed);
            var orderEnumerator = turnOrder.GetEnumerator();
            var battleResult = IsBattleOver();

            //Enquanto a batalha não acabou (um dos lados não está morto).
            while (battleResult == 0)
            {
                //Se não tem mais ninguem pra ir no turno atual
                if (orderEnumerator.MoveNext() == false)
                {
                    CurrentTurn++;

                    //Recalcula a ordem caso haja mudança de speed em alguem.
                    var updatedTurnOrder = Battlers.OrderByDescending(battler => battler.Stats.Speed);
                    orderEnumerator = updatedTurnOrder.GetEnumerator();
                    orderEnumerator.MoveNext();
                }

                //Pega o Battler atual e espera o turno dele.
                CurrentBattler = orderEnumerator.Current;
                if (CurrentBattler.Fainted)
                    continue;
                
                await BattlerTurn(CurrentBattler);
                battleResult = IsBattleOver();
            }
            
            if (battleResult == 1)
            {
                Debug.Log("Ganhou");
            } else if (battleResult == -1)
            {
                Debug.Log("Perdeu");
            }

            GameController.Instance.QueueAction(() =>
            {
                Party.ForEach(partyMember => partyMember.CommitChanges());
                OnBattleEnd.Invoke();
                PlayerController.Instance.UnpauseGame();
                Destroy(battleCanvas.gameObject);
                orderEnumerator.Dispose();

                Debug.Log("Acabou");
            });
        }
        catch (Exception e)
        {
            CancelBattle.Cancel();
            Debug.LogException(e);
            //GameController.Instance.QueueAction(() => Debug.LogException(e));
        }
    }

    private void CommitChanges()
    {
        var playerController = PlayerController.Instance;
        Party.ForEach(partyMember => partyMember.CommitChanges());
        playerController.GainEXP(_encounter.ExpReward);
        playerController.CurrentGold += _encounter.GoldReward;
    }
    
    async Task BattlerTurn(IBattler battler)
    {
        await battler.TurnStart();

        var turn = await battler.GetTurn();

        var usedSkill = turn.Skill;
        var targets = turn.Targets;

        if (usedSkill != null)
        {
            GameController.Instance.QueueAction(() => Debug.Log($"Skill: {usedSkill.SkillName}, Target: {targets.First()}"));
            
            await battler.ExecuteTurn(battler, usedSkill,
                targets); //Usa a skill, toca a animação de usar a skill. Talvez botar pra ser dentro do GetTurn mesmo. Ver.

            //Roda a skill em todos os alvos em parelelo, espera todos eles retornarem os efeitos.
            
            //Dá problema quando usa uma skill que dá dano em vários alvos inimigos. Ver porque. Idealmente é pra usar isso
            var effectResults =
                await Task.WhenAll(targets.EachDo((target) => target.ReceiveSkill(battler, usedSkill)));

            // var effectResults = new List<IEnumerable<EffectResult>>();
            //
            // foreach (var target in targets)
            // {
            //     var effectResult = await target.ReceiveSkill(battler, usedSkill);
            //     effectResults.Add(effectResult);
            // }
            
            var concatResults = effectResults.SelectMany(x => x);
            
            await battler.AfterSkill(
                concatResults); //Ex. caso tenha alguma interação com o que aconteceu. Ex. curar 2% do dano dado, que é afetado pela rolagem de dano, crits, erros, etc.
        }

        await battler.TurnEnd(); //Cleanup ou outros efeitos
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>1: Player win
    /// -1: Enemy win
    /// 0: Battle not Over</returns>
    public int IsBattleOver()
    {
        if (!Party.Exists((partyMember) => !partyMember.Fainted))
            return -1;

        if (!Enemies.Exists(enemy => !enemy.Fainted))
            return 1;

        return 0;
    }

    public Sprite GetPlayerGroundSprite()
    {
        try
        {
            var tileMaps = GameObject.FindObjectsOfType<Tilemap>();
            var groundTilemap = tileMaps.First(tilemap => tilemap.name == "Ground");
            var playerPosition = PlayerController.Instance.transform.position;
            var playerTilemapPosition = groundTilemap.WorldToCell(playerPosition);
            var playerTile = groundTilemap.GetSprite(playerTilemapPosition);
            return playerTile;
        }
        catch (Exception e)
        {
            return null;
        }
    }

    [Button("Test")]
    public void test()
    {
        Party.ForEach((partyMember) => { StartCoroutine(rotina(partyMember)); });
    }

    IEnumerator rotina(CharacterBattler partyMember)
    {
        yield return partyMember.PlayAndWait(CharacterBattler.CharacterBattlerAnimation.Attack);
        Debug.Log(partyMember.Character.Base.CharacterName + " terminou");
    }

    //Ver depois, agora só pra testar
    public float DamageCalculation(IBattler source, IBattler target, DamageEffect effect)
    {
        //Alguma logica aqui (ou antes) que leva em conta as passivas, pegar se é magico ou fisico do effect
        return source.Stats.PhysAtk - target.Stats.PhysDef;
    }
}

public abstract class EffectResult
{
    public SkillInfo skillInfo;
}

public class Turn
{
    public Skill Skill { get; set; }
    public IEnumerable<IBattler> Targets { get; set; }
}
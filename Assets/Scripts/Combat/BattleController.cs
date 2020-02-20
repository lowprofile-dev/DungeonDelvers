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

public class BattleController : AsyncMonoBehaviour
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

    private Task Battle;
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

    public void BeginBattle(GameObject encounterPrefab, Sprite backgroundSprite = null, bool playAnimation = false)
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
        Battle = Task.Run(BattleLoop, CancelBattle.Token);
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

            switch (battleResult)
            {
                case 1:
                    Debug.Log("Ganhou");
                    await Win();
                    break;
                case -1:
                    Debug.Log("Perdeu");
                    Lose();
                    break;
            }
        }
        catch (Exception e)
        {
            CancelBattle.Cancel();
            CancelBattle = new CancellationTokenSource();
            Debug.LogException(e);
            //GameController.Instance.QueueAction(() => Debug.LogException(e));
        }
        finally
        {
            Battle = null;
        }
    }

    
    //?? arrumar algum dia
    private async Task Win()
    {
        bool finished = false;
        //roll items
        var items = new Item[] { };
        
        await QueueActionAndAwait(() =>
        {
            battleCanvas.RewardPanel.ShowRewardPanel(_encounter.ExpReward, _encounter.GoldReward, items);
            var rewardPanelClosedEvent = battleCanvas.RewardPanel.RewardPanelClosed;
            
            void CloseDialog()
            {
                finished = true;
                rewardPanelClosedEvent.RemoveListener(CloseDialog);
            }
            
            battleCanvas.RewardPanel.RewardPanelClosed.AddListener(CloseDialog);
        });
        
        while (!finished)
            await Task.Delay(5);
        
        await QueueActionAndAwait((() => CommitWin(items)));
    }

    private void CommitWin(IEnumerable<Item> rewards)
    {
        PartyCommit();
        PlayerController.Instance.GainEXP(_encounter.ExpReward);
        PlayerController.Instance.CurrentGold += _encounter.GoldReward;
        foreach (var reward in rewards)
        {
            PlayerController.Instance.AddItemToInventory(reward);
        }
    }
    
    private void Lose()
    {
        Application.Quit();
    }

    public void ForceEnd()
    {
        if (Battle != null)
        {
            PartyCommit();
            CancelBattle.Cancel();
            CancelBattle = new CancellationTokenSource();
            Battle = null;
        }
    }

    private void PartyCommit()
    {
        Party.ForEach(partyMember => partyMember.CommitChanges());
        OnBattleEnd.Invoke();
        PlayerController.Instance.UnpauseGame();
        Destroy(battleCanvas.gameObject);
        Debug.Log("Acabou");
    }

    private void CommitChanges()
    {
        var playerController = PlayerController.Instance;
        Party.ForEach(partyMember => partyMember.CommitChanges());
        playerController.GainEXP(GetExpReward());
        playerController.CurrentGold += _encounter.GoldReward;
    }

    private int GetExpReward()
    {
        var partyLevel = PlayerController.Instance.PartyLevel;
        var enemyEncounterAverageLevel = (float)_encounter.Monsters
            .Select(monster => monster.Level)
            .Average();

        var clampedDelta = Mathf.Clamp(enemyEncounterAverageLevel - partyLevel, -5f, 5f);

        float expModifier = 1f;

        if (clampedDelta > 0)
        {
            expModifier += clampedDelta / 10;
        }
        else
        {
            expModifier -= clampedDelta / 5;
        }

        var modifiedExp = (int) (_encounter.ExpReward * expModifier);
        
        Debug.Log($"Calculated Exp Reward -- Base: {_encounter.ExpReward}, Pt. Level: {partyLevel}, Enc. Level: {enemyEncounterAverageLevel:F}, C. Delta: {clampedDelta}, Modifier: {expModifier}, Final: {modifiedExp}");

        return modifiedExp;
    }
    
    async Task BattlerTurn(IBattler battler)
    {
        try
        {
            await battler.TurnStart();

            var turn = await battler.GetTurn();

            await QueueActionAndAwait(() => battleCanvas.UnbindActionArrow());

            if (turn != null)
            {
                var usedSkill = turn.Skill;
                var targets = turn.Targets;
                
                GameController.Instance.QueueAction(() =>
                    Debug.Log($"Skill: {usedSkill.SkillName}, Targets: {String.Join(", ", turn.Targets.Select(target => $"{target.Name}"))}"));

                await battler.ExecuteTurn(battler, usedSkill,
                    targets);
                
                var effectResults =
                    await Task.WhenAll(targets.EachDo((target) => target.ReceiveSkill(battler, usedSkill)));

                var concatResults = effectResults.SelectMany(x => x);

                await battler.AfterSkill(
                    concatResults);
            }

            await battler.TurnEnd();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        
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
            var groundTilemap = tileMaps.First(tilemap => tilemap.CompareTag("PG_Floor"));
            var playerPosition = PlayerController.Instance.transform.position;
            var playerTilemapPosition = groundTilemap.WorldToCell(playerPosition);
            var playerTile = groundTilemap.GetSprite(playerTilemapPosition);
            if (playerTile == null)
            {
                var x = playerTilemapPosition.x;
                var y = playerTilemapPosition.y;
                var z = playerTilemapPosition.z;

                var positionGrid = new Vector3Int[]
                {
                    new Vector3Int(x-1, y-1, z), new Vector3Int(x, y-1, z), new Vector3Int(x+1, y-1, z),
                    new Vector3Int(x-1, y, z), /**/ new Vector3Int(x+1,y,z),
                    new Vector3Int(x-1, y+1, z), new Vector3Int(x,y+1,z), new Vector3Int(x+1,y+1,z),   
                };

                foreach (var position in positionGrid)
                {
                    var tile = groundTilemap.GetSprite(position);
                    if (tile != null)
                        return tile;
                }

                return null;
            }
            else
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

    public int DamageCalculation(IBattler source, IBattler target, DamageEffect effect, float variance = 0.15f)
    {
        ////Alguma logica aqui (ou antes) que leva em conta as passivas, pegar se é magico ou fisico do effect
        //return source.Stats.PhysAtk - target.Stats.PhysDef;

        var damageType = effect.DamageType;

        int damage;

        switch (damageType)
        {
            case DamageType.Physical:
            {
                damage = (int)DamageFormula2(source.Level, source.Stats.PhysAtk, target.Level, target.Stats.PhysDef);
                break;
            }
            case DamageType.Magical:
            {
                damage = (int) DamageFormula2(source.Level, source.Stats.MagAtk, target.Level, target.Stats.MagDef);
                break;
            }
            case DamageType.Pure:
            {
                //damage = (int) DamageFormula1(source.Level, source.Stats.MagAtk, target.Level, target.Stats.MagDef);
                throw new NotImplementedException();
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }

        var finalDamageMultiplier = Random.Range(1 - variance, 1 + variance);

        return (int)(damage*finalDamageMultiplier);
    }

    private float DamageFormula1(int sourceLevel, int sourceAttack, int targetLevel, int targetDefense)
    {
        try
        {
            var levelDelta = ((float) sourceLevel - targetLevel);
            levelDelta = Mathf.Clamp(levelDelta, -10, 10);
            var levelDeltaMultiplier = levelDelta.Remap(-10, 10, 0.5f, 1.5f);

            var baseDamage = 5 * sourceLevel;
            var statDelta = 1 + (1 - (float) targetDefense / (sourceAttack + 1)) / 2;
            var damage = (baseDamage + (sourceAttack) * statDelta) - targetDefense;

            return (damage*levelDeltaMultiplier).Min(0);
        }
        catch (Exception e)
        {
            Debug.LogError($"DF1 {sourceLevel} {sourceAttack} {targetLevel} {targetDefense}");
            return 0;
        }
    }

    private float DamageFormula2(int sourceLevel, int sourceAttack, int targetLevel, int targetDefense)
    {
        try
        {
            var levelDelta = ((float) sourceLevel - targetLevel);
            levelDelta = Mathf.Clamp(levelDelta, -10, 10);
            var levelDeltaMultiplier = levelDelta.Remap(-10, 10, 0.5f, 1.5f);

            var baseDamage = 5 * sourceLevel;
            var statTotal = sourceAttack + targetDefense;
            var attackRatio = (float)sourceAttack / statTotal;

            var damage = (sourceAttack * attackRatio)+baseDamage;

            return (damage*levelDeltaMultiplier).Min(0);
        }
        catch (Exception e)
        {
            Debug.LogError($"DF2 {sourceLevel} {sourceAttack} {targetLevel} {targetDefense}");
            return 0;
        }
    }

    [Serializable]
    public struct CombatAttempt
    {
        public string Name;
        public int SourceLevel;
        public int SourceAttackStat;
        public int TargetLevel;
        public int TargetDefenseStat;
    }

    public List<CombatAttempt> CombatAttempts = new List<CombatAttempt>();

    [Button]
    public void CheckAttempts()
    {
        foreach (var combatAttempt in CombatAttempts)
        {
            var damage = DamageFormula1(combatAttempt.SourceLevel,combatAttempt.SourceAttackStat,combatAttempt.TargetLevel,combatAttempt.TargetDefenseStat);
            Debug.Log($"{combatAttempt.Name} -> {damage}");
        }
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
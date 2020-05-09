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
// ReSharper disable HeuristicUnreachableCode
// ReSharper disable LoopVariableIsNeverChangedInsideLoop
// ReSharper disable ConditionIsAlwaysTrueOrFalse

public class BattleController : AsyncMonoBehaviour
{
    public static BattleController Instance { get; private set; }

    [ReadOnly, ShowInInspector] private EncounterSet _encounterSet;
    public List<CharacterBattler> Party;
    public List<MonsterBattler> Enemies;

    public IEnumerable<Battler> Battlers => Party.Concat<Battler>(Enemies);

    public UnityEvent OnBattleEnd;
    
    public GameObject BattleCanvasPrefab;
    public BattleCanvas battleCanvas;

    [ReadOnly] public int CurrentTurn;
    [ReadOnly] public Battler CurrentBattler;

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

    public void BeginBattle(EncounterSet encounter, Sprite backgroundSprite = null, bool playAnimation = false)
    {
        _encounterSet = encounter;
        PlayerController.Instance.PauseGame();

        if (backgroundSprite == null)
            backgroundSprite = GetPlayerGroundSprite();

        battleCanvas = Instantiate(BattleCanvasPrefab).GetComponent<BattleCanvas>();
        battleCanvas.SetupBattleground(backgroundSprite);

        Party = battleCanvas.SetupParty(PlayerController.Instance.Party);
        Enemies = battleCanvas.SetupMonsters(encounter);
        Party.ForEach((partyMember) => { partyMember.UpdateAnimator(); });

        var bgm = encounter.bgmOverride != null ? encounter.bgmOverride : MapSettings.Instance.BattleBgm;
        
        MapSettings.Instance.PauseBgm();
        battleCanvas.StartBattleMusic(bgm);
        
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
    
    private async Task Win()
    {
        bool finished = false;

        var items = _encounterSet.GetItemReward();

        var expReward = GetExpReward();
        var goldReward = _encounterSet.GetGoldReward();
        
        await QueueActionAndAwait(() =>
        {
            battleCanvas.StopBattleMusic();
            battleCanvas.RewardPanel.ShowRewardPanel(expReward, goldReward, items);
            var rewardPanelClosedEvent = battleCanvas.RewardPanel.RewardPanelClosed;
            
            void CloseDialog()
            {
                finished = true;
                rewardPanelClosedEvent.RemoveListener(CloseDialog);
            }
            
            rewardPanelClosedEvent.AddListener(CloseDialog);
        });
        
        while (!finished)
            await Task.Delay(5);

        await QueueActionAndAwait(() =>
        {
            CommitWin(expReward, goldReward, items);
            Cleanup();
        });
    }

    private void CommitWin(int expReward, int goldReward, IEnumerable<Item> rewards)
    {
        PlayerController.Instance.GainEXP(expReward);
        PlayerController.Instance.CurrentGold += goldReward;
        foreach (var reward in rewards)
        {
            PlayerController.Instance.AddItemToInventory(reward);
        }
        PartyCommit();
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
            Cleanup();
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

    private int GetExpReward()
    {
        if (_encounterSet.overrideExpGain.HasValue)
        {
            return (int)(_encounterSet.overrideExpGain.Value * GameController.Instance.GlobalExperienceModifier);
        }
        
        var partyLevel = PlayerController.Instance.PartyLevel;

        var expReward = Enemies
            .Select(enemy => Mathf.Max(0,enemy.Level-partyLevel+1))
            .Sum();

        Debug.Log($"Calculated Exp Reward -- {expReward}");

        return (int)(expReward*GameController.Instance.GlobalExperienceModifier);
    }

    private void Cleanup()
    {
        battleCanvas.PauseBattleMusic();
        MapSettings.Instance.UnpauseBgm();
    }
    
    async Task BattlerTurn(Battler battler)
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
                    Debug.Log($"Skill: {usedSkill.SkillName}, Targets: {String.Join(", ", turn.Targets.Select(target => $"{target.BattlerName}"))}"));

                await battler.ExecuteTurn(turn);

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

    // public Sprite GetPlayerGroundSprite()
    // {
    //     try
    //     {
    //         var tileMaps = MapSettings.Instance.GetCurrentMapTile()?.Tilemaps;
    //         if (tileMaps == null)
    //             return null;
    //         var playerPosition = PlayerController.Instance.transform.position;
    //         var groundTilemaps = tileMaps
    //             .Where(tileMap => tileMap.CompareTag("PG_Floor"))
    //             .Where(tileMap =>
    //             {
    //                 var localBounds = tileMap.localBounds;
    //                 var worldBounds = new Bounds(tileMap.transform.position,localBounds.size);
    //                 return worldBounds.Contains(playerPosition);
    //             });
    //         foreach (var groundTilemap in groundTilemaps)
    //         {
    //             var playerTilemapPosition = groundTilemap.WorldToCell(playerPosition);
    //             var playerTile = groundTilemap.GetSprite(playerTilemapPosition);
    //             if (playerTile == null)
    //             {
    //                 var x = playerTilemapPosition.x;
    //                 var y = playerTilemapPosition.y;
    //                 var z = playerTilemapPosition.z;
    //
    //                 var positionGrid = new Vector3Int[]
    //                 {
    //                     new Vector3Int(x-1, y-1, z), new Vector3Int(x, y-1, z), new Vector3Int(x+1, y-1, z),
    //                     new Vector3Int(x-1, y, z), /**/ new Vector3Int(x+1,y,z),
    //                     new Vector3Int(x-1, y+1, z), new Vector3Int(x,y+1,z), new Vector3Int(x+1,y+1,z),   
    //                 };
    //
    //                 foreach (var position in positionGrid)
    //                 {
    //                     var tile = groundTilemap.GetSprite(position);
    //                     if (tile != null)
    //                         return tile;
    //                 }
    //             }
    //             else
    //                 return playerTile;
    //         }
    //         return null;
    //     }
    //     catch (Exception e)
    //     {
    //         Debug.LogException(e);
    //         return null;
    //     }
    // }

    public Sprite GetPlayerGroundSprite()
    {
        var currentMapTile = MapSettings.Instance.GetCurrentMapTile();
        return currentMapTile?.BattleSprite;
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

    public int DamageCalculation(DamageEffect.DamageCalculationInfo damageCalculationInfo, float variance = 0.15f)
    {
        var damageType = damageCalculationInfo.DamageType;
        var source = damageCalculationInfo.Source;
        var target = damageCalculationInfo.Target;
        
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
                //damage = (int) DamageFormula2(source.)
                //break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }

        var elementalMultiplier = target.Stats.ElementalResistance[damageCalculationInfo.DamageElement];
        var finalDamageMultiplier = Random.Range(1 - variance, 1 + variance);

        Debug.Log($"Calculado {damage}");
        
        return (int)(damage*elementalMultiplier*finalDamageMultiplier);
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
        catch (Exception)
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
        catch (Exception)
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

    public bool IsAlly(Battler battler) => Party.Contains(battler);
    public bool IsEnemy(Battler battler) => Enemies.Contains(battler);
    
    //Futuramente dar uma olhada na complexidade
    public Battler[][] BuildPossibleTargets(Battler source, Skill.TargetType targetType)
    {
        if (IsAlly(source))
        {
            switch (targetType)
            {
                case Skill.TargetType.Any:
                    return new List<Battler>(Party).Concat(Enemies).Where(b => !b.Fainted).Select(b => new []{b}).ToArray();
                case Skill.TargetType.OneEnemy:
                    return Enemies.Cast<Battler>().Where(e => !e.Fainted).Select(b => new []{b}).ToArray();
                case Skill.TargetType.OneAlly:
                    return Party.Cast<Battler>().Where(p => !p.Fainted).Select(p => new []{p}).ToArray();
                case Skill.TargetType.AllEnemies:
                    return new[] {Enemies.Cast<Battler>().Where(e => !e.Fainted).ToArray()};
                case Skill.TargetType.AllAllies:
                    return new[] {Party.Cast<Battler>().Where(p => !p.Fainted).ToArray()};
                case Skill.TargetType.All:
                    return new[] {new List<Battler>(Party).Concat(Enemies).Where(b => !b.Fainted).ToArray()};
                case Skill.TargetType.Self:
                    return new[] {new[] {source}};
                default:
                    throw new ArgumentOutOfRangeException(nameof(targetType), targetType, null);
            }
        }
        else if (IsEnemy(source))
        {
            switch (targetType)
            {
                case Skill.TargetType.Any:
                    return new List<Battler>(Party).Concat(Enemies).Where(b => !b.Fainted).Select(b => new []{b}).ToArray();
                case Skill.TargetType.OneEnemy:
                    return Party.Cast<Battler>().Where(e => !e.Fainted).Select(p => new []{p}).ToArray();
                case Skill.TargetType.OneAlly:
                    return Enemies.Cast<Battler>().Where(p => !p.Fainted).Select(e => new []{e}).ToArray();
                case Skill.TargetType.AllEnemies:
                    return new[] {Party.Cast<Battler>().Where(p => !p.Fainted).ToArray()};
                case Skill.TargetType.AllAllies:
                    return new[] {Enemies.Cast<Battler>().Where(e => !e.Fainted).ToArray()};
                case Skill.TargetType.All:
                    return new[] {new List<Battler>(Party).Concat(Enemies).Where(b => !b.Fainted).ToArray()};
                case Skill.TargetType.Self:
                    return new[] {new[] {source}};
                default:
                    throw new ArgumentOutOfRangeException(nameof(targetType), targetType, null);
            }
        }
        else
        {
            return null;
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
    public IEnumerable<Battler> Targets { get; set; }
}
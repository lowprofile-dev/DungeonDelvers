using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using DunGen;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class TilemapManager : SerializedMonoBehaviour
{
    public RuntimeDungeon Dungeon;
    public int Seed;

    public List<Tilemap> MainTilemaps;
    public void Start()
    {
        var stopwatch = Stopwatch.StartNew();
        
        var hasBeenGenerated = GameController.Instance.Seeds.TryGetValue(SceneManager.GetActiveScene().buildIndex, out var seed);
        
        if (hasBeenGenerated){
            Dungeon.Generator.Seed = seed;
            Debug.Log($"Seed for Map {SceneManager.GetActiveScene().buildIndex} found: {seed}.");
        } else {
            Dungeon.Generator.RandomizeSeed();
            GameController.Instance.Seeds[SceneManager.GetActiveScene().buildIndex] = Dungeon.Generator.Seed;
            Debug.Log($"No previous seed found for Map {SceneManager.GetActiveScene().buildIndex}. Generated {Dungeon.Generator.Seed}");
        }
        
        Dungeon.Generate();

        stopwatch.Stop();

        Debug.Log($"Generated dungeon in {stopwatch.ElapsedMilliseconds}ms");

        Try();
    }

    private void MergeTilemaps()
    {
        //var tilemaps = GameObject.FindObjectsOfType<Tilemap>();

        //var types = tilemaps.Select(tilemap => tilemap.name).Distinct();

        //var tilemapGroups = types.Select(type => tilemaps.Where(tilemap => tilemap.name == type).ToArray()).ToArray();

        //foreach (var tilemapGroup in tilemapGroups)
        //{
        //    var mainTilemap = tilemapGroup[0];

        //    for (int i = 1; i < tilemapGroup.Length; i++)
        //    {
        //        RelocateTiles(tilemapGroup[i], mainTilemap);
        //    }

        //    tilemapGroup.Except(new[] { mainTilemap }).Select(tilemap => tilemap.gameObject).ForEach(gO => Destroy(gO));
        //}
        var Tilemaps = new Dictionary<string, Tilemap>();

        MainTilemaps.ForEach(mainTilemap => { Tilemaps.Add(mainTilemap.tag, mainTilemap); });

        var tilemapsInScene = GameObject.FindObjectsOfType<Tilemap>().Except(Tilemaps.Values).ToArray();

        foreach (var tilemapType in Tilemaps.Keys)
        {
            var tilemapsOfType = tilemapsInScene.Where(tilemap => tilemap.tag == tilemapType);

            var mainTilemap = Tilemaps[tilemapType];

            tilemapsOfType.ForEach(tilemapOfType => RelocateTiles(tilemapOfType, mainTilemap));
        }

        foreach (var tilemaps in Tilemaps.Values)
        {
            if (tilemaps.TryGetComponent<CompositeCollider2D>(out var compositeCollider2D))
            {
                compositeCollider2D.GenerateGeometry();
            }
        }
    }

    private void RelocateTiles(Tilemap source, Tilemap target)
    {
        var bounds = source.cellBounds;
        var positions = bounds.allPositionsWithin;

        var toErase = new List<Vector3Int>();
        var toMerge = (new List<Vector3Int>(), new List<TileBase>());

        foreach (var position in positions)
        {
            var tile = source.GetTile(position);
            
            if (tile != null)
            {
                var worldPosition = source.CellToWorld(position);
                var targetPosition = target.WorldToCell(worldPosition);
                toMerge.Item1.Add(targetPosition);
                toMerge.Item2.Add(tile);
                toErase.Add(position);
            }
        }

        source.SetTiles(toErase.ToArray(), Enumerable.Repeat<TileBase>(null, toErase.Count).ToArray());
        target.SetTiles(toMerge.Item1.ToArray(), toMerge.Item2.ToArray());
    }

    [Button]
    public void Try()
    {
        var stopwatch = Stopwatch.StartNew();

        MergeTilemaps();

        stopwatch.Stop();

        Debug.Log($"Merged tilemaps in {stopwatch.ElapsedMilliseconds}ms");
    }

    [Button("New Seed")]
    public void GetNewSeed()
    {
        //Seed = Random.Range(Int32.MinValue, Int32.MaxValue);
        Dungeon.Generator.RandomizeSeed();
    }
}

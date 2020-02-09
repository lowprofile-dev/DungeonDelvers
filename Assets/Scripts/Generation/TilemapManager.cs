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
    public int? ForcedSeed;

    public List<Tilemap> MainTilemaps;
    public void Start()
    {
        var stopwatch = Stopwatch.StartNew();
        
        var hasBeenGenerated = GameController.Instance.Seeds.TryGetValue(SceneManager.GetActiveScene().buildIndex, out var seed);

        if (ForcedSeed.HasValue)
        {
            Dungeon.Generator.Seed = ForcedSeed.Value;
            Debug.Log($"Map using forced seed {ForcedSeed.Value}.");
        } else if (hasBeenGenerated){
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
        var Tilemaps = new Dictionary<string, Tilemap>();

        MainTilemaps.ForEach(mainTilemap => { Tilemaps.Add(mainTilemap.tag, mainTilemap); });

        var tilemapsInScene = GameObject.FindObjectsOfType<Tilemap>().Except(Tilemaps.Values).ToArray();

        foreach (var tilemapType in Tilemaps.Keys)
        {
            var tilemapsOfType = tilemapsInScene.Where(tilemap => tilemap.tag == tilemapType);

            var mainTilemap = Tilemaps[tilemapType];

            tilemapsOfType.ForEach(tilemapOfType => RelocateTiles(tilemapOfType, mainTilemap));
        }

        //Cleanup unused
        tilemapsInScene.ForEach(tilemap => Destroy(tilemap.gameObject));
        
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
        Dungeon.Generator.RandomizeSeed();
    }
}

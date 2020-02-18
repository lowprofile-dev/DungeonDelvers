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
    private TilesetMerger _tilesetMerger;

    public List<Tilemap> MainTilemaps;
    public void Start()
    {
        _tilesetMerger = GetComponent<TilesetMerger>();
        
        if (Dungeon != null)
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
        }
        
        _tilesetMerger.MergeTilemaps();
    }

    [Button("New Seed")]
    public void GetNewSeed()
    {
        Dungeon.Generator.RandomizeSeed();
    }
}

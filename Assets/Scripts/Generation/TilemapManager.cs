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
using System.IO;
using UnityEngine.U2D;
using UnityEngine.UI;
using Tile = DunGen.Tile;

public class TilemapManager : SerializedMonoBehaviour
{
    public RuntimeDungeon Dungeon;
    public int? ForcedSeed;
    private TilesetMerger _tilesetMerger;
    private Camera minimapCamera;

    public Image image;

    public Bounds resultBounds;
    
    public void Start()
    {
        // Tilemaps = GameObject.FindGameObjectWithTag("MainGrid").transform.Find("Main Tilemaps")
        //     ?.GetComponentsInChildren<Tilemap>();
        //
        // _tilesetMerger = GetComponent<TilesetMerger>();
        
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

            GenerateMinimapCamera();
            
            stopwatch.Stop();

            Debug.Log($"Generated dungeon in {stopwatch.ElapsedMilliseconds}ms");
        }
    }

    [Button("New Seed")]
    public void GetNewSeed()
    {
        Dungeon.Generator.RandomizeSeed();
    }

    public void GenerateMinimapCamera()
    {
        var secondaryCamera = new GameObject("Secondary Camera");
        var cameraComponent = secondaryCamera.AddComponent<Camera>();
        
        var totalBounds = new Bounds(Dungeon.Root.transform.position,Vector3.zero);

        foreach (Transform t in Dungeon.Root.transform)
        {
            if (t.gameObject.TryGetComponent<Tile>(out var tile))
            {
                totalBounds.Encapsulate(tile.Bounds);
            }
        }
            
        secondaryCamera.transform.position = new Vector3(totalBounds.center.x,totalBounds.center.y, -10);
        cameraComponent.cullingMask = ~(1 << 9);
        cameraComponent.orthographic = true;
        cameraComponent.orthographicSize = Mathf.Max(totalBounds.extents.y,totalBounds.extents.x);
        cameraComponent.backgroundColor = MapSettings.Instance.OverrideBackgroundColor;
        resultBounds = totalBounds;
        cameraComponent.enabled = false;
        MapSettings.Instance.MinimapCamera = cameraComponent;
    }
}

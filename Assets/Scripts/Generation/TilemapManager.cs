﻿using System;
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
using System.Drawing;
using System.IO;
using UnityEngine.UI;

public class TilemapManager : SerializedMonoBehaviour
{
    public RuntimeDungeon Dungeon;
    public int? ForcedSeed;
    private TilesetMerger _tilesetMerger;

    public Image image;
    
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
        
        //_tilesetMerger.MergeTilemaps();

        // var bitmapStopwatch = Stopwatch.StartNew();
        //
        // CreateBitmap();
        //
        // bitmapStopwatch.Stop();
        //
        // Debug.Log($"Created bitmap in {bitmapStopwatch.ElapsedMilliseconds}ms");

        var mapTiles = FindObjectsOfType<MapTile>().Select(mapTile => mapTile.GetComponent<MapTile>()).ToArray();
        var tileMaps = mapTiles.SelectMany(mapTile => mapTile.Tilemaps).ToArray();
        var ColorDictionary = new Dictionary<string, Color>
        {
            {"PG_Floor", Color.white},
            {"PG_Wall", Color.black},
            {"PG_Ceiling", Color.black}
        };
        
        int initalX = Int32.MaxValue;
        int finalX = Int32.MinValue;
        int initialY = Int32.MaxValue;
        int finalY = Int32.MinValue;

        foreach (var tilemap in tileMaps)
        {
            var bounds = tilemap.cellBounds;

            if (bounds.x < initalX)
                initalX = bounds.x;
            if (bounds.y < initialY)
                initialY = bounds.y;
            if (bounds.xMax > finalX)
                finalX = bounds.xMax;
            if (bounds.yMax > finalY)
                finalY = bounds.yMax;
        }

        var padding = 5;
        var texture = new Texture2D(finalX-initalX+(padding*2), finalY-initialY+(padding*2));

        Vector2Int cellPositionToTexturePosition(Vector3Int cellPosition, BoundsInt source)
        {
            return new Vector2Int(cellPosition.x-source.x+padding, cellPosition.y-source.y+padding);
        }

        var destinationTilemaps = _tilesetMerger.MainTilemaps;

        foreach (var mapTile in mapTiles)
        {
            mapTile.MergeTilemap(destinationTilemaps,cellPositionToTexturePosition,new Vector2Int(texture.width,texture.height), ColorDictionary);
        }

        int counter = 0;
        var time = DateTime.Now.ToString("MM-dd-yyyy-HH:mm:ss");
        
        void SaveTex(Texture2D tx)
        {
            var png = tx.EncodeToPNG();
            File.WriteAllBytes(Path.Combine(Application.persistentDataPath,$"/tile{counter++}-{time}.png"), png);
        }
        
//        SaveTex(texture);
//
//        mapTiles.ForEach(mapTile => { SaveTex(mapTile.MinimapCell); });
        
        Debug.Log(Application.persistentDataPath);
    }

    [Button("New Seed")]
    public void GetNewSeed()
    {
        Dungeon.Generator.RandomizeSeed();
    }

    [Button("Try Create Bitmap")]
    public void CreateBitmap()
    {
        var wallTilemap = _tilesetMerger.MainTilemaps.First(tilemap => tilemap.CompareTag("PG_Wall"));
        var floorTilemap = _tilesetMerger.MainTilemaps.First(tilemap => tilemap.CompareTag("PG_Floor"));
        var ceilingTilemap = _tilesetMerger.MainTilemaps.First(tilemap => tilemap.CompareTag("PG_Ceiling"));

        var tilemaps = new[] {floorTilemap, wallTilemap, ceilingTilemap};
        var colors = new[] {MapSettings.Instance.MinimapFloorColor, MapSettings.Instance.MinimapWallColor, MapSettings.Instance.MinimapWallColor};
        
        int initalX = Int32.MaxValue;
        int finalX = Int32.MinValue;
        int initialY = Int32.MaxValue;
        int finalY = Int32.MinValue;

        foreach (var tilemap in tilemaps)
        {
            var bounds = tilemap.cellBounds;

            if (bounds.x < initalX)
                initalX = bounds.x;
            if (bounds.y < initialY)
                initialY = bounds.y;
            if (bounds.xMax > finalX)
                finalX = bounds.xMax;
            if (bounds.yMax > finalY)
                finalY = bounds.yMax;
        }

        var padding = 5;
        var texture = new Texture2D(finalX-initalX+(padding*2), finalY-initialY+(padding*2));

        (int x, int y) cellPositionToTexturePosition(Vector3Int cellPosition, BoundsInt source)
        {
            return (cellPosition.x-source.x+padding, cellPosition.y-source.y+padding);
        }

        //Paint black
        var pix = texture.GetPixels();
        for(int i = 0; i < pix.Length; i++)
            pix[i] = Color.black;
        texture.SetPixels(pix);

        for (int i = 0; i < tilemaps.Length; i++)
        {
            var tilemap = tilemaps[i];
            var color = colors[i];

            var bounds = tilemap.cellBounds;
            var positions = bounds.allPositionsWithin;

            foreach (var position in positions)
            {
                if (tilemap.HasTile(position))
                {
                    var texturePosition = cellPositionToTexturePosition(position, bounds);
                    
                    texture.SetPixel(texturePosition.x,texturePosition.y,color);
                }
            }
        }

        texture.Apply();
        var png = texture.EncodeToPNG();
        File.WriteAllBytes(Path.Combine(Application.persistentDataPath, $"minimap{DateTime.Now:yy-MM-dd-hh-mm-ss}.png"), png);

        var sprite = Sprite.Create(texture, new Rect(0,0,texture.width,texture.height), new Vector2(0,0));
        
        image.sprite = sprite;
    }
}

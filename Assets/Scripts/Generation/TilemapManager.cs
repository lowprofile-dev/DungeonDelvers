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
using UnityEngine.UI;

public class TilemapManager : SerializedMonoBehaviour
{
    public RuntimeDungeon Dungeon;
    public Tilemap[] Tilemaps;
    public Transform CopyableParent;
    public Transform MoveableParent;
    public int? ForcedSeed;
    private TilesetMerger _tilesetMerger;

    public Image image;
    
    public void Start()
    {
        Tilemaps = GameObject.FindGameObjectWithTag("MainGrid").transform.Find("Main Tilemaps")
            ?.GetComponentsInChildren<Tilemap>();
        
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
        
        _tilesetMerger.MergeTilemaps(Tilemaps, MoveableParent,CopyableParent);

         var bitmapStopwatch = Stopwatch.StartNew();
        
         CreateBitmap();
        
         bitmapStopwatch.Stop();
        
         Debug.Log($"Created bitmap in {bitmapStopwatch.ElapsedMilliseconds}ms");
    }

    [Button("New Seed")]
    public void GetNewSeed()
    {
        Dungeon.Generator.RandomizeSeed();
    }

    [Button("Try Create Bitmap")]
    public void CreateBitmap()
    {
        var wallTilemap = Tilemaps.First(tilemap => tilemap.CompareTag("PG_Wall"));
        var floorTilemap = Tilemaps.First(tilemap => tilemap.CompareTag("PG_Floor"));
        var ceilingTilemap = Tilemaps.First(tilemap => tilemap.CompareTag("PG_Ceiling"));

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
        var sizeX = finalX - initalX + (padding * 2);
        var sizeY = finalY - initialY + (padding * 2);
        var texture = new Texture2D(sizeX, sizeY);

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

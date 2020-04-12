using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class MapTile : SerializedMonoBehaviour
{
    private MapSettings Settings;
    private Tilemap[] _tilemaps = null;
    [ShowInInspector, HideInEditorMode] public Tilemap[] Tilemaps
    {
        get
        {
            if (_tilemaps == null)
                _tilemaps = GetComponentsInChildren<Tilemap>();
            return _tilemaps;
        }
    }
    public int TileIndex;
    private DunGen.Tile _tile;
    public Texture2D MinimapCell;
    
    private void Awake()
    {
        _tile = GetComponent<DunGen.Tile>();
        if (_tile == null)
        {
            DestroyImmediate(this);
            return;
        }
        Settings = MapSettings.Instance;
        TileIndex = Settings.MapTiles.Count;
        Settings.MapTiles.Add(this);
        Settings.MapTileSeen.Add(TileIndex, false);
    }

    private void Start()
    {
        var bounds = _tile.Bounds;
        DestroyImmediate(gameObject.GetComponent<BoxCollider>());
        var triggerCollider = gameObject.AddComponent<BoxCollider2D>();
        triggerCollider.isTrigger = true;
        triggerCollider.offset = bounds.center - transform.position;
        triggerCollider.size = bounds.size;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Settings.MapTileSeen[TileIndex] == false)
        {
            Settings.MapTileSeen[TileIndex] = true;
        }
    }

    // public void MergeTilemap(List<Tilemap> destination, Func<Vector3Int, BoundsInt, Vector2Int> CellToArray, Vector2Int TextureSize, Dictionary<string,Color> MinimapColors)
    // {
    //     var texture = new Texture2D(TextureSize.x,TextureSize.y);
    //     var merged = new List<GameObject>();
    //     
    //     foreach (var tilemap in Tilemaps)
    //     { 
    //         var dest = destination.FirstOrDefault(t => t.CompareTag(tilemap.tag));
    //
    //         if (dest == null)
    //             continue;
    //
    //         var bounds = tilemap.cellBounds;
    //         var positions = bounds.allPositionsWithin;
    //         
    //         var toErase = new List<Vector3Int>();
    //         var toMerge = (new List<Vector3Int>(), new List<TileBase>());
    //
    //         foreach (var position in positions)
    //         {
    //             var tile = tilemap.GetTile(position);
    //         
    //             if (tile != null)
    //             {
    //                 var worldPosition = tilemap.CellToWorld(position);
    //                 var targetPosition = dest.WorldToCell(worldPosition);
    //                 toMerge.Item1.Add(targetPosition);
    //                 toMerge.Item2.Add(tile);
    //                 toErase.Add(position);
    //
    //                 var texturePosition = CellToArray(position, bounds);
    //                 var color = MinimapColors[tilemap.tag];
    //                 
    //                 texture.SetPixel(texturePosition.x,texturePosition.y, color);
    //             }
    //         }
    //
    //         dest.SetTiles(toErase.ToArray(), Enumerable.Repeat<TileBase>(null, toErase.Count).ToArray());
    //         dest.SetTiles(toMerge.Item1.ToArray(), toMerge.Item2.ToArray());
    //         merged.Add(tilemap.gameObject);
    //     }
    //
    //     merged.ForEach(Destroy);
    //     MinimapCell = texture;
    // }
    //
    // public Texture2D MergeTilemap(List<Tilemap> destination, Func<Vector3Int, Tilemap, Vector2Int> CellToArray, Vector2Int TextureSize, Dictionary<string,Color> MinimapColors)
    // {
    //     var texture = new Texture2D(TextureSize.x,TextureSize.y);
    //     var merged = new List<GameObject>();
    //     
    //     foreach (var tilemap in Tilemaps)
    //     { 
    //         var dest = destination.FirstOrDefault(t => t.CompareTag(tilemap.tag));
    //
    //         if (dest == null)
    //             continue;
    //
    //         var bounds = tilemap.cellBounds;
    //         var positions = bounds.allPositionsWithin;
    //         
    //         var toErase = new List<Vector3Int>();
    //         var toMerge = (new List<Vector3Int>(), new List<TileBase>());
    //
    //         foreach (var position in positions)
    //         {
    //             var tile = tilemap.GetTile(position);
    //         
    //             if (tile != null)
    //             {
    //                 var worldPosition = tilemap.CellToWorld(position);
    //                 var targetPosition = dest.WorldToCell(worldPosition);
    //                 toMerge.Item1.Add(targetPosition);
    //                 toMerge.Item2.Add(tile);
    //                 toErase.Add(position);
    //
    //                 var texturePosition = CellToArray(position, tilemap);
    //                 var color = MinimapColors[tilemap.tag];
    //                 
    //                 texture.SetPixel(texturePosition.x,texturePosition.y, color);
    //             }
    //         }
    //
    //         dest.SetTiles(toErase.ToArray(), Enumerable.Repeat<TileBase>(null, toErase.Count).ToArray());
    //         dest.SetTiles(toMerge.Item1.ToArray(), toMerge.Item2.ToArray());
    //         merged.Add(tilemap.gameObject);
    //     }
    //
    //     merged.ForEach(Destroy);
    //     MinimapCell = texture;
    //     return texture;
    // }
}
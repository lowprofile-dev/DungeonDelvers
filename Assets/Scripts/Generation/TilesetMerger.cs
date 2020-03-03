using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Tilemaps;
using Debug = UnityEngine.Debug;

public class TilesetMerger : MonoBehaviour
{
    public List<Tilemap> MainTilemaps = new List<Tilemap>();

    public void MergeTilemaps(bool destroyUnused = true)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        var stopwatch = Stopwatch.StartNew();
#endif
        var tilemaps = new Dictionary<string, Tilemap>();
        
        MainTilemaps.ForEach(mainTilemap => tilemaps.Add(mainTilemap.tag, mainTilemap));

        var tilemapsInScene = GameObject.FindObjectsOfType<Tilemap>()
            .Except(MainTilemaps)
            .Where(tilemap => tilemaps.ContainsKey(tilemap.tag))
            .ToArray();

        foreach (var tilemapType in tilemaps.Keys)
        {
            var tilemapsOfType = tilemapsInScene.Where(tilemap => tilemap.tag == tilemapType);

            var mainTilemap = tilemaps[tilemapType];

            tilemapsOfType.ForEach(tilemapOfType => RelocateTiles(tilemapOfType, mainTilemap));
        }
        
        //Cleanup unused
        if (destroyUnused)
            tilemapsInScene.ForEach(tilemap => Destroy(tilemap.gameObject));
        
        foreach (var tilemap in tilemaps.Values)
        {
            if (tilemap.TryGetComponent<CompositeCollider2D>(out var compositeCollider2D))
            {
                compositeCollider2D.GenerateGeometry();
            }
        }
        
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        stopwatch.Stop();
        Debug.Log($"Merged tilemaps in {stopwatch.ElapsedMilliseconds}ms");
#endif
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
    
    #if UNITY_EDITOR || DEVELOPMENT_BUILD
    [Button("Force Merge")]
    private void _forceMerge()
    {
        MergeTilemaps();
    }
    #endif
}

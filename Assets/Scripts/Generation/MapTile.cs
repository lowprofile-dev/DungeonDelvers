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
    private BoxCollider2D _collider2D;
    public Bounds Bounds => _collider2D.bounds;
    private DunGen.Tile _tile;
    public Sprite BattleSprite;

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
        _collider2D = gameObject.AddComponent<BoxCollider2D>();
        _collider2D.isTrigger = true;
        _collider2D.offset = bounds.center - transform.position;
        _collider2D.size = bounds.size;
        SetTilemapLayer(LayerMask.NameToLayer("MinimapHidden"));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        SetTilemapLayer(LayerMask.NameToLayer("Default"));
    }

    public void SetTilemapLayer(int layerIndex)
    {
        foreach (var tilemap in Tilemaps)
        {
            tilemap.gameObject.layer = layerIndex;
        }
    }
}
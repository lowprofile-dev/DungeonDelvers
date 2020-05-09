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
    [SerializeField] private BoxCollider2D _collider2D;
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
    }

    private void Start()
    {
        Settings = MapSettings.Instance;
        TileIndex = Settings.MapTiles.Count;
        Settings.MapTiles.Add(this);
        Settings.MapTileSeen.Add(TileIndex, false);
        
        var bounds = _tile.Bounds;
        DestroyImmediate(gameObject.GetComponent<BoxCollider>());
        _collider2D = gameObject.AddComponent<BoxCollider2D>();
        _collider2D.isTrigger = true;
        _collider2D.offset = bounds.center - transform.position;
        _collider2D.size = bounds.size;
        SetLayerRecursive(transform,LayerMask.NameToLayer("MinimapHidden"), 5);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        SetLayerRecursive(transform,LayerMask.NameToLayer("Default"));
    }

    void SetLayerRecursive(Transform root, int layer, int maxDepth = Int32.MaxValue) {
        if (maxDepth < 0) return;
        if (root.gameObject.layer != LayerMask.NameToLayer("Interactable")) root.gameObject.layer = layer;
        foreach(Transform child in root)
            SetLayerRecursive(child, layer, maxDepth-1);
    }
}
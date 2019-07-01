using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class WorldTile {

    [Serializable]
    public class TileEvent : UnityEvent<WorldTile> { }

    public Vector3Int LocalPlace { get; set; }
    public Vector2Int OffsetCoords { get; set; }
    public Vector3 WorldLocation { get; set; }

    public TileBase TileBase { get; set; }
    public Tile HighlightTile { get; set; }
    public Tile SelectTile { get; set; }
    public Tile ImpassableTile { get; set; }

    public Tilemap TerrainTilemap { get; set; }
    public Tilemap HighlightTilemap { get; set; }
    public Tilemap PlayerBorderTilemap { get; set; }

    public string Name { get; set; }

    // Below is needed for Breadth First Searching
    public bool IsExplored { get; set; }
    public WorldTile ExploredFrom { get; set; }
    public int Cost { get; set; }

    //TODO: rework how armies are moved
    private Army _army;
    public Army army {
        get { return _army; }
        set {
            _army = value;
            if (_army) _army.MoveToTile(this);
        }
    }
    
    private TileEvent tileCaptured = new TileEvent();
    public void Capture(Tile newBorder, UnityAction<WorldTile> capturedCallback) {
        PlayerBorderTilemap.SetTile(LocalPlace, newBorder);

        tileCaptured.RemoveAllListeners();
        tileCaptured.AddListener(capturedCallback);
    }

    #region Control methods
    private bool isSelected = false;

    public void Highlight(Tile hightlightTile) {
        if (!isSelected) {
            HighlightTilemap.SetTile(LocalPlace, hightlightTile);
        }
    }

    public void Dehighlight() {
        if (!isSelected) {
            HighlightTilemap.SetTile(LocalPlace, null);
        }
    }

    public void Select(Tile selectedTile) {
        isSelected = true;
        HighlightTilemap.SetTile(LocalPlace, selectedTile);
    } 

    public void Deselect() {
        isSelected = false;
        HighlightTilemap.SetTile(LocalPlace, null);
    }

    public bool IsPassable() {
        return TerrainTilemap.GetTile(LocalPlace) != ImpassableTile;
    }

    public override bool Equals(object obj) {
        WorldTile otherTile = obj as WorldTile;

        if (otherTile == null) {
            return false;
        }

        return OffsetCoords.Equals(otherTile.OffsetCoords);
    }

    public override int GetHashCode() {
        return base.GetHashCode();
    }
    #endregion
}

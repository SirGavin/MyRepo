using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldTile {

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

    private ArmyMap _army;
    public ArmyMap army {
        get { return _army; }
        set {
            _army = value;
            if (_army) _army.MoveToTile(this);
        }
    }

    public override bool Equals(object obj) {
        WorldTile otherTile = obj as WorldTile;

        if (otherTile == null) {
            return false;
        }

        return this.OffsetCoords.Equals(otherTile.OffsetCoords);
    }

    public override int GetHashCode() {
        return base.GetHashCode();
    }

    public void SetPlayerBorderTile(Tile newBorder) {
        PlayerBorderTilemap.SetTile(LocalPlace, newBorder);
    }

    public void Highlight() {
        if (HighlightTilemap.GetTile(LocalPlace) != SelectTile) {
            HighlightTilemap.SetTile(LocalPlace, HighlightTile);
        }
    }

    public void Dehighlight() {
        if (HighlightTilemap.GetTile(LocalPlace) == HighlightTile) {
            HighlightTilemap.SetTile(LocalPlace, null);
        }
    }

    public void Select() {
        HighlightTilemap.SetTile(LocalPlace, SelectTile);
    }

    public void Deselect() {
        HighlightTilemap.SetTile(LocalPlace, null);
    }

    public bool IsPassable() {
        return TerrainTilemap.GetTile(LocalPlace) != ImpassableTile;
    }
}

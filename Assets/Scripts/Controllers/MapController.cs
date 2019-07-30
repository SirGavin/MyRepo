using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapController : MonoBehaviour {
    
    public Tilemap terrainTileMap;
    public Tilemap highlightTileMap;
    public Tilemap playerBorderTileMap;
    public Tile highlightTile;
    public Tile selectTile;
    public Tile impassableTile;

    public Dictionary<Vector3Int, WorldTile> tiles;

    private WorldTile hovoredTile;
    private WorldTile selectedTile;

    public void ProcessWorldTiles() {
        tiles = new Dictionary<Vector3Int, WorldTile>();
        foreach (Vector3Int pos in terrainTileMap.cellBounds.allPositionsWithin) {
            var localPlace = new Vector3Int(pos.x, pos.y, pos.z);

            if (!terrainTileMap.HasTile(localPlace)) continue;
            terrainTileMap.SetTileFlags(localPlace, TileFlags.None);

            var tile = new WorldTile {
                LocalPlace = localPlace,
                OffsetCoords = new Vector2Int(localPlace.x, localPlace.y),
                WorldLocation = terrainTileMap.CellToWorld(localPlace),
                TileBase = terrainTileMap.GetTile(localPlace),
                HighlightTile = highlightTile,
                ImpassableTile = impassableTile,
                SelectTile = selectTile,
                TerrainTilemap = terrainTileMap,
                HighlightTilemap = highlightTileMap,
                PlayerBorderTilemap = playerBorderTileMap,
                Name = localPlace.x + "," + localPlace.y,
                Cost = 1 // TODO: Change this with the proper cost from ruletile
            };
            
            tiles.Add(tile.LocalPlace, tile);
        }
    }

    public WorldTile GetCurrentTile() {
        return hovoredTile;
    }

    public void ClearSelection() {
        if (selectedTile != null) {
            selectedTile.Deselect();
            selectedTile = null;
        }
    }

    public void TryHover(Vector3 mousePosition) {
        Vector3Int cellLocation = terrainTileMap.WorldToCell(mousePosition);

        WorldTile currentTile;
        if (tiles.TryGetValue(cellLocation, out currentTile)) {
            if (hovoredTile != null) {
                hovoredTile.Dehighlight();
                hovoredTile = null;
            }

            if (currentTile.IsPassable()) {
                hovoredTile = currentTile;
                hovoredTile.Highlight(highlightTile);
            }
        }
    }

    public void TrySelect() {
        if (hovoredTile != null && hovoredTile.IsPassable()) {
            if (hovoredTile.Equals(selectedTile)) {
                //Deselect if clicking on a selected tile
                selectedTile.Deselect();
                selectedTile = null;
                hovoredTile.Highlight(highlightTile);
            } else {
                //Deselect old tile
                if (selectedTile != null) {
                    selectedTile.Deselect();
                    selectedTile = null;
                }
                    
                //Select new tile
                selectedTile = hovoredTile;
                selectedTile.Select(selectTile);
            }
        }
    }

    public bool CanMove() {
        return selectedTile != null && selectedTile.army != null && selectedTile.army.CanMove() && hovoredTile.IsPassable() && HexUtils.AreNeighbors(selectedTile.LocalPlace, hovoredTile.LocalPlace);
    }

    public bool TryMove() {
        if (CanMove() && hovoredTile.army == null) {
            hovoredTile.army = selectedTile.army;
            selectedTile.army = null;
            selectedTile.Deselect();
            selectedTile = hovoredTile;
            selectedTile.Select(selectTile);
            return true;
        }

        return false;
    } 

    public void GetArmies(out Army selectedArmy, out Army occupyingArmy) {
        selectedArmy = selectedTile.army;
        occupyingArmy = hovoredTile.army;
    }

    //This makes the selected army "push" the losing army post-battle
    public void Push() {
        List<Vector2Int> pushDirections = HexUtils.GetPushDirections(selectedTile.OffsetCoords, hovoredTile.OffsetCoords);

        foreach (Vector2Int pushDirection in pushDirections) {
            WorldTile pushTile;
            if (tiles.TryGetValue(new Vector3Int(hovoredTile.LocalPlace.x + pushDirection.x, hovoredTile.LocalPlace.y + pushDirection.y, hovoredTile.LocalPlace.z), out pushTile)) {
                if (pushTile.IsPassable() && pushTile.army == null) {
                    pushTile.army = hovoredTile.army;
                    hovoredTile.army = selectedTile.army;

                    selectedTile.army = null;
                    selectedTile.Deselect();
                    selectedTile = hovoredTile;
                    selectedTile.Select(selectTile);
                        
                    break;
                }
            }
        }
    }

    public void PlaceArmy(Army army) {
        hovoredTile.army = army;
    }

    public void RandomlyPlaceArmy(Army army) {
        while (true) {
            int ranX = Random.Range(terrainTileMap.cellBounds.xMin, terrainTileMap.cellBounds.xMax);
            int ranY = Random.Range(terrainTileMap.cellBounds.yMin, terrainTileMap.cellBounds.yMax);

            WorldTile currentTile;
            if (tiles.TryGetValue(new Vector3Int(ranX, ranY, 0), out currentTile)) {
                if (currentTile.army == null && currentTile.IsPassable()) {
                    currentTile.army = army;
                    return;
                }
            }
        }
    }

    public List<WorldTile> GetNeighbors(WorldTile tile) {
        List<Vector2Int> neighborDirections = HexUtils.GetNeighborDirections(tile.LocalPlace);

        List<WorldTile> neighbors = new List<WorldTile>();

        foreach (Vector2Int direction in neighborDirections) {
            WorldTile currentTile;
            if (tiles.TryGetValue(new Vector3Int(tile.LocalPlace.x + direction.x, tile.LocalPlace.y + direction.y, tile.LocalPlace.z), out currentTile)) {
                neighbors.Add(currentTile);
            }
        }

        return neighbors;
    }

    private void Update() {
        Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0)) {
            TrySelect();
        } else {
            TryHover(point);
        }
    }

    //Used for AI turns
    public void SetHovoredTile(WorldTile tile) {
        hovoredTile = tile;
    }
    public void SetSelectedTile(WorldTile tile) {
        selectedTile = tile;
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapController : MonoBehaviour {

    public static MapController instance;

    public Tilemap terrainTileMap;
    public Tilemap highlightTileMap;
    public Tile highlightTile;
    public Tile selectTile;

    public GameObject armyPrefab;
    public BattleController battleController;
    
    public Dictionary<Vector3Int, WorldTile> tiles;

    private WorldTile hovoredTile;
    private WorldTile selectedTile;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }

        GetWorldTiles();
    }

    private void GetWorldTiles() {
        tiles = new Dictionary<Vector3Int, WorldTile>();
        foreach (Vector3Int pos in terrainTileMap.cellBounds.allPositionsWithin) {
            var localPlace = new Vector3Int(pos.x, pos.y, pos.z);

            if (!terrainTileMap.HasTile(localPlace)) continue;
            terrainTileMap.SetTileFlags(localPlace, TileFlags.None);

            var tile = new WorldTile {
                LocalPlace = localPlace,
                OffsetCoords = new Vector2(localPlace.x, localPlace.y),
                WorldLocation = terrainTileMap.CellToWorld(localPlace),
                TileBase = terrainTileMap.GetTile(localPlace),
                HighlightTile = highlightTile,
                SelectTile = selectTile,
                TerrainTilemap = terrainTileMap,
                HighlightTilemap = highlightTileMap,
                Name = localPlace.x + "," + localPlace.y,
                Cost = 1 // TODO: Change this with the proper cost from ruletile
            };
            
            tiles.Add(tile.LocalPlace, tile);

            if (localPlace.x == 1 && localPlace.y == 1) {
                GameObject newArmy = Instantiate(armyPrefab, new Vector3(tile.WorldLocation.x, tile.WorldLocation.y, -9), Quaternion.identity);
                tile.army = newArmy.GetComponent<ArmyMap>();
            }
        }
    }

    private void Update() {
        Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellLocation = terrainTileMap.WorldToCell(point);

        WorldTile currentTile;
        if (tiles.TryGetValue(cellLocation, out currentTile)) {
            if (hovoredTile != null) {
                hovoredTile.Dehighlight();
                hovoredTile = null;
            }

            if (Input.GetMouseButtonDown(0)) {
                if (selectedTile != null) {
                    selectedTile.Deselect();
                    selectedTile = null;
                }

                if (!currentTile.Equals(selectedTile)) {
                    selectedTile = currentTile;
                    selectedTile.Select();
                }
            }

            if (Input.GetMouseButtonDown(1) && selectedTile != null && selectedTile.army != null && HexUtils.AreNeighbors(selectedTile.LocalPlace, currentTile.LocalPlace)) {
                if (currentTile.army == null) {
                    currentTile.army = selectedTile.army;
                    selectedTile.army = null;
                    selectedTile.Deselect();
                    selectedTile = currentTile;
                    selectedTile.Select();
                } else {
                    //battleController.Fight();
                }
            }

            if (!currentTile.Equals(selectedTile)) {
                hovoredTile = currentTile;
                hovoredTile.Highlight();
            }
        } else if (hovoredTile != null) {
            hovoredTile.Dehighlight();
            hovoredTile = null;
        }
        }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapController : MonoBehaviour {

    public static MapController instance;

    public GameController gameController;

    public Tilemap terrainTileMap;
    public Tilemap highlightTileMap;
    public Tilemap playerBorderTileMap;
    public Tile highlightTile;
    public Tile selectTile;
    public Tile impassableTile;

    public GameObject armyPrefab;
    public BattleController battleController;
    
    public Dictionary<Vector3Int, WorldTile> tiles;

    private WorldTile hovoredTile;
    private WorldTile selectedTile;
    private WorldTile attackedTile;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }

        GetWorldTiles();
        gameController.GeneratePlayers();
    }

    private void GetWorldTiles() {
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

    private void Update() {
        Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellLocation = terrainTileMap.WorldToCell(point);

        WorldTile currentTile;
        if (tiles.TryGetValue(cellLocation, out currentTile)) {
            if (hovoredTile != null) {
                hovoredTile.Dehighlight();
                hovoredTile = null;
            }

            if (Input.GetMouseButtonDown(0) && currentTile.IsPassable()) {
                if (selectedTile != null) {
                    selectedTile.Deselect();
                    selectedTile = null;
                }

                if (!currentTile.Equals(selectedTile)) {
                    selectedTile = currentTile;
                    selectedTile.Select();
                }
            }

            if (Input.GetMouseButtonDown(1) && selectedTile != null && selectedTile.army != null && currentTile.IsPassable() && HexUtils.AreNeighbors(selectedTile.LocalPlace, currentTile.LocalPlace)) {
                if (currentTile.army == null) {
                    currentTile.army = selectedTile.army;
                    selectedTile.army = null;
                    selectedTile.Deselect();
                    selectedTile = currentTile;
                    selectedTile.Select();
                } else {
                    enabled = false;
                    attackedTile = currentTile;
                    battleController.StartBattle(selectedTile.army, attackedTile.army);
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

    public void UpdateArmies(ArmyMap attacker, ArmyMap defender, bool attackerWon) {
        Debug.Log("attackerWon: " + attackerWon);
        if (attackerWon) {
            List<Vector2Int> pushDirections = HexUtils.GetPushDirections(selectedTile.OffsetCoords, attackedTile.OffsetCoords);

            foreach (Vector2Int pushDirection in pushDirections) {
                WorldTile pushTile;
                if (tiles.TryGetValue(new Vector3Int(attackedTile.LocalPlace.x + pushDirection.x, attackedTile.LocalPlace.y + pushDirection.y, attackedTile.LocalPlace.z), out pushTile)) {
                    if (pushTile.IsPassable() && pushTile.army == null) {
                        pushTile.army = defender;
                        attackedTile.army = attacker;

                        selectedTile.army = null;
                        selectedTile.Deselect();
                        selectedTile = attackedTile;
                        selectedTile.Select();
                        attackedTile = null;
                        
                        break;
                    }
                }
            }
        } else {
            selectedTile.army = attacker;
            attackedTile.army = defender;
            attackedTile = null;
        }

        enabled = true;
    }

    public ArmyMap GetRandomArmy() {
        ArmyMap ranArmy = null;

        while (ranArmy == null) {
            int ranX = Random.Range(terrainTileMap.cellBounds.xMin, terrainTileMap.cellBounds.xMax);
            int ranY = Random.Range(terrainTileMap.cellBounds.yMin, terrainTileMap.cellBounds.yMax);

            WorldTile currentTile;
            if (tiles.TryGetValue(new Vector3Int(ranX, ranY, 0), out currentTile)) {
                if (currentTile.army == null && currentTile.IsPassable()) {
                    GameObject newArmy = Instantiate(armyPrefab, new Vector3(currentTile.WorldLocation.x, currentTile.WorldLocation.y, -9), Quaternion.identity);
                    ranArmy = newArmy.GetComponent<ArmyMap>();
                    currentTile.army = ranArmy;
                }
            }
        }

        return ranArmy;
    }

    public bool TryCreateArmy(Vector3Int tilePosition, out ArmyMap newArmy) {

        newArmy = null;
        return false;
    }
}

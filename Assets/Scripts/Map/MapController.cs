using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapController : MonoBehaviour {

    public static MapController instance;
    public Tilemap terrainTileMap;
    public Color hovorColor;
    public Color selectedColor;

    public GameObject armyPrefab;

    public Dictionary<Vector2, WorldTile> tiles;
    public Dictionary<Vector3Int, WorldTile> tiles2;

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
        tiles = new Dictionary<Vector2, WorldTile>();
        tiles2 = new Dictionary<Vector3Int, WorldTile>();
        foreach (Vector3Int pos in terrainTileMap.cellBounds.allPositionsWithin) {
            var localPlace = new Vector3Int(pos.x, pos.y, pos.z);

            if (!terrainTileMap.HasTile(localPlace)) continue;
            //Debug.Log("localPlace" + localPlace);
            var tile = new WorldTile {
                LocalPlace = localPlace,
                OffsetCoords = new Vector2(localPlace.x, localPlace.y),
                WorldLocation = terrainTileMap.CellToWorld(localPlace),
                TileBase = terrainTileMap.GetTile(localPlace),
                TilemapMember = terrainTileMap,
                Name = localPlace.x + "," + localPlace.y,
                Cost = 1 // TODO: Change this with the proper cost from ruletile
            };
            //Debug.Log("tile " + tile.OffsetCoords);
            tiles.Add(tile.OffsetCoords, tile);
            tiles2.Add(tile.LocalPlace, tile);

            if (localPlace.x == 1 && localPlace.y == 1) {
                GameObject newArmy = Instantiate(armyPrefab, new Vector3(tile.WorldLocation.x, tile.WorldLocation.y, -9), Quaternion.identity);
                //Debug.Log("tile.army: " + tile.army);
                tile.army = newArmy.GetComponent<ArmyMap>();
                //Debug.Log("tile.army: " + tile.army);
            }
        }

       /* WorldTile testTile;
        if (tiles.TryGetValue(new Vector2(-1, 1), out testTile)) {
            testTile.TilemapMember.SetTileFlags(testTile.LocalPlace, TileFlags.None);
            testTile.TilemapMember.SetColor(testTile.LocalPlace, Color.black);
        }*/
    }

    private void Update() {
        Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellLocation = terrainTileMap.WorldToCell(point);

        WorldTile currentTile;
        if (tiles2.TryGetValue(cellLocation, out currentTile)) {
            if (selectedTile == null || !currentTile.Equals(selectedTile)) {
                if (hovoredTile != null && !hovoredTile.Equals(selectedTile)) {
                    hovoredTile.TilemapMember.SetTileFlags(hovoredTile.LocalPlace, TileFlags.None);
                    hovoredTile.TilemapMember.SetColor(hovoredTile.LocalPlace, Color.green);
                }

                hovoredTile = currentTile;
                hovoredTile.TilemapMember.SetTileFlags(hovoredTile.LocalPlace, TileFlags.None);
                hovoredTile.TilemapMember.SetColor(hovoredTile.LocalPlace, hovorColor);
            }

            if (Input.GetMouseButtonDown(0)) {
                if (selectedTile != null && !currentTile.Equals(selectedTile)) {
                    selectedTile.TilemapMember.SetTileFlags(selectedTile.LocalPlace, TileFlags.None);
                    selectedTile.TilemapMember.SetColor(selectedTile.LocalPlace, Color.green);
                }
                
                selectedTile = currentTile;
                selectedTile.TilemapMember.SetTileFlags(selectedTile.LocalPlace, TileFlags.None);
                selectedTile.TilemapMember.SetColor(selectedTile.LocalPlace, selectedColor);
            }

            if (Input.GetMouseButtonDown(1) && selectedTile != null && selectedTile.army != null) {
                currentTile.army = selectedTile.army;
                selectedTile.army = null;
                selectedTile = currentTile;
                selectedTile.TilemapMember.SetTileFlags(selectedTile.LocalPlace, TileFlags.None);
                selectedTile.TilemapMember.SetColor(selectedTile.LocalPlace, selectedColor);
            }
        } else if (hovoredTile != null) {
            hovoredTile.TilemapMember.SetTileFlags(hovoredTile.LocalPlace, TileFlags.None);
            hovoredTile.TilemapMember.SetColor(hovoredTile.LocalPlace, Color.green);
            hovoredTile = null;
        }

        /*if (hovoredTile == null || !cellLocation.Equals(hovoredTile.LocalPlace)) {
            if (hovoredTile != null && !hovoredTile.Equals(selectedTile)) {
                hovoredTile.TilemapMember.SetTileFlags(hovoredTile.LocalPlace, TileFlags.None);
                hovoredTile.TilemapMember.SetColor(hovoredTile.LocalPlace, Color.green);
            }

            if ((selectedTile == null || !cellLocation.Equals(selectedTile.LocalPlace))
                    && tiles2.TryGetValue(cellLocation, out hovoredTile)) {
                hovoredTile.TilemapMember.SetTileFlags(hovoredTile.LocalPlace, TileFlags.None);
                hovoredTile.TilemapMember.SetColor(hovoredTile.LocalPlace, hovorColor);
            }
        }

        if (Input.GetMouseButtonDown(0)) {
            if (selectedTile != null) {
                selectedTile.TilemapMember.SetTileFlags(selectedTile.LocalPlace, TileFlags.None);
                selectedTile.TilemapMember.SetColor(selectedTile.LocalPlace, Color.green);
            }

            if (hovoredTile != null) {
                selectedTile = hovoredTile;
                selectedTile.TilemapMember.SetTileFlags(selectedTile.LocalPlace, TileFlags.None);
                selectedTile.TilemapMember.SetColor(selectedTile.LocalPlace, selectedColor);
            }
        }

        if (Input.GetMouseButtonDown(1) && selectedTile != null && selectedTile.army != null) {
            selectedTile.army.MoveToTile(hovoredTile);
            hovoredTile.army = selectedTile.army;
            selectedTile.army = null;
            selectedTile = hovoredTile;
        }*/

            /*Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 offsetCoords = HexUtils.PositionToOffsetCoords(point);

            if (hovoredTile == null || !offsetCoords.Equals(hovoredTile.OffsetCoords)) {
                if (hovoredTile != null && !hovoredTile.Equals(selectedTile)) {
                    hovoredTile.TilemapMember.SetTileFlags(hovoredTile.LocalPlace, TileFlags.None);
                    hovoredTile.TilemapMember.SetColor(hovoredTile.LocalPlace, Color.green);
                }

                if ((selectedTile == null || !offsetCoords.Equals(selectedTile.OffsetCoords))
                        && tiles.TryGetValue(offsetCoords, out hovoredTile)) {
                    hovoredTile.TilemapMember.SetTileFlags(hovoredTile.LocalPlace, TileFlags.None);
                    hovoredTile.TilemapMember.SetColor(hovoredTile.LocalPlace, hovorColor);
                }
            }

            if (Input.GetMouseButtonDown(0)) {
                if (selectedTile != null) {
                    selectedTile.TilemapMember.SetTileFlags(selectedTile.LocalPlace, TileFlags.None);
                    selectedTile.TilemapMember.SetColor(selectedTile.LocalPlace, Color.green);
                }

                if (hovoredTile != null) {
                    selectedTile = hovoredTile;
                    selectedTile.TilemapMember.SetTileFlags(selectedTile.LocalPlace, TileFlags.None);
                    selectedTile.TilemapMember.SetColor(selectedTile.LocalPlace, selectedColor);
                }
            }*/
        }
}

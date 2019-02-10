using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class Player {

    private const float MinReinforcements = 3;

    public Color color;
    public int playerNum;

    public Tile borderTile { get; set; }
    public Tile hovorTile { get; set; }
    public Tile selectedTile { get; set; }

    private List<ArmyMap> armies;
    private List<WorldTile> ownedTiles;

    public void InitTiles(Tile borderTile, Tile highlightTile) {
        this.borderTile = UnityEngine.Object.Instantiate(borderTile);
        this.borderTile.color = color;

        hovorTile = UnityEngine.Object.Instantiate(highlightTile);
        color.a = 0.25f;
        hovorTile.color = color;

        selectedTile = UnityEngine.Object.Instantiate(highlightTile);
        color.a = 0.5f;
        selectedTile.color = color;
    }

    public void AddArmy(ArmyMap newArmy) {
        if (armies == null) armies = new List<ArmyMap>();
        
        armies.Add(newArmy);
    }

    public void AddTile(WorldTile tile) {
        if (ownedTiles == null) ownedTiles = new List<WorldTile>();

        if (!ControlsTile(tile)) ownedTiles.Add(tile);
    }

    public void RemoveTile(WorldTile tile) {
        if (ownedTiles == null) return;

        ownedTiles.Remove(tile);
    }

    public int GetReinforcementCount() {
        return Mathf.FloorToInt(Mathf.Max(ownedTiles.Count / 2f, MinReinforcements));
    }

    public bool ControlsTile(WorldTile tile) {
        foreach (WorldTile ownedTile in ownedTiles) {
            if (ownedTile.Equals(tile)) return true;
        }

        return false;
    }
}

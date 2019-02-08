using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class Player {

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

        newArmy.player = this;
        armies.Add(newArmy);
    }

    public void AddTile(WorldTile tile) {
        if (ownedTiles == null) ownedTiles = new List<WorldTile>();

        ownedTiles.Add(tile);
    }
}

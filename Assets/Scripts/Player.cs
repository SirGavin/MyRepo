using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class Player {

    public Color color;
    private Tile borderTile;

    private List<ArmyMap> armies { get; set; }

    public void SetBorderTile(Tile tile) {
        borderTile = tile;
    }

    public void AddArmy(ArmyMap newArmy) {
        if (armies == null) armies = new List<ArmyMap>();

        newArmy.SetPlayerColor(color);
        newArmy.SetBorderTile(borderTile);

        armies.Add(newArmy);
    }
}

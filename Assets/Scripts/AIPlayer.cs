using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AIPlayer : Player {

    //TODO: rewrite this, it's shit

    private enum Priority {
        High = 100,
        Medium = 75,
        Low = 50,
        None = 0
    };

    private class HeatMapValue {
        public WorldTile tile;
        public Priority priority;
        public float strength;

        public HeatMapValue(WorldTile tile, Priority priority, float strength) {
            this.tile = tile;
            this.priority = priority;
            this.strength = strength;
        }

        public float GetWeightedStrength() {
            return strength * GetPriorityPercent();
        }

        private float GetPriorityPercent() {
            return (int)priority / 100f;
        }
    }

    public MapController mapController;

    private Dictionary<Vector3Int, HeatMapValue> heatMap = new Dictionary<Vector3Int, HeatMapValue>();

    public override void AddTile(WorldTile tile) {
        base.AddTile(tile);

        heatMap.Add(tile.LocalPlace, new HeatMapValue(tile, Priority.High, CalculateTileStrength(tile)));
    }

    public void UpdateTileAndNeighbors(WorldTile tile) {
        UpdateTile(tile);

        List<WorldTile> neighbors = mapController.GetNeighbors(tile);
        foreach (WorldTile neighbor in neighbors) {
            UpdateTile(neighbor);
        }
    }

    public void UpdateTile(WorldTile tile) {
        HeatMapValue heatMapValue;
        if (heatMap.TryGetValue(tile.LocalPlace, out heatMapValue)) {
            heatMapValue.strength = CalculateTileStrength(tile);
        }
    }

    public WorldTile GetNextReinforcementTile() {
        KeyValuePair<Vector3Int, HeatMapValue> nextTile = default(KeyValuePair<Vector3Int, HeatMapValue>);

        foreach (KeyValuePair<Vector3Int, HeatMapValue> heatTile in heatMap) {
            if (nextTile.Equals(default(KeyValuePair<Vector3Int, HeatMapValue>)) || nextTile.Value.GetWeightedStrength() < heatTile.Value.GetWeightedStrength()) {
                nextTile = heatTile;
            }
        }

        return nextTile.Value.tile;
    }

    public Army GetNextArmy() {
        armies.Sort((x, y) => y.armySize.CompareTo(x.armySize));

        foreach (Army army in armies) {
            if (army.CanMove()) return army;
        }

        return null;
    }

    private float CalculateTileStrength(WorldTile tile) {
        List<WorldTile> neighbors = mapController.GetNeighbors(tile);

        float totalStrength = tile.army ? tile.army.armySize : 0;

        foreach (WorldTile neighbor in neighbors) {
            if (neighbor.army) {
                if (ControlsTile(neighbor)) {
                    totalStrength += neighbor.army.armySize;
                } else {
                    totalStrength -= neighbor.army.armySize;
                }
            }
        }

        //return Mathf.Lerp(0f, 1f, Mathf.Clamp(totalStrength, -50, 50) / 50);
        return totalStrength;
    }
}

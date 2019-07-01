using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Army : MonoBehaviour {

    [Serializable]
    public class TileEvent : UnityEvent<WorldTile> { }

    public int defaultMovement = 1;
    public int armySize;
    public Text armySizeDisplay;
    public Image backgroundPanel;
    public List<Strategy> strategies;
    public TileEvent captureTile;
    
    private int movement;

    private void Awake() {
        armySizeDisplay.text = armySize.ToString();
        movement = defaultMovement;
    }

    public void SetArmySize(int newSize) {
        if (newSize <= 0) {
            Destroy(gameObject);
        }

        armySize = newSize;
        armySizeDisplay.text = armySize.ToString();
    }

    public void MoveToTile(WorldTile tile) {
        movement--;
        transform.position = new Vector3(tile.WorldLocation.x, tile.WorldLocation.y, -9);
        captureTile.Invoke(tile);
    }

    public void ResetMovement() {
        movement = defaultMovement;
    }

    public bool CanMove() {
        return movement > 0;
    }

    public void AddStrategies(List<Strategy> newStrategies) {
        strategies.AddRange(newStrategies);
    }

    /*
     * Partial code to track movement at unit level
     * 
    private Dictionary<int, int> unitMovements;
    private void SetUnitMovement(int toAdd) {
        SetUnitMovement(toAdd, defaultMovement);
    }

    private void SetUnitMovement(int toAdd, int movement) {
        if (unitMovements.ContainsKey(movement)) {
            unitMovements[movement] = unitMovements[movement] + toAdd;
        } else {
            unitMovements.Add(movement, toAdd);
        }
    }

    public bool CanMove() {
        int count = 0;
        foreach (KeyValuePair<int, int> entry in unitMovements) {
            if (entry.Key > 0) count += entry.Value;
        }
        return count > 0;
    }

    public void ResetMovement() {
        unitMovements = new Dictionary<int, int>();
        SetUnitMovement(armySize);
    }
    */
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ArmyMap : MonoBehaviour {

    [Serializable]
    public class TileEvent : UnityEvent<WorldTile> { }

    public int armySize;
    public Text armySizeDisplay;
    public Image backgroundPanel;
    public List<Strategy> strategies;
    public TileEvent captureTile;

    private void Awake() {
        armySizeDisplay.text = armySize.ToString();
    }

    public void UpdateArmySize(int newSize) {
        if (newSize <= 0) {
            Destroy(gameObject);
        }

        armySize = newSize;
        armySizeDisplay.text = armySize.ToString();
    }

    public void MoveToTile(WorldTile tile) {
        transform.position = new Vector3(tile.WorldLocation.x, tile.WorldLocation.y, -9);
        captureTile.Invoke(tile);
    }

    public void AddStrategies(List<Strategy> newStrategies) {
        strategies.AddRange(newStrategies);
    }
}

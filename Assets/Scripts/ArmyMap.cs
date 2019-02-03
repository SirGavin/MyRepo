using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class ArmyMap : MonoBehaviour {

    public int armySize;
    public Text armySizeDisplay;
    public Image backgroundPanel;
    public List<Strategy> strategies;
    //public Tile borderTile;

    private Tile _borderTile;
    public Tile borderTile {
        get { return _borderTile; }
        set {
            _borderTile = Instantiate(value);
        }
    }

    private Color playerColor;
    private Tile playerBorderTile;

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
        playerBorderTile.color = playerColor;
        tile.SetPlayerBorderTile(borderTile);
    }

    public void AddStrategies(List<Strategy> newStrategies) {
        strategies.AddRange(newStrategies);
    }

    public Color GetPlayerColor() { return playerColor; }
    public void SetPlayerColor(Color color) {
        playerColor = color;
        backgroundPanel.color = color;
        //borderTile.color = playerColor;
    }

    public Tile GetPlayerBorderTile() { return playerBorderTile; }
    public void SetBorderTile(Tile tile) {
        playerBorderTile = tile;
    }
}

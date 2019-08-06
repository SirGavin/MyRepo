using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

[Serializable]
public class Player {

    [Serializable]
    public class PlayerEvent : UnityEvent<Player> { }

    public List<Strategy> strategies;
    public Color color;
    public int playerNum;

    public Tile borderTile { get; set; }
    public Tile hovorTile { get; set; }
    public Tile selectedTile { get; set; }

    protected List<Army> armies;
    protected List<WorldTile> ownedTiles;

    private const float MinReinforcements = 3;
    private GameObject armyPrefab;
    private PlayerEvent playerLost = new PlayerEvent();

    public Player() {}
    public Player(int playerNum, Color color, List<Strategy> defaultStrategies, GameObject armyPrefab, Tile borderTile, Tile highlightTile, UnityAction<Player> playerLostCallback) {
        this.playerNum = playerNum;
        this.color = color;
        strategies = defaultStrategies;
        this.armyPrefab = armyPrefab;
        playerLost.AddListener(playerLostCallback);

        InitTiles(borderTile, highlightTile);
    }

    public void Init(Color color, List<Strategy> defaultStrategies, GameObject armyPrefab, Tile borderTile, Tile highlightTile) {
        this.color = color;
        strategies = defaultStrategies;
        this.armyPrefab = armyPrefab;

        InitTiles(borderTile, highlightTile);
    }

    private void InitTiles(Tile borderTile, Tile highlightTile) {
        this.borderTile = UnityEngine.Object.Instantiate(borderTile);
        this.borderTile.color = color;

        hovorTile = UnityEngine.Object.Instantiate(highlightTile);
        color.a = 0.25f;
        hovorTile.color = color;

        selectedTile = UnityEngine.Object.Instantiate(highlightTile);
        color.a = 0.5f;
        selectedTile.color = color;
    }

    public Army CreateArmy(int armySize) {
        GameObject armyObj = GameObject.Instantiate(armyPrefab);
        Army army = armyObj.GetComponent<Army>();
        army.AddStrategies(strategies);
        army.SetArmySize(armySize);
        army.captureTile.AddListener(CaptureTile);
        AddArmy(army);

        return army;
    }

    private void CaptureTile(WorldTile tile) {
        tile.Capture(borderTile, RemoveTile);
        AddTile(tile);
    }

    public void AddArmy(Army newArmy) {
        if (armies == null) armies = new List<Army>();
        
        armies.Add(newArmy);
    }

    virtual public void AddTile(WorldTile tile) {
        if (ownedTiles == null) ownedTiles = new List<WorldTile>();

        if (!ControlsTile(tile)) ownedTiles.Add(tile);
    }

    public void RemoveTile(WorldTile tile) {
        if (ownedTiles == null) return;

        ownedTiles.Remove(tile);

        if (ownedTiles.Count == 0) {
            playerLost.Invoke(this);
            playerLost.RemoveAllListeners();
        }
    }

    public List<WorldTile> GetTiles() {
        return ownedTiles;
    }

    public int GetReinforcementCount() {
        return Mathf.FloorToInt(Mathf.Max(ownedTiles.Count / 2f, MinReinforcements));
    }

    public void ResetArmies() {
        foreach (Army army in armies) {
            army.ResetMovement();
        }
    }

    public bool ControlsTile(WorldTile tile) {
        foreach (WorldTile ownedTile in ownedTiles) {
            if (ownedTile.Equals(tile)) return true;
        }

        return false;
    }

    public bool ControlsArmy(Army army) {
        foreach (Army ownedArmy in armies) {
            if (ownedArmy == army) return true;
        }

        return false;
    }
}

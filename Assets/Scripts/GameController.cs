using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour {

    public GameObject armyPrefab;

    public BattleController battleController;
    public ArmyTransferController transferController;
    public MapController mapController;

    public List<Player> players;
    public List<Strategy> defaultStrategies;

    public Tile borderTile;
    public Tile highlightTile;

    private int currentPlayerIndex;
    private Player currentPlayer;

    private void Awake() {
        mapController.ProcessWorldTiles();
        GeneratePlayers();
        SetMapTiles();
    }

    private void GeneratePlayers() {
        foreach (Player player in players) {
            player.InitTiles(borderTile, highlightTile);

            GameObject armyObj = Instantiate(armyPrefab);
            ArmyMap army = armyObj.GetComponent<ArmyMap>();
            army.UpdateArmySize(50);
            army.AddStrategies(defaultStrategies);
            player.AddArmy(army);
            mapController.RandomlyPlaceArmy(army);

            if (currentPlayer == null) {
                currentPlayer = player;
                currentPlayerIndex = 0;
            }
        }
    }

    private void SetMapTiles() {
        mapController.highlightTile = currentPlayer.hovorTile;
        mapController.selectTile = currentPlayer.selectedTile;
    }

    private void Update() {
        Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(1) && Input.GetKey(KeyCode.LeftShift) && mapController.CanMove()) {
            ArmyMap selectedArmy;
            ArmyMap occupyingArmy;
            mapController.GetArmies(out selectedArmy, out occupyingArmy);

            int occupyingArmySize = occupyingArmy == null ? 0 : occupyingArmy.armySize;
            
            enabled = false;
            transferController.SetValues(selectedArmy.armySize, occupyingArmySize);
        } else if (Input.GetMouseButtonDown(1) && mapController.CanMove()) {
            if (!mapController.TryMove()) {
                ArmyMap selectedArmy;
                ArmyMap occupyingArmy;
                mapController.GetArmies(out selectedArmy, out occupyingArmy);
                if (selectedArmy != null && occupyingArmy != null) {
                    enabled = false;
                    battleController.StartBattle(selectedArmy, occupyingArmy);
                }
            }
        } else if (Input.GetMouseButtonDown(0)) {
            mapController.TrySelect();
        } else {
            mapController.TryHover(point);
        }
    }

    public void ResolveBattle(bool attackerWon) {
        if (attackerWon) {
            mapController.Push();
        }

        enabled = true;
    }

    public void ResolveTransfer(int leftSize, int rightSize) {
        ArmyMap selectedArmy;
        ArmyMap occupyingArmy;
        mapController.GetArmies(out selectedArmy, out occupyingArmy);

        if (occupyingArmy == null) {
            GameObject armyObj = Instantiate(armyPrefab);
            occupyingArmy = armyObj.GetComponent<ArmyMap>();
            occupyingArmy.AddStrategies(defaultStrategies);
            selectedArmy.player.AddArmy(occupyingArmy);
            mapController.PlaceArmy(occupyingArmy);
        }

        selectedArmy.UpdateArmySize(leftSize);
        occupyingArmy.UpdateArmySize(rightSize);

        enabled = true;
    }

    public void EndTurn() {
        currentPlayerIndex = currentPlayerIndex == players.Count - 1 ? 0 : ++currentPlayerIndex;
        currentPlayer = players[currentPlayerIndex];
        SetMapTiles();
    }
}

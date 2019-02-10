using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour {

    private enum TurnStages {
        ArmyPlacement,
        Attack
    }

    public GameObject armyPrefab;

    public BattleController battleController;
    public ArmyTransferController transferController;
    public MapController mapController;

    public List<Player> players;
    public List<Strategy> defaultStrategies;

    public Tile borderTile;
    public Tile highlightTile;
    
    private Player currentPlayer;
    private TurnStages currentTurnStage;
    private int reinforcementCount;

    private void Awake() {
        mapController.ProcessWorldTiles();
        GeneratePlayers();
        SetMapTiles();
        StartPlacementStage();
    }

    private void GeneratePlayers() {
        foreach (Player player in players) {
            currentPlayer = player;

            player.InitTiles(borderTile, highlightTile);
            
            ArmyMap army = InitializeArmy();
            army.SetArmySize(5);
            player.AddArmy(army);
            mapController.RandomlyPlaceArmy(army);
        }
    }

    private void SetMapTiles() {
        mapController.highlightTile = currentPlayer.hovorTile;
        mapController.selectTile = currentPlayer.selectedTile;
    }

    private ArmyMap InitializeArmy() {
        GameObject armyObj = Instantiate(armyPrefab);
        ArmyMap army = armyObj.GetComponent<ArmyMap>();
        army.AddStrategies(defaultStrategies);
        army.captureTile.AddListener(CaptureTile);

        return army;
    }

    private void StartPlacementStage() {
        currentTurnStage = TurnStages.ArmyPlacement;

        reinforcementCount = currentPlayer.GetReinforcementCount();
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
        } else if (Input.GetMouseButtonDown(0) && currentPlayer.ControlsTile(mapController.GetCurrentTile())) {
            if (currentTurnStage == TurnStages.ArmyPlacement) {
                WorldTile currentTile = mapController.GetCurrentTile();
                if (currentTile.army) {
                    currentTile.army.SetArmySize(currentTile.army.armySize + 1);
                } else {
                    ArmyMap army = InitializeArmy();
                    army.SetArmySize(1);
                    currentPlayer.AddArmy(army);
                    mapController.PlaceArmy(army);
                }

                reinforcementCount--;
                if (reinforcementCount == 0) {
                    currentTurnStage = TurnStages.Attack;
                }
            } else if (currentTurnStage == TurnStages.Attack) {
                mapController.TrySelect();
            }
        } else {
            mapController.TryHover(point);
        }
    }

    public void CaptureTile(WorldTile tile) {
        foreach (Player player in players) {
            if (player.playerNum != currentPlayer.playerNum) player.RemoveTile(tile);
        }

        tile.SetPlayerBorderTile(currentPlayer.borderTile);
        currentPlayer.AddTile(tile);
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
            occupyingArmy = InitializeArmy();
            currentPlayer.AddArmy(occupyingArmy);
            mapController.PlaceArmy(occupyingArmy);
        }

        selectedArmy.SetArmySize(leftSize);
        occupyingArmy.SetArmySize(rightSize);

        enabled = true;
    }

    public void EndTurn() {
        mapController.ClearSelection();

        int nextPlayerNum = currentPlayer.playerNum == players.Count ? 1 : currentPlayer.playerNum + 1;
        currentPlayer = players.Find(player => player.playerNum == nextPlayerNum);
        SetMapTiles();
        StartPlacementStage();
    }
}

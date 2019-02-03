using System.Collections;
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

    private void Awake() {
        mapController.ProcessWorldTiles();
        GeneratePlayers();
    }

    private void GeneratePlayers() {
        foreach (Player player in players) {
            player.SetBorderTile(Instantiate(borderTile));

            GameObject armyObj = Instantiate(armyPrefab);
            ArmyMap army = armyObj.GetComponent<ArmyMap>();
            army.UpdateArmySize(50);
            army.AddStrategies(defaultStrategies);
            player.AddArmy(army);
            mapController.RandomlyPlaceArmy(army);
        }
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
            occupyingArmy.SetPlayerColor(selectedArmy.GetPlayerColor());
            occupyingArmy.SetBorderTile(selectedArmy.GetPlayerBorderTile());
            mapController.PlaceArmy(occupyingArmy);
        }

        selectedArmy.UpdateArmySize(leftSize);
        occupyingArmy.UpdateArmySize(rightSize);

        enabled = true;
    }
}

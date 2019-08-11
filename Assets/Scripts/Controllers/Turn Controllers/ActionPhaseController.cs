using UnityEngine;

public class ActionPhaseController : PhaseController {

    public GameController gameController;
    public BattleController battleController;
    public ArmyTransferController transferController;

    override public void StartPhase(Player player) {
        gameObject.SetActive(true);

        this.player = player;
    }

    void Update () {
        if (Input.GetMouseButtonDown(1) && Input.GetKey(KeyCode.LeftShift) && mapController.CanMove()) {
            Army selectedArmy;
            Army occupyingArmy;
            mapController.GetArmies(out selectedArmy, out occupyingArmy);

            int occupyingArmySize = occupyingArmy == null ? 0 : occupyingArmy.armySize;
            
            enabled = false;
            mapController.enabled = false;
            transferController.SetValues(selectedArmy.armySize, occupyingArmySize);
        } else if (Input.GetMouseButtonDown(1) && mapController.CanMove()) {
            if (!mapController.TryMove()) {
                Army selectedArmy;
                Army occupyingArmy;
                mapController.GetArmies(out selectedArmy, out occupyingArmy);
                if (selectedArmy != null && occupyingArmy != null) {
                    enabled = false;
                    mapController.enabled = false;
                    battleController.StartBattle(player, selectedArmy, gameController.GetArmiesPlayer(occupyingArmy), occupyingArmy, ResolveBattle);
                }
            }
        }
    }

    private void ResolveBattle(bool attackerWon) {
        if (attackerWon) {
            mapController.Push();
        }

        enabled = true;
        mapController.enabled = true;
    }

    public void ResolveTransfer(int leftSize, int rightSize) {
        Army selectedArmy;
        Army occupyingArmy;
        mapController.GetArmies(out selectedArmy, out occupyingArmy);

        if (occupyingArmy == null) {
            occupyingArmy = player.CreateArmy(rightSize);
            mapController.GetCurrentTile().army = occupyingArmy;
        }

        selectedArmy.SetArmySize(leftSize);
        occupyingArmy.SetArmySize(rightSize);

        enabled = true;
        mapController.enabled = true;
    }
}

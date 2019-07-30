using System.Collections.Generic;
using UnityEngine;

public class AIActionPhaseController : PhaseController {

    public BattleController battleController;
    public ArmyTransferController transferController;

    private List<WorldTile> playerTiles;
    private List<WorldTile> enemyNeighbors;
    private List<WorldTile> ownedNeighbors;

    override public void StartPhase(Player player) {
        gameObject.SetActive(true);
        mapController.enabled = false;

        this.player = player;
        player.ResetArmies();
        //Put player tiles in a new list so we don't destroy the original
        playerTiles = new List<WorldTile>(this.player.GetTiles());
        CheckNextTile();
    }

    private void CheckNextTile() {
        if (playerTiles.Count > 0) {
            WorldTile playerTile = playerTiles[0];
            playerTiles.RemoveAt(0);
            mapController.SetSelectedTile(playerTile);

            Army army = playerTile.army;
            if (army && army.CanMove()) {
                ownedNeighbors = new List<WorldTile>();
                enemyNeighbors = new List<WorldTile>();
                
                foreach (WorldTile neighbor in mapController.GetNeighbors(playerTile)) {
                    if (player.ControlsTile(neighbor) && !neighbor.army) {
                        ownedNeighbors.Add(neighbor);
                    } else if (neighbor.IsPassable() && !player.ControlsTile(neighbor)) {
                        enemyNeighbors.Add(neighbor);
                    }
                }

                WorldTile tileToMoveTo = null;
                if (enemyNeighbors.Count > 0) {
                    tileToMoveTo = enemyNeighbors[Random.Range(0, enemyNeighbors.Count)];
                } else if (ownedNeighbors.Count > 0) {
                    tileToMoveTo = ownedNeighbors[Random.Range(0, ownedNeighbors.Count)];
                }
                if (tileToMoveTo != null) {
                    mapController.SetHovoredTile(tileToMoveTo);
                    if (!mapController.TryMove()) {
                        battleController.StartBattle(army, tileToMoveTo.army, ResolveBattle);
                        return;
                    }
                }
            }

            CheckNextTile();
        } else {
            mapController.enabled = true;
            EndPhase();
        }
    }

    private void ResolveBattle(bool attackerWon) {
        if (attackerWon) {
            mapController.Push();
        }

        CheckNextTile();
    }
}

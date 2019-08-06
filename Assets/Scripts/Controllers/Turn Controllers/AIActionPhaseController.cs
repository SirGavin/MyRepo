using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIActionPhaseController : PhaseController {

    public GameController gameController;
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
        //CheckNextTile();
        StartCoroutine(CheckNextTile());
    }

    private IEnumerator CheckNextTile() {
        yield return new WaitForSeconds(0.2f);
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
                        battleController.StartBattle(player, army, gameController.GetArmiesPlayer(tileToMoveTo.army), tileToMoveTo.army, ResolveBattle);
                        yield break;
                    }
                }
            }

            StartCoroutine(CheckNextTile());
        } else {
            mapController.enabled = true;
            EndPhase();
        }
    }

    private void ResolveBattle(bool attackerWon) {
        if (attackerWon) {
            mapController.Push();
        }

        StartCoroutine(CheckNextTile());
    }
}

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
        //Put player tiles in a new list so we don't destroy the original
        playerTiles = new List<WorldTile>(this.player.GetTiles());
        StartCoroutine(CheckNextTile());
    }

    private IEnumerator CheckNextTile() {
        if (playerTiles.Count > 0) {
            WorldTile playerTile = playerTiles[0];
            playerTiles.RemoveAt(0);

            Army army = playerTile.army;
            if (army && army.CanMove()) {
                ownedNeighbors = new List<WorldTile>();
                enemyNeighbors = new List<WorldTile>();
                
                foreach (WorldTile neighbor in mapController.GetNeighbors(playerTile)) {
                    if (player.ControlsTile(neighbor) && !neighbor.army) {
                        ownedNeighbors.Add(neighbor);
                    } else if (!player.ControlsTile(neighbor)) {
                        enemyNeighbors.Add(neighbor);
                    }
                }

                WorldTile tileToMoveTo = null;
                if (enemyNeighbors.Count > 0) {
                    tileToMoveTo = enemyNeighbors[Random.Range(0, enemyNeighbors.Count)];
                } else if (ownedNeighbors.Count > 0) {
                    tileToMoveTo = ownedNeighbors[Random.Range(0, ownedNeighbors.Count)];
                }

                if (tileToMoveTo != null && !mapController.TryMove(playerTile, tileToMoveTo)) {
                    battleController.StartBattle(player, army, gameController.GetArmiesPlayer(tileToMoveTo.army), tileToMoveTo.army,
                        (bool attackerWon) => ResolveBattle(playerTile, tileToMoveTo, attackerWon));
                    yield return new WaitForSeconds(0.2f);
                    yield break;
                } else {
                    yield return new WaitForSeconds(0.2f);
                    StartCoroutine(CheckNextTile());
                    yield break;
                }
            } else {
                yield return new WaitForSeconds(0.2f);
                StartCoroutine(CheckNextTile());
                yield break;
            }
        } else {
            mapController.enabled = true;
            EndPhase();
        }
    }

    private void ResolveBattle(WorldTile pusher, WorldTile pushee, bool attackerWon) {
        if (attackerWon) {
            mapController.Push(pusher, pushee);
        }
        
        StartCoroutine(CheckNextTile());
    }
}

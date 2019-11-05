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
    private List<WorldTile> borderTiles;

    override public void StartPhase(Player player) {
        gameObject.SetActive(true);
        mapController.enabled = false;

        this.player = player;
        //Put player tiles in a new list so we don't destroy the original
        playerTiles = new List<WorldTile>(this.player.GetTiles());
        FindBorderTiles();
        StartCoroutine(CheckNextTile());
    }

    private IEnumerator CheckNextTile() {
        while (playerTiles.Count > 0) {
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

                //playerTile.ColorBlack();
                yield return new WaitForSeconds(0.2f);
                //playerTile.ResetColor();
                if (enemyNeighbors.Count > 0) {
                    WorldTile tileToMoveTo = enemyNeighbors[Random.Range(0, enemyNeighbors.Count)];
                    
                    if ((tileToMoveTo.army == null || army.armySize >= tileToMoveTo.army.armySize ) && !mapController.TryMove(playerTile, tileToMoveTo)) {
                        battleController.StartBattle(player, army, gameController.GetArmiesPlayer(tileToMoveTo.army), tileToMoveTo.army,
                            (bool attackerWon) => ResolveBattle(playerTile, tileToMoveTo, attackerWon));
                        yield break;
                    }
                } else if (ownedNeighbors.Count > 0) {
                    WorldTile tileToMoveTo = GetTileToMove(playerTile, borderTiles);

                    //playerTile.ColorBlack();
                    //tileToMoveTo.ColorBlack();
                    //yield return new WaitForSeconds(0.5f);
                    //playerTile.ResetColor();
                    //tileToMoveTo.ResetColor();

                    if (tileToMoveTo.army == null) {
                        mapController.TryMove(playerTile, tileToMoveTo);
                    } else {
                        tileToMoveTo.army.SetArmySize(tileToMoveTo.army.armySize + playerTile.army.armySize);
                        playerTile.army.SetArmySize(0);
                    }

                    //tileToMoveTo = ownedNeighbors[Random.Range(0, ownedNeighbors.Count)];
                }

                /*
                if (tileToMoveTo != null && !mapController.TryMove(playerTile, tileToMoveTo)) {
                    yield return new WaitForSeconds(0.2f);
                    battleController.StartBattle(player, army, gameController.GetArmiesPlayer(tileToMoveTo.army), tileToMoveTo.army,
                        (bool attackerWon) => ResolveBattle(playerTile, tileToMoveTo, attackerWon));
                    yield break;
                } else {
                    yield return new WaitForSeconds(0.2f);
                    StartCoroutine(CheckNextTile());
                }
                */
            }
        }

        mapController.enabled = true;
        EndPhase();
    }

    //TODO: extrapolate this into a common location
    private void FindBorderTiles() {
        borderTiles = new List<WorldTile>();
        foreach (WorldTile ownedTile in player.GetTiles()) {
            foreach (WorldTile neighbor in mapController.GetNeighbors(ownedTile)) {
                if (!player.ControlsTile(neighbor)) {
                    borderTiles.Add(ownedTile);
                    break;
                }
            }
        }
    }

    //TODO: moves to nearest border, not neighbor tile on the way to the border
    private WorldTile GetTileToMove(WorldTile tile, List<WorldTile> ownedTiles) {
        int shortestDistance = int.MaxValue;
        WorldTile foundTile = null;

        foreach (WorldTile ownedTile in ownedTiles) {
            int distance = HexUtils.GetDistance(tile.LocalPlace, ownedTile.LocalPlace);
            if (distance == 1) return ownedTile;
            if (distance < shortestDistance) {
                shortestDistance = distance;
                foundTile = ownedTile;
            }
        }

        return foundTile;
    }

    private void ResolveBattle(WorldTile pusher, WorldTile pushee, bool attackerWon) {
        if (attackerWon) {
            mapController.Push(pusher, pushee);
        }
        
        StartCoroutine(CheckNextTile());
    }
}

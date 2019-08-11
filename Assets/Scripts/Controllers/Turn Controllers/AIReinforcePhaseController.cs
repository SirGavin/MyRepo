using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class AIReinforcePhaseController : PhaseController {

    public Text reinforceCounter;

    private int reinforcementCount;
    private List<WorldTile> borderTiles;

    override public void StartPhase(Player player) {
        gameObject.SetActive(true);

        this.player = player;
        reinforcementCount = player.GetReinforcementCount();

        reinforceCounter.color = player.color;
        UpdateReinforceText();

        //float start = Time.realtimeSinceStartup;
        FindBorderTiles();
        //UnityEngine.Debug.Log("find boarder time: " + (Time.realtimeSinceStartup - start));

        StartCoroutine(Reinforce());
    }

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

    private IEnumerator Reinforce() {
        while (reinforcementCount > 0) {
            int ranIndex = UnityEngine.Random.Range(0, borderTiles.Count);
            WorldTile tile = borderTiles[ranIndex]; //((AIPlayer)player).GetRandomBorderTile();
            PlaceReinforcements(tile, 1);
            yield return new WaitForSeconds(0.1f);
        }

        EndPhase();
    }

    public void PlaceReinforcements(WorldTile tile, int reinforceAmount) {
        if (tile.army) {
            tile.army.SetArmySize(tile.army.armySize + reinforceAmount);
        } else {
            Army army = player.CreateArmy(reinforceAmount);
            tile.army = army;
        }

        reinforcementCount -= reinforceAmount;
        UpdateReinforceText();
    }

    private void UpdateReinforceText() {
        reinforceCounter.text = reinforcementCount.ToString();
    }
}

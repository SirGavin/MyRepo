using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AIController : MonoBehaviour {

    //TODO: rewrite this, it's shit

    [Serializable]
    public class TileIntEvent : UnityEvent<WorldTile, int> { }

    public TileIntEvent reinforceTile;
    public UnityEvent endTurn;

    private int reinforcementCount;

    public void DoAITurn(AIPlayer aiPlayer) {
        ReinforceArmies(aiPlayer);
        MoveArmies(aiPlayer);

        endTurn.Invoke();
    }

    private void ReinforceArmies(AIPlayer aiPlayer) {
        reinforcementCount = aiPlayer.GetReinforcementCount();

        while (reinforcementCount > 0) {
            WorldTile tileToReinforce = aiPlayer.GetNextReinforcementTile();

            reinforceTile.Invoke(tileToReinforce, GetReinforceAmount());
            aiPlayer.UpdateTileAndNeighbors(tileToReinforce);
        }
    }

    private int GetReinforceAmount() {
        int reinforceAmount = Mathf.CeilToInt(reinforcementCount / 2f);
        reinforcementCount -= reinforceAmount;
        return reinforceAmount;
    }

    private void MoveArmies(AIPlayer aiPlayer) {
        Army army = aiPlayer.GetNextArmy();
        do {
            army = aiPlayer.GetNextArmy();
        } while (army != null);
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AIReinforcePhaseController : PhaseController {

    public Text reinforceCounter;

    private int reinforcementCount;

    override public void StartPhase(Player player) {
        gameObject.SetActive(true);

        this.player = player;
        reinforcementCount = player.GetReinforcementCount();

        reinforceCounter.color = player.color;
        UpdateReinforceText();
        StartCoroutine(Reinforce());
    }

    private IEnumerator Reinforce() {
        while (reinforcementCount > 0) {
            WorldTile tile = ((AIPlayer)player).GetRandomOwnedTile();
            PlaceReinforcements(tile, 1);
            yield return new WaitForSeconds(0.2f);
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
        reinforceCounter.text = reinforcementCount.ToString(); ;
    }
}

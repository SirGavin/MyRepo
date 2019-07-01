using UnityEngine;
using UnityEngine.UI;

public class ReinforcePhaseController : PhaseController {

    public Text reinforceCounter;

    private int reinforcementCount;

    override public void StartPhase(Player player) {
        gameObject.SetActive(true);

        this.player = player;
        reinforcementCount = player.GetReinforcementCount();
        
        reinforceCounter.color = player.color;
        UpdateReinforceText();
    }
	
	void Update () {
        if (Input.GetMouseButtonDown(0) && player.ControlsTile(mapController.GetCurrentTile())) {
            PlaceReinforcements(mapController.GetCurrentTile(), 1);
        }
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
        if (reinforcementCount == 0) {
            EndPhase();
        }
    }

    private void UpdateReinforceText() {
        reinforceCounter.text = reinforcementCount.ToString(); ;
    }
}

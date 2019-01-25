using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public MapController mapController;
    public List<Player> players;
    public List<Strategy> defaultStrategies;

    public void GeneratePlayers() {
        foreach (Player player in players) {
            ArmyMap newArmy = mapController.GetRandomArmy();
            newArmy.UpdateArmySize(50);
            newArmy.AddStrategies(defaultStrategies);
            player.AddArmy(newArmy);
        }
    }
}

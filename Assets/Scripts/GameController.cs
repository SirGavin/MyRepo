using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public MapController mapController;
    public List<Strategy> defaultStrategies;
    public int playerCount;

    private List<Player> players;

    private void Awake() {
        players = new List<Player>(playerCount);
    }

    public void GeneratePlayers() {
        for (int i = 0; i < playerCount; i++) {
            Player player = new Player();
            player.AddArmy(mapController.GetRandomArmy());
        }
    }
}

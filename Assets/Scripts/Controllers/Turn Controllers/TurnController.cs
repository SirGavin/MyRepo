using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TurnController : MonoBehaviour {

    //TODO: change how endTurn is set?
    public UnityEvent endTurn;
    public List<PhaseController> phaseControllers;

    private Player player;
    private int currentPhase;

    public void StartTurn(Player player) {
        this.player = player;
        currentPhase = -1; //NextPhase will increment to 0

        NextPhase();
    }

    public void NextPhase() {
        currentPhase += 1;

        if (currentPhase == phaseControllers.Count) {
            endTurn.Invoke();
        } else {
            phaseControllers.ToArray()[currentPhase].StartPhase(player);
        }
    }
}

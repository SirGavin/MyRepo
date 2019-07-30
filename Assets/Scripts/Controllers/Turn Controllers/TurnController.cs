using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TurnController : MonoBehaviour {

    //TODO: change how endTurn is set?
    public UnityEvent endTurn;
    public List<PhaseController> phaseControllers;
    public List<PhaseController> aiPhaseControllers;

    private Player player;
    private List<PhaseController> phases;
    private int currentPhase;

    public void StartTurn(Player player) {
        this.player = player;
        currentPhase = -1; //NextPhase will increment to 0

        if (player is AIPlayer) {
            phases = aiPhaseControllers;
        } else {
            phases = phaseControllers;
        }

        NextPhase();
    }

    public void NextPhase() {
        currentPhase += 1;

        if (currentPhase == phases.Count) {
            endTurn.Invoke();
        } else {
            phases.ToArray()[currentPhase].StartPhase(player);
        }
    }
}

using UnityEngine;
using UnityEngine.Events;

public abstract class PhaseController: MonoBehaviour {

    public MapController mapController;
    public UnityEvent nextPhase;

    protected Player player;

    abstract public void StartPhase(Player player);

    public void EndPhase() {
        gameObject.SetActive(false);

        nextPhase.Invoke();
    }
}

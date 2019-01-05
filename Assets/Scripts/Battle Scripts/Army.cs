using UnityEngine;
using UnityEngine.UI;

public class Army : MonoBehaviour {

    public float armySize = 0;
    public StrategySO selectedStrategy;

    public StrategySO[] strategies = new StrategySO[0];

    private void Awake()
    {
        selectedStrategy = strategies[0];
    }

    public void UpdateArmySize(float newSize)
    {
        armySize = newSize;
    }

    public void SetStrategy(int strategyId)
    {
        StrategyButton stratBtn = selectedStrategy.gameObject.GetComponent<StrategyButton>();
        stratBtn.Deselect();

        selectedStrategy = strategies[strategyId - 1];
        stratBtn = selectedStrategy.gameObject.GetComponent<StrategyButton>();
        stratBtn.Select();
    }
}

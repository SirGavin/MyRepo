using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StrategyPanel : MonoBehaviour {

    [Serializable]
    public class StratEvent : UnityEvent<Strategy> { }

    public GameObject stratPrefab;
    public HorizontalLayoutGroup stratPane;
    public StrategyButton selectedStrat;

    private List<Strategy> strategies;
    private StratEvent setStrat = new StratEvent();

    public void GenerateUI(Army army, UnityAction<Strategy> setStrat) {
        this.setStrat.AddListener(setStrat);

        Clear();
        foreach (Strategy strat in army.strategies) {
            AddStrategy(strat);
        }
    }

    private void Clear() {
        strategies = new List<Strategy>();

        for (int i = 0; i < transform.childCount; i++) {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    private void AddStrategy(Strategy strat) {
        GameObject stratObj = Instantiate(stratPrefab);
        StrategyButton stratBtn = stratObj.GetComponent<StrategyButton>();
        stratBtn.SetStrategy(strat, SelectStrategy);
        stratObj.transform.SetParent(stratPane.transform);
    }

    private void SelectStrategy(StrategyButton stratBtn) {
        if (selectedStrat != null) selectedStrat.Deselect();

        selectedStrat = stratBtn;
        selectedStrat.Select();

        setStrat.Invoke(stratBtn.strat);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StrategyPanel : MonoBehaviour {

    public GameObject stratPrefab;
    public HorizontalLayoutGroup stratPane;
    public StrategyButton selectedStrat;

    private List<Strategy> strategies;

    public void Clear() {
        strategies = new List<Strategy>();

        for (int i = 0; i < transform.childCount; i++) {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    public void AddStrategy(Strategy strat) {
        GameObject stratObj = Instantiate(stratPrefab);
        StrategyButton stratBtn = stratObj.GetComponent<StrategyButton>();
        stratBtn.SetStrategy(strat, SelectStrategy);
        stratObj.transform.SetParent(stratPane.transform);
    }

    public Strategy GetSelected() {
        return selectedStrat.strat;
    }

    private void SelectStrategy(StrategyButton stratBtn) {
        if (selectedStrat != null) selectedStrat.Deselect();

        selectedStrat = stratBtn;
        selectedStrat.Select();
    }
}

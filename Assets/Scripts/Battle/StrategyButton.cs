using System;
using UnityEngine;
using UnityEngine.UI;

public class StrategyButton : MonoBehaviour {

    public Sprite deselectedSprite;
    public Sprite selectedSprite;
    public Image backgroundImg;
    public Image strategyImg;

    private Button btn;
    public Strategy strat;

    private void Awake() {
        btn = GetComponent<Button>();
    }

    public void Deselect()
    {
        backgroundImg.sprite = deselectedSprite;
    }

    public void Select()
    {
        backgroundImg.sprite = selectedSprite;
    }

    public void SetStrategy(Strategy strategy, Action<StrategyButton> selectCallBack) {
        strat = strategy;
        strategyImg.sprite = strategy.strategyImg;

        btn.onClick.AddListener(delegate {
            selectCallBack(this);
        });
    }
}

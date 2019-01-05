using UnityEngine;
using UnityEngine.UI;

public class StrategyButton : MonoBehaviour {

    public Sprite deselectedSprite;
    public Sprite selectedSprite;
    public Image backgroundImg;

    public void Deselect()
    {
        backgroundImg.sprite = deselectedSprite;
    }

    public void Select()
    {
        backgroundImg.sprite = selectedSprite;
    }
}

using UnityEngine;
using UnityEngine.UI;

public class TileObject : MonoBehaviour {

    public Army army;
    public Sprite selectedSprite;
    public Image backgroundImg;

    public void Deselect()
    {
        backgroundImg.sprite = null;
    }

    public void Select()
    {
        backgroundImg.sprite = selectedSprite;
    }

    public void UpdateArmy(Army newArmy)
    {
        army = newArmy;
    }
}

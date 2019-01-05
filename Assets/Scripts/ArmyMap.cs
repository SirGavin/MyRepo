using UnityEngine;
using UnityEngine.UI;

public class ArmyMap : MonoBehaviour {

    public int armySize;
    public Text armySizeDisplay;

    private void Awake() {
        armySizeDisplay.text = armySize.ToString();
    }

    public void UpdateArmySize(int newSize) {
        armySize = newSize;
        armySizeDisplay.text = armySize.ToString();
    }

    public void MoveToTile(WorldTile tile) {
        transform.position = new Vector3(tile.WorldLocation.x, tile.WorldLocation.y, -9);
    }
}

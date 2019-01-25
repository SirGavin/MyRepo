using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArmyTransferController : MonoBehaviour {

    public DoubleSidedSliderTextUpdater sliderUpdater;
    public MapController mapController;

    private ArmyMap leftArmy;
    private ArmyMap rightArmy;

    public void SetArmies(ArmyMap left, ArmyMap right) {
        gameObject.SetActive(true);

        leftArmy = left;
        rightArmy = right;

        sliderUpdater.SetValues(leftArmy.armySize + rightArmy.armySize, rightArmy.armySize);
    }

    public void Transfer() {
        leftArmy.UpdateArmySize(Mathf.RoundToInt(sliderUpdater.slider.maxValue - sliderUpdater.slider.value));
        rightArmy.UpdateArmySize(Mathf.RoundToInt(sliderUpdater.slider.value));
        gameObject.SetActive(false);
        mapController.enabled = true;
    }
}

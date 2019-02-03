﻿using System;
using UnityEngine;
using UnityEngine.Events;

public class ArmyTransferController : MonoBehaviour {

    [Serializable]
    public class IntIntEvent : UnityEvent<int, int> { }

    public IntIntEvent resolveTransfer;
    public DoubleSidedSliderTextUpdater sliderUpdater;

    public void SetValues(int leftSize, int rightSize) {
        gameObject.SetActive(true);

        sliderUpdater.SetValues(leftSize + rightSize, rightSize);
    }

    public void Transfer() {
        resolveTransfer.Invoke(sliderUpdater.GetLeftValue(), sliderUpdater.GetRightValue());

        gameObject.SetActive(false);
    }
}

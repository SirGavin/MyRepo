using UnityEngine;
using UnityEngine.UI;

public class DoubleSidedSliderTextUpdater : MonoBehaviour {

    public Slider slider;
    public Text leftSliderText;
    public Text rightSliderText;
    
    public void SetValues(int maxValue, int value) {
        slider.maxValue = maxValue;
        slider.value = value;
    }

    public void SetSliderValue(float newSliderVal) {
        rightSliderText.text = newSliderVal.ToString("0");
        leftSliderText.text = (slider.maxValue - newSliderVal).ToString("0");
    }

    public int GetLeftValue() {
        return Mathf.RoundToInt(slider.maxValue - slider.value);
    }

    public int GetRightValue() {
        return Mathf.RoundToInt(slider.value);
    }
}

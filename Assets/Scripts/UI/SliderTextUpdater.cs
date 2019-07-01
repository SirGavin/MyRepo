using UnityEngine;
using UnityEngine.UI;

public class SliderTextUpdater : MonoBehaviour {

    public Text sliderText;

    private void Awake()
    {
        sliderText.text = "0";
    }

    public void SetSliderValue(float newSliderVal){
        sliderText.text = newSliderVal.ToString("0");
    }
}

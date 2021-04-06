using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LEDControlPanel : MonoBehaviour
{
    public Slider ColorH_Slider;
    public Slider ColorV_Slider;

    static public Color ControlColor { get; private set; } = Color.black;

    private void Update()
    {
        ControlColor = Color.HSVToRGB(ColorH_Slider.value, 1, ColorV_Slider.value);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LEDControlPanel : MonoBehaviour
{
    public Slider ColorR_Slider;
    public Slider ColorG_Slider;
    public Slider ColorB_Slider;
    public Slider ColorA_Slider;

    static public Color ControlColor { get; private set; } = Color.black;

    private void Update()
    {
        ControlColor = new Color(ColorR_Slider.value, ColorG_Slider.value, ColorB_Slider.value, ColorA_Slider.value);
    }

}

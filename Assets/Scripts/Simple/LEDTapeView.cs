using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LEDTapeView : MonoBehaviour
{
    private Image LEDView_Image;
    private Sprite sprite_led; //Image用のSprite
    private Texture2D texture2D_led;

    private Color[] LEDColors;

    // LED Info
    private int NumOfLEDs; //LEDの個数

    public void Setup(int num_of_leds)
    {
        LEDView_Image = transform.Find("LEDColorView").GetComponent<Image>();

        NumOfLEDs = num_of_leds; //LEDの数を設定

        LEDColors = new Color[num_of_leds];

        SetColor(new Color(0.5f, 0.5f, 0.5f));
    }

    public void SetColor(Color color)
    {
        for (int i = 0; i < NumOfLEDs; i++)
        {
            LEDColors.SetValue(color, i);
        }

        texture2D_led = new Texture2D(NumOfLEDs, 1);
        texture2D_led.filterMode = FilterMode.Point;
        texture2D_led.SetPixels(LEDColors);
        texture2D_led.Apply();

        sprite_led = Sprite.Create(texture2D_led, new Rect(0, 0, texture2D_led.width, texture2D_led.height), new Vector2(0.5f, 0.5f));
        LEDView_Image.sprite = sprite_led;
    }

    public void ApplyColor()
    {

    }
}

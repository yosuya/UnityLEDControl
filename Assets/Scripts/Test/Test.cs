using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yosuya.UnityLEDControl;

public class Test : MonoBehaviour
{
    protected LEDCam _ledCam;

    void Start()
    {
        _ledCam = GameObject.Find("LEDCam").GetComponent<LEDCam>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            var tex2d = _ledCam.ReadPixels();
            var color = ColorPicker.Pick(tex2d, new Vector2(0, 0));
            Debug.Log(color);
        }
    }
}

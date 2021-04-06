using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LEDSimpleController : MonoBehaviour
{
    private int NumOfTapes = 8; //LEDテープの数
    private int NumOfLEDs = 60; //LEDの個数


    void Start()
    {
        LEDTapeViewGroup.Instance.Setup(NumOfTapes, NumOfLEDs);
    }

    void Update()
    {
        LEDTapeViewGroup.Instance.SetColor(LEDControlPanel.ControlColor);
    }
}

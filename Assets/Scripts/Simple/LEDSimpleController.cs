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

        if (SerialController.isConnected) SendData();
    }


    private Vector3Int prevColor = Vector3Int.zero;
    void SendData()
    {
        if (LEDTapeViewGroup.Instance == null) return;

        Color curColor = LEDTapeViewGroup.Instance.GetColor();
        Vector3Int sendColor = new Vector3Int
        (
            (int)(curColor.r * 255 / 8),
            (int)(curColor.g * 255 / 8),
            (int)(curColor.b * 255 / 8)
        );

        if (prevColor == sendColor) return;
        prevColor = sendColor;

        int idx = 0;
        string send_data = $"{idx.ToString("000")}{sendColor.x.ToString("00")}{sendColor.y.ToString("00")}{sendColor.z.ToString("00")}"; //idは3桁、色は2桁ずつ
        SerialController.Write(send_data);
        Debug.Log(send_data);
    }
}

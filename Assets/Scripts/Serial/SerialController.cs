using System;
using WinRPiSerial;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

public class SerialController : MonoBehaviour
{
    private static SerialController Instance = null;

    public static bool isConnected { get; private set; } = false;
    WinSerial serial = null;

    /// UI
    public TMP_Dropdown Device_Dropdown;
    public Button Play_Button;
    private TMP_Text DeviceName_Text;

    string selectedDevice = String.Empty;


    public void Setup()
    {
        Instance = this;

        //ドロップダウンのリストを更新するためのイベントを設定
        EventTrigger trigger = Device_Dropdown.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener((x) => UpdateDeviceDropdown());
        trigger.triggers.Add(entry);

        //デバイス選択時のイベントを設定
        Device_Dropdown.onValueChanged.AddListener((x) => SetDevice(x));
        SetDevice(0);

        UpdateDeviceDropdown(); //表示を更新する

        Play_Button.onClick.AddListener(() => PlayButton_OnClick()); // プレイボタンのイベントを設定
        DeviceName_Text = Device_Dropdown.transform.Find("Label").gameObject.GetComponent<TMP_Text>();
    }


    /// <summary>ドロップダウンから使用するデバイスを設定(ドロップダウンのコールバック)</summary>
    public void SetDevice(int num)
    {
        selectedDevice = WinSerial.GetPortNames()[num]; //選択されているデバイス
        OutputView.Write($"シリアルデバイスが設定されました -> {selectedDevice}");
    }


    void Callback(string text)
    {
        Debug.Log($"<From RPi> {text}");


    }


    ///<summary>デバイス選択のドロップダウンを更新</summary>
    public void UpdateDeviceDropdown()
    {
        string[] port = WinSerial.GetPortNames();

        Device_Dropdown.options.Clear();
        foreach (string name in port)
        {
            Device_Dropdown.options.Add(new TMP_Dropdown.OptionData { text = name });
        }
        Device_Dropdown.RefreshShownValue();
    }


    /// <summary>プレイボタンを押したときの処理</summary>
    public void PlayButton_OnClick()
    {
        if (!isConnected) //再生されていない時(-> 再生したい)
        {
            try
            {
                OutputView.Write($"{selectedDevice} に接続します");

                serial = new WinSerial(selectedDevice);
                serial.Open();
                serial.SetCallback(Callback);
                isConnected = true;

                OutputView.Write($"接続しました -> {selectedDevice}");

                // UIを更新
                Device_Dropdown.interactable = false; //ドロップダウンを無効化
                DeviceName_Text.SetAlpha(0.25f); //テキストの色を薄くする
                Play_Button.GetComponent<Image>().color = new Color(0.2f, 0.42f, 0.6f, 1); //ボタンの色を変更する
            }
            catch
            {
                OutputView.Write($"シリアルデバイスに接続できませんでした-> {selectedDevice}");
                Debug.Log("シリアルに接続できませんでした");
            }

        }
        else //再生されている時(-> 停止したい)
        {
            //UIを更新
            Device_Dropdown.interactable = true; //ドロップダウンを有効化
            DeviceName_Text.SetAlpha(1.0f);//テキストの色を通常に戻す
            Play_Button.GetComponent<Image>().color = new Color(0.22f, 0.22f, 0.22f, 1); //ボタンの色を変更する

            serial.Close();

            OutputView.Write($"クローズしました -> {selectedDevice}");

            isConnected = false;
        }

    }

    public void OnDisable()
    {
        if (!isConnected) return;

        serial.Close();
        isConnected = false;
    }

    public static void Write(string msg) => Instance.Write_NS(msg);
    public void Write_NS(string msg)
    {
        if (!isConnected) return;

        serial.Write(msg);
    }
}


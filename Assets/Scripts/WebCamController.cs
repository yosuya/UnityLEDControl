using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class WebCamController : MonoBehaviour
{
    protected TMP_Dropdown Device_Dropdown;

    private WebCamTexture webCamTex;

    public void Setup()
    {
        //ドロップダウンの取得と設定
        Device_Dropdown = GameObject.Find("CameraDevice_Dropdown").GetComponent<TMP_Dropdown>();

        //ドロップダウンのリストを更新するためのイベントを設定
        EventTrigger trigger = Device_Dropdown.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener((x) => UpdateDeviceDropdown());
        trigger.triggers.Add(entry);

        //デバイス選択時のイベントを設定
        Device_Dropdown.onValueChanged.AddListener((x) => SetDevice(x));



        webCamTex = new WebCamTexture();
    }

    ///<summary>デバイス選択のドロップダウンを更新</summary>
    public void UpdateDeviceDropdown()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        string[] deviceNames = devices.Select(x => x.name).ToArray();

        Device_Dropdown.options.Clear();
        foreach (string name in deviceNames)
        {
            Device_Dropdown.options.Add(new TMP_Dropdown.OptionData { text = name });
        }
        Device_Dropdown.RefreshShownValue();
    }

    /// <summary>ドロップダウンから使用するデバイスを設定</summary>
    public void SetDevice(int num)
    {
        WebCamDevice selectedDevice = WebCamTexture.devices[num];

        OutputView.Write($"デバイスが設定されました -> {selectedDevice.name}");
    }
}

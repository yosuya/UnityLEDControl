using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;


public class WebCamController : MonoBehaviour
{
    /// UI
    public TMP_Dropdown CameraDevice_Dropdown;
    public Button Play_Button;
    public Button Pause_Button;
    public Slider AlphaSlider;

    private TMP_Text CameraDeviceName_Text;

    /// Webカメラ表示関連
    private WebCamTexture webCamTex = null;
    public RawImage WebCam_RawImage; //Webカメラで撮影した画像を表示する
    private bool isPlay = false;
    private bool isPause = false;


    public void Setup()
    {
        /// -------------------------------------
        /// カメラデバイスを選択するドロップダウンの設定
        /// -------------------------------------

        //ドロップダウンのリストを更新するためのイベントを設定
        EventTrigger trigger = CameraDevice_Dropdown.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener((x) => UpdateDeviceDropdown());
        trigger.triggers.Add(entry);

        UpdateDeviceDropdown(); //表示を更新する

        //デバイス選択時のイベントを設定
        CameraDevice_Dropdown.onValueChanged.AddListener((x) => SetDevice(x));


        /// ------------------------
        /// ボタンやスライダーの設定
        /// ------------------------

        Play_Button.onClick.AddListener(() => PlayButton_OnClick()); // プレイボタンのイベントを設定
        CameraDeviceName_Text = CameraDevice_Dropdown.transform.Find("Label").gameObject.GetComponent<TMP_Text>();

        Pause_Button.onClick.AddListener(() => PauseButton_OnClick()); // ポーズボタンのイベントを設定
        Pause_Button.interactable = false;　//再生していないときはポーズボタンを無効化

        // スライダーのイベントを設定
        AlphaSlider.onValueChanged.AddListener((x) => AlphaSlider_OnValueChanged(x));

        /// ------------------------


        webCamTex = new WebCamTexture();
    }

    ///<summary>デバイス選択のドロップダウンを更新</summary>
    public void UpdateDeviceDropdown()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        string[] deviceNames = devices.Select(x => x.name).ToArray();

        CameraDevice_Dropdown.options.Clear();
        foreach (string name in deviceNames)
        {
            CameraDevice_Dropdown.options.Add(new TMP_Dropdown.OptionData { text = name });
        }
        CameraDevice_Dropdown.RefreshShownValue();
    }

    /// <summary>ドロップダウンから使用するデバイスを設定(ドロップダウンのコールバック)</summary>
    public void SetDevice(int num)
    {
        if (webCamTex.isPlaying) //すでに表示されていたときの処理
        {
            webCamTex.Stop();   //Webカメラを停止する
            WebCam_RawImage.texture = null; //テクスチャをリセット
            WebCam_RawImage.color = new Color(0.05f, 0.05f, 0.05f, 1); //映像のRawImageを暗くする
        }

        WebCamDevice selectedDevice = WebCamTexture.devices[num]; //選択されているデバイス
        webCamTex = new WebCamTexture(selectedDevice.name, 1280, 720, 30); //テクスチャの設定
        WebCam_RawImage.texture = webCamTex;

        OutputView.Write($"デバイスが設定されました -> {selectedDevice.name}");
    }


    /// <summary>プレイボタンを押したときの処理</summary>
    public void PlayButton_OnClick()
    {
        if (!isPlay) //再生されていない時(-> 再生したい)
        {
            try
            {
                webCamTex.Play();
            }
            catch (Exception e)
            {
                OutputView.Write($"デバイスを開けませんでした.");
                OutputView.Write(e.ToString()); //例外を出力
                return;
            }


            WebCam_RawImage.texture = webCamTex;    //表示するテクスチャを設定
            WebCam_RawImage.color = new Color(1, 1, 1, AlphaSlider.value); //映像を表示するため色を更新

            // UIを更新
            CameraDevice_Dropdown.interactable = false; //ドロップダウンを無効化
            CameraDeviceName_Text.SetAlpha(0.25f); //テキストの色を薄くする
            Play_Button.GetComponent<Image>().color = new Color(0.2f, 0.42f, 0.6f, 1); //ボタンの色を変更する

            Pause_Button.interactable = true; //再生中にポーズボタンを有効化


            isPlay = true;
        }
        else //再生されている時(-> 停止したい)
        {
            webCamTex.Stop(); //カメラを停止
            WebCam_RawImage.texture = null; //テクスチャをリセット
            WebCam_RawImage.color = new Color(0.05f, 0.05f, 0.05f, 1); //映像のRawImageを暗くする

            //UIを更新
            CameraDevice_Dropdown.interactable = true; //ドロップダウンを有効化
            CameraDeviceName_Text.SetAlpha(1.0f);//テキストの色を通常に戻す
            Play_Button.GetComponent<Image>().color = new Color(0.22f, 0.22f, 0.22f, 1); //ボタンの色を変更する

            if (isPause) PauseButton_OnClick(); //ポーズを解除
            Pause_Button.interactable = false;　//再生していないときはポーズボタンを無効化

            isPlay = false;
        }

    }

    /// <summary>ポーズボタンを押したときの処理</summary>
    public void PauseButton_OnClick()
    {
        if (!isPause) //現在ポーズされていない(-> ポーズしたい)
        {
            webCamTex.Pause();

            // UIを更新
            Pause_Button.GetComponent<Image>().color = new Color(0.2f, 0.42f, 0.6f, 1); //ボタンの色を変更する

            isPause = true;
        }
        else //現在ポーズされている(-> ポーズを解除したい)
        {
            webCamTex.Play();

            //UIを更新
            Pause_Button.GetComponent<Image>().color = new Color(0.22f, 0.22f, 0.22f, 1); //ボタンの色を変更する

            isPause = false;
        }
    }

    /// <summary>透明度スライダーを更新したときの処理</summary>
    public void AlphaSlider_OnValueChanged(float value)
    {
        if (isPlay)
        {
            //映像の透明度を変更
            WebCam_RawImage.color = new Color(1, 1, 1, value);
        }
    }
}

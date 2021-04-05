using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppManager : MonoBehaviour
{
    //スクリプトのインスタンス
    protected UIController _uiController;
    protected WebCamController _webCamController;
    protected MarkerManager _markerManager;

    public SceneMode sceneMode;

    void Start()
    {
        //インスタンスを取得
        _uiController = GetComponent<UIController>();
        //スクリプトの初期化を実行
        _uiController.Setup();

        if (sceneMode == SceneMode.Display)
        {
            _webCamController = GetComponent<WebCamController>();
            _markerManager = GetComponent<MarkerManager>();
            _webCamController.Setup();
            _markerManager.Setup();
        }

    }

    public enum SceneMode
    {
        Display = 0,
        Simple = 1
    }

}

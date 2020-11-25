using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppManager : MonoBehaviour
{
    //スクリプトのインスタンス
    protected UIController _uiController;
    protected WebCamController _webCamController;
    protected MarkerManager _markerManager;


    void Start()
    {
        //インスタンスを取得
        _uiController = GetComponent<UIController>();
        _webCamController = GetComponent<WebCamController>();
        _markerManager = GetComponent<MarkerManager>();

        //スクリプトの初期化を実行
        _uiController.Setup();
        _webCamController.Setup();
        _markerManager.Setup();

    }

}

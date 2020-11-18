using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppManager : MonoBehaviour
{
    //スクリプトのインスタンス
    protected UIController _uiController;
    protected WebCamController _webCamController;


    void Start()
    {
        //インスタンスを取得
        _uiController = GetComponent<UIController>();
        _webCamController = GetComponent<WebCamController>();

        //スクリプトの初期化を実行
        _uiController.Setup();
        _webCamController.Setup();

    }

}

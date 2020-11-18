using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    /// UI オブジェクト
    protected RawImage LEDCam_RawImage;
    protected RawImage WebCam_RawImage;



    protected RenderTexture _ledCamRT;
    protected RenderTexture _WebCamRT;


    public void Setup()
    {
        //コンポーネントの取得


    }
}

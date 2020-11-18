using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebCamController : MonoBehaviour
{
    private WebCamTexture webCamTex;

    private void Start()
    {
        webCamTex = new WebCamTexture();
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yosuya.UnityLEDControl
{
    [RequireComponent(typeof(Camera))]
    public class LEDCam : MonoBehaviour
    {
        protected RenderTexture renderTex;
        protected Texture2D tex2d;

        protected Camera _cam;

        void Start()
        {
            renderTex = new RenderTexture(1280, 720, 16, RenderTextureFormat.ARGB32);
            renderTex.Create();

            _cam = GetComponent<Camera>();
            _cam.targetTexture = renderTex;
        }


        ///<summary>カメラのピクセルデータを読み込む</summary>
        public Texture2D ReadPixels()
        {
            tex2d = new Texture2D(renderTex.width, renderTex.height, TextureFormat.ARGB32, false, false);
            _cam.Render();

            RenderTexture.active = renderTex;
            tex2d.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            tex2d.Apply();
            return tex2d;
        }
    }
}
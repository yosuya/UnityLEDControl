using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yosuya.UnityLEDControl
{
    public class ColorPicker
    {
        ///<summary>テクスチャの特定の位置の色を取得</summary>
        static public Color Pick(Texture2D texture2D, Vector2 pos)
        {
            if (pos.x < 0.0f || pos.x > 1.0f || pos.y < 0.0f || pos.y > 1.0f)
            {
                Debug.LogWarning("0.0 - 1.0 の範囲で入力してください.");
                return Color.black;
            }

            return texture2D.GetPixel(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
        }
    }
}
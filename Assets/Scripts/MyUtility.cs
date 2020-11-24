using UnityEngine;
using TMPro;

static public class MyUtility
{
    ///<summary>テキストのカラーのアルファ値のみを変更する拡張メソッド</summary>
    public static void SetAlpha(this TMP_Text text, float alpha)
    {
        Color newColor = text.color;
        newColor.a = alpha;
        text.color = newColor;
    }
}